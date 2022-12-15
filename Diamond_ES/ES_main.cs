using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Diamond.Core.Models;

namespace Diamond_ES
{
    class ES_main
    {
        public static DirectoryInfo PathToTetml;

        public static string gazzeteName;
        public static FileInfo currentFile = null;
        private static int ink = 0;

        public static void Main(string[] args)
        {
            Console.WriteLine("Country:\t\t\tES\n\n");
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
            List<XElement> subCode3 = new List<XElement>();
            List<XElement> subCode11 = new List<XElement>();
            List<XElement> subCode12 = new List<XElement>();

            foreach (var file in files)
            {
                currentFile = new FileInfo(file);
                elem = XElement.Load(file);

                allElementsList = elem.Descendants().Where(e => e.Name.LocalName == "Text")
                    .ToList();

                string[] chaptersName = new[]
                {
                    "1. PATENTES", "2. MODELOS DE", "3. CERTIFICADOS",
                    "5. SOLICITUDES Y",
                    "6. TRANSMISIONES DE", "7. EXPLOTACIÓN Y", "8. RESTABLECIMIENTO DE",
                    "9. AVISOS Y", "10. RECTIFICACIONES", "11. RECURSOS"
                };


                var result = FindAllChapters(allElementsList, chaptersName);
                //var allPositionsForChapters = NornalizePositions(result, allElementsList);
                int startPos, endPos, temp;

                if (result[0].Index != -1)
                {
                    //subcode 3
                    startPos = -1;
                    endPos = -1;

                    startPos = result[0].Index;

                    if (result[1].Index != -1)
                        endPos = result[1].Index;

                    if (startPos > -1 && endPos > -1)
                    {
                        for (int i = startPos; i < endPos; i++)
                        {
                            subCode3.Add(allElementsList[i]);
                        }
                    }
                }

                if (result[1].Index != -1)
                {
                    //subcode 11

                    startPos = -1;
                    endPos = -1;

                    startPos = result[1].Index;

                    if (result[2].Index != -1)
                        endPos = result[2].Index;

                    if (result[3].Index != -1 && endPos == -1)
                        endPos = result[3].Index;

                    if (startPos > -1 && endPos > -1)
                    {
                        for (int i = startPos; i < endPos; i++)
                        {
                            subCode11.Add(allElementsList[i]);
                        }
                    }
                }

                if (result[3].Index != -1)
                {
                    //subcode 12
                    startPos = -1;
                    endPos = -1;

                    startPos = result[3].Index;

                    if (result[4].Index != -1 && endPos == -1)
                        endPos = result[4].Index;

                    if (result[5].Index != -1 && endPos == -1)
                        endPos = result[5].Index;

                    if (result[6].Index != -1 && endPos == -1)
                        endPos = result[6].Index;

                    if (result[7].Index != -1 && endPos == -1)
                        endPos = result[7].Index;

                    if (result[8].Index != -1 && endPos == -1)
                        endPos = result[8].Index;

                    if (result[9].Index != -1 && endPos == -1)
                        endPos = result[9].Index;

                    if (startPos > -1 && endPos > -1)
                    {
                        for (int i = startPos; i < endPos; i++)
                        {
                            subCode12.Add(allElementsList[i]);
                        }
                    }
                }

                List<LegalStatusEvent> resultSubCode3 = new List<LegalStatusEvent>();
                List<LegalStatusEvent> resultSubCode11 = new List<LegalStatusEvent>();
                List<LegalStatusEvent> resultSubCode12 = new List<LegalStatusEvent>();
                /*
                if (subCode3 != null && subCode3.Count > 0)
                {
                    var allElementSubCode3 = SubCode3Processing.ProcessSubCode3(subCode3);
                    resultSubCode3 = ConvertToDiamond.Sub3Convert(allElementSubCode3, gazzeteName);
                }
                */
                /*if (subCode11 != null && subCode11.Count > 0)
                {
                    var allElementsSubCode11 = SubCode11Processing.ProcessSubCode11(subCode11);
                    resultSubCode11 = ConvertToDiamond.Sub11Convert(allElementsSubCode11, gazzeteName);
                    Console.WriteLine("Subcode 11 конвертирован успешно...");
                }*/

                if (subCode12 != null && subCode12.Count > 0)
                {
                    var allElementsSubCode12 = SubCode12Processing.ProcessSubCode12(subCode12);
                    resultSubCode12 = ConvertToDiamond.Sub12Convert(allElementsSubCode12, gazzeteName);
                    Console.WriteLine("SubCode12 Обработан успешно...");
                }
                /*
                if (resultSubCode3 != null && resultSubCode3.Count > 0)
                {
                    DiamondSender.SendToDiamond(resultSubCode3);
                    Console.WriteLine("Subcode 3 отправлен на Diamond");
                }
                */
                /*if (resultSubCode11 != null && resultSubCode11.Count > 0)
                {
                    DiamondSender.SendToDiamond(resultSubCode11);
                    Console.WriteLine("Subcode 11 отправлен на Diamond");
                }*/

                if (resultSubCode12 != null && resultSubCode12.Count > 0)
                {
                    DiamondSender.SendToDiamond(resultSubCode12);
                    Console.WriteLine("Subcode 12 отправлен на Diamond");
                }
            }
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\tDONE SENDING");
            Console.ReadKey();
        }

        static List<ElementForChecking> FindAllChapters(List<XElement> list, string[] allChaptersArr)
        {
            List<ElementForChecking> outList = new List<ElementForChecking>();
            ElementForChecking element;

            int k = 0;
            foreach (var item in allChaptersArr)
            {
                int county = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Value.Contains(item) && !list[i].Value.Contains(" ......."))
                    {
                        element = new ElementForChecking(i, list[i].Value);
                        outList.Add(element);
                        county++;
                    }

                    if (i == list.Count - 1)
                    {
                        if (county == 0)
                            outList.Add(new ElementForChecking(-1, null));
                    }
                }
            }
            return outList;
        }
    }

    public class ElementForChecking
    {
        public int Index { get; set; }
        public string Value { get; set; }

        public ElementForChecking(int index, string value)
        {
            Index = index;
            Value = value;
        }
    }
}
