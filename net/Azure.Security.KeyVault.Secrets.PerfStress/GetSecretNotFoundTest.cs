using Azure.Test.PerfStress;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.Security.KeyVault.Secrets.PerfStress
{
    public class GetSecretNotFoundTest : PerfStressTest<PerfStressOptions>
    {
        public GetSecretNotFoundTest(PerfStressOptions options) : base(options)
        {
        }

        public override void Run(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task RunAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
