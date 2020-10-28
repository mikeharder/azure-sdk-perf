using Azure.Test.PerfStress;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.Messaging.ServiceBus.PerfStress.Core
{
    public abstract class ServiceTest<TOptions> : PerfStressTest<TOptions> where TOptions : PerfStressOptions
    {
        protected ServiceBusClient ServiceBusClient { get; private set; }

        public ServiceTest(TOptions options) : base(options)
        {
            var connectionString = GetEnvironmentVariable("SERVICEBUS_CONNECTION_STRING");

            ServiceBusClient = new ServiceBusClient(connectionString);
        }
    }
}
