using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_OA
{
    class Diamond_OA_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\OA\Reg\");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> firstList = null; //Patent of invention
            List<XElement> secondList = null; //Utility models

            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                /*TETML elements*/
                /*Patent of invention*/
                firstList = tet.Descendants().Where(d => (d.Name.LocalName == "Text" || d.Name.LocalName == "PlacedImage")
                && !d.Value.Contains("_______________")
                && !d.Value.StartsWith("Consulter le mémoire"))
                    .SkipWhile(e => !e.Value.StartsWith("A\nREPERTOIRE NUMERIQUE") && !Regex.IsMatch(e.Value, @"A\nREPERTOIRE\s*N\s*UMERIQUE$"))
                    .TakeWhile(e => !e.Value.StartsWith("B\nREPERTOIRE SUIVANT")
                    && !e.Value.Contains("B\nREPERTOIRE SUIVANT")
                    && !Regex.IsMatch(e.Value, @"REPERTOIRE\s*SUI\s*VANT"))
                    .ToList();
                /*Utility models*/
                secondList = tet.Descendants().Where(d => (d.Name.LocalName == "Text" || d.Name.LocalName == "PlacedImage")
                && !d.Value.Contains("_______________")
                && !d.Value.StartsWith("Consulter le mémoire"))
                    .SkipWhile(e => e.Value != "TROISIEME PARTIE"
                    //&& !e.Value.StartsWith("TROISIEME PARTIE\nMODELES D’UTILITE")
                    //&& !Regex.IsMatch(e.Value, @"^MODELES\s*D\s*\’UTILITE$")
                    )
                    //.TakeWhile(e => !e.Value.Contains("B\nREPERTOIRE SUIVANT") && !e.Value.Contains("B\nREPERTOIRE SU"))
                    .ToList();


                Console.WriteLine("lal");

                if (firstList != null && firstList.Count() > 0)
                {
                    ProcessFirstList grantedEP = new ProcessFirstList();
                    List<ProcessFirstList.ElementOut> el = grantedEP.OutputValue(firstList);
                    var legalStatusEvents = ConvertToDiamond.FirstListConvertation(el);
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

                /*Utility models*/
                if (secondList != null && secondList.Count() > 0)
                {
                    ProcessSecondtList grantedEP = new ProcessSecondtList();
                    List<ProcessSecondtList.ElementOut> el = grantedEP.OutputValue(secondList);
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
