using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_EP
{
    class EP_LE_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\AP\20191121\RenewAppPat10");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> sub1 = null; // • Alterations and Corrections Applications
            List<XElement> sub2 = null; // • Alterations and Corrections 


            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                /*TETML elements*/
                /*
                 Программа не готова! инструкция не готова!
                 поэтому пришлось отложить, жду разъяснений от Наташи                 
                 */
                /*1 subcode*/
                sub1 = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(x => !x.Value.Contains("• Applicant (transfer of rights)"))
                    .SkipWhile(x => !x.Value.StartsWith("(11)"))
                    .Where(e => e.Value != "")
                    .ToList();

                /*2 subcode*/
                sub2 = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .Where(e => e.Value != "")
                    .ToList();

                Console.WriteLine($"Sub 1 elements count: {sub1.Count}");
                Console.WriteLine($"Sub 2 elements count: {sub2.Count}");
                Console.WriteLine("Lists of elements are prepared");

                if (sub1 != null && sub1.Count() > 0)
                {
                    Process.Sub10.Run(sub1);
                }
                if (sub2 != null && sub2.Count() > 0)
                {
                    //Process.Sub7.Run(sub7);
                }
            }
        }
    }
}
