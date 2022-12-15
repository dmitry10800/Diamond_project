using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Diamond.Core.Models;

namespace Diamond_IE_Subcodes_7_8
{
    class IE_main
    {
        public static DirectoryInfo PathToTetml;
        public static string gazzeteName;
        public static FileInfo currentFile = null;

        private static void Main(string[] args)
        {
            Console.WriteLine("Enter path to folder with PDF file");
            string pathToPDF = Console.ReadLine();
            PathToTetml = new DirectoryInfo(pathToPDF);
            var countFiles = PathToTetml.GetFiles().Count();
            if (countFiles < 2)
            {
                CreateTetml.CreateTetmlDocument(pathToPDF);
            }
            else
            {
                Console.WriteLine("Tetml exists in this folder.");
            }

            var listFiles = PathToTetml.GetFiles();
            foreach (var file in listFiles)
            {
                if (file.Name.EndsWith(".pdf"))
                {
                    gazzeteName = file.Name.Trim();
                    break;
                }
            }

            var files = new List<string>();
            foreach (FileInfo file in PathToTetml.GetFiles("*.tetml", SearchOption.AllDirectories))
                files.Add(file.FullName);

            XElement elem = null;
            List<XElement> allElementsList = new List<XElement>(); // all elements in processing gazette
            List<XElement> subCode7_8 = new List<XElement>();

            foreach (var file in files)
            {
                string nameFile = file.Split(@"\".ToCharArray()).Last().Replace(".tetml", ".pdf").Trim();
                currentFile = new FileInfo(file);
                elem = XElement.Load(file);

                allElementsList = elem.Descendants().Where(e => e.Name.LocalName == "Text")
                    .ToList();

                string startChapterStr = "Patents Expired";
                string endChapterStr = "Request for Grant of Supplementary Protection Certificate";

                List<int> startsChapterList = new List<int>();
                List<int> endsChapterList = new List<int>();

                for (int i = 0; i < allElementsList.Count; i++)
                {
                    var value = allElementsList[i].Value;

                    if (value.StartsWith(startChapterStr))
                        startsChapterList.Add(i);

                    if (value.StartsWith(endChapterStr) || value.StartsWith("Designs Registered Under The Industrial Designs Act"))
                        endsChapterList.Add(i);

                }

                int startSubCode7_8 = startsChapterList.Max();
                int endSubCode7_8 = endsChapterList.Max();

                List<XElement> allElementsForSubCodes = new List<XElement>();

                if (startSubCode7_8 > 0 && endSubCode7_8 > 0)
                {
                    for (int i = startSubCode7_8; i < endSubCode7_8; i++)
                    {
                        allElementsForSubCodes.Add(allElementsList[i]);
                    }
                }

                string tempValue = "";

                for (int i = 0; i < allElementsForSubCodes.Count; i++)
                {
                    tempValue += allElementsForSubCodes[i].Value + "\n";
                }

                var result = Methods.RecSplit(tempValue);

                List<string> Subcodes7_8 = new List<string>();
                List<string> Subcode7 = new List<string>();
                List<string> Subcode8 = new List<string>();

                Regex regex;
                MatchCollection matches;

                foreach (var s in result)
                {
                    regex = new Regex(@"^S?\d{5}\b");
                    matches = regex.Matches(s);
                    if (matches.Count > 0)
                    {
                        Subcodes7_8.Add(s);
                    }
                }

                foreach (var item in Subcodes7_8)
                {
                    if (item.StartsWith("S"))
                        Subcode8.Add(item);
                    else
                        Subcode7.Add(item);
                }

                List<LegalStatusEvent> legalEventsSubCode7 = new List<LegalStatusEvent>();
                List<LegalStatusEvent> legalEventsSubCode8 = new List<LegalStatusEvent>();
                if (Subcode7 != null && Subcode7.Count > 0)
                {
                    var resultSubCode_7 = SubCodesProcessing.SubCodes7_8_Processing(Subcode7, nameFile);
                    legalEventsSubCode7 = ConvertToDiamond.Sub7_8Convert(resultSubCode_7, nameFile, "7");
                    Console.WriteLine("Subcode 7 обработан...");
                }

                if (Subcode8 != null && Subcode8.Count > 0)
                {
                    var resultSubCode_8 = SubCodesProcessing.SubCodes7_8_Processing(Subcode8, nameFile);
                    legalEventsSubCode8 = ConvertToDiamond.Sub7_8Convert(resultSubCode_8, nameFile, "8");
                    Console.WriteLine("Subcode 8 обработан...");
                }

                if (legalEventsSubCode7 != null && legalEventsSubCode7.Count > 0)
                {
                    DiamondSender.SendToDiamond(legalEventsSubCode7);
                    Console.WriteLine("Subcode 7 отправлен на Diamond");
                }

                if (legalEventsSubCode8 != null && legalEventsSubCode8.Count > 0)
                {
                    DiamondSender.SendToDiamond(legalEventsSubCode8);
                    Console.WriteLine("Subcode 8 отправлен на Diamond");
                }

                Console.WriteLine("--------------------------------------------//DONE//------------------------------------------------------");
                Console.ReadKey();
            }
        }
    }
}
