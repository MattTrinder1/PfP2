using CanvasAppRewriter;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace CustomConnectorRewriter
{
    class Program
    {
        static string defaultLevel = "../../..";

        static void Main(string[] args)
        {

            var files = new List<FileToModify>();


            var pnbGetFile = new FileToModify("../../Schema/CustomConnectorsCP/unpacked/Connectors/cp_5Fpnb-20get_openapidefinition.json", "RewrittenConnectors/Cumbria/SystemTest/Schema/CustomConnectorsCP/unpacked/Connectors/cp_5Fpnb-20get_openapidefinition.json", false, true);
            pnbGetFile.ReplaceStrings.Add(new ReplaceString("pfpdev-ab46c5b7b4fa-apiwebapp.azurewebsites.net", "cumbriasystest-bc51-apiwebapp.azurewebsites.net"));
            pnbGetFile.ReplaceStrings.Add(new ReplaceString("35C87175-6BF7-EB11-94EF-0022489CC1DC", "systestguid"));
            files.Add(pnbGetFile);

            var pnbGetFile2 = new FileToModify("../../Schema/CustomConnectorsCP/unpacked/Connectors/cp_5Fpnb-20get_openapidefinition.json", "RewrittenConnectors/PfP/SystemTest/Schema/CustomConnectorsCP/unpacked/Connectors/cp_5Fpnb-20get_openapidefinition.json", false, true);
            pnbGetFile2.ReplaceStrings.Add(new ReplaceString("pfpdev-ab46c5b7b4fa-apiwebapp.azurewebsites.net", "pfpsystest-ec70-apiwebapp.azurewebsites.net"));
            pnbGetFile2.ReplaceStrings.Add(new ReplaceString("35C87175-6BF7-EB11-94EF-0022489CC1DC", "systestguid"));
            files.Add(pnbGetFile2);

            var pnbGetFile3 = new FileToModify("../../Schema/CustomConnectorsCP/unpacked/Connectors/cp_5Fpnb-20get_openapidefinition.json", "RewrittenConnectors/PfP/UAT/Schema/CustomConnectorsCP/unpacked/Connectors/cp_5Fpnb-20get_openapidefinition.json", false, true);
            pnbGetFile3.ReplaceStrings.Add(new ReplaceString("pfpdev-ab46c5b7b4fa-apiwebapp.azurewebsites.net", "pfpuat-8975-apiwebapp.azurewebsites.net"));
            pnbGetFile3.ReplaceStrings.Add(new ReplaceString("35C87175-6BF7-EB11-94EF-0022489CC1DC", "systestguid"));
            files.Add(pnbGetFile3);

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

        private static void Save(string filePath, string contents)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(Path.Combine(defaultLevel, filePath));
            file.Directory.Create();
            File.WriteAllText(Path.Combine(defaultLevel, filePath), contents);

        }
    }

}
