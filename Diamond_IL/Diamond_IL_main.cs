using System;
using System.Collections.Generic;
using System.IO;

namespace Diamond_IL
{
    class Diamond_IL_main
    {
        public class ElementOut
        {
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string[] I31 { get; set; }
            public string[] I32 { get; set; }
            public string[] I33 { get; set; }
            public string I51Version { get; set; }
            public string[] I51Number { get; set; }
            public string I54Eng { get; set; }
            public string I54Hebrew { get; set; }
            public string I62 { get; set; }
            public string[] I71NameENG { get; set; }
            public string[] I71CountryENG { get; set; }
            public string[] I71NameIL { get; set; }
            public string[] I71CountryIL { get; set; }
            public string[] I72Eng { get; set; }
            public string[] I72IL { get; set; }
            public string I74EngName { get; set; }
            public string I74EngAddr { get; set; }
            public string I74HebName { get; set; }
            public string I74HebAddr { get; set; }
            public string I87 { get; set; }
        }
        public static string CurrentFileName;
        static List<ElementOut> ElementsOut = new List<ElementOut>();

        static void Main(string[] args)
        {
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\IL\20200203\1");
            /*list of txt files with applications_with_inids for 3 section code */
            var filesA = new List<string>();
            /*list of txt files with applications_with_inids for 1 section code */
            var filesB = new List<string>();
            /*list of txt files with applications_without_inids*/
            var filesC = new List<string>();
            /*list of txt files with only reg numbers*/
            var filesD = new List<string>();
            var filesE = new List<string>();
            var filesF = new List<string>();
            var filesG = new List<string>();
            var filesH = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*_APPA.txt", SearchOption.AllDirectories)) { filesA.Add(file.FullName); } /*3 subcode from big file with other chapters*/
            foreach (FileInfo file in dir.GetFiles("*_APPB.txt", SearchOption.AllDirectories)) { filesB.Add(file.FullName); } /*1 subcode from file with only */
            foreach (FileInfo file in dir.GetFiles("*_APPNOINIDS.txt", SearchOption.AllDirectories)) { filesC.Add(file.FullName); }
            foreach (FileInfo file in dir.GetFiles("*_NUM.txt", SearchOption.AllDirectories)) { filesD.Add(file.FullName); } // _REG_NUM, _EXP_NUM, _NONPAY_NUM, _REN_NUM, _REN20_NUM
            /*Applications with INIDs processing, subcode 1*/
            if (filesA.Count > 0) foreach (var txtFile in filesA)
                {
                    CurrentFileName = txtFile;
                    /**/
                    string txtFileValue = File.ReadAllText(txtFile);

                    Console.WriteLine("Applications with INIDs processing, sub 3");

                    if (txtFileValue != null)
                    {
                        Process_Apps_INID appWithInidThirdChapter = new Process_Apps_INID();
                        List<Process_Apps_INID.ElementOut> el = appWithInidThirdChapter.OutputValue(txtFileValue);
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
            /*Applications with INIDs processing, subcode 3*/
            if (filesB.Count > 0) foreach (var txtFile in filesB)
                {
                    CurrentFileName = txtFile;
                    /**/
                    string txtFileValue = File.ReadAllText(txtFile);

                    Console.WriteLine("Applications with INIDs processing, sub 1");

                    if (txtFileValue != null)
                    {
                        Process_Apps_INID appWithInidThirdChapter = new Process_Apps_INID();
                        List<Process_Apps_INID.ElementOut> el = appWithInidThirdChapter.OutputValue(txtFileValue);
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
            /*Applications without INIDs processing, subcode 2*/
            if (filesC.Count > 0) foreach (var txtFile in filesC)
                {
                    CurrentFileName = txtFile;
                    /**/
                    string txtFileValue = File.ReadAllText(txtFile);

                    Console.WriteLine("Applications without INIDs processing, sub 2");

                    if (txtFileValue != null)
                    {
                        ProcessAppNoInids appNoInids = new ProcessAppNoInids();
                        List<ProcessAppNoInids.ElementOut> el = appNoInids.OutputValue(txtFileValue);
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
            /*Only numbers processing*/
            if (filesD.Count > 0) foreach (var txtFile in filesD)
                {
                    CurrentFileName = txtFile;
                    string txtFileValue = File.ReadAllText(txtFile);
                    Console.WriteLine("numbers processing, subs 4,5,6,7,8");

                    if (txtFileValue != null)
                    {
                        ProcessNumbers numbers = new ProcessNumbers();
                        List<ProcessNumbers.ElementOut> el = numbers.OutputValue(txtFileValue);
                        var legalStatusEvents = ConvertToDiamond.FourthListConvertation(el);
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
