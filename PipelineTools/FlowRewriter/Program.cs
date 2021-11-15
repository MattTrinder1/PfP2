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
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["source"] = "me";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["drive"] = "b!k32VxZOE5U-HwWt6Ey23yGSuL7xVALpHlDcSd7uGMIo67u-9iOaWRrln1WJDTm3I";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["file"] = "01JPJUSCL4ROCWG4HJZ5BZCEGFUO2EHHKI";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["metadata"].AddAnnotation("01JPJUSCL4ROCWG4HJZ5BZCEGFUO2EHHKI");
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["metadata"]["01JPJUSCL4ROCWG4HJZ5BZCEGFUO2EHHKI"] = "/Templates/Covid/Covid Template.docx";
            Save("../../../RewrittenFlows/Durham/SystemTest/Schema/FlowsPDFCP/unpacked/Workflows/GenerateCovidPDFs-67CA08CD-5D8D-EB11-B1AC-000D3ADB469C.json", j.ToString().Replace("'", @"\u0027"));

            json = File.ReadAllText("../../../../../Schema/FlowsPDFCP/unpacked/Workflows/GenerateCovidPDFs-67CA08CD-5D8D-EB11-B1AC-000D3ADB469C.json");
            j = JObject.Parse(json);
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["source"] = "me";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["drive"] = "b!k32VxZOE5U-HwWt6Ey23yGSuL7xVALpHlDcSd7uGMIo67u-9iOaWRrln1WJDTm3I";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["file"] = "01JPJUSCL4ROCWG4HJZ5BZCEGFUO2EHHKI";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["metadata"].AddAnnotation("01JPJUSCL4ROCWG4HJZ5BZCEGFUO2EHHKI");
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["metadata"]["01JPJUSCL4ROCWG4HJZ5BZCEGFUO2EHHKI"] = "/Templates/Covid/Covid Template.docx";
            Save("../../../RewrittenFlows/Cumbria/UAT/Schema/FlowsPDFCP/unpacked/Workflows/GenerateCovidPDFs-67CA08CD-5D8D-EB11-B1AC-000D3ADB469C.json", j.ToString().Replace("'", @"\u0027"));

            json = File.ReadAllText("../../../../../Schema/FlowsPDFCP/unpacked/Workflows/GenerateCovidPDFs-67CA08CD-5D8D-EB11-B1AC-000D3ADB469C.json");
            j = JObject.Parse(json);
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["source"] = "me";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["drive"] = "b!k32VxZOE5U-HwWt6Ey23yGSuL7xVALpHlDcSd7uGMIo67u-9iOaWRrln1WJDTm3I";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["file"] = "01JPJUSCL4ROCWG4HJZ5BZCEGFUO2EHHKI";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["metadata"].AddAnnotation("01JPJUSCL4ROCWG4HJZ5BZCEGFUO2EHHKI");
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["metadata"]["01JPJUSCL4ROCWG4HJZ5BZCEGFUO2EHHKI"] = "/Templates/Covid/Covid Template.docx";
            Save("../../../RewrittenFlows/Cumbria/Live/Schema/FlowsPDFCP/unpacked/Workflows/GenerateCovidPDFs-67CA08CD-5D8D-EB11-B1AC-000D3ADB469C.json", j.ToString().Replace("'", @"\u0027"));

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

            Console.WriteLine("Rewriting Uof PDF FLow");


            Console.WriteLine("Rewriting UoF PDF FLow");
            json = File.ReadAllText("../../../../../Schema/FlowsPDFCP/unpacked/Workflows/GenerateUoFPDF-BDAE81A8-29EE-EB11-BACB-0022489C5E28.json");
            j = JObject.Parse(json);
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["source"] = "me";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["drive"] = "b!k32VxZOE5U-HwWt6Ey23yGSuL7xVALpHlDcSd7uGMIo67u-9iOaWRrln1WJDTm3I";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["file"] = "01JPJUSCPQL76JG7BTPVG2OGRLZTRSDHLU";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["metadata"].AddAnnotation("01JPJUSCPQL76JG7BTPVG2OGRLZTRSDHLU");
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["metadata"]["01JPJUSCPQL76JG7BTPVG2OGRLZTRSDHLU"] = "/Templates/UoF/Use of Force PDF Template.docx";
            Save("../../../RewrittenFlows/Cumbria/UAT/Schema/FlowsPDFCP/unpacked/Workflows/GenerateUoFPDF-BDAE81A8-29EE-EB11-BACB-0022489C5E28.json", j.ToString().Replace("'", @"\u0027"));

            json = File.ReadAllText("../../../../../Schema/FlowsPDFCP/unpacked/Workflows/GenerateUoFPDF-BDAE81A8-29EE-EB11-BACB-0022489C5E28.json");
            j = JObject.Parse(json);
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["source"] = "me";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["drive"] = "b!k32VxZOE5U-HwWt6Ey23yGSuL7xVALpHlDcSd7uGMIo67u-9iOaWRrln1WJDTm3I";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["file"] = "01JPJUSCPQL76JG7BTPVG2OGRLZTRSDHLU";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["metadata"].AddAnnotation("01JPJUSCPQL76JG7BTPVG2OGRLZTRSDHLU");
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["metadata"]["01JPJUSCPQL76JG7BTPVG2OGRLZTRSDHLU"] = "/Templates/UoF/Use of Force PDF Template.docx";
            Save("../../../RewrittenFlows/Cumbria/Live/Schema/FlowsPDFCP/unpacked/Workflows/GenerateUoFPDF-BDAE81A8-29EE-EB11-BACB-0022489C5E28.json", j.ToString().Replace("'", @"\u0027"));

            json = File.ReadAllText("../../../../../Schema/FlowsPDFCP/unpacked/Workflows/GenerateUoFPDF-BDAE81A8-29EE-EB11-BACB-0022489C5E28.json");
            j = JObject.Parse(json);
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["source"] = "sites/tisskilimited.sharepoint.com,5891e259-4b18-4e57-872a-76a7432bb0b6,0e904291-9378-4ff1-b183-6c7236bd194f";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["drive"] = "b!WeKRWBhLV06HKnanQyuwtpFCkA54k_FPsYNscja9GU-_YgpLYOi6T49L6ZbolPrM";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["file"] = "014YZTY7YS5AQQM355X5CKRB4JFXG6W562";
            Save("../../../RewrittenFlows/Durham/SystemTest/Schema/FlowsPDFCP/unpacked/Workflows/GenerateUoFPDF-BDAE81A8-29EE-EB11-BACB-0022489C5E28.json", j.ToString().Replace("'", @"\u0027"));

            json = File.ReadAllText("../../../../../Schema/FlowsPDFCP/unpacked/Workflows/GenerateUoFPDF-BDAE81A8-29EE-EB11-BACB-0022489C5E28.json");
            j = JObject.Parse(json);
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["source"] = "sites/tisskilimited.sharepoint.com,5891e259-4b18-4e57-872a-76a7432bb0b6,0e904291-9378-4ff1-b183-6c7236bd194f";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["drive"] = "b!WeKRWBhLV06HKnanQyuwtpFCkA54k_FPsYNscja9GU-_YgpLYOi6T49L6ZbolPrM";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["file"] = "014YZTY7YS5AQQM355X5CKRB4JFXG6W562";
            Save("../../../RewrittenFlows/Cumbria/SystemTest/Schema/FlowsPDFCP/unpacked/Workflows/GenerateUoFPDF-BDAE81A8-29EE-EB11-BACB-0022489C5E28.json", j.ToString().Replace("'", @"\u0027"));

            Console.WriteLine("Uof PDF FLow done");



            Console.WriteLine("Rewriting Sudden Death PDF FLow");
            json = File.ReadAllText("../../../../../Schema/FlowsPDFCP/unpacked/Workflows/GenerateSuddenDeathPDF-9DE7B140-61B2-EB11-8236-000D3AD7EB5D.json");
            j = JObject.Parse(json);
            //[JSON].properties.definition.actions.Condition_If_medical_History_exists.actions.Populate_a_Microsoft_Word_template_No_Medical_History
            j["properties"]["definition"]["actions"]["Condition_If_medical_History_exists"]["actions"]["Populate_a_Microsoft_Word_template_No_Medical_History"]["inputs"]["parameters"]["source"] = "me";
            j["properties"]["definition"]["actions"]["Condition_If_medical_History_exists"]["actions"]["Populate_a_Microsoft_Word_template_No_Medical_History"]["inputs"]["parameters"]["drive"] = "b!k32VxZOE5U-HwWt6Ey23yGSuL7xVALpHlDcSd7uGMIo67u-9iOaWRrln1WJDTm3I";
            j["properties"]["definition"]["actions"]["Condition_If_medical_History_exists"]["actions"]["Populate_a_Microsoft_Word_template_No_Medical_History"]["inputs"]["parameters"]["file"] = "01JPJUSCOUQT5V4QTEGRE333HS2XKJWJ4J";
            j["properties"]["definition"]["actions"]["Condition_If_medical_History_exists"]["actions"]["Populate_a_Microsoft_Word_template_No_Medical_History"]["metadata"].AddAnnotation("01JPJUSCOUQT5V4QTEGRE333HS2XKJWJ4J");
            j["properties"]["definition"]["actions"]["Condition_If_medical_History_exists"]["actions"]["Populate_a_Microsoft_Word_template_No_Medical_History"]["metadata"]["01JPJUSCOUQT5V4QTEGRE333HS2XKJWJ4J"] = "/Templates/Sudden Death/SuddenDeathTemplate.docx";
            Save("../../../RewrittenFlows/Cumbria/UAT/Schema/FlowsPDFCP/unpacked/Workflows/GenerateSuddenDeathPDF-9DE7B140-61B2-EB11-8236-000D3AD7EB5D.json", j.ToString().Replace("'", @"\u0027"));

            json = File.ReadAllText("../../../../../Schema/FlowsPDFCP/unpacked/Workflows/GenerateSuddenDeathPDF-9DE7B140-61B2-EB11-8236-000D3AD7EB5D.json");
            j = JObject.Parse(json);
            //[JSON].properties.definition.actions.Condition_If_medical_History_exists.actions.Populate_a_Microsoft_Word_template_No_Medical_History
            j["properties"]["definition"]["actions"]["Condition_If_medical_History_exists"]["actions"]["Populate_a_Microsoft_Word_template_No_Medical_History"]["inputs"]["parameters"]["source"] = "me";
            j["properties"]["definition"]["actions"]["Condition_If_medical_History_exists"]["actions"]["Populate_a_Microsoft_Word_template_No_Medical_History"]["inputs"]["parameters"]["drive"] = "b!k32VxZOE5U-HwWt6Ey23yGSuL7xVALpHlDcSd7uGMIo67u-9iOaWRrln1WJDTm3I";
            j["properties"]["definition"]["actions"]["Condition_If_medical_History_exists"]["actions"]["Populate_a_Microsoft_Word_template_No_Medical_History"]["inputs"]["parameters"]["file"] = "01JPJUSCOUQT5V4QTEGRE333HS2XKJWJ4J";
            j["properties"]["definition"]["actions"]["Condition_If_medical_History_exists"]["actions"]["Populate_a_Microsoft_Word_template_No_Medical_History"]["metadata"].AddAnnotation("01JPJUSCOUQT5V4QTEGRE333HS2XKJWJ4J");
            j["properties"]["definition"]["actions"]["Condition_If_medical_History_exists"]["actions"]["Populate_a_Microsoft_Word_template_No_Medical_History"]["metadata"]["01JPJUSCOUQT5V4QTEGRE333HS2XKJWJ4J"] = "/Templates/Sudden Death/SuddenDeathTemplate.docx";
            Save("../../../RewrittenFlows/Cumbria/Live/Schema/FlowsPDFCP/unpacked/Workflows/GenerateSuddenDeathPDF-9DE7B140-61B2-EB11-8236-000D3AD7EB5D.json", j.ToString().Replace("'", @"\u0027"));

            json = File.ReadAllText("../../../../../Schema/FlowsPDFCP/unpacked/Workflows/GenerateSuddenDeathPDF-9DE7B140-61B2-EB11-8236-000D3AD7EB5D.json");
            j = JObject.Parse(json);
            j["properties"]["definition"]["actions"]["Condition_If_medical_History_exists"]["actions"]["Populate_a_Microsoft_Word_template_No_Medical_History"]["inputs"]["parameters"]["source"] = "sites/tisskilimited.sharepoint.com,5891e259-4b18-4e57-872a-76a7432bb0b6,0e904291-9378-4ff1-b183-6c7236bd194f";
            j["properties"]["definition"]["actions"]["Condition_If_medical_History_exists"]["actions"]["Populate_a_Microsoft_Word_template_No_Medical_History"]["inputs"]["parameters"]["drive"] = "b!WeKRWBhLV06HKnanQyuwtpFCkA54k_FPsYNscja9GU-_YgpLYOi6T49L6ZbolPrM";
            j["properties"]["definition"]["actions"]["Condition_If_medical_History_exists"]["actions"]["Populate_a_Microsoft_Word_template_No_Medical_History"]["inputs"]["parameters"]["file"] = "014YZTY732SDRV4Q7ILJE3MBVGNPRGDHCY";
            Save("../../../RewrittenFlows/Durham/SystemTest/Schema/FlowsPDFCP/unpacked/Workflows/GenerateSuddenDeathPDF-9DE7B140-61B2-EB11-8236-000D3AD7EB5D.json", j.ToString().Replace("'", @"\u0027"));

            json = File.ReadAllText("../../../../../Schema/FlowsPDFCP/unpacked/Workflows/GenerateSuddenDeathPDF-9DE7B140-61B2-EB11-8236-000D3AD7EB5D.json");
            j = JObject.Parse(json);
            j["properties"]["definition"]["actions"]["Condition_If_medical_History_exists"]["actions"]["Populate_a_Microsoft_Word_template_No_Medical_History"]["inputs"]["parameters"]["source"] = "sites/tisskilimited.sharepoint.com,5891e259-4b18-4e57-872a-76a7432bb0b6,0e904291-9378-4ff1-b183-6c7236bd194f";
            j["properties"]["definition"]["actions"]["Condition_If_medical_History_exists"]["actions"]["Populate_a_Microsoft_Word_template_No_Medical_History"]["inputs"]["parameters"]["drive"] = "b!WeKRWBhLV06HKnanQyuwtpFCkA54k_FPsYNscja9GU-_YgpLYOi6T49L6ZbolPrM";
            j["properties"]["definition"]["actions"]["Condition_If_medical_History_exists"]["actions"]["Populate_a_Microsoft_Word_template_No_Medical_History"]["inputs"]["parameters"]["file"] = "014YZTY732SDRV4Q7ILJE3MBVGNPRGDHCY";
            Save("../../../RewrittenFlows/Cumbria/SystemTest/Schema/FlowsPDFCP/unpacked/Workflows/GenerateSuddenDeathPDF-9DE7B140-61B2-EB11-8236-000D3AD7EB5D.json", j.ToString().Replace("'", @"\u0027"));

            Console.WriteLine("Sudden Death PDF FLow done");

            Console.WriteLine("Rewriting Vehicle PDF FLow");
            json = File.ReadAllText("../../../../../Schema/FlowsPDFCP/unpacked/Workflows/GenerateVehicleTicketPDF-FE26A830-AEC3-EB11-BACC-000D3AB9F7B1.json");
            j = JObject.Parse(json);
            //[JSON].properties.definition.actions.Populate_a_Microsoft_Word_template.inputs.parameters
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["source"] = "me";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["drive"] = "b!k32VxZOE5U-HwWt6Ey23yGSuL7xVALpHlDcSd7uGMIo67u-9iOaWRrln1WJDTm3I";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["file"] = "01JPJUSCO5DQDVH5PUWJGJQMOYJFBOQ5BN";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["metadata"].AddAnnotation("01JPJUSCO5DQDVH5PUWJGJQMOYJFBOQ5BN");
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["metadata"]["01JPJUSCO5DQDVH5PUWJGJQMOYJFBOQ5BN"] = "/Templates/Vehicle/Vehicle PDF.docx";
            Save("../../../RewrittenFlows/Cumbria/UAT/Schema/FlowsPDFCP/unpacked/Workflows/GenerateVehicleTicketPDF-FE26A830-AEC3-EB11-BACC-000D3AB9F7B1.json", j.ToString().Replace("'", @"\u0027"));

            json = File.ReadAllText("../../../../../Schema/FlowsPDFCP/unpacked/Workflows/GenerateVehicleTicketPDF-FE26A830-AEC3-EB11-BACC-000D3AB9F7B1.json");
            j = JObject.Parse(json);
            //[JSON].properties.definition.actions.Populate_a_Microsoft_Word_template.inputs.parameters
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["source"] = "me";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["drive"] = "b!k32VxZOE5U-HwWt6Ey23yGSuL7xVALpHlDcSd7uGMIo67u-9iOaWRrln1WJDTm3I";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["file"] = "01JPJUSCO5DQDVH5PUWJGJQMOYJFBOQ5BN";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["metadata"].AddAnnotation("01JPJUSCO5DQDVH5PUWJGJQMOYJFBOQ5BN");
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["metadata"]["01JPJUSCO5DQDVH5PUWJGJQMOYJFBOQ5BN"] = "/Templates/Vehicle/Vehicle PDF.docx";
            Save("../../../RewrittenFlows/Cumbria/Live/Schema/FlowsPDFCP/unpacked/Workflows/GenerateVehicleTicketPDF-FE26A830-AEC3-EB11-BACC-000D3AB9F7B1.json", j.ToString().Replace("'", @"\u0027"));

            json = File.ReadAllText("../../../../../Schema/FlowsPDFCP/unpacked/Workflows/GenerateVehicleTicketPDF-FE26A830-AEC3-EB11-BACC-000D3AB9F7B1.json");
            j = JObject.Parse(json);
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["source"] = "sites/tisskilimited.sharepoint.com,5891e259-4b18-4e57-872a-76a7432bb0b6,0e904291-9378-4ff1-b183-6c7236bd194f";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["drive"] = "b!WeKRWBhLV06HKnanQyuwtpFCkA54k_FPsYNscja9GU-_YgpLYOi6T49L6ZbolPrM";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["file"] = "014YZTY74K622O3HSPCFC3MFOZYXXXDLEG";
            Save("../../../RewrittenFlows/Durham/SystemTest/Schema/FlowsPDFCP/unpacked/Workflows/GenerateVehicleTicketPDF-FE26A830-AEC3-EB11-BACC-000D3AB9F7B1.json", j.ToString().Replace("'", @"\u0027"));

            json = File.ReadAllText("../../../../../Schema/FlowsPDFCP/unpacked/Workflows/GenerateVehicleTicketPDF-FE26A830-AEC3-EB11-BACC-000D3AB9F7B1.json");
            j = JObject.Parse(json);
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["source"] = "sites/tisskilimited.sharepoint.com,5891e259-4b18-4e57-872a-76a7432bb0b6,0e904291-9378-4ff1-b183-6c7236bd194f";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["drive"] = "b!WeKRWBhLV06HKnanQyuwtpFCkA54k_FPsYNscja9GU-_YgpLYOi6T49L6ZbolPrM";
            j["properties"]["definition"]["actions"]["Populate_a_Microsoft_Word_template"]["inputs"]["parameters"]["file"] = "014YZTY74K622O3HSPCFC3MFOZYXXXDLEG";
            Save("../../../RewrittenFlows/Cumbria/SystemTest/Schema/FlowsPDFCP/unpacked/Workflows/GenerateVehicleTicketPDF-FE26A830-AEC3-EB11-BACC-000D3AB9F7B1.json", j.ToString().Replace("'", @"\u0027"));

            Console.WriteLine("Vehicle PDF FLow done");


        }




        private static void Save(string filePath, string contents)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(filePath);
            file.Directory.Create();
            File.WriteAllText(filePath, contents);

        }

    }
}
