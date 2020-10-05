using Azure.Messaging.EventHubs.Producer;
using Azure.Messaging.EventHubs.Stress.Core;
using Azure.Test.Stress;
using CommandLine;
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
            await Produce(cancellationToken);
        }

        private async Task Produce(CancellationToken cancellationToken)
        {
            var producer = new EventHubProducerClient(EventHubsConnectionString, EventHubName);
            while (!cancellationToken.IsCancellationRequested)
            {
                using var batch = await producer.CreateBatchAsync();
                
                for (var i=0; i < Options.PublishBatchSize; i++)
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

        public class EventProcessorMetrics : StressMetrics
        {
            public long EventsPublished;
            public long TotalServiceOperations;
        }

        public class EventProcessorOptions : StressOptions
        {
            [Option("processor-count", Default = 3, HelpText = "Processor instances")]
            public int ProcessorCount { get; set; }

            [Option("publish-batch-size", Default = 50, HelpText = "Messages per publish batch")]
            public int PublishBatchSize { get; set; }

            [Option("publish-delay-ms", Default = 15, HelpText = "Delay between publishes (in ms)")]
            public int PublishDelayMs { get; set; }
        }
    }
}
