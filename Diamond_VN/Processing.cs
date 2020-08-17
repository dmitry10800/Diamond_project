using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Diamond_VN
{
    class Processing
    {
        public static Regex PubNumber = new Regex(@"\(11\)\s*\d+");
        public static Regex AppNumber = new Regex(@"\(21\)\s*\d{1}-\d{4}-\d+");
        public static Regex AppDate = new Regex(@"\(22\)\s*\d{2}(\.|\/)\d{2}(\.|\/)\d{4}");
        public static Regex Priority = new Regex(@"\(30\)\s*(\d+|PCT\/)");
        public static Regex Inventor12 = new Regex(@"\(7(2|5)\)");
        public static Regex Inventor14 = new Regex(@"\(7[2|6]\)");
        public static string PubDate43 = "(43)";
        public static string PubDate45 = "(45)";
        public static string Ipc = "(51)";
        public static string Title = "(54)";
        public static string Abstract = "(57)";
        public static Regex Applicant = new Regex(@"\(71\)\s*(\d+\.|\p{Lu}+)");
        public static string Agent = "(74)";
        public static Regex PCT86 = new Regex(@"\(86\)\s*PCT\/");
        public static string PCT87 = "(87)";
        public static string ApplicantInventor = "(75)";
        public static Regex Grantee = new Regex(@"\(73\)\s*(\p{Lu}+|\d\.)");
        public static string Related = "(67)";
        public static string InvAppGrant = "(76)";
        public static string Inventor15 = "(72)";
        public static string Note = "(15)";
        public static string Field85 = "(85)";
        public static string Related62 = "(62)";

        public static List<Applications> SubCode12Process(MatchCollection elements)
        {
            List<Applications> elementsOut = new List<Applications>();

            if (elements == null)
                return null;

            for (int i = 0; i < elements.Count; i++)
            {
                int tmpInc = i;
                string tmpValue = null;

                var value = elements[i].Value;

                if (PubNumber.Match(value).Success)
                {
                    var currentElement = new Applications();
                    do
                    {
                        tmpValue += elements[tmpInc].Value + "\n";
                        ++tmpInc;
                    } while (tmpInc < elements.Count && !PubNumber.Match(elements[tmpInc].Value).Success && !elements[tmpInc].Value.StartsWith("(11)"));

                    tmpValue = tmpValue.Replace("<Text>", "").Replace("</Text>", "");

                    var splittedRecords = Methods.RecSplit(tmpValue, new string[] { PubNumber.Match(tmpValue).Value, AppNumber.Match(tmpValue).Value, AppDate.Match(tmpValue).Value, PCT86.Match(tmpValue).Value, Priority.Match(tmpValue).Value, Ipc, PubDate43, PCT87, Applicant.Match(tmpValue).Value, Inventor15, ApplicantInventor, Agent, Title, Abstract, Field85 });

                    foreach (var record in splittedRecords)
                    {
                        if (PubNumber.Match(record).Success)
                            currentElement.PubNumber = record.Replace("(11)", "").Trim();

                        if (record.StartsWith(Field85))
                            currentElement.Date85 = Methods.DateNormalize(record.Replace(Field85, "").Trim());

                        if (currentElement.PubNumber == "69853 A")
                        {

                        }

                        if (AppNumber.Match(record).Success)
                            currentElement.AppNumber = record.Replace("(21)", "").Trim();

                        if (AppDate.Match(record).Success)
                            currentElement.AppDate = Methods.DateNormalize(record.Replace("(22)", "").Trim());

                        if (PCT86.Match(record).Success)
                        {
                            var text = Methods.PCT(record.Replace("(86)", "").Trim());
                            var pattern = Regex.Match(text, @"(?<number>.+)\s+(?<date>\d{2}(\.|\/)\d{2}(\.|\/)\d{4})");
                            currentElement.Pct86 = new PCT86
                            {
                                AppDate = Methods.DateNormalize(pattern.Groups["date"].Value),
                                AppNumber = pattern.Groups["number"].Value
                            };
                        }

                        if (record.StartsWith(PCT87))
                        {
                            var pattern = Regex.Match(record.Replace(PCT87, "").Trim(), @"(?<number>.+)\s*(?<kind>[A-Z]{1}\d{0,1})?\s*(?<date>\d{2}(\.|\/)\d{2}(\.|\/)\d{4})");
                            currentElement.Pct87 = new PCT87
                            {
                                PubDate = Methods.DateNormalize(pattern.Groups["date"].Value),
                                PubNumber = pattern.Groups["number"].Value,
                                Kind = pattern.Groups["kind"].Value
                            };
                        }

                        if (Priority.Match(record).Success)
                        {
                            var text = record.Replace("(30)", "").Trim();
                            if (!string.IsNullOrEmpty(text))
                            {
                                currentElement.Priorities = new List<Priority>();
                                var priorities = Regex.Matches(text, @"(?<number>.*?)\s*(?<date>\d{2}(\.|\/)\d{2}(\.|\/)\d{4})\s*(?<country>\p{L}{2})");
                                foreach (Match priority in priorities)
                                {
                                    currentElement.Priorities.Add(new Priority
                                    {
                                        Country = Methods.PCT(priority.Groups["country"].Value),
                                        Date = Methods.DateNormalize(priority.Groups["date"].Value),
                                        Number = priority.Groups["number"].Value
                                    });
                                }
                            }
                        }

                        if (record.StartsWith(Ipc))
                        {
                            var text = record.Replace("\n", " ").Replace(Ipc, "").Trim();
                            if (string.IsNullOrEmpty(text))
                                continue;
                            if (!string.IsNullOrEmpty(text))
                            {
                                var edition = "";
                                var ipcs = text.Split(' ');
                                if (text.Contains(";") || text.Length < 11)
                                {
                                    ipcs = text.Split(';');
                                }
                                else
                                {
                                    edition = text.Substring(0, 1);
                                    ipcs = text.Replace("\n", " ").Substring(1).Trim().Split(',');
                                }

                                var classNew = ipcs.First().Split(' ').First().Trim();
                                currentElement.ClassificationIpcs = new List<ClassificationIpc>();
                                var @class = "";
                                foreach (var ipc in ipcs)
                                {
                                    if (ipc.Length < 8)
                                        @class = $"{classNew} {ipc}";
                                    else
                                        @class = ipc;
                                    currentElement.ClassificationIpcs.Add(new ClassificationIpc
                                    {
                                        Edition = Methods.ToInt(edition),
                                        Classification = Methods.PCT(@class.Trim())
                                    });
                                }
                            }
                        }

                        if (record.StartsWith(PubDate43))
                            currentElement.Date43 = Methods.DateNormalize(record.Replace(PubDate43, "").Trim());

                        if (Applicant.Match(record).Success)
                        {
                            var text = Methods.PCT(record.Replace("(71)", "").Trim());
                            var applicants = Regex.Matches(text, @"\d{1}\.(?<name>.+)\s*(?<country>\(\p{L}{2}\))\s*(?<address>.+)\n?");
                            if (applicants.Count > 0)
                            {
                                currentElement.Applicants = new List<PartyMember>();
                                for (int j = 0; j < applicants.Count; j++)
                                {
                                    currentElement.Applicants.Add(
                                        new PartyMember
                                        {
                                            Name = applicants[j].Groups["name"].Value.Trim(),
                                            Address = applicants[j].Groups["address"].Value,
                                            Country = applicants[j].Groups["country"].Value.Replace("(", "").Replace(")", "").Trim(),
                                            Language = Methods.ToLang(applicants[j].Groups["country"].Value.Replace("(", "").Replace(")", "").Trim())
                                        });
                                    if (currentElement.Applicants[j].Country.Contains("VI") || currentElement.Applicants[j].Country.Contains("VN"))
                                    {
                                        currentElement.Applicants[j].TransCountry = "EN";
                                        currentElement.Applicants[j].TransName = Methods.ToParties(applicants[j].Groups["name"].Value.Trim());
                                    }
                                }
                            }
                            else
                            {
                                var country = Regex.Match(text, @"\(\p{L}{2}\)");
                                var name = "";
                                var address = "";
                                if (country.Success)
                                {
                                    var strings = text.Split(country.Value);
                                    name = strings[0].Trim();
                                    address = strings[1].Trim();
                                }
                                else
                                {
                                    name = text;
                                }


                                currentElement.Applicants = new List<PartyMember>
                                {
                                    new PartyMember
                                    {
                                        Name = Methods.PCT(name),
                                        Country = country.Value
                                        .Replace("(", "")
                                        .Replace(")", "")
                                        .Trim(),
                                        Language = Methods.ToLang(country.Value
                                        .Replace("(", "")
                                        .Replace(")", "")
                                        .Trim())
                                    }
                                };

                                if (!string.IsNullOrEmpty(address))
                                    currentElement.Applicants.First().Address = address.Replace(address.Split(',').Last(), "").Trim(' ', ',', ' ');

                                if (country.Value.Contains("VN") || country.Value.Contains("VI"))
                                {
                                    currentElement.Applicants[0].TransCountry = "EN";
                                    currentElement.Applicants[0].TransName = Methods.ToParties(name);
                                }

                            }

                        }

                        if (record.StartsWith(Agent))
                        {
                            var engPart = Regex.Match(record.Replace(Agent, "").Replace("\n", " ").Trim(), @"\(.+\)");
                            currentElement.Agents = new List<PartyMember>
                                {
                                    new PartyMember
                                    {
                                        Name = Methods.PCT(record.Replace(Agent, "").Replace("\n", " ").Replace(engPart.Value, "").Trim()),
                                        Country = "VN",
                                        Language = "VI"
                                    }
                                };

                            currentElement.Agents[0].TransName = engPart.Value.Replace("(", "").Replace(")", "").Trim();
                            currentElement.Agents[0].TransCountry = "EN";
                        }


                        if (record.StartsWith(Inventor15))
                        {
                            currentElement.Inventors = new List<PartyMember>();
                            var text = Methods.PCT(record.Replace("\n", " ").Trim());
                            var inventors = Regex.Matches(text.Replace("(72)", "").Trim(), @"(?<name>.*?)\s*(?<country>\(\p{L}{2}\))");
                            for (var j = 0; j < inventors.Count; j++)
                            {
                                currentElement.Inventors.Add(new PartyMember
                                {
                                    Name = Methods.PCT(inventors[j]
                                    .Groups["name"].Value
                                    .Trim(' ', ',', ';', ' ')),
                                    Country = inventors[j]
                                    .Groups["country"].Value
                                    .Replace("(", "")
                                    .Replace(")", "")
                                    .Trim(),
                                    Language = Methods.ToLang(inventors[j]
                                    .Groups["country"].Value
                                    .Replace("(", "")
                                    .Replace(")", "")
                                    .Trim())
                                });

                                if (currentElement.Inventors[j].Country.Contains("VN") || currentElement.Inventors[j].Country.Contains("VI"))
                                {
                                    currentElement.Inventors[j].TransName = Methods.ToParties(inventors[j].Groups["name"].Value.Trim(' ', ',', ';', ' '));
                                    currentElement.Inventors[j].TransCountry = "EN";
                                }

                            }
                        }

                        if (record.StartsWith(ApplicantInventor))
                        {
                            currentElement.InvOrApp = new List<PartyMember>();
                            var text = Methods.PCT(record.Replace("\n", " ").Trim());
                            text = text.Replace("(75)", "").Trim();
                            //var inventors = Regex.Matches(text, @"(?<id>\d{1}\.)?\s*(?<name>.*?)\s*(?<country>\([A-Z]{2}\))\s*(?<address>.+)(\n|\s*)?");
                            var inventors = Regex.Matches(text, @"\d{1}\.");
                            if (inventors.Count > 0)
                            {
                                for (int j = 0; j < inventors.Count; j++)
                                {
                                    var tmp = j + 1;
                                    var inventor = "";
                                    if (tmp < inventors.Count)
                                    {
                                        var start = text.IndexOf(inventors[j].Value);
                                        var end = text.IndexOf(inventors[tmp].Value);
                                        var length = text.Length;
                                        inventor = text.Substring(start, end - start);
                                    }
                                    else
                                        inventor = text.Substring(text.IndexOf(inventors[j].Value));

                                    var match = Regex.Match(inventor, @"(?<id>\d{1}\.)?\s*(?<name>.*?)\s*(?<country>\([A-Z]{2}\))\s*(?<address>.+)(\n|\s*)?");
                                    currentElement.InvOrApp.Add(new PartyMember
                                    {
                                        Name = Methods.PCT(match.Groups["name"].Value),
                                        Country = match
                                            .Groups["country"].Value
                                            .Replace("(", "")
                                            .Replace(")", ""),
                                        Address = match.Groups["address"].Value,
                                        Language = Methods.ToLang(match
                                            .Groups["country"].Value
                                            .Replace("(", "")
                                            .Replace(")", ""))
                                    });

                                    if (currentElement.InvOrApp[j].Country.Contains("VI") || currentElement.InvOrApp[j].Country.Contains("VN"))
                                    {
                                        currentElement.InvOrApp[j].TransName = Methods.ToParties(match.Groups["name"].Value);
                                        currentElement.InvOrApp[j].TransCountry = "EN";
                                    }
                                }
                            }
                            else
                            {
                                var country = Regex.Matches(text, @"\([A-Z]{2}\)");
                                if (country.Count > 1)
                                {

                                }

                                currentElement.InvOrApp.Add(new PartyMember
                                {
                                    Name = text.Substring(0, text.IndexOf(country.First().Value)).Trim(),
                                    Address = text.Substring(text.IndexOf(country.First().Value) + country.First().Value.Length),
                                    Country = country.First().Value.Replace("(", "").Replace(")", "").Trim(),
                                    Language = Methods.ToLang(country.First().Value.Replace("(", "").Replace(")", "").Trim())
                                });

                                if (currentElement.InvOrApp.First().Country == "VI" || currentElement.InvOrApp.First().Country == "VN")
                                {
                                    currentElement.InvOrApp.First().TransCountry = "EN";
                                    currentElement.InvOrApp.First().TransName = Methods.ToParties(text.Substring(0, text.IndexOf(country.First().Value))).Trim();
                                }
                            }
                        }

                        if (record.StartsWith(Abstract))
                            currentElement.Abstract = new Abstract
                            {
                                Text = record.Replace(Abstract, "").Replace(record.Split('\n').Last(), "").Trim(),
                                Language = "VI"
                            };

                        if (record.StartsWith(Title))
                            currentElement.Title = record.Replace(Title, "").Trim();

                        if (record.Contains("Ngày yêu cầu thẩm định nội dung"))
                        {
                            var date = Methods.DateNormalize(Regex.Match(record.Substring(record.IndexOf("Ngày yêu cầu thẩm định nội dung")), @"\d{2}(\.|\/)\d{2}(\.|\/)\d{4}").Value);
                            var original = $"|| Ngày yêu cầu thẩm định nội dung | {date}";
                            var translation = $"|| Date of request for substantive examination | {date}";

                            currentElement.Note = original;
                            currentElement.Translation = translation;
                        }
                    }
                    if (string.IsNullOrEmpty(currentElement.Note))
                    {

                    }
                    if (splittedRecords.Count > 1)
                    {
                        elementsOut.Add(currentElement);
                    }
                }
            }

            return elementsOut;
        }

        public static List<Applications> SubCode13Process(MatchCollection elements)
        {
            List<Applications> elementsOut = new List<Applications>();

            if (elements == null)
                return null;

            for (int i = 0; i < elements.Count; i++)
            {
                int tmpInc = i;
                string tmpValue = null;

                var value = elements[i].Value;

                if (PubNumber.Match(value).Success)
                {
                    var currentElement = new Applications();
                    do
                    {
                        tmpValue += elements[tmpInc].Value + "\n";
                        ++tmpInc;
                    } while (tmpInc < elements.Count && !PubNumber.Match(elements[tmpInc].Value).Success && !elements[tmpInc].Value.StartsWith("(11)"));

                    tmpValue = tmpValue.Replace("<Text>", "").Replace("</Text>", "");

                    var splittedRecords = Methods.RecSplit(tmpValue, new string[] { PubNumber.Match(tmpValue).Value, AppNumber.Match(tmpValue).Value, AppDate.Match(tmpValue).Value, PCT86.Match(tmpValue).Value, Priority.Match(tmpValue).Value, Ipc, PubDate43, PCT87, ApplicantInventor, Agent, Title, Abstract, Inventor15, Applicant.Match(tmpValue).Value, Field85 });

                    foreach (var record in splittedRecords)
                    {
                        if (PubNumber.Match(record).Success)
                            currentElement.PubNumber = record.Replace("(11)", "").Trim();

                        if (record.StartsWith(Field85))
                            currentElement.Date85 = Methods.DateNormalize(record.Replace(Field85, "").Trim());

                        if (currentElement.PubNumber == "4483 A")
                        {

                        }

                        if (AppNumber.Match(record).Success)
                            currentElement.AppNumber = record.Replace("(21)", "").Trim();

                        if (AppDate.Match(record).Success)
                            currentElement.AppDate = Methods.DateNormalize(record.Replace("(22)", "").Trim());

                        if (PCT86.Match(record).Success)
                        {
                            var pattern = Regex.Match(record.Replace("(86)", "").Trim(), @"(?<number>[A-Z]{3}\/[A-Z]{2}\d{4}\/\d+)\s+(?<date>\d{2}(\.|\/)\d{2}(\.|\/)\d{4})");
                            currentElement.Pct86 = new PCT86
                            {
                                AppDate = Methods.DateNormalize(pattern.Groups["date"].Value),
                                AppNumber = pattern.Groups["number"].Value
                            };
                        }

                        if (record.StartsWith(PCT87))
                        {
                            var pattern = Regex.Match(record.Replace(PCT87, "").Trim(), @"(?<number>[A-Z]{2}\d{4}\/\d+)\s*(?<date>\d{2}(\.|\/)\d{2}(\.|\/)\d{4})");
                            currentElement.Pct87 = new PCT87
                            {
                                PubDate = Methods.DateNormalize(pattern.Groups["date"].Value),
                                PubNumber = pattern.Groups["number"].Value
                            };
                        }

                        if (Applicant.Match(record).Success)
                        {
                            var text = Methods.PCT(record.Replace("\n", " ").Replace("(71)", "").Trim());
                            var applicants = Regex.Matches(text, @"(?<id>\d{1}\.)?\s*(?<name>.*?)\s*(?<country>\([A-Z]{2}\))\s*(?<address>.+)\n?");
                            currentElement.Applicants = new List<PartyMember>();

                            for (var j = 0; j < applicants.Count; j++)
                            {
                                currentElement.Applicants.Add(new PartyMember
                                {
                                    Name = Methods.PCT(applicants[j].Groups["name"].Value),
                                    Address = applicants[j].Groups["address"].Value.Replace(applicants[j].Groups["address"].Value.Split(',').Last(), "").Trim(' ', ',', ' '),
                                    Country = applicants[j].Groups["country"].Value.Replace("(", "").Replace(")", "").Trim(),
                                    Language = Methods.ToLang(applicants[j].Groups["country"].Value.Replace("(", "").Replace(")", "").Trim())
                                });

                                if (currentElement.Applicants[j].Country.Contains("VI") || currentElement.Applicants[j].Country.Contains("VN"))
                                {
                                    currentElement.Applicants[j].TransName = Methods.ToParties(applicants[j].Groups["name"].Value);
                                    currentElement.Applicants[j].TransCountry = "EN";
                                }

                            }
                        }

                        if (record.StartsWith(Inventor15))
                        {
                            var text = Methods.PCT(record.Replace("\n", " ").Replace("(72)", "").Trim());
                            var countries = Regex.Matches(text, @"\(\p{L}{2}\)");

                            foreach (Match country in countries)
                            {
                                text = text.Replace(country.Value, $"{country.Value}***");
                            }

                            currentElement.Inventors = new List<PartyMember>();

                            var inventors = text.Split("***", StringSplitOptions.RemoveEmptyEntries);

                            for (int j = 0; j < inventors.Count(); j++)
                            {
                                var inventor = inventors[j].Trim(' ', ';', ' ');
                                var country = Regex.Match(inventor, @"\([A-Z]{2}\)");
                                currentElement.Inventors.Add(new PartyMember
                                {
                                    Name = Methods.PCT(inventor.Replace(country.Value, "")),
                                    Country = country.Value.Replace("(", "").Replace(")", "").Trim(),
                                    Language = Methods.ToLang(country.Value.Replace("(", "").Replace(")", "").Trim())
                                });

                                if (currentElement.Inventors[j].Country.Contains("VI") || currentElement.Inventors[j].Country.Contains("VN"))
                                {
                                    currentElement.Inventors[j].TransName = Methods.ToParties(inventor.Replace(country.Value, ""));
                                    currentElement.Inventors[j].TransCountry = "EN";
                                }
                            }
                        }

                        if (Priority.Match(record).Success)
                        {
                            var text = record.Replace("(30)", "").Trim();
                            if (!string.IsNullOrEmpty(text))
                            {
                                currentElement.Priorities = new List<Priority>();
                                var priorities = Regex.Matches(text, @"(?<number>.*?)\s*(?<date>\d{2}(\.|\/)\d{2}(\.|\/)\d{4})\s*(?<country>\p{L}{2})");
                                foreach (Match priority in priorities)
                                {
                                    currentElement.Priorities.Add(new Priority
                                    {
                                        Country = Methods.PCT(priority.Groups["country"].Value),
                                        Date = Methods.DateNormalize(priority.Groups["date"].Value),
                                        Number = priority.Groups["number"].Value
                                    });
                                }
                            }
                        }

                        if (record.StartsWith(Ipc))
                        {
                            var text = record.Replace("\n", " ").Replace(Ipc, "").Trim();
                            if (string.IsNullOrEmpty(text))
                                continue;
                            if (!string.IsNullOrEmpty(text))
                            {
                                var edition = "";
                                var ipcs = text.Split(' ');
                                if (text.Contains(";") || text.Length < 11)
                                {
                                    ipcs = text.Split(';');
                                }
                                else
                                {
                                    edition = text.Substring(0, 1);
                                    ipcs = text.Replace("\n", " ").Substring(1).Trim().Split(',');
                                }

                                var classNew = ipcs.First().Split(' ').First().Trim();
                                currentElement.ClassificationIpcs = new List<ClassificationIpc>();
                                var @class = "";
                                foreach (var ipc in ipcs)
                                {
                                    if (ipc.Length < 8)
                                        @class = $"{classNew} {ipc}";
                                    else
                                        @class = ipc;
                                    currentElement.ClassificationIpcs.Add(new ClassificationIpc
                                    {
                                        Edition = Methods.ToInt(edition),
                                        Classification = Methods.PCT(@class.Trim())
                                    });
                                }
                            }
                        }

                        if (record.StartsWith(PubDate43))
                        {
                            currentElement.Date43 = Methods.DateNormalize(record.Replace(PubDate43, "").Trim().Split('\n').First());
                        }


                        if (record.StartsWith(ApplicantInventor))
                        {
                            var text = Methods.PCT(record.Replace(ApplicantInventor, "").Replace("\n", " ").Trim());
                            var country = Regex.Match(text, @"\(\p{L}{2}\)");
                            var name = "";
                            var address = "";
                            if (country.Success)
                            {
                                var strings = text.Split(country.Value);
                                name = strings[0].Trim();
                                address = strings[1].Trim();
                            }
                            else
                            {
                                name = text;
                            }

                            currentElement.InvOrApp = new List<PartyMember>
                                {
                                    new PartyMember
                                    {
                                        Name = Methods.PCT(name),
                                        Address = address.Replace(address.Split(',').Last(), "").Trim(' ', ',', ' '),
                                        Country = country.Value
                                        .Replace("(", "")
                                        .Replace(")", "")
                                        .Trim(),
                                        Language = Methods.ToLang(country.Value
                                        .Replace("(", "")
                                        .Replace(")", "")
                                        .Trim())
                                    }
                                };

                            if (currentElement.InvOrApp[0].Country.Contains("VI") || currentElement.InvOrApp[0].Country.Contains("VN"))
                            {
                                currentElement.InvOrApp[0].TransCountry = "EN";
                                currentElement.InvOrApp[0].TransName = Methods.ToParties(name);
                            }
                        }

                        if (record.StartsWith(Agent))
                        {
                            var engPart = Regex.Match(record.Replace(Agent, "").Trim(), @"\(.+\)");
                            currentElement.Agents = new List<PartyMember>
                                {
                                    new PartyMember
                                    {
                                        Name = Methods.PCT(record.Replace(Agent, "").Replace(engPart.Value, "").Trim()),
                                        Country = "VN",
                                        Language = "VI"
                                    }
                                };


                            currentElement.Agents[0].TransName = engPart.Value.Replace("(", "").Replace(")", "").Trim();
                            currentElement.Agents[0].TransCountry = "EN";
                        }


                        if (record.StartsWith(Abstract))
                            currentElement.Abstract = new Abstract
                            {
                                Text = record.Replace(Abstract, "").Replace(record.Split('\n').Last(), "").Trim(),
                                Language = "VI"
                            };

                        if (record.StartsWith(Title))
                            currentElement.Title = record.Replace(Title, "").Trim();

                        if (record.Contains("Ngày yêu cầu thẩm định nội dung") || record.Contains("Ngày yờu cầu thẩm định nội dung"))
                        {
                            var date = Methods.DateNormalize(Regex.Match(record, @"\d{2}(\.|\/)\d{2}(\.|\/)\d{4}").Value);
                            var original = $"|| Ngày yêu cầu thẩm định nội dung | {date}";
                            var translation = $"|| Date of request for substantive examination | {date}";

                            currentElement.Note = original;
                            currentElement.Translation = translation;
                        }

                        if (record.Contains("Ngày yêu cầu công bốsớm") || record.Contains("Ngày yờu cầu cụng bố sớm"))
                        {
                            var date = Methods.DateNormalize(Regex.Match(record, @"\d{2}(\.|\/)\d{2}(\.|\/)\d{4}").Value);
                            currentElement.Note = currentElement.Note + "\n" + $"|| Ngày yêu cầu công bốsớm | {date}";
                            currentElement.Translation = currentElement.Translation + "\n" + $"|| Date of request for early publication | {date}";
                        }
                    }
                    if (string.IsNullOrEmpty(currentElement.Note))
                    {

                    }
                    if (splittedRecords.Count > 1)
                    {
                        elementsOut.Add(currentElement);
                    }
                }
            }

            return elementsOut;
        }

        public static List<Applications> SubCode14Process(MatchCollection elements)
        {
            List<Applications> elementsOut = new List<Applications>();

            if (elements == null)
                return null;

            for (int i = 0; i < elements.Count; i++)
            {
                int tmpInc = i;
                string tmpValue = null;

                var value = elements[i].Value;

                if (PubNumber.Match(value).Success)
                {
                    var currentElement = new Applications();
                    do
                    {
                        tmpValue += elements[tmpInc].Value + "\n";
                        ++tmpInc;
                    } while (tmpInc < elements.Count && !PubNumber.Match(elements[tmpInc].Value).Success && !elements[tmpInc].Value.StartsWith("(11)"));

                    tmpValue = tmpValue.Replace("<Text>", "").Replace("</Text>", "");

                    if (tmpValue.Contains("1-0023451 B"))
                    {

                    }

                    var splittedRecords = Methods.RecSplit(tmpValue, new string[] { PubNumber.Match(tmpValue).Value, AppNumber.Match(tmpValue).Value, AppDate.Match(tmpValue).Value, PCT86.Match(tmpValue).Value, Priority.Match(tmpValue).Value, Ipc, PubDate43, PubDate45, PCT87, Grantee.Match(tmpValue).Value, Inventor14.Match(tmpValue).Value, Agent, Title, Related, InvAppGrant, Field85, Related62 });

                    foreach (var record in splittedRecords)
                    {
                        if (PubNumber.Match(record).Success)
                        {
                            var index = record.IndexOf("(15)");
                            if (index > -1)
                                currentElement.PubNumber = record.Substring(0, index).Replace("(11)", "").Trim();
                            else
                                currentElement.PubNumber = record.Split('\n').First().Replace("(11)", "").Trim();

                            var date = Methods.DateNormalize(Regex.Match(record.Trim().Split('\n').Last().Trim(), @"\d{2}(\.|\/)\d{2}(\.|\/)\d{4}").Value);
                            var original = $"|| (15) Ngày cấp | {date}";
                            var translation = $"|| (15) Issue date | {date}";

                            currentElement.Note = original;
                            currentElement.Translation = translation;
                        }

                        if (AppNumber.Match(record).Success)
                            currentElement.AppNumber = record.Replace("(21)", "").Trim();

                        if (record.StartsWith(Field85))
                            currentElement.Date85 = Methods.DateNormalize(record.Replace(Field85, "").Trim());

                        if (currentElement.PubNumber == "1-0023601 B")
                        {

                        }

                        if (record.StartsWith(Related) || record.StartsWith(Related62))
                        {
                            var inid = "";
                            if (record.Contains(Related))
                                inid = "67";
                            else
                                inid = "62";
                            currentElement.Related = new List<Related>
                            {
                                new Related
                                {
                                    Inid = inid,
                                    Number = record.Replace("(67)", "").Replace("(62)", "").Trim()
                                }
                            };
                        }

                        if (AppDate.Match(record).Success)
                            currentElement.AppDate = Methods.DateNormalize(record.Replace("(22)", "").Trim());

                        if (PCT86.Match(record).Success)
                        {
                            var pattern = Regex.Match(record.Replace("(86)", "").Trim(), @"(?<number>.+)\s+(?<date>\d{2}(\.|\/)\d{2}(\.|\/)\d{4})");
                            currentElement.Pct86 = new PCT86
                            {
                                AppDate = Methods.DateNormalize(pattern.Groups["date"].Value),
                                AppNumber = pattern.Groups["number"].Value
                            };
                        }

                        if (record.StartsWith(PCT87))
                        {
                            var pattern = Regex.Match(record.Replace(PCT87, "").Trim(), @"(?<number>[A-Z]{2}\d{4}\/\d+)\s*(?<kind>[A-Z]{1}\d{0,1})?\s*(?<date>\d{2}(\.|\/)\d{2}(\.|\/)\d{4})");
                            currentElement.Pct87 = new PCT87
                            {
                                PubDate = Methods.DateNormalize(pattern.Groups["date"].Value),
                                PubNumber = pattern.Groups["number"].Value,
                                Kind = pattern.Groups["kind"].Value
                            };
                        }

                        if (Priority.Match(record).Success)
                        {
                            var text = record.Replace("(30)", "").Trim();
                            if (!string.IsNullOrEmpty(text))
                            {
                                currentElement.Priorities = new List<Priority>();
                                var priorities = Regex.Matches(text, @"(?<number>.*?)\s*(?<date>\d{2}(\.|\/)\d{2}(\.|\/)\d{4})\s*(?<country>\p{L}{2})");
                                foreach (Match priority in priorities)
                                {
                                    var country = Methods.PCT(priority.Groups["country"].Value);
                                    if (country == "us")
                                        country = "US";
                                    currentElement.Priorities.Add(new Priority
                                    {
                                        Country = country,
                                        Date = Methods.DateNormalize(priority.Groups["date"].Value),
                                        Number = priority.Groups["number"].Value
                                    });
                                }
                            }
                        }

                        if (record.StartsWith(Ipc))
                        {
                            var text = record.Replace("\n", " ").Replace(Ipc, "").Trim();
                            var matches = Regex.Matches(text, @"\p{L}{1}\d{2}\p{L} ?\d{1,2}\/\d{2}");
                            if (string.IsNullOrEmpty(text))
                                continue;
                            if (!string.IsNullOrEmpty(text))
                            {
                                var edition = "";
                                var ipcs = text.Split(' ');
                                if (text.Contains(";") || text.Length < 11)
                                {
                                    ipcs = text.Split(';');
                                }
                                else
                                {
                                    edition = text.Substring(0, 1);
                                    ipcs = text.Replace("\n", " ").Substring(1).Trim().Split(',');
                                }

                                var classNew = ipcs.First().Split(' ').First().Trim();
                                currentElement.ClassificationIpcs = new List<ClassificationIpc>();
                                var @class = "";
                                foreach (var ipc in ipcs)
                                {
                                    if (ipc.Length < 8)
                                        @class = $"{classNew} {ipc.Trim()}";
                                    else
                                        @class = ipc.Trim();

                                    if (!@class.Contains(" "))
                                    {
                                        @class = $"{@class.Substring(0, 4)} {@class.Substring(4)}";
                                    }

                                    currentElement.ClassificationIpcs.Add(new ClassificationIpc
                                    {
                                        Edition = Methods.ToInt(edition),
                                        Classification = Methods.PCT(@class.Replace(".", "").Trim())
                                    });
                                }
                            }
                        }

                        if (record.StartsWith(PubDate43))
                        {
                            var matchDate = Regex.Match(record, @"\d{2}(\.|\/)\d{2}(\.|\/)\d{4}");
                            currentElement.PubDate = Methods.DateNormalize(matchDate.Value);
                        }

                        if (record.StartsWith(PubDate45))
                        {
                            var matchDate = Regex.Match(record, @"\d{2}(\.|\/)\d{2}(\.|\/)\d{4}");
                            currentElement.Date45 = Methods.DateNormalize(matchDate.Value);
                        }


                        if (Grantee.Match(record).Success)
                        {
                            try
                            {
                                var numbers = Regex.Matches(record, @"\d\.");
                                if (numbers.Count > 0)
                                {
                                    var text = record;
                                    currentElement.GrantAssigOwner = new List<PartyMember>();
                                    foreach (Match number in numbers)
                                    {
                                        text = text.Replace(number.Value, $"***{number.Value}");
                                    }

                                    var grantees = text.Split("***", StringSplitOptions.RemoveEmptyEntries);
                                    foreach (var grantee in grantees)
                                    {
                                        text = Methods.PCT(grantee.Replace("(73)", "").Replace("\n", " ").Trim());
                                        if (!string.IsNullOrEmpty(text))
                                        {
                                            var country = Regex.Match(text, @"\(\p{L}{2}\)");
                                            var name = "";
                                            var address = "";
                                            if (country.Success)
                                            {
                                                var strings = text.Split(country.Value);
                                                name = strings[0].Trim();
                                                address = strings[1].Trim();
                                            }
                                            else
                                            {
                                                name = text;
                                            }



                                            currentElement.GrantAssigOwner.Add(new PartyMember
                                            {
                                                Name = name.Substring(2),
                                                Country = country.Value
                                        .Replace("(", "")
                                        .Replace(")", "")
                                        .Trim(),
                                                Language = Methods.ToLang(country.Value
                                        .Replace("(", "")
                                        .Replace(")", "")
                                        .Trim())
                                            });

                                            if (!string.IsNullOrEmpty(address))
                                                currentElement.GrantAssigOwner.First().Address = address.Replace(address.Split(',').Last(), "").Trim(' ', ',', ' ');

                                            if (currentElement.GrantAssigOwner[0].Country == "VI" || currentElement.GrantAssigOwner[0].Country == "VN")
                                            {
                                                currentElement.GrantAssigOwner[0].TransCountry = "EN";
                                                currentElement.GrantAssigOwner[0].TransName = Methods.ToParties(name);
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    var text = Methods.PCT(record.Replace("(73)", "").Replace("\n", " ").Trim());
                                    var country = Regex.Match(text, @"\(\p{L}{2}\)");
                                    var name = "";
                                    var address = "";
                                    if (country.Success)
                                    {
                                        var strings = text.Split(country.Value);
                                        name = strings[0].Trim();
                                        address = strings[1].Trim();
                                    }
                                    else
                                    {
                                        name = text;
                                    }



                                    currentElement.GrantAssigOwner = new List<PartyMember>
                                {
                                    new PartyMember
                                    {
                                        Name = name,
                                        Country = country.Value
                                        .Replace("(", "")
                                        .Replace(")", "")
                                        .Trim(),
                                        Language = Methods.ToLang(country.Value
                                        .Replace("(", "")
                                        .Replace(")", "")
                                        .Trim())
                                    }
                                };

                                    if (!string.IsNullOrEmpty(address))
                                        currentElement.GrantAssigOwner.First().Address = address.Replace(address.Split(',').Last(), "").Trim(' ', ',', ' ');

                                    if (currentElement.GrantAssigOwner[0].Country == "VI" || currentElement.GrantAssigOwner[0].Country == "VN")
                                    {
                                        currentElement.GrantAssigOwner[0].TransCountry = "EN";
                                        currentElement.GrantAssigOwner[0].TransName = Methods.ToParties(name);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {

                            }

                        }

                        if (record.StartsWith(InvAppGrant))
                        {
                            var text = Methods.PCT(record.Replace("(76)", "").Replace("\n", " ").Trim());
                            var partyMember = Regex.Match(text, @"(?<name>.+)\s*(?<country>\([A-Z]{2}\))\s*(?<address>.+)");

                            currentElement.InvAppGrant = new List<PartyMember>
                            {
                                new PartyMember
                                {
                                    Address = partyMember.Groups["address"].Value.Replace(partyMember.Groups["address"].Value.Split(',').Last(), "").Trim(' ', ',',';', ' '),
                                    Name = Methods.PCT(partyMember.Groups["name"].Value),
                                    Country = partyMember.Groups["country"].Value.Replace("(", "").Replace(")", "").Trim(),
                                    Language = Methods.ToLang(partyMember.Groups["country"].Value.Replace("(", "").Replace(")", "").Trim())
                                },
                            };

                            if (currentElement.InvAppGrant.First().Country == "VI" || currentElement.InvAppGrant.First().Country == "VN")
                            {
                                currentElement.InvAppGrant[0].TransName = Methods.ToParties(partyMember.Groups["name"].Value);
                                currentElement.InvAppGrant[0].TransCountry = "EN";
                            }
                        }

                        if (record.StartsWith(Agent))
                        {
                            var engPart = Regex.Match(record.Replace(Agent, "").Replace("\n", " ").Trim(), @"\(.+\)");
                            if (!string.IsNullOrEmpty(engPart.Value))
                            {
                                var agent = record.Replace(Agent, "").Replace(engPart.Value, "").Trim();
                                currentElement.Agents = new List<PartyMember>
                                {
                                    new PartyMember
                                    {
                                        Name = Methods.PCT(record.Replace(Agent, "").Replace(engPart.Value, "").Trim()),
                                        Country = "VN",
                                        Language = "VI"
                                    },
                                };

                                currentElement.Agents[0].TransCountry = "EN";
                                currentElement.Agents[0].TransName = engPart.Value.Replace("(", "").Replace(")", "").Trim();
                            }
                        }

                        if (Inventor14.Match(record).Success)
                        {
                            currentElement.Inventors = new List<PartyMember>();
                            if (record.Contains("(76)"))
                            {
                                var text = Methods.PCT(record.Replace("(76)", "").Trim());
                                var inventors = Regex.Matches(text, @"(?<id>\d{1}\.)?\s*(?<name>.*?)\s*(?<country>\(\p{L}{2}\))\n(?<address>.+)\n?");
                                for (var j = 0; j < inventors.Count; j++)
                                {
                                    currentElement.Inventors.Add(new PartyMember
                                    {
                                        Name = Methods.PCT(inventors[j].Groups["name"].Value),
                                        Country = inventors[j]
                                        .Groups["country"].Value
                                        .Replace("(", "")
                                        .Replace(")", ""),
                                        Address = inventors[j].Groups["address"].Value.Replace(inventors[j].Groups["address"].Value.Split(',').Last(), "").Trim(' ', ',', ';', ' '),
                                        Language = Methods.ToLang(inventors[j]
                                        .Groups["country"].Value
                                        .Replace("(", "")
                                        .Replace(")", ""))
                                    });

                                    if (currentElement.Inventors[j].Country == "VI" || currentElement.Inventors[j].Country == "VN")
                                    {
                                        currentElement.Inventors[j].TransName = Methods.ToParties(inventors[j].Groups["name"].Value);
                                        currentElement.Inventors[j].TransCountry = "EN";
                                    }
                                }
                            }
                            else
                            {
                                var inventors = Regex.Matches(Methods.PCT(record.Replace("(72)", "").Trim()), @"(?<name>.*?)\s*(?<country>\(\p{L}{2}\))");
                                for (var j = 0; j < inventors.Count; j++)
                                {
                                    currentElement.Inventors.Add(new PartyMember
                                    {
                                        Name = Methods.PCT(inventors[j]
                                        .Groups["name"].Value
                                        .Trim(' ', ',', ';', ' ')),
                                        Country = inventors[j]
                                        .Groups["country"].Value
                                        .Replace("(", "")
                                        .Replace(")", "")
                                        .Trim(),
                                        Language = Methods.ToLang(inventors[j]
                                        .Groups["country"].Value
                                        .Replace("(", "")
                                        .Replace(")", "")
                                        .Trim())
                                    });

                                    if (currentElement.Inventors[j].Country == "VI" || currentElement.Inventors[j].Country == "VN")
                                    {
                                        currentElement.Inventors[j].TransName = Methods.ToParties(inventors[j].Groups["name"].Value);
                                        currentElement.Inventors[j].TransCountry = "EN";
                                    }
                                }
                            }
                        }

                        if (record.StartsWith(Abstract))
                            currentElement.Abstract = new Abstract
                            {
                                Text = record.Replace(Abstract, "").Replace(record.Split('\n').Last(), "").Trim(),
                                Language = "VI"
                            };

                        if (record.StartsWith(Title))
                            currentElement.Title = record.Replace(Title, "").Trim();
                    }
                    if (splittedRecords.Count > 1)
                    {
                        elementsOut.Add(currentElement);
                    }
                }
            }

            return elementsOut;
        }

        public static List<Applications> SubCode15Process(MatchCollection elements)
        {
            List<Applications> elementsOut = new List<Applications>();

            if (elements == null)
                return null;

            for (int i = 0; i < elements.Count; i++)
            {
                int tmpInc = i;
                string tmpValue = null;

                var value = elements[i].Value;

                if (PubNumber.Match(value).Success)
                {
                    var currentElement = new Applications();
                    do
                    {
                        tmpValue += elements[tmpInc].Value + "\n";
                        ++tmpInc;
                    } while (tmpInc < elements.Count && !PubNumber.Match(elements[tmpInc].Value).Success && !elements[tmpInc].Value.StartsWith("(11)"));

                    tmpValue = tmpValue.Replace("<Text>", "").Replace("</Text>", "");

                    var splittedRecords = Methods.RecSplit(tmpValue, new string[] { PubNumber.Match(tmpValue).Value, AppNumber.Match(tmpValue).Value, AppDate.Match(tmpValue).Value, PCT86.Match(tmpValue).Value, Priority.Match(tmpValue).Value, Ipc, PubDate43, PubDate45, PCT87, Grantee.Match(tmpValue).Value, Agent, Title, Abstract, InvAppGrant, Related, Inventor15 });

                    foreach (var record in splittedRecords)
                    {
                        if (PubNumber.Match(record).Success)
                        {
                            var index = record.IndexOf("(15)");
                            if (index > -1)
                                currentElement.PubNumber = record.Substring(0, index).Replace("(11)", "").Trim();
                            else
                                currentElement.PubNumber = record.Split('\n').First().Replace("(11)", "").Trim();

                            var date = Methods.DateNormalize(Regex.Match(record.Trim().Split('\n').Last().Trim(), @"\d{2}(\.|\/)\d{2}(\.|\/)\d{4}").Value);
                            var original = $"|| (15) Ngày cấp | {date}";
                            var translation = $"|| (15) Issue date | {date}";

                            currentElement.Note = original;
                            currentElement.Translation = translation;
                        }

                        if (currentElement.PubNumber.Contains("2-0002288"))
                        {

                        }

                        if (AppNumber.Match(record).Success)
                            currentElement.AppNumber = record.Replace("(21)", "").Trim();

                        if (AppDate.Match(record).Success)
                            currentElement.AppDate = Methods.DateNormalize(record.Replace("(22)", "").Trim());

                        if (record.StartsWith(Related))
                        {
                            var related = Regex.Matches(record.Replace("(67)", "").Trim(), @"\d{1}-\d{4}-\d{5,}");
                            currentElement.Related = new List<Related>();
                            foreach (Match match in related)
                            {
                                currentElement.Related.Add(new Related
                                {
                                    Number = match.Value,
                                    Inid = "67"
                                });
                            }
                        }

                        if (PCT86.Match(record).Success)
                        {
                            var pattern = Regex.Match(record.Replace("(86)", "").Trim(), @"(?<number>[A-Z]{3}\/[A-Z]{2}\d{4}\/\d+)\s+(?<date>\d{2}\.\d{2}\.\d{4})");
                            currentElement.Pct86 = new PCT86
                            {
                                AppDate = Methods.DateNormalize(pattern.Groups["date"].Value),
                                AppNumber = pattern.Groups["number"].Value
                            };
                        }

                        if (record.StartsWith(PCT87))
                        {
                            var pattern = Regex.Match(record.Replace(PCT87, "").Trim(), @"(?<number>[A-Z]{2}\d{4}\/\d+)\s*(?<kind>[A-Z]{1}\d{0,1})?\s*(?<date>\d{2}\.\d{2}\.\d{4})");
                            currentElement.Pct87 = new PCT87
                            {
                                PubDate = Methods.DateNormalize(pattern.Groups["date"].Value),
                                PubNumber = pattern.Groups["number"].Value
                            };
                        }

                        if (Priority.Match(record).Success)
                        {
                            var text = record.Replace("(30)", "").Trim();
                            if (!string.IsNullOrEmpty(text))
                            {
                                currentElement.Priorities = new List<Priority>();
                                var priorities = Regex.Matches(text, @"(?<number>.*?)\s*(?<date>\d{2}\.\d{2}\.\d{4})\s*(?<country>\p{L}{2})");
                                foreach (Match priority in priorities)
                                {
                                    currentElement.Priorities.Add(new Priority
                                    {
                                        Country = Methods.PCT(priority.Groups["country"].Value),
                                        Date = Methods.DateNormalize(priority.Groups["date"].Value),
                                        Number = priority.Groups["number"].Value
                                    });
                                }
                            }
                        }

                        if (record.StartsWith(Ipc))
                        {
                            var text = record.Replace(Ipc, "").Trim();
                            if (string.IsNullOrEmpty(text))
                                continue;
                            var matches = Regex.Matches(text, @"\p{L}{1}\d{2}\p{L} ?\d{1,2}\/\d{2}");
                            if (matches.Count() > 0)
                                currentElement.ClassificationIpcs = new List<ClassificationIpc>();

                            foreach (Match ipc in matches)
                            {
                                var item = ipc.Value;
                                if (!ipc.Value.Contains(" "))
                                    item = $"{item.Substring(0, 4)} {item.Substring(4)}";
                                currentElement.ClassificationIpcs.Add(new ClassificationIpc
                                {
                                    Classification = Methods.PCT(item)
                                });
                            }
                        }

                        if (record.StartsWith(PubDate43))
                            currentElement.Date43 = Methods.DateNormalize(record.Replace(PubDate43, "").Trim().Split(' ').First());

                        if (record.StartsWith(PubDate45))
                            currentElement.Date45 = Methods.DateNormalize(record.Replace(PubDate45, "").Trim().Split(' ').First());

                        if (Grantee.Match(record).Success)
                        {
                            var text = Methods.PCT(record.Replace("(73)", "").Replace("\n", " ").Trim());
                            var country = Regex.Match(text, @"\(\p{L}{2}\)");
                            var name = "";
                            var address = "";
                            if (country.Success)
                            {
                                var strings = text.Split(country.Value);
                                name = strings[0].Trim();
                                address = strings[1].Trim();
                            }
                            else
                            {
                                name = text;
                            }

                            currentElement.GrantAssigOwner = new List<PartyMember>
                                {
                                    new PartyMember
                                    {
                                        Name = name,
                                        Language = Methods.ToLang(country.Value
                                        .Replace("(", "")
                                        .Replace(")", "")
                                        .Trim()),
                                        Country = country.Value
                                        .Replace("(", "")
                                        .Replace(")", "")
                                        .Trim()
                                    }
                                };

                            if (!string.IsNullOrEmpty(address))
                                currentElement.GrantAssigOwner.First().Address = address.Replace(address.Split(',').Last(), "").Trim(' ', ',', ' ');

                            if (country.Value.Contains("VI") || country.Value.Contains("VN"))
                            {
                                currentElement.GrantAssigOwner.First().TransCountry = "EN";
                                currentElement.GrantAssigOwner.First().TransName = Methods.ToParties(currentElement.GrantAssigOwner.First().Name);
                            }
                        }

                        if (record.StartsWith(InvAppGrant))
                        {
                            currentElement.InvAppGrant = new List<PartyMember>();
                            var text = Methods.PCT(record.Replace("(76)", "").Trim());
                            var inventors = Regex.Matches(text, @"(?<id>\d{1}\.)?\s*(?<name>.*?)\s*(?<country>\(\p{L}{2}\))\n(?<address>.+)\n?");
                            for (int j = 0; j < inventors.Count; j++)
                            {
                                currentElement.InvAppGrant.Add(new PartyMember
                                {
                                    Name = Methods.PCT(inventors[j].Groups["name"].Value),
                                    Country = inventors[j]
                                    .Groups["country"].Value
                                    .Replace("(", "")
                                    .Replace(")", ""),
                                    Address = inventors[j].Groups["address"].Value.Replace(inventors[j].Groups["address"].Value.Split(',').Last(), "").Trim(' ', ',', ' '),
                                    Language = Methods.ToLang(inventors[j]
                                    .Groups["country"].Value
                                    .Replace("(", "")
                                    .Replace(")", ""))
                                });

                                if (currentElement.InvAppGrant[j].Country == "VI" || currentElement.InvAppGrant[j].Country == "VN")
                                {
                                    currentElement.InvAppGrant[j].TransName = Methods.ToParties(inventors[j].Groups["name"].Value);
                                    currentElement.InvAppGrant[j].TransCountry = "EN";
                                }
                            }
                        }

                        if (record.StartsWith(Inventor15))
                        {
                            var text = Methods.PCT(record.Replace(Inventor15, "").Replace(".", "").Trim());
                            var pattern = Regex.Matches(text, @"(?<name>\p{L}+(\p{L}+|\s*)+)\s+(?<country>\(\p{L}{2}\))(;|\.|\s*)");
                            currentElement.Inventors = new List<PartyMember>();

                            for (var j = 0; j < pattern.Count(); j++)
                            {
                                currentElement.Inventors.Add(new VN.PartyMember
                                {
                                    Name = Methods.PCT(pattern[j].Groups["name"].Value),
                                    Country = pattern[j].Groups["country"].Value.Replace("(", "").Replace(")", "").Trim(),
                                    Language = Methods.ToLang(pattern[j].Groups["country"].Value.Replace("(", "").Replace(")", "").Trim())
                                });

                                if (pattern[j].Groups["country"].Value.Contains("VN") || pattern[j].Groups["country"].Value.Contains("VI"))
                                {
                                    currentElement.Inventors[j].TransCountry = pattern[j].Groups["country"].Value.Replace("(", "").Replace(")", "").Trim();
                                    currentElement.Inventors[j].TransName = Methods.ToParties(pattern[j].Groups["name"].Value);
                                }
                            }
                        }

                        if (record.StartsWith(Agent))
                        {
                            var engPart = Regex.Match(record.Replace(Agent, "").Replace("\n", " ").Trim(), @"\(.+\)");
                            if (!string.IsNullOrEmpty(engPart.Value))
                            {
                                var agent = record.Replace(Agent, "").Replace(engPart.Value, "").Trim();
                                currentElement.Agents = new List<PartyMember>
                                {
                                    new PartyMember
                                    {
                                        Name = Methods.PCT(record.Replace(Agent, "").Replace(engPart.Value, "").Trim()),
                                        Country = "VN",
                                        Language = "VI"
                                    },
                                };

                                currentElement.Agents[0].TransCountry = "EN";
                                currentElement.Agents[0].TransName = engPart.Value.Replace("(", "").Replace(")", "").Trim();
                            }
                        }

                        if (record.StartsWith(Abstract))
                            currentElement.Abstract = new Abstract
                            {
                                Text = record.Replace(Abstract, "").Replace(record.Split('\n').Last(), "").Trim(),
                                Language = "VI"
                            };

                        if (record.StartsWith(Title))
                            currentElement.Title = record.Replace(Title, "").Trim();
                    }
                    if (splittedRecords.Count > 1)
                    {
                        elementsOut.Add(currentElement);
                    }
                }
            }

            return elementsOut;
        }
    }
}
