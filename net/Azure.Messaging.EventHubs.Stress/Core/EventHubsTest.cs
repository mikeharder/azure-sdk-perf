using Azure.Storage.Blobs;
using Azure.Test.Stress;
using System;
using System.Threading.Tasks;

namespace Azure.Messaging.EventHubs.Stress.Core
{
    public abstract class EventHubsTest<TOptions, TMetrics> : StressTest<TOptions, TMetrics> where TOptions: StressOptions where TMetrics : StressMetrics
    {
        protected string EventHubsConnectionString { get; private set; }
        protected string EventHubName { get; private set; }
        
        protected string StorageConnectionString { get; private set; }
        protected string BlobContainerName { get; } = "stress-" + Guid.NewGuid();
        protected BlobContainerClient BlobContainerClient { get; private set; }

        protected EventHubsTest(TOptions options, TMetrics metrics) : base(options, metrics)
        {
            EventHubsConnectionString = Environment.GetEnvironmentVariable("EVENT_HUBS_CONNECTION_STRING");
            if (string.IsNullOrEmpty(EventHubsConnectionString))
            {
                throw new InvalidOperationException("Undefined environment variable EVENT_HUBS_CONNECTION_STRING");
            }

            EventHubName = Environment.GetEnvironmentVariable("EVENT_HUB_NAME");
            if (string.IsNullOrEmpty(EventHubName))
            {
                throw new InvalidOperationException("Undefined environment variable EVENT_HUB_NAME");
            }

            StorageConnectionString = Environment.GetEnvironmentVariable("STORAGE_CONNECTION_STRING");
            if (string.IsNullOrEmpty(StorageConnectionString))
            {
                throw new InvalidOperationException("Undefined environment variable STORAGE_CONNECTION_STRING");
            }

            BlobContainerClient = new BlobContainerClient(StorageConnectionString, BlobContainerName);
        }

        public override async Task SetupAsync()
        {
            await base.SetupAsync();
            await BlobContainerClient.CreateAsync();
        }

        public override async Task CleanupAsync()
        {
            await BlobContainerClient.DeleteAsync();
            await base.CleanupAsync();
        }

    }
}
