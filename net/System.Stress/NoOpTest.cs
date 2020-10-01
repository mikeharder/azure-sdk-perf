using Azure.Test.Stress;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Stress
{
    public class NoOpTest : StressTest<StressOptions>
    {
        public NoOpTest(StressOptions options) : base(options)
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
