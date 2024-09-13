using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IN
{
    internal class Diamond_IN
    {
        public const string PathToFiles = @"D:\LENS\TET\IN\IN_20240830_35(1)";
        public static string Sub = "10";
        public static bool IsStaging = true;
        public static FileInfo CurrentFileInProcess;
        public static List<string> Txt;
        public static List<string> Sub1Elements;
        public static List<string> Sub3Elements;

        private static void Main(string[] args)
        {
            var textFiles = Methods.GetTxtFiles(PathToFiles);
            var xlsFiles = Methods.GetXlsFiles(PathToFiles);


                foreach (var textFile in textFiles)
                {
                    Txt = File.ReadAllLines(textFile.FullName).ToList();
                    Sub1Elements = new List<string>();
                    Sub3Elements = new List<string>();
                    CurrentFileInProcess = textFile;

                    var currentFileName = Path.GetFileNameWithoutExtension(textFile.FullName);
                    /*TETML elements*/

                    Sub1Elements = Txt
                        .SkipWhile(e => !e.StartsWith(@"Early Publication:") && !e.StartsWith("CONTINUED FROM PART"))
                        .SkipWhile(e => !e.StartsWith(@"(12) PATENT APPLICATION PUBLICATION"))
                        .TakeWhile(e => !e.StartsWith(@"WEEKLY ISSUED") && !e.StartsWith(@"Publication After 18 Months") && !e.StartsWith(@"CONTINUED TO PART- 2"))
                        .ToList();
                    Sub3Elements = Txt
                        //.SkipWhile(e => !e.StartsWith(@"Publication After 18 Months"))
                        .SkipWhile(e => !e.StartsWith(@"(12) PATENT APPLICATION PUBLICATION"))
                        .TakeWhile(e => !e.StartsWith(@"PUBLICATION U/R 84(3) IN RESPECT OF APPLICATION"))
                        .ToList();

                    Console.WriteLine("Elements list completed");

                if (Sub1Elements != null && Sub1Elements.Any())
                {
                    var sub1Records = Subcodes.Process1SubCode(Sub1Elements, "1", "BZ", currentFileName + ".pdf");

                    Console.WriteLine();

                      DiamondSender.SendToDiamond(sub1Records, IsStaging);

                }

                if (Sub3Elements != null && Sub3Elements.Any())
                {
                    var sub1Records = Subcodes.Process1SubCode(Sub3Elements, "3", "BZ", currentFileName + ".pdf");

                    Console.WriteLine(); 
                    DiamondSender.SendToDiamond(sub1Records, IsStaging);
                }
            }


            foreach (var xlsFile in xlsFiles)
            {
                switch (Sub)
                {
                    case "2":
                    {
                        var currentFileName = Path.GetFileNameWithoutExtension(xlsFile.FullName);
                        var sub2records = Subcodes.Process2SubCode(xlsFile.FullName, "2", "FG", currentFileName + ".pdf");
                        DiamondSender.SendToDiamond(sub2records, IsStaging);
                        break;
                    }
                    case "10":
                    {
                        var currentFileName = Path.GetFileNameWithoutExtension(xlsFile.FullName);
                        var sub10records = Subcodes.Process10SubCode(xlsFile.FullName, "10", "EC", currentFileName + ".pdf");

                        Methods.SendToDiamond(sub10records);
                        break;
                    }
                }
            }

        }
    }
}
