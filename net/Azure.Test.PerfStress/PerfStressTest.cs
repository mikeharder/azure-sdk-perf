using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Azure.Test.PerfStress
{
    public abstract class PerfStressTest<TOptions> : PerfStressTestBase<TOptions> where TOptions : PerfStressOptions
    {
        public PerfStressTest(TOptions options) : base(options)
        {
        }

        public abstract void Run(CancellationToken cancellationToken);

        public abstract Task RunAsync(CancellationToken cancellationToken);

        public sealed override void RunLoop(ResultCollector resultCollector, bool latency, Channel<(TimeSpan, Stopwatch)> pendingOperations, CancellationToken cancellationToken)
        {
            var latencySw = new Stopwatch();
            (TimeSpan Start, Stopwatch Stopwatch) operation = (TimeSpan.Zero, null);

            while (!cancellationToken.IsCancellationRequested)
            {
                if (pendingOperations != null)
                {
                    // TODO: Could the perf of this be improved?
                    operation = pendingOperations.Reader.ReadAsync(cancellationToken).AsTask().Result;
                }

                if (latency)
                {
                    latencySw.Restart();
                }

                Run(cancellationToken);

                if (latency)
                {
                    if (pendingOperations != null)
                    {
                        resultCollector.Add(latencySw.Elapsed, operation.Stopwatch.Elapsed - operation.Start);
                    }
                    else
                    {
                        resultCollector.Add(latencySw.Elapsed);
                    }
                }
                else
                {
                    resultCollector.Add();
                }
            }
        }

        public sealed override async Task RunLoopAsync(ResultCollector resultCollector, bool latency, Channel<(TimeSpan, Stopwatch)> pendingOperations, CancellationToken cancellationToken)
        {
            var latencySw = new Stopwatch();
            (TimeSpan Start, Stopwatch Stopwatch) operation = (TimeSpan.Zero, null);

            while (!cancellationToken.IsCancellationRequested)
            {
                if (pendingOperations != null)
                {
                    operation = await pendingOperations.Reader.ReadAsync(cancellationToken);
                }

                if (latency)
                {
                    latencySw.Restart();
                }

                await RunAsync(cancellationToken);

                if (latency)
                {
                    if (pendingOperations != null)
                    {
                        resultCollector.Add(latencySw.Elapsed, operation.Stopwatch.Elapsed - operation.Start);
                    }
                    else
                    {
                        resultCollector.Add(latencySw.Elapsed);
                    }
                }
                else
                {
                    resultCollector.Add();
                }
            }
        }
    }
}
