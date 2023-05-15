using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_MK_Andrey
{
    public class Subcodes
    {
        private static readonly string I11 = "(11)";
        private static readonly string I21 = "(21)";
        private static readonly string I13 = "(13)";
        private static readonly string I22 = "(22)";
        private static readonly string I45 = "(45)";
        private static readonly string I30 = "(30)";
        private static readonly string I73 = "(73)";
        private static readonly string I74 = "(74)";
        private static readonly string I72 = "(72)";
        private static readonly string I54 = "(54)";
        private static readonly string I57 = "(57)";
        private static readonly string I51 = "(51)";
        private static readonly string I96 = "(96)";
        private static readonly string I97 = "(97)";

        public class ElementsOut
        {
            public string PubNumber { get; set; }
            public string AppNumber { get; set; }
            public string Kind { get; set; }
            public string AppDate { get; set; }
            public string Date { get; set; }
            public List<Priority> Priorities { get; set; }
            public List<PartyMember> Assignees { get; set; }
            public PartyMember Agent { get; set; }
            public List<PartyMember> Inventors { get; set; }
            public string Title { get; set; }
            public string Abstract { get; set; }
            public Related96 Related96 { get; set; }
            public Related97 Related97 { get; set; }
            public List<string> Ipc { get; set; }
        }

        public class Priority
        {
            public string Number { get; set; }
            public string Country { get; set; }
            public string Date { get; set; }
        }

        public class PartyMember
        {
            public string Name { get; set; }
            public string Country { get; set; }
            public string Address { get; set; }
        }

        public class Related96
        {
            public string AppNumber { get; set; }
            public string AppDate { get; set; }
        }

        public class Related97
        {
            public string PubNumber { get; set; }
            public string PubDate { get; set; }
        }


        public static List<ElementsOut> Sub3Process(List<XElement> elements)
        {
            var elementsOut = new List<ElementsOut>();

            for (var i = 0; i < elements.Count; i++)
            {
                var inc = i;
                string val = null;

                var value = elements[i].Value;
                if (value.StartsWith(I51))
                {
                    var curElem = new ElementsOut();
                    do
                    {
                        val += elements[inc].Value + "\n";
                        inc++;
                    } while (inc < elements.Count && !elements[inc].Value.StartsWith(I51));

                    var abs = val.Substring(val.IndexOf(I57));
                    val = val.Replace(abs, "").Trim();
                    var splittedRecords = Methods.RecSplit(val, new string[] { I11, I13, I21, I22, I30, I45, I51, I54, I57, I72, I73, I74, I96, I97 });
                    splittedRecords.Add(abs);

                    foreach (var record in splittedRecords)
                    {
                        if (record.StartsWith(I11))
                            curElem.PubNumber = record.Replace(I11, "").Trim();

                        if (record.StartsWith(I13))
                            curElem.Kind = record.Replace(I13, "").Trim();

                        if (record.StartsWith(I21))
                            curElem.AppNumber = record.Replace(I21, "").Trim();

                        if (record.StartsWith(I22))
                            curElem.AppDate = Methods.DateNormalize(record.Replace(I22, "").Trim());

                        if (record.StartsWith(I45))
                            curElem.Date = Methods.DateNormalize(record.Replace(I45, "").Trim());

                        if (record.StartsWith(I30))
                        {
                            var priorities = Regex.Matches(record, @"(?<number>\p{Lu}{2,}(\w+)+) (?<date>\d{2}\/\d{2}\/\d{4}) (?<country>\p{Lu}{2})");

                            curElem.Priorities = new List<Priority>();

                            for (var j = 0; j < priorities.Count(); j++)
                            {
                                curElem.Priorities.Add(new Priority
                                {
                                    Number = priorities[j].Groups["number"].Value,
                                    Date = Methods.DateNormalize(priorities[j].Groups["date"].Value),
                                    Country = priorities[j].Groups["country"].Value
                                });
                            }
                        }

                        if (record.StartsWith(I73))
                        {
                            var assignee = record.Replace(I73, "").Trim();
                            var strings = assignee.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                            var name = strings.First();
                            var address = assignee
                                .Replace(strings.First(), "")
                                .Replace("\n", " ")
                                .Replace(assignee.Split(',', StringSplitOptions.RemoveEmptyEntries).Last(), "")
                                .Trim(' ', ',', ' ');
                            var country = assignee.Split(',', StringSplitOptions.RemoveEmptyEntries).Last().Trim();

                            curElem.Assignees = new List<PartyMember>
                            {
                                new PartyMember
                                {
                                    Name = name,
                                    Address = address,
                                    Country = country
                                }
                            };
                        }

                        if (record.StartsWith(I74))
                        {
                            var agent = record.Replace(I74, "").Replace("\n", " ").Trim();
                            var name = "";
                            var address = "";
                            if (agent.Contains("Ул."))
                            {
                                address = agent.Substring(agent.IndexOf("Ул."));
                                name = agent.Replace(address, "").Trim();
                            }
                            else if (Regex.Match(agent, @"(\p{Lu}{2,} ?)+").Success)
                            {
                                name = Regex.Match(agent, @"(\p{Lu}{2,} ?)+").Value.Trim();
                                address = agent.Replace(name, "").Trim();
                            }
                            else
                            {
                                var number = Regex.Match(agent, @"\d+");
                                var indexOfAddress = agent.IndexOf(number.Value);
                                if (indexOfAddress > -1)
                                {
                                    address = agent.Substring(indexOfAddress);
                                    name = agent.Replace(address, "").Trim();
                                }
                            }


                            curElem.Agent = new PartyMember
                            {
                                Name = name,
                                Address = address,
                                Country = "MK"
                            };
                        }

                        if (record.StartsWith(I72))
                        {
                            var inventors = Regex.Matches(record, @"\p{Lu}{2,}, \p{Lu}{1}\p{Ll}+");
                            curElem.Inventors = new List<PartyMember>();

                            foreach (Match inventor in inventors)
                            {
                                curElem.Inventors.Add(new PartyMember
                                {
                                    Name = inventor.Value
                                });
                            }
                        }

                        if (record.StartsWith(I54))
                            curElem.Title = record.Replace(I54, "").Trim();

                        if (record.StartsWith(I57))
                            curElem.Abstract = record.Replace(I57, "").Trim();

                        if (record.StartsWith(I96))
                        {
                            var text = record.Replace(I96, "").Trim();
                            curElem.Related96 = new Related96
                            {
                                AppNumber = text.Split(' ').Last(),
                                AppDate = Methods.DateNormalize(text.Split(' ').First())
                            };
                        }

                        if (record.StartsWith(I97))
                        {
                            var text = record.Replace(I97, "").Trim();
                            curElem.Related97 = new Related97
                            {
                                PubNumber = text.Split(' ').Last(),
                                PubDate = Methods.DateNormalize(text.Split(' ').First())
                            };
                        }

                        if (record.StartsWith(I51))
                        {
                            var ipcs = record.Replace(I51, "").Replace("\n", " ").Split(',', StringSplitOptions.RemoveEmptyEntries);
                            var ipcsOut = new List<string>();

                            for (var j = 0; j < ipcs.Count(); j++)
                            {
                                ipcsOut.Add(ipcs[j]);
                            }

                            curElem.Ipc = ipcsOut;
                        }
                    }

                    elementsOut.Add(curElem);
                }
            }

            return elementsOut;
        }
    }
}
