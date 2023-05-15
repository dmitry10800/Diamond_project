using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_AP_LE
{
    class AP_LE_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"C:\1WORK\AP\AP_20210731_07");
            //var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\AP\20191121\RenewAppPat10");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (var file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> sub10 = null; // Applications Terminated before Publication under Section 16(1)
            List<XElement> sub7 = null; // Applications Terminated after Publication under Section 16(1)


            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                /*TETML elements*/

                /*10 subcode*/
                sub10 = tet.Descendants().Where(d => d.Name.LocalName == "Text")
                    .Where(e => e.Value != "")
                    .ToList();

                /*7 subcode*/
                sub7 = tet.Descendants().Where(d => d.Name.LocalName == "Text")
                    .Where(e => e.Value != "")
                    .ToList();

                Console.WriteLine($"Sub 10 elements count: {sub10.Count}");
                Console.WriteLine($"Sub 10 elements count: {sub7.Count}");
                Console.WriteLine("Lists of elements are prepared");

                if (sub10 != null && sub10.Count() > 0)
                {
                   // Process.Sub10.Run(sub10);
                }
                if (sub7 != null && sub7.Count() > 0)
                {
                    Process.Sub7.Run(sub7);
                }
            }
        }
    }
}
