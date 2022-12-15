using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_AR
{
    class Diamond_AR_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\AR\20190930\3");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> firstList = null;

            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                /*TETML elements*/

                firstList = tet.Descendants().Where(d => d.Name.LocalName == "Text" || d.Name.LocalName == "PlacedImage")
                    .SkipWhile(e => !e.Value.StartsWith("PUBLICACIONES ADELANTADAS") && !e.Value.StartsWith("PUBLICACIONES DE TRAMITE NORMAL"))
                    .TakeWhile(e => !e.Value.Contains("Boletín de Marcas y/o Patentes por ejemp") && !e.Value.Contains("República Argentina - Poder Ejecutivo Nacional"))
                    .Where(e => !e.Value.StartsWith("PUBLICACIONES DE TRAMITE") && !e.Value.StartsWith("PUBLICACIONES ADELANTADAS") && !e.Value.StartsWith("SOLICITUDES DE PATENTE") && !e.Value.StartsWith("SOLICITUDES E PATENTE"))
                    .ToList();

                Console.WriteLine("lal");

                if (firstList != null && firstList.Count() > 0)
                {
                    ProcessApplications grantedEP = new ProcessApplications();
                    List<ProcessApplications.ElementOut> el = grantedEP.OutputValue(firstList);
                    var legalStatusEvents = ConvertToDiamond.ApplicationsConvertation(el);
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
