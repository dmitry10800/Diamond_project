using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_MX
{
    class MX_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\MX\20200210\2");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> subSecondThird = null;


            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                /*TETML elements*/

                subSecondThird = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => e.Value != "Transmisión de Derechos")
                    .TakeWhile(e => e.Value != "Licencias de Explotación")
                    .Where(e => e.Value != "" && e.Value != "Transmisión de Derechos")
                    .ToList();

                Console.WriteLine($"Sub 2 and 3 elements count: {subSecondThird.Count}");
                Console.WriteLine("Lists of elements are prepared");

                if (subSecondThird != null && subSecondThird.Count() > 0)
                {
                    Process.SubSecondThird.Run(subSecondThird);
                }
            }
        }
    }
}
