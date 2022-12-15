using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_NL
{
    class NL_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\NL\20200207");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> subCombo = null; // 13,17,18,19,21
            List<XElement> sub26 = null; // 26


            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                /*TETML elements*/

                subCombo = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => !e.Value.StartsWith("Verval of nietigverklaring."))
                    .TakeWhile(e => e.Value != "VRB")
                    .Where(e => e.Value != "" && !e.Value.Contains("Verval of nietigverklaring."))
                    .ToList();

                sub26 = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => !e.Value.StartsWith("Ingeschreven overdrachten"))
                    .TakeWhile(e => e.Value != "TVH")
                    .Where(e => e.Value != "" && !e.Value.Contains("Ingeschreven overdrachten"))
                    .ToList();

                Console.WriteLine($"SubCombo elements count: {subCombo.Count}");
                Console.WriteLine($"Sub26 elements count: {sub26.Count}");
                Console.WriteLine("Lists of elements are prepared");

                if (subCombo != null && subCombo.Count() > 0)
                {
                    Process.SubCombo.Run(subCombo);
                }
                if (sub26 != null && sub26.Count() > 0)
                {
                    Process.Sub26.Run(sub26);
                }
            }
        }
    }
}
