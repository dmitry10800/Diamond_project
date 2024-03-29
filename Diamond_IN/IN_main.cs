﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Diamond_IN
{
    class IN_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            var folderPath = @"C:\Work\IN\IN_20210101_01(1)";
            var FerFilesGet = new List<string>(Directory.GetFiles(folderPath, "*_TableFER.txt")); //Copy/Paste text from original pdf to txt file that ends with _TableFER.txt
            if (FerFilesGet.Count > 0)
            {
                foreach (var file in FerFilesGet)
                {
                    CurrentFileName = file.Remove(file.IndexOf("_TableFER.txt")) + ".pdf";
                    var tableFebData = new ProcessFebTableData();
                    var el = tableFebData.OutputValue(file);
                    var legalStatusEvents = ConvertToDiamond.FerTableConvertation(el);
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
