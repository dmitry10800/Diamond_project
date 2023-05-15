using BG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_BG_Subcode_21
{
    class BG_main
    {
        public static DirectoryInfo PathToTetml;
        public static string gazzeteName;
        public static FileInfo currentFile = null;
        private static void Main(string[] args)
        {
            Console.WriteLine("Country:\t\t\tBG\n\n");
            var outListSubCode21 = new List<Subcode21>();
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
            var subCode21List = new List<string>();

            foreach (var file in files)
            {
                var nameFile = file.Split(@"\".ToCharArray()).Last().Replace(".tetml", ".pdf").Trim();
                currentFile = new FileInfo(file);
                elem = XElement.Load(file);

                allElementsList = elem.Descendants().Where(e => e.Name.LocalName == "Text")
                    .ToList();

                var startChapterStr = "ПРЕКРАТИЛИ ДЕЙСТВИЕТО СИ ЕВРОПЕЙСКИ ПАТЕНТИ ПОРАДИ";
                var endChapterStr = "ПРЕКРАТЯВАНЕ ДЕЙСТВИЕТО НА ЕВРОПЕЙСКИ ПАТЕНТ";

                int tempInc;

                for (var i = 0; i < allElementsList.Count; i++)
                {
                    var value = allElementsList[i].Value;

                    tempInc = i;

                    if (value.StartsWith(startChapterStr) || value.StartsWith("ПРЕКРАТЕНИ ЕВРОПЕЙСКИ ПАТЕНТИ ЗА ИЗОБРЕТЕНИЯ"))
                    {
                        do
                        {
                            subCode21List.Add(allElementsList[tempInc].Value);
                            tempInc++;
                        } while (tempInc < allElementsList.Count && !allElementsList[tempInc].Value.StartsWith(endChapterStr) && !allElementsList[tempInc].Value.StartsWith("ВПИСВАНИЯ В ДЪРЖАВНИЯ РЕГИСТЪР НА ПАТЕНТИТЕ") && !allElementsList[tempInc].Value.StartsWith("ВЪЗСТАНОВЕНИ ЕВРОПЕЙСКИ ПАТЕНТИ ЗА ИЗОБРЕТЕНИЯ"));
                    }
                }

                if (subCode21List.Count > 0)
                {
                    outListSubCode21 = ProcessSubCode21.ProcessingSubCode21(subCode21List, gazzeteName);
                    Console.WriteLine("Обработка SubCode 21 прошла успешно...");
                }

                if (outListSubCode21.Count > 0)
                {
                    Console.WriteLine("Конвертация Subcode 21 начало:");
                    var convertedSubCode21 = ConvertToDiamond.Sub21Convert(outListSubCode21, gazzeteName);
                    Console.WriteLine("Конвертация в фортмат Diamond прошла успешно...");
                    if (convertedSubCode21.Count > 0)
                    {
                        DiamondSender.SendToDiamond(convertedSubCode21);
                        Console.WriteLine("Было отправлено в Diamond:\t" + convertedSubCode21.Count + "\tзаписей");
                        Console.WriteLine("Отправка на Diamond SubCode 21  завершена...");
                    }
                }
            }
            Console.WriteLine("---------------------------------------------------------------------------------------------");
            Console.WriteLine("\t\t\t\t\t\t\t\t\tDONE SENDING");
            Console.ReadKey();
        }
    }
}
