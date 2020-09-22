using Azure.Test.PerfStress;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace System.PerfStress
{
    public class TimerTest : PerfStressTest<PerfStressOptions>
    {
        public TimerTest(PerfStressOptions options) : base(options)
        {
        }

        public override async Task RunLoopAsync(ResultCollector resultCollector, bool latency, Channel<(TimeSpan, Stopwatch)> pendingOperations, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();

            using var timer = new Timer(_ =>
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

            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
        }
    }
}
