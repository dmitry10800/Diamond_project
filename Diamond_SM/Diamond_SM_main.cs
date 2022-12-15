using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_SM
{
    class Diamond_SM_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\SM\20200203");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> firstList = null; // 
            List<XElement> secondList = null; //


            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                /*TETML elements*/

                /*European Patents Validated in the Republic of San Marino*/
                firstList = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => e.Value != "European Patents Validated in the Republic of San Marino")
                    .TakeWhile(e => e.Value != "Rinnovi/Renewals" && e.Value != "Rinnovi/R enewals")
                    .Where(e => e.Value != ""
                    && e.Value != "Repubblica di San Marino" && e.Value != "Repubblica d i San Marino"
                    && e.Value != "Bollettino Marchi, Brevetti e Disegni"
                    && e.Value != "BREVETTI/PATENTS" && e.Value != "BREVETTI/ PATENTS"
                    && e.Value != "Rinnovi/Renewals" && e.Value != "Rinnovi/R enewals")
                    .ToList();

                /*European Patents Validated in the Republic of San Marino*/
                secondList = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => e.Value != "BREVETTI/PATENTS")
                    .SkipWhile(e => e.Value != "Rinnovi/Renewals")
                    .TakeWhile(e => e.Value != "Repubblica d i San Marino" && e.Value != "Bollettino Marchi, Brevetti e Disegni" && e.Value != "Repubblica di San Marino")
                    .Where(e => e.Value != ""
                    && e.Value != "Repubblica d i San Marino"
                    && e.Value != "Bollettino Marchi, Brevetti e Disegni"
                    && e.Value != "BREVETTI/ PATENTS"
                    && e.Value != "Rinnovi/R enewals")
                    .ToList();

                Console.WriteLine("Lists of elements are prepared");

                if (firstList != null && firstList.Count() > 0)
                {
                    ProcessFirstList firstListValues = new ProcessFirstList();
                    List<ProcessFirstList.ElementOut> el = firstListValues.OutputValue(firstList);
                    var legalStatusEvents = ConvertToDiamond.FirstListConvertation(el);
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

                /*Utility models*/
                if (secondList != null && secondList.Count() > 0)
                {
                    ProcessSecondtList grantedEP = new ProcessSecondtList();
                    ProcessSecondtList.ElementOut el = grantedEP.OutputValue(secondList);
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
            }
        }
    }
}
