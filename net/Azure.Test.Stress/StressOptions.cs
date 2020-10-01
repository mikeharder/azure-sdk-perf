using CommandLine;

namespace Azure.Test.Stress
{
    public class StressOptions
    {
        [Option('d', "duration", Default = 10, HelpText = "Duration of test in seconds")]
        public int Duration { get; set; }

        [Option("no-cleanup", HelpText = "Disables test cleanup")]
        public bool NoCleanup { get; set; }
    }
}
