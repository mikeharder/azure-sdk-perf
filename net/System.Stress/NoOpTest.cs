using Azure.Test.Stress;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Stress
{
    public class NoOpTest : StressTest<StressOptions, StressMetrics>
    {
        public NoOpTest(StressOptions options, StressMetrics metrics) : base(options, metrics)
        {
        }

        public override async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
