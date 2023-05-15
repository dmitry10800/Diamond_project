using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_ZA
{
    class Diamond_ZA_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\ZA\20200203");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (var file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> firstList = null; // APPLICATIONS FOR PATENTS
            List<XElement> secondList = null; // COMPLETE SPECIFICATIONS ACCEPTED AND ABRIDGEMENTS OR ABSTRACTS THEREOF

            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);

                /*TETML elements*/
                /*Publication of Patent Application*/
                firstList = tet.Descendants().Where(d => d.Name.LocalName == "Text")
                    .SkipWhile(e => e.Value != "APPLICATIONS FOR PATENTS") //one of the possible starting key
                                                                           //.SkipWhile(e => !e.Value.StartsWith("APPLICATIONS FOR PATENTS"))
                    .TakeWhile(e => e.Value != "INSPECTION OF SPECIFICATIONS"
                    && e.Value != "COMPLETE SPECIFICATIONS ACCEPTED AND ABRIDGEMENTS OR ABSTRACTS THEREOF")
                    .Where(e => !e.Value.StartsWith("Application number ~ Nature ~ 54: Representation of mark"))
                    .ToList();

                /*Publication of Patent Application*/
                secondList = tet.Descendants().Where(d => d.Name.LocalName == "Text")
                    .SkipWhile(e => e.Value != "COMPLETE SPECIFICATIONS ACCEPTED AND ABRIDGEMENTS OR ABSTRACTS THEREOF")
                    .TakeWhile(e => e.Value != "HYPOTHECATIONS"
                    && e.Value != "JUDGMENTS"
                    && e.Value != "OFFICE PRACTISE NOTICES"
                    && e.Value != "3. DESIGNS")
                    .Where(e => !e.Value.StartsWith("Application number ~ Nature ~ 54: Representation of mark")
                    && e.Value != "THE PATENTS ACT, 1978 (ACT NO. 57 OF 1978)"
                    && e.Value != "No records available."
                    && !e.Value.StartsWith("In terms of section 42 (b) of the Patents Act,")
                    && !e.Value.StartsWith("The numerical references denote the following: (21) Number of application")
                    && e.Value != "No records available")
                    .ToList();

                Console.WriteLine("lal");

                /*Applications for patents*/
                if (firstList != null && firstList.Count() > 0)
                {
                    var appl = new ProcessFirstList();
                    var el = appl.OutputValue(firstList);
                    var legalStatusEvents = ConvertToDiamond.FirstlistConvertation(el);
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

                /*Complete specifications accepted and abridgements or abstracts thereof*/
                if (secondList != null && secondList.Count() > 0)
                {
                    var completeSpec = new ProcessSecondList();
                    var el = completeSpec.OutputValue(secondList);
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
