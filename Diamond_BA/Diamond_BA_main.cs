using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_BA
{
    class Diamond_BA_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\BA\20200104");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (var file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> firstList = null; //Publication of Patent Application
            List<XElement> secondList = null; //Publication of European Patents Entered in Patent Register
            List<XElement> thirdList = null; //Consensual patent (Granted Consensual Patent)
            List<XElement> fourthList = null; //Patent Desicions (Article 44)

            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);

                /*TETML elements*/
                /*Publication of Patent Application*/
                firstList = tet.Descendants().Where(d => d.Name.LocalName == "Text" && d.Value != "PATENTS")
                    .SkipWhile(e => !Regex.IsMatch(e.Value, @"\(21\)\sBAP\d{6}A"))
                    .TakeWhile(e => !Regex.IsMatch(e.Value, @"INDEKS PREMA RASTUĆEM BROJU PRIJAVE PATENATA"))
                    .Where(e => !e.Value.StartsWith("BA GLASNIK / GLASILO / ГЛАСНИК")
                    && !e.Value.StartsWith("Broj ostalih patentnih"))
                    .ToList();
                /*Publication of European Patents Entered in Patent Register*/
                secondList = tet.Descendants().Where(d => d.Name.LocalName == "Text" && d.Value != "PATENTS")
                    .SkipWhile(e => !Regex.IsMatch(e.Value, @"OBJAVA PROŠIRENIH EVROPSKIH PATENATA UPISANIH U REGISTAR PATENATA"))
                    .TakeWhile(e => !Regex.IsMatch(e.Value, @"OBJAVA ZAHTJEVA ZA KONSENZUALNI PATENT"))
                    .Where(e => !e.Value.StartsWith("BA GLASNIK / GLASILO / ГЛАСНИК")
                    && !e.Value.StartsWith("OBJAVA PROŠIRENIH EVROPSKIH PATENATA UPISANIH")
                    /*&& !e.Value.StartsWith("Broj ostalih patentnih")*/)
                    .ToList();

                /*Consensual patent (Granted Consensual Patent)*/
                thirdList = tet.Descendants().Where(d => d.Name.LocalName == "Text" || d.Name.LocalName == "Page")
                    .SkipWhile(e => !e.Value.Contains("DODJELJEN KONSEZUALNI PATENT")
                    && !Regex.IsMatch(e.Value, @"DODJELJEN KONSEZUALNI PATENT")
                    && !Regex.IsMatch(e.Value, @"DODJELJEN KONSENZUALNI PATENT"))
                    .TakeWhile(e => !Regex.IsMatch(e.Value, @"RJEŠENJA O PATENTU-RJEŠENJA"))
                    .Where(e => !e.Value.StartsWith("BA GLASNIK / GLASILO / ГЛАСНИК")
                    && !e.Value.StartsWith("DODJELJEN KONSENZUALNI PATENT")
                    && !e.Value.StartsWith("Broj ostalih patentnih")
                    && !e.Value.Contains("tetml=")
                    )
                    .ToList();

                /*Patent Desicions (Article 44)*/
                fourthList = tet.Descendants().Where(d => d.Name.LocalName == "Text" && d.Value != "PATENTS")
                    .SkipWhile(e => !Regex.IsMatch(e.Value, @"RJEŠENJA O PATENTU-RJEŠENJA"))
                    .TakeWhile(e => !Regex.IsMatch(e.Value, @"OBJAVA PROŠIRENIH EVROPSKIH PATENATA"))
                    .Where(e => !e.Value.StartsWith("BA GLASNIK / GLASILO / ГЛАСНИК")
                    && !e.Value.StartsWith("RJEŠENJA O PATENTU-RJEŠENJA")
                    && !e.Value.StartsWith("Broj ostalih patentnih"))
                    .ToList();
                Console.WriteLine("lal");

                /*Publication of Patent Application**/
                if (firstList != null && firstList.Count() > 0)
                {
                    var pubOfApp = new ProcessFirstList();
                    var el = pubOfApp.OutputValue(firstList);
                    var legalStatusEvents = ConvertToDiamond.FirstlistConvertation(el);
                    try
                    {
                        //Methods.SendToDiamond(legalStatusEvents);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Sending error");
                        throw;
                    }
                }

                /*Publication of European Patents Entered in Patent Register 4 subcode*/
                if (secondList != null && secondList.Count() > 0)
                {
                    var grantedEP = new ProcessSecondList();
                    var el = grantedEP.OutputValue(secondList);
                    var legalStatusEvents = ConvertToDiamond.SecondListConvertation(el);
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

                /*Consensual patent (Granted Consensual Patent)*/
                if (thirdList != null && thirdList.Count() > 0)
                {
                    var consPatent = new ProcessThirdList();
                    var el = consPatent.OutputValue(thirdList);
                    var legalStatusEvents = ConvertToDiamond.ThirdListConvertation(el);
                    try
                    {
                        //Methods.SendToDiamond(legalStatusEvents);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Sending error");
                        throw;
                    }
                }

                /*Patent Desicions (Article 44)*/
                if (fourthList != null && fourthList.Count() > 0)
                {
                    var patDesicion = new ProcessFourthList();
                    var el = patDesicion.OutputValue(fourthList);
                    var legalStatusEvents = ConvertToDiamond.FourthListConvertation(el);
                    try
                    {
                        //Methods.SendToDiamond(legalStatusEvents);
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
