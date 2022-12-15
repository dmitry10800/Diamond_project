using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LT
{
    public struct Sub7Elements
    {
        public string Date { get; set; }
        public string I11 { get; set; }
    }

    class LT_main
    {
        public static string IPath { get; set; } = @"D:\_DFA_main\_Patents\LT\20200210";
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            TxtProcess();
        }

        public static List<string> GetTxtFiles(string path)
        {
            return new DirectoryInfo(path).GetFiles("*.txt").Select(x => x.FullName).ToList();
        }
        public static void TxtProcess()
        {
            var files = GetTxtFiles(IPath);
            if (files.Count == 0)
            {
                Console.WriteLine("Txt files was not detected!");
                return;
            }
            else
            {
                foreach (var file in files)
                {
                    CurrentFileName = file;
                    if (Path.GetFileNameWithoutExtension(file).EndsWith("sub7"))
                    {
                        var elements = new List<Sub7Elements>();
                        var data = File.ReadAllLines(file);
                        var pattern = new Regex(@"(?<Number>.*)\s(?<Date>\d{4}-\d{2}-\d{2})");
                        foreach (var record in data)
                        {
                            var match = pattern.Match(record);
                            if (match.Success)
                            {
                                elements.Add(new Sub7Elements
                                {
                                    I11 = match.Groups["Number"].Value.Trim(),
                                    Date = match.Groups["Date"].Value.Trim()
                                });
                            }
                            else
                                Console.WriteLine("Record doesn't match:\t" + record);
                        }
                        if (elements.Count > 0)
                        {
                            Methods.SendToDiamond(ConvertToDiamond.SubCode7(elements));
                        }
                    }
                }
            }
        }
    }
}
