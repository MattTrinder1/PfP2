using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;

namespace CanvasAppRewriter
{


    class Program
    {
        static string defaultLevel = "../../..";

        public static string GetFileContent(string fileName)
        {
            return File.ReadAllText(Path.Combine(defaultLevel, fileName));
        }
        public static JObject GetJsonFileObject(string fileName)
        {
            return JObject.Parse(GetFileContent(fileName));
        }
        public static JArray GetJsonFileArray(string fileName)
        {
            return JArray.Parse(GetFileContent(fileName));
        }


        static void Main(string[] args)
        {
            var files = new List<FileToModify>();

            Console.WriteLine("Setting Up Files");

            //Cumbria System Test

            var connectionFile = new FileToModify("../../Schema/CanvasAppsVehicleCP/unpackedapp/Connections/Connections.json", "RewrittenCanvasApps/Cumbria/SystemTest/Schema/CanvasAppsVehicleCP/unpackedapp/Connections/Connections.json");
            connectionFile.ReplaceFiles.Add(new ReplaceFile(Guid.Parse("01fea641-8b1b-4cec-9cba-f87f7ad1d4e8"), Guid.Parse("5b186da5-eb85-4c38-9ea0-bab7f14c7eaa"), "Cumbria/SystemTest/VehicleTickets/NDIPNCAPIConnection.json"));
            connectionFile.ReplaceFiles.Add(new ReplaceFile(Guid.Parse("f891a58d-bb9a-4e98-87da-f5ab5c8dfbda"), Guid.Parse("107cd446-f700-4759-ac44-ca0e698943ee"), "Cumbria/SystemTest/VehicleTickets/PfPVehicleTickets.json"));
            files.Add(connectionFile);


            var ndiAPIFile = new FileToModify("../../Schema/CanvasAppsVehicleCP/unpackedapp/DataSources/NDIPNCAPI.json", "RewrittenCanvasApps/Cumbria/SystemTest/Schema/CanvasAppsVehicleCP/unpackedapp/DataSources/NDIPNCAPI.json", true);
            ndiAPIFile.ReplaceInJsons.Add(new ReplaceInJson(0, "ApiId", "/providers/microsoft.powerapps/apis/shared_ndi-20pnc-20api-5f65d572cd5285e38f-5fec37e65b69cd854d"));
            files.Add(ndiAPIFile);

            var policeAPIFile = new FileToModify("../../Schema/CanvasAppsVehicleCP/unpackedapp/DataSources/PoliceAPI.json", "RewrittenCanvasApps/Cumbria/SystemTest/Schema/CanvasAppsVehicleCP/unpackedapp/DataSources/PoliceAPI.json", true);
            policeAPIFile.ReplaceInJsons.Add(new ReplaceInJson(0, "ApiId", "/providers/microsoft.powerapps/apis/shared_pfp-20vehicle-20tickets-5f65d572cd5285e38f-5ff0f85a3e2578d081"));
            files.Add(policeAPIFile);

            var ndiAPIXML = new FileToModify("../../Schema/CanvasAppsVehicleCP/unpackedapp/pkgs/wadl/ndipncapi.xml", "RewrittenCanvasApps/Cumbria/SystemTest/Schema/CanvasAppsVehicleCP/unpackedapp/pkgs/wadl/NDIPNCAPI.xml", false, true);
            ndiAPIXML.ReplaceStrings.Add(new ReplaceString("5fec56f3da3c70054e-5fec37e65b69cd854d", "5f65d572cd5285e38f-5fec37e65b69cd854d"));
            files.Add(ndiAPIXML);

            var policeAPIXML = new FileToModify("../../Schema/CanvasAppsVehicleCP/unpackedapp/pkgs/wadl/policeapi.xml", "RewrittenCanvasApps/Cumbria/SystemTest/Schema/CanvasAppsVehicleCP/unpackedapp/pkgs/wadl/policeAPI.xml", false, true);
            policeAPIXML.ReplaceStrings.Add(new ReplaceString("5fec56f3da3c70054e-5f68dcef26c4a019c7", "5f65d572cd5285e38f-5ff0f85a3e2578d081"));
            files.Add(policeAPIXML);

            var meta = new FileToModify("../../Schema/CanvasAppsVehicleCP/unpacked/CanvasApps/cp_vehicletickets_8a92b.meta.xml", "RewrittenCanvasApps/Cumbria/SystemTest/Schema/CanvasAppsVehicleCP/unpacked/CanvasApps/cp_vehicletickets_8a92b.meta.xml", false, true);
            meta.ReplaceStrings.Add(new ReplaceString("5fec56f3da3c70054e-5fec37e65b69cd854d", "5f65d572cd5285e38f-5fec37e65b69cd854d"));
            meta.ReplaceStrings.Add(new ReplaceString("01fea641-8b1b-4cec-9cba-f87f7ad1d4e8", "5b186da5-eb85-4c38-9ea0-bab7f14c7eaa"));
            meta.ReplaceStrings.Add(new ReplaceString("5fec56f3da3c70054e-5f68dcef26c4a019c7", "5f65d572cd5285e38f-5ff0f85a3e2578d081"));
            meta.ReplaceStrings.Add(new ReplaceString("f891a58d-bb9a-4e98-87da-f5ab5c8dfbda", "107cd446-f700-4759-ac44-ca0e698943ee"));
            files.Add(meta);

            //Durham System Test

            var connectionFile2 = new FileToModify("../../Schema/CanvasAppsVehicleCP/unpackedapp/Connections/Connections.json", "RewrittenCanvasApps/Durham/SystemTest/Schema/CanvasAppsVehicleCP/unpackedapp/Connections/Connections.json");
            connectionFile2.ReplaceFiles.Add(new ReplaceFile(Guid.Parse("01fea641-8b1b-4cec-9cba-f87f7ad1d4e8"), Guid.Parse("699f8a98-93dd-45bc-b982-0586ae890370"), "Durham/SystemTest/VehicleTickets/NDIPNCAPIConnection.json"));
            connectionFile2.ReplaceFiles.Add(new ReplaceFile(Guid.Parse("f891a58d-bb9a-4e98-87da-f5ab5c8dfbda"), Guid.Parse("8c2d5ee9-0c21-4803-b84f-81a17a934eca"), "Durham/SystemTest/VehicleTickets/PfPVehicleTickets.json"));
            files.Add(connectionFile2);


            var ndiAPIFile2 = new FileToModify("../../Schema/CanvasAppsVehicleCP/unpackedapp/DataSources/NDIPNCAPI.json", "RewrittenCanvasApps/Durham/SystemTest/Schema/CanvasAppsVehicleCP/unpackedapp/DataSources/NDIPNCAPI.json", true);
            ndiAPIFile2.ReplaceInJsons.Add(new ReplaceInJson(0, "ApiId", "/providers/microsoft.powerapps/apis/shared_ndi-20pnc-20api-5fe418214eeea4bce9-5fec37e65b69cd854d"));
            files.Add(ndiAPIFile2);

            var policeAPIFile2 = new FileToModify("../../Schema/CanvasAppsVehicleCP/unpackedapp/DataSources/PoliceAPI.json", "RewrittenCanvasApps/Durham/SystemTest/Schema/CanvasAppsVehicleCP/unpackedapp/DataSources/PoliceAPI.json", true);
            policeAPIFile2.ReplaceInJsons.Add(new ReplaceInJson(0, "ApiId", "/providers/microsoft.powerapps/apis/shared_pfp-20vehicle-20tickets-5fe418214eeea4bce9-5fec37e65b69cd854d"));
            files.Add(policeAPIFile2);

            var ndiAPIXML2 = new FileToModify("../../Schema/CanvasAppsVehicleCP/unpackedapp/pkgs/wadl/ndipncapi.xml", "RewrittenCanvasApps/Durham/SystemTest/Schema/CanvasAppsVehicleCP/unpackedapp/pkgs/wadl/NDIPNCAPI.xml", false, true);
            ndiAPIXML2.ReplaceStrings.Add(new ReplaceString("5fec56f3da3c70054e-5fec37e65b69cd854d", "5fe418214eeea4bce9-5fec37e65b69cd854d"));
            files.Add(ndiAPIXML2);

            var policeAPIXML2 = new FileToModify("../../Schema/CanvasAppsVehicleCP/unpackedapp/pkgs/wadl/policeapi.xml", "RewrittenCanvasApps/Durham/SystemTest/Schema/CanvasAppsVehicleCP/unpackedapp/pkgs/wadl/policeAPI.xml", false, true);
            policeAPIXML2.ReplaceStrings.Add(new ReplaceString("5fec56f3da3c70054e-5f68dcef26c4a019c7", "5fe418214eeea4bce9-5fec37e65b69cd854d"));
            files.Add(policeAPIXML2);

            var meta2 = new FileToModify("../../Schema/CanvasAppsVehicleCP/unpacked/CanvasApps/cp_vehicletickets_8a92b.meta.xml", "RewrittenCanvasApps/Durham/SystemTest/Schema/CanvasAppsVehicleCP/unpacked/CanvasApps/cp_vehicletickets_8a92b.meta.xml", false, true);
            meta2.ReplaceStrings.Add(new ReplaceString("5fec56f3da3c70054e-5fec37e65b69cd854d", "5fe418214eeea4bce9-5fec37e65b69cd854d"));
            meta2.ReplaceStrings.Add(new ReplaceString("01fea641-8b1b-4cec-9cba-f87f7ad1d4e8", "699f8a98-93dd-45bc-b982-0586ae890370"));
            meta2.ReplaceStrings.Add(new ReplaceString("5fec56f3da3c70054e-5f68dcef26c4a019c7", "5fe418214eeea4bce9-5fec37e65b69cd854d"));
            meta2.ReplaceStrings.Add(new ReplaceString("f891a58d-bb9a-4e98-87da-f5ab5c8dfbda", "8c2d5ee9-0c21-4803-b84f-81a17a934eca"));
            files.Add(meta2);


            foreach (var file in files)
            {

                Console.WriteLine(file.FileName);

                if (file.StringOnly)
                {
                    Console.WriteLine("\t replacing strings");
                    var contents = GetFileContent(file.FileName);
                    foreach (var replace in file.ReplaceStrings)
                    {
                        Console.WriteLine($"\t\t {replace.Find} : {replace.Replace} ");
                        contents = contents.Replace(replace.Find, replace.Replace);
                    }
                    Save(file.Destination, contents);
                }
                else
                {
                    JContainer connFileJ;
                    if (file.Array)
                    {
                        connFileJ = GetJsonFileArray(file.FileName);
                    }
                    else
                    {
                        connFileJ = GetJsonFileObject(file.FileName);
                    }


                    foreach (var replace in file.ReplaceFiles)
                    {
                        Console.WriteLine($"\t replacing json sections {replace.FindId.ToString().ToLower()} : {replace.ReplaceId.ToString().ToLower()}");

                        connFileJ[replace.FindId.ToString().ToLower()].Parent.Remove();
                        connFileJ[replace.ReplaceId.ToString().ToLower()] = GetJsonFileObject(replace.ReplaceFileName);
                    }

                    foreach (var replaceInJson in file.ReplaceInJsons)
                    {
                        Console.WriteLine($"\t replacing json text {replaceInJson.Index.ToString()} : {replaceInJson.Element} : {replaceInJson.NewValue}");
                        connFileJ[replaceInJson.Index][replaceInJson.Element] = replaceInJson.NewValue;
                    }

                    Save(file.Destination, connFileJ.ToString());
                }
            }

            Console.WriteLine("Complete");



        }

        private static void Save(string filePath, string contents)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(Path.Combine(defaultLevel, filePath));
            file.Directory.Create();
            File.WriteAllText(Path.Combine(defaultLevel, filePath), contents);

        }
    }
}
