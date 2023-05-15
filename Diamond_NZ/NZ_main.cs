using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_NZ
{
    class NZ_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\NZ\Test");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (var file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> firstList = null;


            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                /*TETML elements*/

                /*Subcode 2 - Patent Lapsed (MM)*/
                firstList = tet.Descendants().Where(d => d.Name.LocalName == "Text")
                    .Where(e => e.Value.Contains("Patent Lapsed:"))
                    .ToList();

                Console.WriteLine("Lists of elements are prepared");

                /*Subcode 2 - Patent Lapsed (MM)*/
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
