using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_NL
{
    class Process
    {
        public class SubCombo
        {
            public static void Run(List<XElement> v)
            {
                Console.WriteLine("SubCombo processing started");
                var elements = new List<OutElements.SubCombo>();
                var pat = new Regex(@"(?=\(11\))");
                var splitPat = new Regex(@"\(11\)(?<elem11>.*)\s*\(00\)\s*(?<elemDate>\d{2}\.\d{2}\.\d{4})\s*\(00\)\s*(?<elemOther>.*)");
                var mergeText = string.Join("\n", v.Select(x => x.Value)).Replace("\n", " ");
                var splitText = pat.Split(mergeText).Where(x => x != "").ToList();
                foreach (var item in splitText)
                {
                    var a = splitPat.Match(item.Trim());
                    if (a.Success)
                    {
                        elements.Add(new OutElements.SubCombo
                        {
                            PublNumber = a.Groups["elem11"].Value.Trim(),
                            DateI24 = Methods.DateNormalize(a.Groups["elemDate"].Value.Trim()),
                            LePatNumber = a.Groups["elem11"].Value.Trim(),
                            LeNoteValue = a.Groups["elemOther"].Value.Trim()
                        });
                    }
                    else
                        Console.WriteLine("Value pattern doesn't match!");
                }

                if (elements.Count > 0)
                {
                    Methods.SendToDiamond(ConvertToDiamond.SubCombo(elements));
                    Console.WriteLine("SubCombo processing finished");
                }
                else
                    Console.WriteLine("Something went wrong...");
            }
        }
        public class Sub26
        {
            public static void Run(List<XElement> v)
            {
                Console.WriteLine("Sub26 processing started");
                var elements = new List<OutElements.Sub26>();
                var pat = new Regex(@"(?=\(71\))");
                var splitPat = new Regex(@"(\(71\)(?<I71>.*)\s*)*\(73\)\s*(?<I73>.*)\s*\(11\)\s*(?<I11>.*)\(00\)\s*(?<I00Date>\d{2}\.\d{2}\.\d{4})\s*\(51\)\s*(?<I51>.*)\s*\(00\)\s*(?<I00Other>.*)");
                var mergeText = string.Join("\n", v.Select(x => x.Value)).Replace("\n", " ");
                var splitText = pat.Split(mergeText).Where(x => x != "").ToList();
                foreach (var item in splitText)
                {
                    var a = splitPat.Match(item.Trim());
                    if (a.Success)
                    {
                        elements.Add(new OutElements.Sub26
                        {
                            New71Applicant = a.Groups["I71"].Value.Trim(),
                            New73Assignee = a.Groups["I73"].Value.Trim(),
                            IntClass = Methods.IntClassProcess(a.Groups["I51"].Value.Trim()),
                            PublNumber = a.Groups["I11"].Value.Trim(),
                            DateI24 = Methods.DateNormalize(a.Groups["I00Date"].Value.Trim()),
                            LePatNumber = a.Groups["I11"].Value.Trim(),
                            LeNoteValue = a.Groups["I00Other"].Value.Trim()
                        });
                    }
                    else
                        Console.WriteLine("Value pattern doesn't match!");
                }

                if (elements.Count > 0)
                {
                    Methods.SendToDiamond(ConvertToDiamond.Sub26(elements));
                    Console.WriteLine("Sub26 processing finished");
                }
                else
                    Console.WriteLine("Something went wrong...");
            }
        }
    }
}
