using Newtonsoft.Json;

namespace Azure.Test.Stress
{
    public class StressMetrics
    {
        public override string ToString()
        {
            // Use Newtonsoft.Json to serialize fields.  System.Text.Json does not support serializing fields until .NET 5.0.
            return JsonConvert.SerializeObject(this);
        }
    }
}
