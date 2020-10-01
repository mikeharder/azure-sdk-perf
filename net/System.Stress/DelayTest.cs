using Azure.Test.Stress;
using CommandLine;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Stress
{
    public class DelayTest : StressTest<DelayTest.DelayOptions, DelayTest.DelayMetrics>
    {
        public DelayTest(DelayOptions options, DelayMetrics metrics) : base(options, metrics)
        {
        }

        public override async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(Options.InitialDelayMs), cancellationToken);

                    // Increment metrics
                    Interlocked.Increment(ref Metrics.TotalOperations);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        public class DelayOptions : StressOptions
        {
            [Option("initialDelayMs", Default = 1000, HelpText = "Initial delay (in milliseconds)")]
            public int InitialDelayMs { get; set; }
        }

        public class DelayMetrics : StressMetrics {
            public long TotalOperations;
        }
    }
}
