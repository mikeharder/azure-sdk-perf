using Azure.Test.Stress;
using System.Threading.Tasks;

namespace System.Stress
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await StressProgram.Main(typeof(Program).Assembly, args);
        }
    }
}
