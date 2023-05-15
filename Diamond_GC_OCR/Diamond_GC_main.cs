using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_GC_OCR
{
    class Diamond_GC_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\GC\20191011");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (var file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> firstList = null;
            List<XElement> secondList = null; //List of Lapsed Patent Applications
            List<XElement> thirdList = null; //List of Rejected Patent Applications
            List<XElement> fourthList = null; //List of Rejected Patent Applications

            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                //currentElement = null;

                /*TETML elements*/

                firstList = tet.Descendants().Where(d => d.Name.LocalName == "Text").ToList();

                /*List of Lapsed Patent Applications*/
                secondList = tet.Descendants().Where(d => d.Name.LocalName == "Text")
                .SkipWhile(e => !e.Value.StartsWith("List of Lapsed Patent Applications"))
                .TakeWhile(e => !e.Value.StartsWith("List of Rejected Patent Applications") && !e.Value.StartsWith("List of Lapsed Patents"))
                .ToList();

                /*List of Rejected Patent Applications*/
                thirdList = tet.Descendants().Where(d => d.Name.LocalName == "Text")
                .SkipWhile(e => !e.Value.StartsWith("List of Rejected Patent Applications"))
                .TakeWhile(e => !e.Value.StartsWith("Change of the Ownership"))
                .ToList();

                /*List of Lapsed Patents*/
                fourthList = tet.Descendants().Where(d => d.Name.LocalName == "Text")
                .SkipWhile(e => !e.Value.StartsWith("List of Lapsed Patents"))
                .TakeWhile(e => !e.Value.StartsWith("List of Rejected Patent Applications"))
                .ToList();



                Console.WriteLine("***************************" + Path.GetFileNameWithoutExtension(tetFile) + "***************************");

                //if (firstList != null && firstList.Count() > 0)
                //{
                //    ProcessGrantedPatents grantedEP = new ProcessGrantedPatents();
                //    List<ProcessGrantedPatents.ElementOut> el = grantedEP.OutputValue(firstList);
                //    var legalStatusEvents = ConvertToDiamond.FirstListConvertation(el);
                //    try
                //    {
                //        Methods.SendToDiamond(legalStatusEvents);
                //    }
                //    catch (Exception)
                //    {
                //        Console.WriteLine("Sending error");
                //        throw;
                //    }
                //}

                if (secondList != null && secondList.Count() > 0)
                {
                    var lapsedPatents = new ProcessLegalEvents();
                    var el = lapsedPatents.OutputValue(secondList);
                    var legalStatusEvents = ConvertToDiamond.LegalEventConvertation(el, "sub14");
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
                if (thirdList != null && thirdList.Count() > 0)
                {
                    var lapsedPatents = new ProcessLegalEvents();
                    var el = lapsedPatents.OutputValue(thirdList);
                    var legalStatusEvents = ConvertToDiamond.LegalEventConvertation(el, "sub26");
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
                if (fourthList != null && fourthList.Count() > 0)
                {
                    var lapsedPatents = new ProcessLegalEvents();
                    var el = lapsedPatents.Sub06OutputValue(fourthList);
                    var legalStatusEvents = ConvertToDiamond.Sub06LegalEventConvertation(el, "sub6");
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
