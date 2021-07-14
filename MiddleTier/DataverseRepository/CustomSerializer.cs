using RestSharp;
using RestSharp.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text.Json;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class CustomJsonSerializer : IRestSerializer
    {
        public string Serialize(object obj) => JsonSerializer.Serialize(obj);

        public string Serialize(Parameter bodyParameter)
        {
            return Serialize(bodyParameter);
        }

        public T Deserialize<T>(IRestResponse response) => JsonSerializer.Deserialize<T>(response.Content);

        public string[] SupportedContentTypes { get; } =
        {
        "application/json", "text/json", "text/x-json", "text/javascript", "*+json"
    };

        public string ContentType { get; set; } = "application/json";

        public DataFormat DataFormat { get; } = DataFormat.Json;
    }
}
