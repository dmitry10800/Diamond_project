using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Lens_AR_Subcodes_2_3
{
    class AR_main
    {
        public static DirectoryInfo PathToTetml;
        public static string gazzeteName;
        public static FileInfo currentFile = null;

        private static void Main(string[] args)
        {
            List<Subcodes2_3> outListSubCode2 = new List<Subcodes2_3>();
            List<Subcodes2_3> outListSubCode3 = new List<Subcodes2_3>();
            List<string> errorsList = new List<string>();
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
            List<string> subCodes2_3 = new List<string>();

            foreach (var file in files)
            {
                string nameFile = file.Split(@"\".ToCharArray()).Last().Replace(".tetml", ".pdf").Trim();
                currentFile = new FileInfo(file);
                elem = XElement.Load(file);

                allElementsList = elem.Descendants().Where(e => e.Name.LocalName == "Text")
                    .ToList();

                string startAllSubcodes = "<Primera>";

                int tempInc;

                for (int i = 0; i < allElementsList.Count; i++)
                {
                    var value = allElementsList[i].Value;

                    tempInc = i;

                    if (value.StartsWith(startAllSubcodes))
                    {
                        do
                        {
                            subCodes2_3.Add(allElementsList[tempInc].Value);
                            tempInc++;
                        } while (tempInc < allElementsList.Count);
                    }
                }

                if (subCodes2_3.Count > 0)
                {
                    var resultsProcessingSubCodes2_3 = ProcessSubcode2.ProcessingSubCodes2_3(subCodes2_3, gazzeteName);
                    outListSubCode2 = resultsProcessingSubCodes2_3.Item1;
                    Console.WriteLine("Обработка SubCode 2 прошла успешно...");
                    outListSubCode3 = resultsProcessingSubCodes2_3.Item2;
                    Console.WriteLine("Обработка SubCode 3 прошла успешно...");
                    errorsList = resultsProcessingSubCodes2_3.Item3;
                }
            }
        }
    }
}
