﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.Test.Stress
{
    public abstract class StressTest<TOptions, TMetrics> : IStressTest where TOptions : StressOptions where TMetrics : StressMetrics
    {
        protected TOptions Options { get; private set; }
        protected TMetrics Metrics { get; private set; }

        public StressTest(TOptions options, TMetrics metrics)
        {
            Options = options;
            Metrics = metrics;
        }

        public virtual Task SetupAsync()
        {
            return Task.CompletedTask;
        }

        public abstract Task RunAsync(CancellationToken cancellationToken);

        public virtual Task CleanupAsync()
        {
            return Task.CompletedTask;
        }

        // https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#implement-both-dispose-and-async-dispose-patterns
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            Dispose(disposing: false);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
        }

        public virtual ValueTask DisposeAsyncCore()
        {
            return default;
        }
    }
}
