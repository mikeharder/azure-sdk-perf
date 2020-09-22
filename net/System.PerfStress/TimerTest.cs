using Azure.Test.PerfStress;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace System.PerfStress
{
    public class TimerTest : PerfStressTestBase<PerfStressOptions>
    {
        public TimerTest(PerfStressOptions options) : base(options)
        {
        }

        private Timer CreateTimer(ResultCollector resultCollector, bool latency)
        {
            var sw = Stopwatch.StartNew();

            return new Timer(_ =>
            {
                if (latency)
                {
                    resultCollector.Add(sw.Elapsed);
                    sw.Restart();
                }
                else
                {
                    resultCollector.Add();
                }
            },
            state: null, dueTime: TimeSpan.FromSeconds(1), period: TimeSpan.FromSeconds(1));
        }

        public override void RunLoop(ResultCollector resultCollector, bool latency, Channel<(TimeSpan, Stopwatch)> pendingOperations, CancellationToken cancellationToken)
        {
            using var timer = CreateTimer(resultCollector, latency);
            cancellationToken.WaitHandle.WaitOne();
        }

        public override async Task RunLoopAsync(ResultCollector resultCollector, bool latency, Channel<(TimeSpan, Stopwatch)> pendingOperations, CancellationToken cancellationToken)
        {
            using var timer = CreateTimer(resultCollector, latency);
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
        }
    }
}
