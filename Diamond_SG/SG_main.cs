using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_SG
{
    class SG_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\SG\20200204");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> firstList = null; // APPLICATIONS WITHDRAWN, REFUSED, TAKEN TO BE ABANDONED
            List<XElement> secondList = null; // PATENTS RENEWED UNDER SECTION 36
            List<XElement> thirdList = null; // PATENTS CEASED THROUGH NON-PAYMENT OF RENEWAL FEES
            List<XElement> fourthList = null; // PATENTS CEASED AFTER THE TERM OF THE PATENT


            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                /*TETML elements*/

                /*APPLICATIONS WITHDRAWN, REFUSED, TAKEN TO BE ABANDONED*/
                firstList = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => e.Value != "APPLICATIONS WITHDRAWN, REFUSED, TAKEN TO BE ABANDONED")
                    .TakeWhile(e => e.Value != "APPLICATIONS MADE FOR RESTORATION UNDER SECTION 39")
                    .Where(e => e.Value != ""
                    && e.Value != "This index lists applications in ascending application number order"
                    && e.Value != "APPLICATIONS WITHDRAWN, REFUSED, TAKEN TO BE ABANDONED"
                    )
                    .ToList();

                /*PATENTS RENEWED UNDER SECTION 36*/
                secondList = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => !(e.Value.StartsWith("PATENTS RENEWED UNDER SECTION 36") && e.Value.Contains("This Index lists patents in ascending")))
                    .TakeWhile(e => !e.Value.StartsWith("PATENTS CEASED THROUGH NON-PAYMENT OF RENEWAL FEES"))
                    .Where(e => e.Value != "")
                    .ToList();

                /*PATENTS CEASED THROUGH NON-PAYMENT OF RENEWAL FEES*/
                thirdList = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => !(e.Value.StartsWith("PATENTS CEASED THROUGH NON-PAYMENT OF RENEWAL FEES") && e.Value.Contains("This Index lists patents in Application")))
                    .TakeWhile(e => e.Value != "PUBLICATIONS UNDER RULE 109(1) OF PATENT RULES" && e.Value != "TRANSLATIONS FILED IN ACCORDANCE TO SECTION 86(3) OR (6) OF THE PATENTS ACT")
                    .Where(e => e.Value != "")
                    .ToList();
                /*PATENTS CEASED AFTER THE TERM OF THE PATENT*/
                fourthList = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => e.Value != "PATENTS CEASED AFTER THE TERM OF THE PATENT")
                    .TakeWhile(e => !e.Value.StartsWith("TRANSLATIONS FILED IN ACCORDANCE TO SECTION 86(3) OR (6) OF THE PATENTS ACT"))
                    .Where(e => e.Value != "" && e.Value != "This Index lists patents in Application No order")
                    .ToList();

                Console.WriteLine("Lists of elements are prepared");
                /*APPLICATIONS WITHDRAWN, REFUSED, TAKEN TO BE ABANDONED*/
                if (firstList != null && firstList.Count() > 0)
                {
                    var v = Process.FirstList.Run(firstList);
                    var legalStatusEvents = ConvertToDiamond.FirstList(v);
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

                /*PATENTS RENEWED UNDER SECTION 36 5th subcode*/
                if (secondList != null && secondList.Count() > 0)
                {
                    var v = Process.SecondList.Run(secondList);
                    var legalStatusEvents = ConvertToDiamond.SecondList(v);
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
                /*PATENTS CEASED THROUGH NON-PAYMENT OF RENEWAL FEES 6th subcode*/
                if (thirdList != null && thirdList.Count() > 0)
                {
                    var v = Process.SecondList.Run(thirdList);
                    var legalStatusEvents = ConvertToDiamond.ThirdList(v);
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
                /*PATENTS CEASED AFTER THE TERM OF THE PATENT 7th subcode*/
                if (fourthList != null && fourthList.Count() > 0)
                {
                    var v = Process.SecondList.Run(fourthList);
                    var legalStatusEvents = ConvertToDiamond.FourthList(v);
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
