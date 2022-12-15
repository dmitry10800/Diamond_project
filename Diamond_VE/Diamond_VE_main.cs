using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_VE
{
    class Diamond_VE_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\VE\FullTest");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> firstList = null; //Solicitudes de patente de invencion publicadas a efecto de oposiciones
            //List<XElement> secondList = null; //Publication of European Patents Entered in Patent Register

            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);

                /*TETML elements*/
                /*Publication of Patent Application*/
                firstList = tet.Descendants().Where(d => d.Name.LocalName == "Text")
                    .SkipWhile(e => !e.Value.StartsWith("SOLICITUDES DE PATENTE DE INVENCION PUBLICADAS A EFECTO DE OPOSICIONES"))
                    .TakeWhile(e => !Regex.IsMatch(e.Value, @"SOLICITADAS DE PATENTES")
                    && !e.Value.StartsWith("BOLETÍN DE LA PROPIEDAD INDUSTRIAL")
                    && !e.Value.StartsWith("Total de Solicitudes :")
                    )
                    .Where(e => !e.Value.StartsWith("BOLETÍN DE LA PROPIEDAD INDUSTRIAL")
                    && !e.Value.StartsWith("SOLICITUDES DE PATENTE DE INVENCION PUBLICADAS A EFECTO DE OPOSICIONES")
                    && !e.Value.StartsWith("________________________"))
                    .ToList();

                Console.WriteLine("lal");

                /*Solicitudes de patente de invencion publicadas a efecto de oposiciones**/
                if (firstList != null && firstList.Count() > 0)
                {
                    ProcessFirstList patGrant = new ProcessFirstList();
                    List<ProcessFirstList.ElementOut> el = patGrant.OutputValue(firstList);
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
            }
        }
    }
}
