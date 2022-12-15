using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IN
{
    class Diamond_IN
    {
        public const string _pathToFiles = @"C:\Work\IN\IN_20210827_35(1)";
        public static string sub = "10";
        public static bool _isStaging = true;
        public static FileInfo _currentFileInProcess;
        public static List<string> _txt;
        public static List<string> _sub1Elements;
        public static List<string> _sub3Elements;

        static void Main(string[] args)
        {
            var textFiles = Methods.GetTxtFiles(_pathToFiles);
            var xlsFiles = Methods.GetXlsFiles(_pathToFiles);


                foreach (var textFile in textFiles)
                {
                    _txt = File.ReadAllLines(textFile.FullName).ToList();
                    _sub1Elements = new List<string>();
                    _sub3Elements = new List<string>();
                    _currentFileInProcess = textFile;

                    string currentFileName = Path.GetFileNameWithoutExtension(textFile.FullName);
                    /*TETML elements*/

                    _sub1Elements = _txt
                        .SkipWhile(e => !e.StartsWith(@"Early Publication:") && !e.StartsWith("CONTINUED FROM PART"))
                        .SkipWhile(e => !e.StartsWith(@"(12) PATENT APPLICATION PUBLICATION"))
                        .TakeWhile(e => !e.StartsWith(@"WEEKLY ISSUED") && !e.StartsWith(@"Publication After 18 Months") && !e.StartsWith(@"CONTINUED TO PART- 2"))
                        .ToList();
                    _sub3Elements = _txt
                        //.SkipWhile(e => !e.StartsWith(@"Publication After 18 Months"))
                        .SkipWhile(e => !e.StartsWith(@"(12) PATENT APPLICATION PUBLICATION"))
                        .TakeWhile(e => !e.StartsWith(@"PUBLICATION U/R 84(3) IN RESPECT OF APPLICATION"))
                        .ToList();

                    Console.WriteLine("Elements list completed");

                if (_sub1Elements != null && _sub1Elements.Count() > 0)
                {
                    var sub1records = Subcodes.Process1SubCode(_sub1Elements, "1", "BZ", currentFileName + ".pdf");

                    Console.WriteLine();

                    //   DiamondSender.SendToDiamond(sub1records, _isStaging);

                }

                if (_sub3Elements != null && _sub3Elements.Count() > 0)
                {
                    var sub1records = Subcodes.Process1SubCode(_sub3Elements, "3", "BZ", currentFileName + ".pdf");

                    Console.WriteLine();
                    //     DiamondSender.SendToDiamond(sub1records, _isStaging);
                }
            }


            foreach (var xlsFile in xlsFiles)
            {
                if (sub == "2")
                {
                    string currentFileName = Path.GetFileNameWithoutExtension(xlsFile.FullName);
                    var sub2records = Subcodes.Process2SubCode(xlsFile.FullName, "2", "FG", currentFileName + ".pdf");
                    DiamondSender.SendToDiamond(sub2records, _isStaging);
                }

                if(sub == "10")
                {
                    string currentFileName = Path.GetFileNameWithoutExtension(xlsFile.FullName);
                    var sub10records = Subcodes.Process10SubCode(xlsFile.FullName, "10", "EC", currentFileName + ".pdf");

                    Methods.SendToDiamond(sub10records);
                }
            }

        }
    }
}
