using Newtonsoft.Json.Linq;
using System;
using System.Dynamic;
using System.IO;

namespace CanvasAppRewriter
{
    class Program
    {
        static void Main(string[] args)
        {


            string json = File.ReadAllText(@"../../../../../Schema/CanvasAppsVehicleCP/unpackedapp/Connections/Connections.json");
            var j = JObject.Parse(json);
            //Save("../../../RewrittenCanvasApps/Cumbria/UAT/Schema/CanvasAppsVehicleCP/unpackedapp/Connections/Connections.json", j.ToString());

            Console.WriteLine("Rewriting Vehicle App NDI PNC API Connection");

            //[JSON].
            j["01fea641-8b1b-4cec-9cba-f87f7ad1d4e8"].Parent.Remove();
            string replacement = File.ReadAllText(@"../../../Cumbria/SystemTest/VehicleTickets/NDIPNCAPIConnection.json");
            var replacementJ = JObject.Parse(replacement);
            //j.AddAfterSelf(replacementJ.ToString()) ;
            j["13bff99a-3b95-4c86-8cd3-8c5e717d3a74"] = new JObject(replacementJ);

            Save("../../../RewrittenCanvasApps/Cumbria/SystemTest/Schema/CanvasAppsVehicleCP/unpackedapp/Connections/Connections.json", j.ToString());

            Console.WriteLine("Rewriting Vehicle App NDI PNC API Connection");


        }

        private static void Save(string filePath, string contents)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(filePath);
            file.Directory.Create();
            File.WriteAllText(filePath, contents);

        }
    }
}
