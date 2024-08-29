using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_NG_Maksim
{

    class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> statusEvents = new();

            DirectoryInfo directory = new(path);

            List<string> files = new();

            foreach (var file in directory.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            List<XElement> xElements = new();

            foreach (var tetml in files)
            {
                _currentFileName = tetml;

                tet = XElement.Load(tetml);

                if (subCode == "1")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                         .SkipWhile(val => !val.Value.Contains("1. APLICATION"))
                         .TakeWhile(val => !val.Value.StartsWith("DESIGNS"))
                         .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?=Application|APLICATION\sNUMBER)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (var note in notes)
                    {
                       // statusEvents.Add(MakePatent(note, subCode, "AA"));
                    }
                }
            }

            return statusEvents;
        }


        internal string MakeText(List<XElement> xElements, string subCode)
        {
            string text = null;

            if(subCode == "1")
            {
                foreach (var xElement in xElements)
                {
                    text += xElement.Value + "\n";
                }
            }

            return text;
        }
    }
}
