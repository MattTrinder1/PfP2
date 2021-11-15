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

            Console.WriteLine("Rewriting Vehicle App NDIWrapper-Vehicle");

            //[JSON].
            j["15a9e74b-4a8e-4609-a8ed-b26f140fc6a2"].Parent.Remove();
            string replacement = File.ReadAllText(@"../../../NDIWrapperVehicle.json");
            var replacementJ = JObject.Parse(replacement);
            //j.AddAfterSelf(replacementJ.ToString()) ;
            j["1786d678-cd31-4cd5-8aec-981e5ff93093"] = new JObject(replacementJ);

            Save("../../../RewrittenCanvasApps/Cumbria/UAT/Schema/CanvasAppsVehicleCP/unpackedapp/Connections/Connections.json", j.ToString());

            Console.WriteLine("Rewriting Vehicle App NDIWrapper-Vehicle Done");


        }

        private static void Save(string filePath, string contents)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(filePath);
            file.Directory.Create();
            File.WriteAllText(filePath, contents);

        }
    }
}
