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
            Console.WriteLine("Rewriting Covid PDF FLow");

            string json = File.ReadAllText("../../../../../Schema/FlowsPDFCP/unpacked/Workflows/GenerateCovidPDFs-67CA08CD-5D8D-EB11-B1AC-000D3ADB469C.json");
            var j = JObject.Parse(json);
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["file"] = "014YZTY7YZKJ4I3PQSI5G24DBDZA2COZLE";
            File.WriteAllText("../../../RewrittenFlows/Cumbria/SystemTest/Schema/FlowsPDFCP/unpacked/Workflows/GenerateCovidPDFs-67CA08CD-5D8D-EB11-B1AC-000D3ADB469C.json", j.ToString().Replace("'", @"\u0027"));

            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["file"] = "wibble";
            File.WriteAllText("../../../RewrittenFlows/Durham/SystemTest/Schema/FlowsPDFCP/unpacked/Workflows/GenerateCovidPDFs-67CA08CD-5D8D-EB11-B1AC-000D3ADB469C.json", j.ToString().Replace("'", @"\u0027"));

            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["file"] = "wobble";
            File.WriteAllText("../../../RewrittenFlows/Cumbria/UAT/Schema/FlowsPDFCP/unpacked/Workflows/GenerateCovidPDFs-67CA08CD-5D8D-EB11-B1AC-000D3ADB469C.json", j.ToString().Replace("'", @"\u0027"));

            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["file"] = "jibble";
            File.WriteAllText("../../../RewrittenFlows/Durham/UAT/Schema/FlowsPDFCP/unpacked/Workflows/GenerateCovidPDFs-67CA08CD-5D8D-EB11-B1AC-000D3ADB469C.json", j.ToString().Replace("'", @"\u0027"));

            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["file"] = "areibfsdfdas";
            File.WriteAllText("../../../RewrittenFlows/Cumbria/Live/Schema/FlowsPDFCP/unpacked/Workflows/GenerateCovidPDFs-67CA08CD-5D8D-EB11-B1AC-000D3ADB469C.json", j.ToString().Replace("'", @"\u0027"));


            Console.WriteLine("Covid PDF FLow done");


        }
    }
}
