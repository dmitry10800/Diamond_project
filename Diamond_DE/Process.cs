using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_DE
{
    class Process
    {
        public class Sub1
        {
            public static void Run(List<XElement> v)
            {
                Console.WriteLine("Sub1 processing started");
                List<OutElements.Sub1> elements = new List<OutElements.Sub1>();
                var I71Separator = "Anm:";
                var I74Separator = "Vtr:";
                var checkPattern = new Regex(@"(?<number>.*)\s(?<date>\d{2}\.\d{2}\.\d{4}).*");
                for (int i = 0; i < v.Count; i++)
                {
                    var strValue = "";
                    do
                    {
                        strValue += v[i].Value + "\n";
                        ++i;

                    } while (i < v.Count && !(v[i - 1].Value.Contains(I71Separator) || v[i - 1].Value.Contains(I74Separator)));
                    i--;
                    if (strValue != "")
                    {
                        string tmpOwner = null;
                        string tmpAgent = null;
                        if (strValue.Contains(I71Separator))
                        {
                            tmpOwner = strValue.Substring(strValue.IndexOf(I71Separator)).Replace(I71Separator, "").Trim();
                            strValue = strValue.Remove(strValue.IndexOf(I71Separator)).Trim();
                        }
                        else if (strValue.Contains(I74Separator))
                        {
                            tmpAgent = strValue.Substring(strValue.IndexOf(I74Separator)).Replace(I74Separator, "").Trim();
                            strValue = strValue.Remove(strValue.IndexOf(I74Separator)).Trim();
                        }
                        else
                            Console.WriteLine($"Record doesn't match required rules!\t{strValue}");
                        var splValues = strValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var item in splValues)
                        {
                            var match = checkPattern.Match(item);
                            if (match.Success)
                            {
                                elements.Add(new OutElements.Sub1
                                {
                                    AppNumber = match.Groups["number"].Value,
                                    LePatNumber = match.Groups["number"].Value,
                                    DateI43 = Methods.DateNormalize(match.Groups["date"].Value),
                                    New71Applicant = Methods.NamesProcess(tmpOwner),
                                    New74Agent = Methods.NamesProcess(tmpAgent)
                                });
                            }
                            else
                                Console.WriteLine($"Number and Date pattern doesn't match!\t{item}");
                        }
                    }
                }
                if (elements.Count > 0)
                {
                    Methods.SendToDiamond(ConvertToDiamond.Sub1(elements));
                    Console.WriteLine("Sub1 processing finished");
                }
                else
                    Console.WriteLine("Something went wrong...");
            }
        }
        public class Sub3
        {
            public static void Run(List<XElement> v)
            {
                Console.WriteLine("Sub3 processing started");
                List<OutElements.Sub3> elements = new List<OutElements.Sub3>();
                var I73Separator = "Inh:";
                var I74Separator = "Vtr:";
                var checkPattern = new Regex(@"(?<number>.*)\s(?<date>\d{2}\.\d{2}\.\d{4}).*");
                for (int i = 0; i < v.Count; i++)
                {
                    var strValue = "";
                    do
                    {
                        strValue += v[i].Value + "\n";
                        ++i;

                    } while (i < v.Count && !(v[i - 1].Value.Contains(I73Separator) || v[i - 1].Value.Contains(I74Separator)));
                    i--;
                    if (strValue != "")
                    {
                        string tmpAssignee = null;
                        string tmpAgent = null;
                        if (strValue.Contains(I73Separator))
                        {
                            tmpAssignee = strValue.Substring(strValue.IndexOf(I73Separator)).Replace(I73Separator, "").Trim();
                            strValue = strValue.Remove(strValue.IndexOf(I73Separator)).Trim();
                        }
                        else if (strValue.Contains(I74Separator))
                        {
                            tmpAgent = strValue.Substring(strValue.IndexOf(I74Separator)).Replace(I74Separator, "").Trim();
                            strValue = strValue.Remove(strValue.IndexOf(I74Separator)).Trim();
                        }
                        else
                            Console.WriteLine($"Record doesn't match required rules!\t{strValue}");
                        var splValues = strValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var item in splValues)
                        {
                            var match = checkPattern.Match(item);
                            if (match.Success)
                            {
                                elements.Add(new OutElements.Sub3
                                {
                                    AppNumber = match.Groups["number"].Value,
                                    LePatNumber = match.Groups["number"].Value,
                                    DateI45 = Methods.DateNormalize(match.Groups["date"].Value),
                                    New73Assignee = Methods.NamesProcess(tmpAssignee),
                                    New74Agent = Methods.NamesProcess(tmpAgent)
                                });
                            }
                            else
                                Console.WriteLine($"Number and Date pattern doesn't match!\t{item}");
                        }
                    }
                }
                if (elements.Count > 0)
                {
                    Methods.SendToDiamond(ConvertToDiamond.Sub3(elements));
                    Console.WriteLine("Sub3 processing finished");
                }
                else
                    Console.WriteLine("Something went wrong...");
            }
        }

        public class Sub6
        {
            public static void Run(List<XElement> v)
            {
                Console.WriteLine("Sub6 processing started");
                List<OutElements.Sub6> elements = new List<OutElements.Sub6>();
                Regex recordPattern = new Regex(@"(?<IPC>[A-Z]{1}\d{2}[A-Z]{1}\s\d+\/\d+)\s(?<AppNumber>.*)\s(?<Date>\d{2}\.\d{2}\.\d{4})");
                foreach (var record in v)
                {
                    var match = recordPattern.Match(record.Value.Replace("\n", " "));
                    if (match.Success)
                    {
                        elements.Add(new OutElements.Sub6
                        {
                            AppNumber = match.Groups["AppNumber"].Value.Trim(),
                            IpcClass = match.Groups["IPC"].Value.Trim(),
                            DateI43 = Methods.DateNormalize(match.Groups["Date"].Value.Trim())
                        });
                    }
                    else
                        Console.WriteLine("Value doesn't match with pattern");
                }

                if (elements.Count > 0)
                {
                    Methods.SendToDiamond(ConvertToDiamond.Sub6(elements));
                    Console.WriteLine("Sub6 processing finished");
                }
                else
                    Console.WriteLine("Something went wrong...");
            }
        }

        public class Sub7
        {
            public static void Run(List<XElement> v)
            {
                Console.WriteLine("Sub7 processing started");
                List<OutElements.Sub7> elements = new List<OutElements.Sub7>();
                Regex recordPattern = new Regex(@"(?<IPC>[A-Z]{1}\d{2}[A-Z]{1}\s\d+\/\d+)\s(?<AppNumber>.*)\s(?<Date>\d{2}\.\d{2}\.\d{4})");
                foreach (var record in v)
                {
                    var match = recordPattern.Match(record.Value.Replace("\n", " "));
                    if (match.Success)
                    {
                        elements.Add(new OutElements.Sub7
                        {
                            AppNumber = match.Groups["AppNumber"].Value.Trim(),
                            IpcClass = match.Groups["IPC"].Value.Trim(),
                            DateI45 = Methods.DateNormalize(match.Groups["Date"].Value.Trim())
                        });
                    }
                    else
                        Console.WriteLine("Value doesn't match with pattern");
                }

                if (elements.Count > 0)
                {
                    Methods.SendToDiamond(ConvertToDiamond.Sub7(elements));
                    Console.WriteLine("Sub7 processing finished");
                }
                else
                    Console.WriteLine("Something went wrong...");
            }
        }
    }
}
