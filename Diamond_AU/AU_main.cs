using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_AU
{
    class AU_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\AU\20200206");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> firstList = null; // APPLICATIONS WITHDRAWN, REFUSED, TAKEN TO BE ABANDONED


            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                /*TETML elements*/

                firstList = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => e.Value != "Assignments Registered")
                    .TakeWhile(e => e.Value != "Extensions of Term of Standard Patents" && e.Value != "Mortgages Registered - Section 187, Reg. 19")
                    .Where(e => e.Value != ""
                    && e.Value != "Assignments Registered"
                    && e.Value != "Extensions of Term of Standard Patents"
                    && e.Value != "Mortgages Registered - Section 187, Reg. 19"
                    )
                    .ToList();

                Console.WriteLine("Lists of elements are prepared");

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
