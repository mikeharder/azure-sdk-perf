using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Messaging.EventHubs.Producer;
using Azure.Messaging.EventHubs.Stress.Core;
using Azure.Storage.Blobs;
using Azure.Test.Stress;
using CommandLine;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.Messaging.EventHubs.Stress
{
    public class EventProcessorTest : EventHubsTest<EventProcessorTest.EventProcessorOptions, EventProcessorTest.EventProcessorMetrics>
    {
        public EventProcessorTest(EventProcessorOptions options, EventProcessorMetrics metrics) : base(options, metrics)
        {
        }

        public override async Task RunAsync(CancellationToken cancellationToken)
        {
            var producerTask = Produce(cancellationToken);

            using var processorCts = new CancellationTokenSource();
            var processorTask = Process(processorCts.Token);

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
            await processorTask;
        }

        private async Task Produce(CancellationToken cancellationToken)
        {
            var producer = new EventHubProducerClient(EventHubsConnectionString, EventHubName);
            while (!cancellationToken.IsCancellationRequested)
            {
                using var batch = await producer.CreateBatchAsync();

                for (var i = 0; i < Options.PublishBatchSize; i++)
                {
                    if (!batch.TryAdd(new EventData(null)))
                    {
                        break;
                    }
                }

                await producer.SendAsync(batch, cancellationToken);

                Interlocked.Add(ref Metrics.EventsPublished, batch.Count);
                Interlocked.Increment(ref Metrics.TotalServiceOperations);
            }
        }

        private async Task Process(CancellationToken cancellationToken)
        {
            var storageClient = new BlobContainerClient(StorageConnectionString, BlobContainerName);
            var processor = new EventProcessorClient(storageClient, EventHubConsumerClient.DefaultConsumerGroupName,
                EventHubsConnectionString, EventHubName);

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
            finally {
                // TODO: Set timeout and log exceptions if cannot be cancelled
                await processor.StopProcessingAsync();
            }

            processor.ProcessEventAsync -= ProcessEventHandler;
            processor.ProcessErrorAsync -= ProcessErrorHandler;
        }

        private Task ProcessErrorHandler(ProcessErrorEventArgs args)
        {
            // TODO: More granular exception categorization
            Metrics.Exceptions.Enqueue(args.Exception);
            return Task.CompletedTask;
        }

        private async Task ProcessEventHandler(ProcessEventArgs args)
        {
            Interlocked.Increment(ref Metrics.TotalServiceOperations);

            if (!args.HasEvent)
            {
                return;
            }

            Interlocked.Increment(ref Metrics.EventsRead);
        }

        public class EventProcessorMetrics : StressMetrics
        {
            public long EventsPublished;
            public long EventsRead;
            public long TotalServiceOperations;
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
