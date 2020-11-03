using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Messaging.EventHubs.Producer;
using Azure.Messaging.EventHubs.Stress.Core;
using Azure.Test.Stress;
using CommandLine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.Messaging.EventHubs.Stress
{
    public class EventProcessorTest : EventHubsTest<EventProcessorTest.EventProcessorOptions, EventProcessorTest.EventProcessorMetrics>
    {
        // Key: message id, Value: processed count
        private readonly ConcurrentDictionary<int, int> _published = new ConcurrentDictionary<int, int>();

        private readonly SemaphoreSlim _partitionsInitialized = new SemaphoreSlim(initialCount: 0);

        public EventProcessorTest(EventProcessorOptions options, EventProcessorMetrics metrics) : base(options, metrics)
        {
        }

        public override async Task RunAsync(CancellationToken cancellationToken)
        {
            var producer = new EventHubProducerClient(EventHubsConnectionString, EventHubName);
            var partitions = (await producer.GetPartitionIdsAsync(cancellationToken)).Length;

            using var processorCts = new CancellationTokenSource();

            // TODO: Test multiple processors with multiple partitions
            var processorTasks = new Task[Options.ProcessorCount];
            for (var i=0; i < Options.ProcessorCount; i++)
            {
                processorTasks[i] = Process(processorCts.Token);
            }

            // Wait for partitions to be initialized, to ensure processor only consumes the newly published messages.
            await Task.WhenAll(Enumerable.Range(0, partitions).Select(_ => _partitionsInitialized.WaitAsync()));

            var producerTask = Produce(producer, cancellationToken);

            try
            {
                await producerTask;
            }
            catch (Exception e) when (ContainsOperationCanceledException(e))
            {
            }

            // Block until all messages have been received OR the timeout is exceeded
            using var unprocessedEventCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(Options.UnprocessedEventTimeoutMs));
            await DelayUntil(() => Metrics.EventsRead >= Metrics.EventsPublished, unprocessedEventCts.Token);

            processorCts.Cancel();
            await Task.WhenAll(processorTasks);
        }

        private async Task Produce(EventHubProducerClient producer, CancellationToken cancellationToken)
        {
            var id = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                var batchEvents = new List<EventData>();
                using var batch = await producer.CreateBatchAsync();

                for (var i = 0; i < Options.PublishBatchSize; i++)
                {
                    var currentId = id++;

                    _published.TryAdd(currentId, 0);
                    var e = new EventData(BitConverter.GetBytes(currentId));

                    if (!batch.TryAdd(e))
                    {
                        break;
                    }

                    batchEvents.Add(e);
                }

                try
                {
                    await producer.SendAsync(batch, cancellationToken);
                    Interlocked.Add(ref Metrics.EventsPublished, batch.Count);
                    Interlocked.Increment(ref Metrics.TotalServiceOperations);
                }
                catch
                {
                    // SendAsync failed, so remove events from tracking
                    foreach (var batchEvent in batchEvents)
                    {
                        var currentId = BitConverter.ToInt32(batchEvent.Body.ToArray(), startIndex: 0);
                        _published.TryRemove(currentId, out _);
                    }
                }
            }
        }

        private async Task Process(CancellationToken cancellationToken)
        {
            var processor = new EventProcessorClient(BlobContainerClient, EventHubConsumerClient.DefaultConsumerGroupName,
                EventHubsConnectionString, EventHubName);

            processor.PartitionInitializingAsync += PartitionInitializingHandler;
            processor.ProcessEventAsync += ProcessEventHandler;
            processor.ProcessErrorAsync += ProcessErrorHandler;

            try
            {
                await processor.StartProcessingAsync(cancellationToken);
                await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                // TODO: Set timeout and log exceptions if cannot be cancelled
                await processor.StopProcessingAsync();
            }

            processor.PartitionInitializingAsync -= PartitionInitializingHandler;
            processor.ProcessEventAsync -= ProcessEventHandler;
            processor.ProcessErrorAsync -= ProcessErrorHandler;
        }

        private Task PartitionInitializingHandler(PartitionInitializingEventArgs args)
        {
            args.DefaultStartingPosition = EventPosition.Latest;
            _partitionsInitialized.Release();
            return Task.CompletedTask;
        }

        private Task ProcessEventHandler(ProcessEventArgs args)
        {
            Interlocked.Increment(ref Metrics.TotalServiceOperations);

            if (!args.HasEvent)
            {
                return Task.CompletedTask;
            }

            var id = BitConverter.ToInt32(args.Data.Body.ToArray(), startIndex: 0);

            if (_published.ContainsKey(id))
            {
                if (_published.TryUpdate(id, 1, 0))
                {
                    Interlocked.Increment(ref Metrics.EventsRead);
                }
                else
                {
                    Interlocked.Increment(ref Metrics.EventsReadDuplicate);
                    while (true)
                    {
                        _published.TryGetValue(id, out var processedCount);
                        if (_published.TryUpdate(id, ++processedCount, processedCount))
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                Interlocked.Increment(ref Metrics.EventsReadUnknown);
            }

            return Task.CompletedTask;
        }

        private Task ProcessErrorHandler(ProcessErrorEventArgs args)
        {
            // TODO: More granular exception categorization
            Metrics.Exceptions.Enqueue(args.Exception);
            return Task.CompletedTask;
        }

        public class EventProcessorMetrics : StressMetrics
        {
            public long EventsPublished;
            public long EventsRead;
            public long EventsReadUnknown;
            public long EventsReadDuplicate;
            public long TotalServiceOperations;

            protected override void ProcessEvent(EventWrittenEventArgs eventArgs, string message)
            {
                // Only track messages from specific sources
                if (eventArgs.EventSource.Name.StartsWith("Azure-Messaging-EventHubs-Processor"))
                {
                    base.ProcessEvent(eventArgs, message);
                }
            }
        }

        public class EventProcessorOptions : StressOptions
        {
            [Option("processor-count", Default = 3, HelpText = "Processor instances")]
            public int ProcessorCount { get; set; }

            [Option("publish-batch-size", Default = 5, HelpText = "Messages per publish batch")]
            public int PublishBatchSize { get; set; }

            [Option("publish-delay-ms", Default = 15, HelpText = "Delay between publishes (in ms)")]
            public int PublishDelayMs { get; set; }

            [Option("unprocessed-event-timeout-ms", Default = 10000, HelpText = "Timeout for unprocessed events (in ms)")]
            public int UnprocessedEventTimeoutMs { get; set; }
        }
    }
}
