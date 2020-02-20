using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_DE
{
    class DE_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\DE\20200207");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> sub1 = null; // 
            List<XElement> sub3 = null; // 
            List<XElement> sub6 = null; // Zurücknahmen
            List<XElement> sub7 = null; // 


            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                /*TETML elements*/

                /*Applications Terminated before Publication under Section 16(1)*/
                sub1 = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => !e.Value.StartsWith("Änderung in der Person, Im Namen oder im") && !e.Value.EndsWith("Wohnort des Vertreters"))
                    .TakeWhile(e => !e.Value.StartsWith("2h) Zurücknahmen, Zurückweisungen") && !e.Value.EndsWith("und sonstige Erledigungen"))
                    .Where(e => e.Value != ""
                    && !e.Value.Contains("Änderung in der Person, Im Namen oder im")
                    && !e.Value.Contains("Wohnort des Vertreters")
                    && !e.Value.Contains("(21) DE-AKZ (43) OT")
                    && !e.Value.Contains("(71) Anmelder")
                    && !e.Value.Contains("Wohnort des Anmelders")
                    && !e.Value.Contains("(74) Vertreter")
                    )
                    .ToList();

                /*Applications Terminated after Publication under Section 16(1)*/
                sub3 = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => !e.Value.StartsWith("Änderung in der Person, im Namen oder im") && !e.Value.EndsWith("Wohnort des Patentinhabers"))
                    .TakeWhile(e => !e.Value.StartsWith("3h) Verzicht, Widerruf und sonstige") && !e.Value.EndsWith("Erledigungen"))
                    .Where(e => e.Value != ""
                    && !e.Value.Contains("Änderung in der Person, Im Namen oder im")
                    && !e.Value.Contains("Wohnort des Patentinhabers")
                    && !e.Value.Contains("(21) DE-AKZ (45) 1.PT")
                    && !e.Value.Contains("(73) Inhaber")
                    && !e.Value.Contains("Wohnort des Vertreters")
                    && !e.Value.Contains("(74) Vertreters")
                    )
                    .ToList();

                sub6 = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => e.Value != "Zurückweisungen")
                    .TakeWhile(e => !e.Value.StartsWith("Sonstige Erledigungen:") && !e.Value.EndsWith("Nichtzahlung der Anmeldegebühr"))
                    .Where(e => e.Value != ""
                    && !e.Value.Contains("Zurückweisungen")
                    && !e.Value.Contains("(51) IPC-HKL")
                    && !e.Value.Contains("(21) DE-AKZ (43) OT")
                    )
                    .ToList();

                sub7 = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => !e.Value.StartsWith("Für folgende Gebrauchsmuster ist die") && !e.Value.EndsWith("Schutzdauer auf 6 Jahre verlängert worden"))
                    .TakeWhile(e => !e.Value.StartsWith("4e) Löschungs- und") && !e.Value.EndsWith("Feststellungsverfahren"))
                    .Where(e => e.Value != ""
                    && !e.Value.Contains("Für folgende Gebrauchsmuster ist die")
                    && !e.Value.Contains("(51) IPC-HKL")
                    && !e.Value.Contains("(21) DE-AKZ (45) BT")
                    )
                    .ToList();

                Console.WriteLine($"Sub 1 elements count: {sub1.Count}");
                Console.WriteLine($"Sub 3 elements count: {sub3.Count}");
                Console.WriteLine($"Sub 6 elements count: {sub6.Count}");
                Console.WriteLine($"Sub 7 elements count: {sub7.Count}");
                Console.WriteLine("Lists of elements are prepared");

                /*Applications Terminated before Publication under Section 16(1)*/
                if (sub1 != null && sub1.Count() > 0)
                {
                    Process.Sub1.Run(sub1);
                }
                /*Applications Terminated after Publication under Section 16(1)*/
                if (sub3 != null && sub3.Count() > 0)
                {
                    Process.Sub3.Run(sub3);
                }

                if (sub6 != null && sub6.Count() > 0)
                {
                    Process.Sub6.Run(sub6);
                }
                if (sub7 != null && sub7.Count() > 0)
                {
                    //Process.Sub7.Run(sub7);
                }
            }
        }
    }
}
