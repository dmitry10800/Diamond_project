using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_GC
{
    class Diamond_GC_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\GC\20191118\3");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> firstList = null;
            List<XElement> secondList = null; //List of Lapsed Patent Applications
            List<XElement> thirdList = null; //List of Rejected Patent Applications
            List<XElement> fourthList = null;
            List<XElement> fifthList = null;

            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                //currentElement = null;

                /*TETML elements*/

                firstList = tet.Descendants().Where(d => d.Name.LocalName == "Text" || d.Name.LocalName == "PlacedImage")
                .SkipWhile(e => !e.Value.StartsWith("[12]"))
                .ToList();

                /*List of Lapsed Patent Applications*/
                secondList = tet.Descendants().Where(d => d.Name.LocalName == "Text")
                .SkipWhile(e => /*!e.Value.StartsWith("List of Lapsed Patent Applications") || */!e.Value.StartsWith("Fallen Applications"))
                .TakeWhile(e => !e.Value.StartsWith("Rejected Patent Applications") && !e.Value.StartsWith("Change of the Ownership"))
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

                /*List of Lapsed Patents*/
                fifthList = tet.Descendants().Where(d => d.Name.LocalName == "Text")
                .SkipWhile(e => !e.Value.StartsWith("Fallen Patents"))
                .TakeWhile(e => !e.Value.StartsWith("Rejected Applications"))
                .ToList();



                Console.WriteLine("***************************" + Path.GetFileNameWithoutExtension(tetFile) + "***************************");

                if (firstList != null && firstList.Count() > 0)
                {
                    ProcessGrantedPatents grantedEP = new ProcessGrantedPatents();
                    List<ProcessGrantedPatents.ElementOut> el = grantedEP.OutputValue(firstList);
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
                if (secondList != null && secondList.Count() > 0)
                {
                    ProcessLegalEvents lapsedPatents = new ProcessLegalEvents();
                    List<ProcessLegalEvents.ElementOut> el = lapsedPatents.OutputValue(secondList);
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
                    ProcessLegalEvents lapsedPatents = new ProcessLegalEvents();
                    List<ProcessLegalEvents.ElementOut> el = lapsedPatents.OutputValue(thirdList);
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
                /*sub 6 v1*/
                if (fourthList != null && fourthList.Count() > 0)
                {
                    //ProcessLegalEvents lapsedPatents = new ProcessLegalEvents();
                    //List<ProcessLegalEvents.Sub06ElementOut> el = lapsedPatents.Sub06OutputValue(fourthList);
                    //var legalStatusEvents = ConvertToDiamond.Sub06LegalEventConvertation(el, "sub6");
                    //try
                    //{
                    //    //Methods.SendToDiamond(legalStatusEvents);
                    //}
                    //catch (Exception)
                    //{
                    //    Console.WriteLine("Sending error");
                    //    throw;
                    //}
                }
                /*sub 6 v2*/
                if (fifthList != null && fifthList.Count() > 0)
                {
                    ProcessLegalEvents lapsedPatents = new ProcessLegalEvents();
                    List<ProcessLegalEvents.Sub06v2ElementOut> el = lapsedPatents.Sub06OutputValue(fifthList);
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
