using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_NZ_Subcodes_1_2_4
{
    class NZ_main
    {
        public static DirectoryInfo PathToTetml;
        public static string gazzeteName;
        public static FileInfo currentFile = null;

        private static void Main(string[] args)
        {
            Console.WriteLine("Enter path to folder with PDF file");
            var pathToPDF = Console.ReadLine();
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
            foreach (var file in PathToTetml.GetFiles("*.tetml", SearchOption.AllDirectories))
                files.Add(file.FullName);

            XElement elem = null;
            var allElementsList = new List<XElement>(); // all elements in processing gazette
            var subCode1 = new List<XElement>();
            var subCode2 = new List<XElement>();
            var subCode4 = new List<XElement>();
            var resultSubcode1 = new List<SubCode1>();
            var resultSubcode2 = new List<SubCode2>();
            var resultSubcode4 = new List<SubCode4>();

            foreach (var file in files)
            {
                var nameFile = file.Split(@"\".ToCharArray()).Last().Replace(".tetml", ".pdf").Trim();
                currentFile = new FileInfo(file);
                elem = XElement.Load(file);

                allElementsList = elem.Descendants().Where(e => e.Name.LocalName == "Text")
                    .ToList();

                var startChapterSubcode1Str = "Patent Assignment";
                var endChapterSubcode1Str = "Select all - Select None";

                var startChapterSubcode2Str = "Patent Lapsed";
                var endChapterSubcode2Str = "Select all - Select None";

                var startChapterSubcode4Str = "Patent Expired";
                var endChapterSubcode4Str = "Select all - Select None";

                var startChapterSubCode3 = "Patent Restored";
                var startChapterSubCode5 = "Patent Request to Restore";

                var listPositionsSubCodes = new List<(int, int)>(); //(int - start position in Subcode, int - end position in Subcode)

                int startSubCode1 = -1, startSubCode2 = -1, endSubCode2 = -1, startSubCode4 = -1, endSubCode4 = -1;
                var subcode1Start = false;
                var subcode2Start = false;
                var subcode4Start = false;
                for (var i = 0; i < allElementsList.Count; i++)
                {
                    if (allElementsList[i].Value.StartsWith(startChapterSubcode1Str) && !subcode1Start && startSubCode1 == -1)
                    {
                        startSubCode1 = i;
                        subcode1Start = true;
                    }

                    if (allElementsList[i].Value.StartsWith(startChapterSubcode2Str) && !subcode2Start && startSubCode2 == -1)
                    {
                        startSubCode2 = i;
                        subcode2Start = true;
                    }

                    if (allElementsList[i].Value.StartsWith(startChapterSubCode3) && endSubCode2 == -1)
                    {
                        endSubCode2 = i;
                    }


                    if (allElementsList[i].Value.StartsWith(startChapterSubcode4Str) && !subcode4Start &&
                        startSubCode4 == -1)
                    {
                        subcode4Start = true;
                        startSubCode4 = i;
                    }

                    if (allElementsList[i].Value.StartsWith(startChapterSubCode5) && endSubCode4 == -1)
                    {
                        endSubCode4 = i;
                        break;
                    }
                }

                if (endSubCode2 == -1 && startSubCode4 > -1)
                {
                    endSubCode2 = startSubCode4;
                }

                if (startSubCode1 > -1 && startSubCode2 > -1 && startSubCode4 > -1 && endSubCode2 > -1 &&
                    endSubCode4 > -1)
                {
                    for (var i = startSubCode1; i < startSubCode2; i++)
                    {
                        subCode1.Add(allElementsList[i]);
                    }

                    for (var i = startSubCode2; i < endSubCode2; i++)
                    {
                        subCode2.Add(allElementsList[i]);
                    }

                    for (var i = startSubCode4; i < endSubCode4; i++)
                    {
                        subCode4.Add(allElementsList[i]);
                    }
                }

                /* if (subCode1.Count > 0)
                 {
                     //обработка SubCode 1
                     resultSubcode1 = ProcessingSubCode1.ProcessSubCode1(subCode1, PathToTetml);
                     Console.WriteLine("Обработка Subcode 1 завершена успешно...");
                     if (resultSubcode1 != null && resultSubcode1.Count > 0)
                     {
                         var convertedSubCode1 = ConvertToDiamond.Sub1Convert(resultSubcode1, gazzeteName);
                         Console.WriteLine("SubCode 1 конвертирован");
                         DiamondSender.SendToDiamond(convertedSubCode1);
                         Console.WriteLine("SubCode 1 отправлен на Diamond");
                     }
                 }*/
                /*
                if (subCode2.Count > 0)
                {
                    //обработка SubCode 2 
                    resultSubcode2 = ProcessingSubCode2.ProcessSubCode2(subCode2);
                    Console.WriteLine("Обработка Subcode 2 завершена успешно...");
                    if (resultSubcode2 != null && resultSubcode2.Count > 0)
                    {
                        var convertedSubCode2 = ConvertToDiamond.Sub2Convert(resultSubcode2, gazzeteName);
                        Console.WriteLine("SubCode 2 конвертирован");
                        DiamondSender.SendToDiamond(convertedSubCode2);
                        Console.WriteLine("SubCode 2 отправлен на Diamond");
                    }
                }*/

                if (subCode4.Count > 0)
                {
                    //обработка SubCode 4 
                    resultSubcode4 = ProcessingSubCode4.ProcessSubCode4(subCode4);
                    Console.WriteLine("Обработка Subcode 4 завершена успешно...");
                    if (resultSubcode4 != null && resultSubcode4.Count > 0)
                    {
                        var convertedSubCode4 = ConvertToDiamond.Sub4Convert(resultSubcode4, gazzeteName);
                        Console.WriteLine("SubCode 4 конвертирован");
                        DiamondSender.SendToDiamond(convertedSubCode4);
                        Console.WriteLine("SubCode 4 отправлен на Diamond");
                    }
                }
            }
            Console.WriteLine("-----------------------------------------------//DONE//--------------------------------------------------------");
            Console.ReadKey();
        }
    }
}
