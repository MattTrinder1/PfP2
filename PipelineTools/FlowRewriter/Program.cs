using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text.Json;

namespace FlowRewriter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string json = File.ReadAllText("../../../../../Schema/FlowsPDFCP/unpacked/Workflows/GenerateCovidPDFs-67CA08CD-5D8D-EB11-B1AC-000D3ADB469C.json");

            var j = JObject.Parse(json);

            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["file"] = "014YZTY7YZKJ4I3PQSI5G24DBDZA2COZLE";

            File.WriteAllText("../../../../../Schema/FlowsPDFCP/unpacked/Workflows/GenerateCovidPDFs-67CA08CD-5D8D-EB11-B1AC-000D3ADB469C.json",   j.ToString().Replace("'",@"\u0027"));
            


        }
    }
}
