using Newtonsoft.Json.Linq;
using System;
using System.Dynamic;
using System.IO;
using YamlDotNet.Serialization.NamingConventions;

namespace CanvasAppRewriter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build(); 
            dynamic myConfig = deserializer.Deserialize<ExpandoObject>(File.ReadAllText(@"../../../../../Schema/CanvasAppsVehicleCP/unpackedapp/Src/Scr_vehicle.fx.yaml"));


            string json = File.ReadAllText(@"../../../../../Schema/CanvasAppsVehicleCP/unpackedapp/Connections/Connections.json");
            var j = JObject.Parse(json);

        }
    }
}
