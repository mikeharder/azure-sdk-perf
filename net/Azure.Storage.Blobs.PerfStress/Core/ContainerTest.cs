﻿using Azure.Test.PerfStress;
using System;
using System.Threading.Tasks;

namespace Azure.Storage.Blobs.PerfStress.Core
{
    public abstract class ContainerTest<TOptions> : ServiceTest<TOptions> where TOptions : PerfStressOptions
    {
        protected static string ContainerName { get; } = "perfstress-" + Guid.NewGuid();

        protected BlobContainerClient BlobContainerClient { get; private set; }

        public ContainerTest(TOptions options) : base(options)
        {
            BlobContainerClient = BlobServiceClient.GetBlobContainerClient(ContainerName);
        }

        public override async Task GlobalSetupAsync()
        {
            await base.GlobalSetupAsync();
            await BlobContainerClient.CreateAsync();
        }

        public override async Task GlobalCleanupAsync()
        {
            await BlobContainerClient.DeleteAsync();
            await base.GlobalCleanupAsync();
        }
    }
}
