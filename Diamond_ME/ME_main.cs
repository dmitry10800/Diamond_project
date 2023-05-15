﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_ME
{
    class ME_main
    {
        public static string CurrentFileName;
        static void Main()
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"C:\Work\ME\ME_20210420_38");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (var file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> firstList = null;

            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                /*TETML elements*/

                /*List of Lapsed Patent Applications*/
                firstList = tet.Descendants().Where(d => d.Name.LocalName == "Text")
                    .SkipWhile(e => e.Value != "Objava zahtjeva za proširenje evropskih prijava patenata")
                    .TakeWhile(e => e.Value != "Indeks brojeva zahtjeva za proširenje evropskih prijava patenta")
                    .ToList();

                Console.WriteLine("***************************" + Path.GetFileNameWithoutExtension(tetFile) + "***************************");

                if (firstList != null && firstList.Count() > 0)
                {
                    var firstListValues = new ProcessPublicationsOfEuroApp();
                    var el = firstListValues.OutputValue(firstList);
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
            }
        }
    }
}
