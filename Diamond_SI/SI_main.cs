using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_SI
{
    class SI_main
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
            List<XElement> subCode3_4 = new List<XElement>();
            List<XElement> subCode20 = new List<XElement>();

            foreach (var file in files)
            {
                string nameFile = file.Split(@"\".ToCharArray()).Last().Replace(".tetml", ".pdf").Trim();
                currentFile = new FileInfo(file);
                elem = XElement.Load(file);

                allElementsList = elem.Descendants().Where(e => e.Name.LocalName == "Text")
                    .ToList();

                string startChapterStr = "Prenehanja veljavnosti patentov";
                string endChapterStr = "Spremembe";
                //string startChapterSubcode20 = "Prevodi zahtevkov evropskih patentov (T1,T2,T4)";
                //string endChapterSubcode20 = "Kazalo po kodah MPK";
                string startChapterSubcode20 = "Prevodi zahtevkov evro";
                string endChapterSubcode20 = "Kazalo po";

                int tempInc = 0;
                for (int i = 0; i < allElementsList.Count; i++)
                {
                    var value = allElementsList[i].Value;
                    tempInc = i;
                    if (value.StartsWith(startChapterStr))
                    {
                        do
                        {
                            subCode3_4.Add(allElementsList[tempInc]);
                            tempInc++;
                        } while (tempInc < allElementsList.Count &&
                                 !allElementsList[tempInc].Value.Contains(endChapterStr));
                    }
                }

                tempInc = 0;
                for (int i = 0; i < allElementsList.Count; i++)
                {
                    var value = allElementsList[i].Value;
                    tempInc = i;
                    if (value.StartsWith(startChapterSubcode20))
                    {
                        do
                        {
                            subCode20.Add(allElementsList[tempInc]);
                            tempInc++;
                        } while (tempInc < allElementsList.Count &&
                                 !allElementsList[tempInc].Value.Contains(endChapterSubcode20));
                    }
                }

                List<Subcode3> Subcode3List = new List<Subcode3>();
                List<Subcode4> Subcode4List = new List<Subcode4>();

                var resultSubcode20 = SubCode20_Processing.Subcode20Process(subCode20);
                if(resultSubcode20 != null && resultSubcode20.Count > 0)
                {
                    var convertedSubCode20 = ConvertToDiamond.Sub20Convert(resultSubcode20, gazzeteName);
                    Console.WriteLine("Конвертация Subcode 20 завершена успешно...");
                    if(convertedSubCode20 != null && convertedSubCode20.Count > 0)
                    {
                        DiamondSender.SendToDiamond(convertedSubCode20);
                        Console.WriteLine("SubCode 20 успешно отправлен на Diamond...");
                    }
                }

                //var resultSubcodes = SubCodes3_4Processing.ProcessSubcodes(subCode3_4); //Item1 -> Subcode 3, Item2 -> 4

                /*  if (resultSubcodes.Item1 != null && resultSubcodes.Item1.Count > 0)
                  {
                      var convertedSubCode3 = ConvertToDiamond.Sub3Convert(resultSubcodes.Item1, gazzeteName);
                      Console.WriteLine("SubCode 3 конвертирован в формат Diamond...");
                      if (convertedSubCode3 != null)
                      {
                          DiamondSender.SendToDiamond(convertedSubCode3);
                          Console.WriteLine("SubCode 3 успешно отправлен на Diamond...");
                      }
                  }
                  */

               /* if (resultSubcodes.Item2 != null && resultSubcodes.Item2.Count > 0)
                {
                    var convertedSubCode4 = ConvertToDiamond.Sub4Convert(resultSubcodes.Item2, gazzeteName);
                    Console.WriteLine("SubCode 4 конвертирован в формат Diamond...");
                    if (convertedSubCode4 != null)
                    {
                        DiamondSender.SendToDiamond(convertedSubCode4);
                        Console.WriteLine("SubCode 4 успешно отправлен на Diamond...");
                    }
                }*/
                Console.WriteLine("--------------------------------------------//DONE//----------------------------------------------------------------");
                Console.ReadKey();
            }
        }
    }
}
