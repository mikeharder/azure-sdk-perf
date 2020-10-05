using Azure.Test.Stress;
using System.Threading.Tasks;

namespace Azure.Messaging.EventHubs.Stress
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await StressProgram.Main(typeof(Program).Assembly, args);
        }
    }
}
