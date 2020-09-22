using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Azure.Test.PerfStress
{
    internal interface IPerfStressTest
    {
        Task GlobalSetupAsync();
        Task SetupAsync();
        void Run(CancellationToken cancellationToken);
        Task RunLoopAsync(ResultCollector resultCollector, bool latency, Channel<(TimeSpan, Stopwatch)> pendingOperations, CancellationToken cancellationToken);
        Task RunAsync(CancellationToken cancellationToken);
        Task CleanupAsync();
        Task GlobalCleanupAsync();
    }
}
