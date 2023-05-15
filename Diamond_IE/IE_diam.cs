using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_IE
{
    class IE_diam
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\IE\20191210");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (var file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> firstList = null; // APPLICATIONS WITHDRAWN, REFUSED, TAKEN TO BE ABANDONED


            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                /*TETML elements*/

                /*APPLICATIONS WITHDRAWN, REFUSED, TAKEN TO BE ABANDONED*/
                firstList = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => e.Value != "Patents Lapsed Through Non-Payment of Renewal Fees")
                    .TakeWhile(e => e.Value != "Patents Expired" && e.Value != "Supplementary Protection Certificates Granted")
                    .Where(e => e.Value != ""
                    && e.Value != "Patents Lapsed Through Non-Payment of Renewal Fees"
                    && e.Value != "Supplementary Protection Certificates Granted"
                    && e.Value != "Patents Expired"
                    )
                    .ToList();

                Console.WriteLine("Lists of elements are prepared");
                /*APPLICATIONS WITHDRAWN, REFUSED, TAKEN TO BE ABANDONED*/
                if (firstList != null && firstList.Count() > 0)
                {
                    var v = Process.FirstList.Run(firstList);
                    var legalStatusEvents = ConvertToDiamond.FirstList(v);
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
