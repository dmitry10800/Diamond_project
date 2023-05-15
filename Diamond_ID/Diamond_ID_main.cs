using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_ID
{
    class Diamond_ID_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"E:\Work\ID\Original");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (var file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> firstList = null;

            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                //currentElement = null;

                /*TETML elements*/

                firstList = tet.Descendants().Where(d => d.Name.LocalName == "Text")
                    .SkipWhile(e => !Regex.IsMatch(e.Value, @"\(20\)\s*RI\s*Permohonan\s*Paten"))
                    .ToList();

                Console.WriteLine("lal");

                if (firstList != null && firstList.Count() > 0)
                {
                    var grantedEP = new ProcessFirstList();
                    var el = grantedEP.OutputValue(firstList);
                    var legalStatusEvents = ConvertToDiamond.FirstListConvertation(el);
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
