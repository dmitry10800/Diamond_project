using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_MK
{
    class Diamond_MK_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\MK\Full\");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> nationalPatents = null; // National patents

            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                //currentElement = null;

                /*TETML elements*/
                /*National Patents and International Patents*/
                nationalPatents = tet.Descendants().Where(d => d.Name.LocalName == "Text" && d.Value != "PATENTS")
                    .SkipWhile(e => !e.Value.StartsWith("(97) Објавување на признаен европски патент (датум на признавање, број на билтен)"))
                    .TakeWhile(e => !e.Value.StartsWith("ПРЕГЛЕДИ")
                    && !e.Value.StartsWith("ПРЕГЛЕД НА НАЦИОНАЛНИ ПАТЕНТИ СПОРЕД МЕЃУНАРОДНАТА"))
                    .Where(e => e.Value != "(97) Објавување на признаен европски патент (датум на признавање, број на билтен)")
                    .ToList();

                Console.WriteLine("lal");

                /*National Patents and International Patents*/
                if (nationalPatents != null && nationalPatents.Count() > 0)
                {
                    ProcessNatAndInternatPatents grantedEP = new ProcessNatAndInternatPatents();
                    List<ProcessNatAndInternatPatents.ElementOut> el = grantedEP.OutputValue(nationalPatents);
                    var legalStatusEvents = ConvertToDiamond.NationalAndInternationalPatentsConvertation(el);
                    try
                    {
                        Methods.SendToDiamond(legalStatusEvents);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Sending error");
                        throw;
                    }
                }
            }
        }
    }
}
