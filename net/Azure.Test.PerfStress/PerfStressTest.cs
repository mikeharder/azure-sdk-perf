using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Azure.Test.PerfStress
{
    public abstract class PerfStressTest<TOptions> : IPerfStressTest where TOptions : PerfStressOptions
    {
        protected TOptions Options { get; private set; }

        public PerfStressTest(TOptions options)
        {
            Options = options;
        }

        public virtual Task GlobalSetupAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task SetupAsync()
        {
            return Task.CompletedTask;
        }

        public virtual void Run(CancellationToken cancellationToken)
        {
        }

        // TODO: Move to new class PerfStressTestBase
        public virtual async Task RunLoopAsync(ResultCollector resultCollector, bool latency, Channel<(TimeSpan, Stopwatch)> pendingOperations, CancellationToken cancellationToken)
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

        public virtual Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual Task CleanupAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task GlobalCleanupAsync()
        {
            return Task.CompletedTask;
        }
    }
}
