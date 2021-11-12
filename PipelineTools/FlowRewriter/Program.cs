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
            Save("../../../RewrittenFlows/Cumbria/SystemTest/Schema/FlowsPDFCP/unpacked/Workflows/GenerateCovidPDFs-67CA08CD-5D8D-EB11-B1AC-000D3ADB469C.json", j.ToString().Replace("'", @"\u0027"));

            json = File.ReadAllText("../../../../../Schema/FlowsPDFCP/unpacked/Workflows/GenerateCovidPDFs-67CA08CD-5D8D-EB11-B1AC-000D3ADB469C.json");
            j = JObject.Parse(json);
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["file"] = "wibble";
            Save("../../../RewrittenFlows/Durham/SystemTest/Schema/FlowsPDFCP/unpacked/Workflows/GenerateCovidPDFs-67CA08CD-5D8D-EB11-B1AC-000D3ADB469C.json", j.ToString().Replace("'", @"\u0027"));

            json = File.ReadAllText("../../../../../Schema/FlowsPDFCP/unpacked/Workflows/GenerateCovidPDFs-67CA08CD-5D8D-EB11-B1AC-000D3ADB469C.json");
            j = JObject.Parse(json);
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["source"] = "me";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["drive"] = "b!k32VxZOE5U-HwWt6Ey23yGSuL7xVALpHlDcSd7uGMIo67u-9iOaWRrln1WJDTm3I";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["file"] = "01JPJUSCL4ROCWG4HJZ5BZCEGFUO2EHHKI";
            Save("../../../RewrittenFlows/Cumbria/UAT/Schema/FlowsPDFCP/unpacked/Workflows/GenerateCovidPDFs-67CA08CD-5D8D-EB11-B1AC-000D3ADB469C.json", j.ToString().Replace("'", @"\u0027"));

            json = File.ReadAllText("../../../../../Schema/FlowsPDFCP/unpacked/Workflows/GenerateCovidPDFs-67CA08CD-5D8D-EB11-B1AC-000D3ADB469C.json");
            j = JObject.Parse(json);
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["file"] = "jibble";
            Save("../../../RewrittenFlows/Durham/UAT/Schema/FlowsPDFCP/unpacked/Workflows/GenerateCovidPDFs-67CA08CD-5D8D-EB11-B1AC-000D3ADB469C.json", j.ToString().Replace("'", @"\u0027"));

            json = File.ReadAllText("../../../../../Schema/FlowsPDFCP/unpacked/Workflows/GenerateCovidPDFs-67CA08CD-5D8D-EB11-B1AC-000D3ADB469C.json");
            j = JObject.Parse(json);
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["file"] = "areibfsdfdas";
            Save("../../../RewrittenFlows/Cumbria/Live/Schema/FlowsPDFCP/unpacked/Workflows/GenerateCovidPDFs-67CA08CD-5D8D-EB11-B1AC-000D3ADB469C.json", j.ToString().Replace("'", @"\u0027"));

            json = File.ReadAllText("../../../../../Schema/FlowsPDFCP/unpacked/Workflows/GenerateCovidPDFs-67CA08CD-5D8D-EB11-B1AC-000D3ADB469C.json");
            j = JObject.Parse(json);
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["file"] = "areibfsdfdas";
            Save("../../../RewrittenFlows/Durham/Live/Schema/FlowsPDFCP/unpacked/Workflows/GenerateCovidPDFs-67CA08CD-5D8D-EB11-B1AC-000D3ADB469C.json", j.ToString().Replace("'", @"\u0027"));


            Console.WriteLine("Covid PDF FLow done");


            Console.WriteLine("Rewriting UoF PDF FLow");
            json = File.ReadAllText("../../../../../Schema/FlowsUseOfForceCP/unpacked/Workflows/GenerateUoFPDF-BDAE81A8-29EE-EB11-BACB-0022489C5E28.json");
            j = JObject.Parse(json);
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["source"] = "me";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["drive"] = "b!k32VxZOE5U-HwWt6Ey23yGSuL7xVALpHlDcSd7uGMIo67u-9iOaWRrln1WJDTm3I";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["file"] = "01JPJUSCPQL76JG7BTPVG2OGRLZTRSDHLU";
            Save("../../../RewrittenFlows/Cumbria/UAT/Schema/FlowsUseOfForce/unpacked/Workflows/GenerateUoFPDF-BDAE81A8-29EE-EB11-BACB-0022489C5E28.json", j.ToString().Replace("'", @"\u0027"));


        }




        private static void Save(string filePath, string contents)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(filePath);
            file.Directory.Create();
            File.WriteAllText(filePath, contents);

        }

    }
}
