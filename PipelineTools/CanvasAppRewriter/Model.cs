using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasAppRewriter
{
    class FileToModify
    {
        public FileToModify(string fileName, string destination, bool array = false, bool stringOnly = false)
        {
            FileName = fileName;
            Destination = destination;
            ReplaceFiles = new List<ReplaceFile>();
            ReplaceInJsons = new List<ReplaceInJson>();
            StringOnly = stringOnly;
            ReplaceStrings = new List<ReplaceString>();


            Array = array;
        }
        public string FileName { get; set; }
        public string Destination { get; set; }
        public List<ReplaceFile> ReplaceFiles { get; set; }
        public List<ReplaceInJson> ReplaceInJsons { get; set; }
        public List<ReplaceString> ReplaceStrings { get; set; }
        public bool Array { get; set; }
        public bool StringOnly { get; set; }
    }

    class ReplaceString
    {
        public ReplaceString(string find, string replace)
        {
            Find = find;
            Replace = replace;
        }
        public string Find { get; set; }
        public string Replace { get; set; }
    }

    class ReplaceFile
    {
        public ReplaceFile(Guid findId, Guid replaceId, string replaceFileName)
        {
            FindId = findId;
            ReplaceId = replaceId;
            ReplaceFileName = replaceFileName;
        }
        public Guid FindId { get; set; }
        public Guid ReplaceId { get; set; }
        public string ReplaceFileName { get; set; }
    }

    class ReplaceInJson
    {
        public ReplaceInJson(int index, string element, string newValue)
        {
            Index = index;
            Element = element;
            NewValue = newValue;
        }
        public int Index { get; set; }
        public string Element { get; set; }
        public string NewValue { get; set; }
    }
}
