using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_FR
{
    class FR_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\FR\20200210");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (var file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> firstList = null; // First subcode
            List<XElement> secondList = null; // Seventh subcode
            List<XElement> thirdList = null; // Fourth subcode


            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                /*TETML elements*/

                /*Transmission totale de propriété // sub 1*/
                firstList = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => e.Value != "Transmission totale de propriété")
                    .TakeWhile(e => e.Value != "Transmission partielle de propriété"
                    && e.Value != "Concession de licence"
                    && e.Value != "Saisie notifiée"
                    && e.Value != "Assignation en revendication de propriété"
                    && e.Value != "Changement de nom, de dénomination").ToList();

                /*Radiation de gage // sub 7*/
                secondList = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => e.Value != "Radiation de gage")
                    .TakeWhile(e => e.Value != "Changement de nom, de dénomination"
                    && e.Value != "Changement de nom, de dénomination"
                    //&& e.Value != "Radiation de gage"
                    ).ToList();

                /*Changement de nom, de dénomination // sub 4*/
                thirdList = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => e.Value != "Changement de nom, de dénomination")
                    .TakeWhile(e => e.Value != "Changement de forme juridique"
                    && e.Value != "Changement de forme juridique"
                    //&& e.Value != "Radiation de gage"
                    ).ToList();

                Console.WriteLine("Lists of elements are prepared");
                /*Transmission totale de propriété // sub 1*/
                if (firstList != null && firstList.Count() > 0)
                {
                    //Process.FirstList.Run(firstList);
                }

                /*Radiation de gage // sub 4*/
                if (thirdList != null && thirdList.Count() > 0)
                {
                    Process.ThirdList.Run(thirdList);
                }

                /*Radiation de gage // sub 7*/
                if (secondList != null && secondList.Count() > 0)
                {
                    //Process.SecondList.Run(secondList);
                }
            }
        }
    }
}
