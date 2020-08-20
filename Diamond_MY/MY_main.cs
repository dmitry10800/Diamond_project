using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_MY
{
    class MY_main
    {
        private static readonly string I11 = "(11)";
        private static readonly string I12 = "(12)";
        private static readonly string I21 = "(21)";
        private static readonly string I22 = "(22)";
        private static readonly string I30 = "(30)";
        private static readonly string I47 = "(47)";
        private static readonly string I51 = "(51)";
        private static readonly string I54 = "(54)";
        private static readonly string I56 = "(56)";
        private static readonly string I57 = "(57)";
        private static readonly string I72 = "(72)";
        private static readonly string I73 = "(73)";
        private static readonly string I74 = "(74)";


        public static string CurrentFileName;
        static void Main()
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"E:\Work\MY\");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> firstList = null;
            List<XElement> secondList = null;

            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                /*TETML elements*/

                /*List of Lapsed Patent Applications*/
                firstList = tet.Descendants().Where(d => d.Name.LocalName == "Text")
                    .Where(x => x.Value != I11
                    //&& x.Value != I12
                    && x.Value != I21
                    && x.Value != I22
                    && x.Value != I30
                    && x.Value != I47
                    && x.Value != I51
                    && x.Value != I54
                    && x.Value != I56
                    && x.Value != I57
                    && x.Value != I72
                    && x.Value != I73
                    && x.Value != I74)
                //.SkipWhile(e => e.Value != "Objava zahtjeva za proširenje evropskih prijava patenata")
                //.TakeWhile(e => e.Value != "Indeks brojeva zahtjeva za proširenje evropskih prijava patenta")
                .ToList();
                secondList = tet.Descendants().Where(x => x.Name.LocalName == "Text")
                    .SkipWhile(x => !x.Value.StartsWith("UTILITY INNOVATIONS HAVE LAPSED"))
                    .TakeWhile(x => !x.Value.StartsWith("REINSTATEMENT OF LAPSED PATENT"))
                    .ToList();

                Console.WriteLine("***************************" + Path.GetFileNameWithoutExtension(tetFile) + "***************************");

                if (firstList != null && firstList.Count() > 0)
                {
                    //ProcessGrants firstListValues = new ProcessGrants();
                    //List<ProcessGrants.ElementOut> el = firstListValues.OutputValue(firstList);
                    //var legalStatusEvents = ConvertToDiamond.FirstListConvertation(el);
                    //try
                    //{
                    //    Methods.SendToDiamond(legalStatusEvents);
                    //}
                    //catch (Exception)
                    //{
                    //    Console.WriteLine("Sending error");
                    //    throw;
                    //}
                }
                if (secondList != null && secondList.Count() > 0)
                {

                    var legalStatusEvents = ProcessLegalEvents.Process2SubCode(secondList);
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
