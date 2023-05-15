using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_RS
{
    class Processing
    {
        public const string PubNumber = "(11)";
        public const string AppNumber = "(21)";
        public const string Ipc = "(51)";
        public const string Agent = "(74)";
        public const string Inventor = "(72)";
        public const string GranteeAssigneeOwner = "(73)";
        public const string Title = "(54)";
        public const string EP96 = "(96)";
        public const string EP97 = "(97)";
        public const string PCT86 = "(86)";
        public const string PCT87 = "(87)";
        public const string Priority = "(30)";
        public const string AppDate = "(22)";

        public static List<Elements> Sub3Process(List<XElement> elements)
        {
            var elementsOut = new List<Elements>();

            for (var i = 0; i < elements.Count; i++)
            {
                var inc = i;
                string val = null;

                var value = elements[i].Value;
                if (value.StartsWith(Ipc))
                {
                    var curElem = new Elements();
                    do
                    {
                        val += elements[inc].Value + "\n";
                        ++inc;
                    } while (inc < elements.Count && !elements[inc].Value.StartsWith(Ipc));

                    var splittedRecords = Methods.RecSplit(val, new string[] { PubNumber, AppNumber, Ipc, Agent, Inventor, GranteeAssigneeOwner, Title, EP96, PCT86, Priority, AppDate });

                    foreach (var record in splittedRecords)
                    {
                        if (record.StartsWith(Ipc))
                        {
                            var strings = record.Replace(Ipc, "").Trim().Split('\n');
                            curElem.Ipcs = new List<ClassificationIpc>();
                            foreach (var str in strings)
                            {
                                curElem.Ipcs.Add(new ClassificationIpc
                                {
                                    Classification = str
                                });
                            }
                        }

                        if (record.StartsWith(PubNumber))
                        {
                            curElem.Publication = new Publication
                            {
                                PubNumber = record.Replace(PubNumber, "").Trim().Split(' ').First(),
                                Kind = record.Replace(PubNumber, "").Trim().Split(' ').Last()
                            };
                        }

                        if (record.StartsWith(Priority))
                        {
                            var strings = record.Replace(Priority, "").Trim().Split(' ');
                            curElem.Priority = new Priority
                            {
                                Country = strings[0],
                                Date = strings[1],
                                Number = strings[2]
                            };
                        }

                        if (record.StartsWith(PCT86))
                        {
                            var pct86 = record.Substring(0, record.IndexOf(PCT87)).Replace(PCT86, "").Trim();
                            var pct87 = record.Replace(pct86, "").Replace(PCT86, "").Replace(PCT87, "").Trim();

                            curElem.Pct = new PCT
                            {
                                AppNumber = pct86.Split(' ')[2],
                                AppDate = pct86.Split(' ')[1],
                                PubDate = pct87.Split(' ')[1],
                                PubNumber = pct87.Split(' ')[2] + "/" + pct87.Split(' ')[3]
                            };
                        }

                        if (record.StartsWith(EP96))
                        {
                            var ep96 = record.Substring(0, record.IndexOf(EP97)).Replace(EP96, "").Trim();
                            var ep97 = record.Replace(ep96, "").Replace(EP96, "").Replace(EP97, "").Trim();

                            curElem.EuropeanPatents = new EuropeanPatents
                            {
                                AppNumber = ep96.Split(' ')[1],
                                AppDate = ep96.Split(' ')[0],
                                PubDate = ep97.Split(' ')[0],
                                PubNumber = ep97.Replace(ep97.Split(' ')[0], "").Trim().Replace(ep97.Split(' ').Last(), "").Trim(),
                                Country = ep97.Split(' ').Last()
                            };
                        }

                        if (record.StartsWith(Title))
                        {
                            curElem.Title = new Title
                            {
                                Text = record.Replace(Title, "").Trim(),
                                Language = "RS"
                            };
                        }

                        if (record.StartsWith(GranteeAssigneeOwner))
                        {
                            var assignees = record.Replace(GranteeAssigneeOwner, "").Split(';', StringSplitOptions.RemoveEmptyEntries);
                            curElem.GranteeAssigneeOwner = new List<PartyMember>();
                            foreach (var assignee in assignees)
                            {
                                var text = assignee.Replace("\n", " ").Trim();
                                curElem.GranteeAssigneeOwner.Add(new PartyMember
                                {
                                    Name = text.Split(',').First(),
                                    Country = text.Split(',').Last(),
                                    Address = text.Replace(text.Split(',').First(), "").Replace(text.Split(',').Last(), "").Trim(' ', ',', ' ')
                                });
                            }
                        }

                        if (record.StartsWith(Inventor))
                        {
                            var assignees = record.Replace(Inventor, "").Split(';', StringSplitOptions.RemoveEmptyEntries);
                            curElem.Inventors = new List<PartyMember>();
                            foreach (var assignee in assignees)
                            {
                                var text = assignee.Replace("\n", " ").Trim();
                                curElem.Inventors.Add(new PartyMember
                                {
                                    Name = text.Split(',').First(),
                                    Country = text.Split(',').Last(),
                                    Address = text.Replace(text.Split(',').First(), "").Replace(text.Split(',').Last(), "").Trim(' ', ',', ' ')
                                });
                            }
                        }

                        if (record.StartsWith(Agent))
                        {
                            var assignees = record.Replace(Agent, "").Split(';', StringSplitOptions.RemoveEmptyEntries);
                            curElem.Agents = new List<PartyMember>();
                            foreach (var assignee in assignees)
                            {
                                var text = assignee.Replace("\n", " ").Trim();
                                curElem.Agents.Add(new PartyMember
                                {
                                    Name = text.Split(',').First(),
                                    Address = text.Replace(text.Split(',').First(), "").Trim(' ', ',', ' ')
                                });
                            }
                        }

                        if (record.StartsWith(AppDate))
                            curElem.AppDate = record.Replace(AppDate, "").Trim();
                    }

                    elementsOut.Add(curElem);
                }
            }

            return elementsOut;
        }
    }
}
