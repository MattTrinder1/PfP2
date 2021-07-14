using System.Text.Json;

namespace SystemTextJsonSamples
{
    public class LookupNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) =>
            name.ToUpper();
    }
}