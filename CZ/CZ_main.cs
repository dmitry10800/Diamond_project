using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CZ
{
    class CZ_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\CZ\Test");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> sub6 = null; // Zurücknahmen


            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                /*TETML elements*/


                sub6 = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => e.Value != "MM4A Zánik evropských patentů nezaplacením poplatků za udržování platnosti patentu")
                    .TakeWhile(e => !e.Value.StartsWith("PD4A Převod práv a ostatní změny") && !e.Value.StartsWith("FG1K Zapsané užitné vzory"))
                    .Where(e => e.Value != ""
                                && !e.Value.Contains("Následující evropské patenty udělené Evropským") && !e.Value.Contains("MM4A Zánik evropských"))
                    .ToList();

                Console.WriteLine($"Sub 3 elements count: {sub6.Count}");
                Console.WriteLine("Lists of elements are prepared");

                if (sub6 != null && sub6.Count() > 0)
                {
                    Process.Sub6.Run(sub6);
                }
            }
        }
    }
}
