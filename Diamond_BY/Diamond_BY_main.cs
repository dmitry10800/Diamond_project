using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_BY
{
    class Diamond_BY_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\BY\Gaz\");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> firstList = null; //PUBLICATION INFORMATION ABOUT APPLICATIONS
            List<XElement> secondList = null; //PUBLICATION INFORMATION ABOUT INTERNATIONAL APPLICATIONS (PCT), INTRODUCED TO THE NATIONAL PHASE IN THE REPUBLIC OF BELARUS
            List<XElement> thirdList = null; //PUBLICATION OF INFORMATION ABOUT PATENTS TO INVENTIONS INCLUDED IN THE STATE REGISTER OF INVENTIONS OF THE REPUBLIC OF BELARUS
            List<XElement> fourthList = null; //Publication of information on patents for utility models, which are included in the State Register of Utility Models of the Republic of Belarus

            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                //currentElement = null;

                /*TETML elements*/
                /*Granted European patents valid in the Republic of Bulgaria*/
                firstList = tet.Descendants().Where(d => d.Name.LocalName == "Text" && d.Value != "PATENTS")
                    .SkipWhile(e => !e.Value.StartsWith("BA1A (12) ПУБЛИКАЦИЯ СВЕДЕНИЙ О ЗАЯВКАХ"))
                    .TakeWhile(e => !e.Value.StartsWith("СИСТЕМАТИЧЕСКИЙ УКАЗАТЕЛЬ ЗАЯВОК НА ИЗОБРЕТЕНИЯ"))
                    .Where(e => e.Value != "BA1A (12) ПУБЛИКАЦИЯ СВЕДЕНИЙ О ЗАЯВКАХ"
                    && e.Value != "ЗАЯВКИ ЗА ЕВРОПЕЙСКИ ПАТЕНТ И\nИЗДАДЕНИ ЕВРОПЕЙСКИ ПАТЕНТИ,\nС ДЕЙСТВИЕ НА ТЕРИТОРИЯТА НА\nРЕПУБЛИКА БЪЛГАРИЯ"
                    && e.Value != "ЭЛЕКТРИЧЕСТВО"
                    && e.Value != "ФИЗИКА"
                    && !Regex.IsMatch(e.Value, @"^[A-Z]{1}\d{2}$")
                    && !e.Value.StartsWith("МЕХАНИКА")
                    && !e.Value.StartsWith("РАЗДЕЛ")
                    && !e.Value.StartsWith("СТРОИТЕЛЬСТВО")
                    && !e.Value.StartsWith("БУМАГА И ТЕКСТИЛЬ")
                    && !e.Value.StartsWith("ХИМИЯ")
                    && !e.Value.StartsWith("РАЗЛИЧНЫЕ\nТЕХНОЛОГИЧЕСКИЕ ПРОЦЕССЫ")
                    && !e.Value.StartsWith("УДОВЛЕТВОРЕНИЕ\nЖИЗНЕННЫХ\nПОТРЕБНОСТЕЙ"))
                    .ToList();

                Console.WriteLine("lal");

                /*Granted European patents valid in the Republic of Bulgaria**/
                if (firstList != null && firstList.Count() > 0)
                {
                    ProcessFirstList grantedEP = new ProcessFirstList();
                    List<ProcessFirstList.ElementOut> el = grantedEP.OutputValue(firstList);
                    //var legalStatusEvents = ConvertToDiamond.GrantedPatentsForInventionsConvertation(el);
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
            }
        }
    }
}
