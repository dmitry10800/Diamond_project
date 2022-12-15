using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_US
{
    class US_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\US\Input");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> sub1 = null; // 


            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                /*TETML elements*/

                /*10 subcode*/
                sub1 = tet.Descendants().Where(d => d.Name.LocalName == "Text")
                    .SkipWhile(x => !x.Value.StartsWith("PATENTS WHICH EXPIRED"))
                    .TakeWhile(x => !x.Value.StartsWith("Top of Notices"))
                    .Where(e => e.Value != "")
                    .ToList();

                Console.WriteLine($"Sub 1 elements count: {sub1.Count}");
                Console.WriteLine("Lists of elements are prepared");

                if (sub1 != null && sub1.Count() > 0)
                {
                    Process.Sub1.Run(sub1);
                }
            }
        }
    }
}
