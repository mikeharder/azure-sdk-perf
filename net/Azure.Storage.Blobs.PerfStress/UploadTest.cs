using Azure.Storage.Blobs.PerfStress.Core;
using Azure.Test.PerfStress;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.Storage.Blobs.PerfStress
{
    public class UploadTest : BlobTest<StorageTransferOptionsOptions>
    {
        private readonly Stream _stream;

        public UploadTest(StorageTransferOptionsOptions options) : base(options)
        {
            _stream = RandomStream.Create(options.Size);
        }

        public override void Dispose(bool disposing)
        {
            _stream.Dispose();
            base.Dispose(disposing);
        }

        public override void Run(CancellationToken cancellationToken)
        {
            _stream.Seek(0, SeekOrigin.Begin);
            BlobClient.Upload(_stream, transferOptions: Options.StorageTransferOptions, cancellationToken: cancellationToken);
        }

        public override async Task RunAsync(CancellationToken cancellationToken)
        {
            _stream.Seek(0, SeekOrigin.Begin);
            await BlobClient.UploadAsync(_stream, transferOptions: Options.StorageTransferOptions,  cancellationToken: cancellationToken);
        }
    }
}
