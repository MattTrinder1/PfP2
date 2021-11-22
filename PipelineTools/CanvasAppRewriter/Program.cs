﻿using Newtonsoft.Json.Linq;
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

            Console.WriteLine("Rewriting Vehicle App NDI PNC API Connections.json");

            //[JSON].
            j["01fea641-8b1b-4cec-9cba-f87f7ad1d4e8"].Parent.Remove();
            string replacement = File.ReadAllText(@"../../../Cumbria/SystemTest/VehicleTickets/NDIPNCAPIConnection.json");
            var replacementJ = JObject.Parse(replacement);
            j["5b186da5-eb85-4c38-9ea0-bab7f14c7eaa"] = new JObject(replacementJ);

            j["f891a58d-bb9a-4e98-87da-f5ab5c8dfbda"].Parent.Remove();
            replacement = File.ReadAllText(@"../../../Cumbria/SystemTest/VehicleTickets/PfPVehicleTickets.json");
            replacementJ = JObject.Parse(replacement);
            j["107cd446-f700-4759-ac44-ca0e698943ee"] = new JObject(replacementJ);


            Save("../../../RewrittenCanvasApps/Cumbria/SystemTest/Schema/CanvasAppsVehicleCP/unpackedapp/Connections/Connections.json", j.ToString());

            string json2 = File.ReadAllText(@"../../../../../Schema/CanvasAppsVehicleCP/unpackedapp/DataSources/NDIPNCAPI.json");
            var j2 = JArray.Parse(json2);

            j2[0]["ApiId"] = "/providers/microsoft.powerapps/apis/shared_ndi-20pnc-20api-5f65d572cd5285e38f-5fec37e65b69cd854d";

            Save("../../../RewrittenCanvasApps/Cumbria/SystemTest/Schema/CanvasAppsVehicleCP/unpackedapp/DataSources/NDIPNCAPI.json", j2.ToString());

            Console.WriteLine("Rewriting Vehicle App NDI PNC API NDIPNCAPI.json");


            var xml = File.ReadAllText(@"../../../../../Schema/CanvasAppsVehicleCP/unpackedapp/pkgs/wadl/ndipncapi.xml");
            xml = xml.Replace("5fec56f3da3c70054e-5fec37e65b69cd854d", "5f65d572cd5285e38f-5fec37e65b69cd854d");
            Save("../../../RewrittenCanvasApps/Cumbria/SystemTest/Schema/CanvasAppsVehicleCP/unpackedapp/pkgs/wadl/NDIPNCAPI.xml", xml);

            var xml2 = File.ReadAllText(@"../../../../../Schema/CanvasAppsVehicleCP/unpacked/CanvasApps/cp_vehicletickets_8a92b.meta.xml");
            xml2 = xml2.Replace("5fec56f3da3c70054e-5fec37e65b69cd854d", "5f65d572cd5285e38f-5fec37e65b69cd854d");
            xml2 = xml2.Replace("01fea641-8b1b-4cec-9cba-f87f7ad1d4e8", "5b186da5-eb85-4c38-9ea0-bab7f14c7eaa");
            Save("../../../RewrittenCanvasApps/Cumbria/SystemTest/Schema/CanvasAppsVehicleCP/unpacked/CanvasApps/cp_vehicletickets_8a92b.meta.xml", xml2);

            Console.WriteLine("Rewriting Vehicle App NDI PNC API NDIPNCAPI.xml");


            //107cd446-f700-4759-ac44-ca0e698943ee

        }

        private static void Save(string filePath, string contents)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(filePath);
            file.Directory.Create();
            File.WriteAllText(filePath, contents);

        }
    }
}
