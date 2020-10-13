﻿using CommandLine;
using System.Diagnostics.Tracing;

namespace Azure.Test.Stress
{
    public class StressOptions
    {
        [Option('d', "duration", Default = 10, HelpText = "Duration of test in seconds")]
        public int Duration { get; set; }

        [Option("event-level", Default = EventLevel.Error, HelpText = "EventLevel for AzureEventSourceListener")]
        public EventLevel EventLevel { get; set; }

        [Option("events-file", HelpText = "Write events to file (in addition to console)")]
        public string EventsFile { get; set; }

        [Option("exceptions-file", HelpText = "Write exceptions to file (in addition to console)")]
        public string ExceptionsFile { get; set; }

        [Option("metrics-file", HelpText = "Write metrics to file (in addition to console)")]
        public string MetricsFile { get; set; }

        [Option("no-cleanup", HelpText = "Disables test cleanup")]
        public bool NoCleanup { get; set; }
    }
}