using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diamond_AT
{
    public struct I73Struct
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
    }
    public struct Sub1Elements
    {
        public string I51 { get; set; }
        public string I21 { get; set; }
        public string I11 { get; set; }
        public List<I73Struct> I73new { get; set; }
    }

    class Diamond_AT_main
    {
        public static string IPath { get; set; } = @"D:\_DFA_main\_Patents\AT\20200129";
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            TxtProcess();
            TetmlProcess();
        }

        public static List<string> GetTxtFiles(string path)
        {
            return new DirectoryInfo(path).GetFiles("*.txt").Select(x => x.FullName).ToList();
        }
        public static List<string> GetTetmlFiles(string path)
        {
            return new DirectoryInfo(path).GetFiles("*.tetml").Select(x => x.FullName).ToList();
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
                    if (Path.GetFileNameWithoutExtension(file).EndsWith("sub2"))
                    {
                        var data = File.ReadAllText(file).Replace("\r\n", " ").Replace("\n", " ").Trim();
                        var pattern = new Regex(@"[A-Z]{1}\s*\b\d+\b");
                        List<string> numbers = pattern.Matches(data).Cast<Match>().Select(x => x.Value).ToList();
                        var exportData = ConvertToDiamond.SecondSubcode(numbers);
                        Methods.SendToDiamond(exportData);
                    }
                    else
                    if (Path.GetFileNameWithoutExtension(file).EndsWith("sub5"))
                    {
                        var data = File.ReadAllText(file).Replace("\r\n", " ").Replace("\n", " ").Trim();
                        var pattern = new Regex(@"[A-Z]{1}\s*\b\d+\b");
                        List<string> numbers = pattern.Matches(data).Cast<Match>().Select(x => x.Value).ToList();
                        var exportData = ConvertToDiamond.FifthSubcode(numbers);
                        Methods.SendToDiamond(exportData);
                    }
                    else
                    if (Path.GetFileNameWithoutExtension(file).EndsWith("sub4"))
                    {
                        var data = File.ReadAllText(file).Replace("\r\n", " ").Replace("\n", " ").Trim();
                        var pattern = new Regex(@"[A-Z]{1}\s*\b\d+\b");
                        List<string> numbers = pattern.Matches(data).Cast<Match>().Select(x => x.Value).ToList();
                        var exportData = ConvertToDiamond.FourthSubcode(numbers);
                        Methods.SendToDiamond(exportData);
                    }
                    else
                    if (Path.GetFileNameWithoutExtension(file).EndsWith("sub1"))
                    {
                        List<Sub1Elements> elements = new List<Sub1Elements>();
                        Regex patternMain = new Regex(@"((?<I51>[A-Z]{1}\d{2}[A-Z]{1})\s)*(?<I11>E*\s*\d+)\s((?<I21>.*\/\d{4})\s)*(?<I73new>.*\.)");
                        Regex patternOnlyName = new Regex(@"(?<Name>^[^\d,]+),\s*.*\((?<Address>[A-Z]{2})\)\.");
                        var data = File.ReadAllText(file).Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Replace("\r", "").Trim()).ToList();
                        string tmp51Value = null;
                        foreach (var item in data)
                        {
                            var tmpItem = item.Replace(", Ltd,", " Ltd,");
                            if (patternMain.IsMatch(tmpItem) && !patternOnlyName.IsMatch(tmpItem))
                            {
                                var matchedValue = patternMain.Match(tmpItem);
                                if (!string.IsNullOrEmpty(matchedValue.Groups["I51"].Value.Trim()))
                                    tmp51Value = matchedValue.Groups["I51"].Value.Trim();
                                Sub1Elements record = new Sub1Elements
                                {
                                    I51 = matchedValue.Groups["I51"].Value.Trim(),
                                    I11 = matchedValue.Groups["I11"].Value.Trim(),
                                    I21 = matchedValue.Groups["I21"].Value.Trim(),
                                    I73new = new List<I73Struct> { Methods.Sub1OwnerSplit(matchedValue.Groups["I73new"].Value.Trim()) }
                                };

                                if (string.IsNullOrEmpty(record.I51))
                                    record.I51 = tmp51Value;

                                elements.Add(record);
                            }
                            else
                            {
                                elements.LastOrDefault().I73new.Add(Methods.Sub1OwnerSplit(tmpItem.Trim()));
                            }
                        }
                        if (elements.Count > 0)
                        {
                            var exportData = ConvertToDiamond.FirstSubcode(elements);
                            //Methods.SendToDiamond(exportData);
                        }
                    }
                }
            }
        }

        public static void TetmlProcess()
        {
            var files = GetTetmlFiles(IPath);
            if (files.Count == 0)
            {
                Console.WriteLine("Tetml files was not detected!");
                return;
            }
            else
            {
                foreach (var file in files)
                {
                    CurrentFileName = file;
                }
            }
        }
    }
}
