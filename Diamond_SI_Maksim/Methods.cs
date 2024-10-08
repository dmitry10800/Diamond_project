﻿using Integration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_SI_Maksim
{
    class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        private const string I11 = "(11)";
        private const string I13 = "(13)";
        private const string I51 = "(51)";
        private const string I21 = "(21)";
        private const string I22 = "(22)";
        private const string I46 = "(46)";
        private const string I86 = "(86)";
        private const string I87 = "(87)";
        private const string I96 = "(96)";
        private const string I97 = "(97)";
        private const string I72 = "(72)";
        private const string I73 = "(73)";
        private const string I74 = "(74)";
        private const string I30 = "(30)";
        private const string I54 = "(54)";

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalStatuses = new();

            DirectoryInfo directory = new(path);

            List<string> files = new();

            foreach (var file in directory.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            List<XElement> xElements = null;

            foreach (var tetml in files)
            {
                _currentFileName = tetml;

                tet = XElement.Load(tetml);

                if(subCode == "20")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                       .SkipWhile(val => !val.Value.StartsWith("Prevodi zahtevkov evropskih patentov (T1,T2,T4)"))
                       .TakeWhile(val => !val.Value.StartsWith("Kazalo po kodah MPK"))
                       .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=\(51\))").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(51)")).ToList();

                    foreach (var note in notes)
                    {
                        legalStatuses.Add(MakeConvertedPatent(note, subCode, "BA"));
                    }
                }
            }
            return legalStatuses;
        }
        internal string MakeText(List<XElement> xElements)
        {
            string text = null;

            foreach (var xElement in xElements)
            {
                text += xElement.Value + " ";
            }

            return text;
        }
        internal Diamond.Core.Models.LegalStatusEvent MakeConvertedPatent (string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent legalStatus = new()
            {
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                CountryCode = "SI",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = _id++
            };

            Biblio biblio = new();

            CultureInfo culture = new("Ru-ru");

            biblio.DOfPublication = new();
            biblio.IntConvention = new();
            biblio.EuropeanPatents = new();
            EuropeanPatent europeanPatent = new();

            foreach (var inid in SplitNote20(note))
            {
                if (inid.StartsWith(I11))
                {
                    biblio.Publication.Number = inid.Replace(I11, "").Trim();
                }
                else
                if (inid.StartsWith(I13))
                {
                    biblio.Publication.Kind = inid.Replace(I13, "").Trim();
                }
                else
                if (inid.StartsWith(I51))
                {
                    var ipcs = Regex.Split(inid.Replace(I51, "").Trim(), @"(?<=[0-9]\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    biblio.Ipcs = new();

                    foreach (var ipc in ipcs)
                    {
                        biblio.Ipcs.Add(new Ipc
                        {
                            Class = ipc
                        });
                    }
                }
                else
                if (inid.StartsWith(I21))
                {
                    biblio.Application.Number = inid.Replace(I21, "").Trim();
                }
                else
                if (inid.StartsWith(I22))
                {
                    biblio.Application.Date = DateTime.Parse(inid.Replace(I22, "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "-").Trim();
                }
                else
                if (inid.StartsWith(I46))
                {
                    biblio.DOfPublication.date_46 = DateTime.Parse(inid.Replace(I46,"").Trim(),culture).ToString("yyyy.MM.dd").Replace(".", "-").Trim();
                }
                else
                if (inid.StartsWith(I86))
                {
                    var match = Regex.Match(inid.Replace(I86, "").Trim(), @"(?<date>[0-9]{2}.[0-9].{2}[0-9]{4})\s(?<code>[A-Z]{2})\s(?<number>.+)");

                    if (match.Success)
                    {
                        biblio.IntConvention.PctApplNumber = match.Groups["number"].Value.Trim();
                        biblio.IntConvention.PctApplDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "-").Trim();
                        biblio.IntConvention.PctApplCountry = match.Groups["code"].Value.Trim();
                    }
                    else Console.WriteLine($"{inid} Не разбился 86");
                }
                else
                if (inid.StartsWith(I87))
                {
                    var match = Regex.Match(inid.Replace(I87, "").Trim(), @"(?<code>[A-Z]{2})\s(?<number>.+),?\s(?<date>[0-9]{2}.[0-9].{2}[0-9]{4})");

                    if (match.Success)
                    {
                        biblio.IntConvention.PctPublNumber = match.Groups["number"].Value.TrimEnd(',').Trim();
                        biblio.IntConvention.PctPublDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "-").Trim();
                        biblio.IntConvention.PctPublCountry = match.Groups["code"].Value.Trim();
                    }
                    else Console.WriteLine($"{inid} Не разбился 87");
                }
                else
                if (inid.StartsWith(I96))
                {
                    var match = Regex.Match(inid.Replace(I96, "").Trim(), @"(?<date>[0-9]{2}.[0-9].{2}[0-9]{4})\s(?<code>[A-Z]{2})\s(?<number>.+)");

                    if (match.Success)
                    {
                        europeanPatent.AppNumber = match.Groups["number"].Value.Trim();
                        europeanPatent.AppDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "-").Trim();
                        europeanPatent.AppCountry = match.Groups["code"].Value.Trim();
                    }
                    else Console.WriteLine($"{inid} Не разбился 96");
                }
                else
                if (inid.StartsWith(I97))
                {
                    var euros = Regex.Split(inid.Replace(I97, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (var euro in euros)
                    {
                        var match = Regex.Match(euro.Trim(), @"(?<code>[A-Z]{2})\s(?<number>.+)\s(?<kind>[A-Z]{1,2}[0-9]{1,2}),?\s(?<date>[0-9]{2}.[0-9].{2}[0-9]{4})");
                        if (euro == euros[0])
                        {
                            if (match.Success)
                            {
                                europeanPatent.PubNumber = match.Groups["number"].Value.Trim();
                                europeanPatent.PubDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "-").Trim();
                                europeanPatent.PubCountry = match.Groups["code"].Value.Trim();
                                europeanPatent.PubKind = match.Groups["kind"].Value.Trim();
                            }
                            biblio.EuropeanPatents.Add(europeanPatent);
                        }
                        else
                        {
                            if (match.Success)
                            {
                                biblio.EuropeanPatents.Add(new EuropeanPatent
                                {
                                    PubNumber = match.Groups["number"].Value.Trim(),
                                    PubDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "-").Trim(),
                                    PubCountry = match.Groups["code"].Value.Trim(),
                                    PubKind = match.Groups["kind"].Value.Trim()
                                });
                            }
                        }
                    }
                }
                else
                if (inid.StartsWith(I72))
                {
                    var inventors = Regex.Split(inid.Replace(I72, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    biblio.Inventors = new();

                    foreach (var inventor in inventors)
                    {
                        var match = Regex.Match(inventor, @"(?<name>.+?),\s(?<adress>.+),\s(?<code>[A-Z]{2})");

                        if (match.Success)
                        {
                            biblio.Inventors.Add(new PartyMember
                            {
                                Name = match.Groups["name"].Value.Trim(),
                                Address1 = match.Groups["adress"].Value.Trim(),
                                Country = match.Groups["code"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inventor} не разбился в 72");
                    }
                }
                else
                if (inid.StartsWith(I73))
                {
                    var assignees = Regex.Split(inid.Replace(I73, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    biblio.Assignees = new();

                    foreach (var assigner in assignees)
                    {
                        var match = Regex.Match(assigner, @"(?<name>.+?),\s(?<adress>.+),\s(?<code>[A-Z]{2})");

                        if (match.Success)
                        {
                            biblio.Assignees.Add(new PartyMember
                            {
                                Name = match.Groups["name"].Value.Trim(),
                                Address1 = match.Groups["adress"].Value.Trim(),
                                Country = match.Groups["code"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{assigner} не разбился в 73");
                    }
                }
                else
                if (inid.StartsWith(I74))
                {
                    var agents = Regex.Split(inid.Replace(I74, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    biblio.Agents = new();

                    foreach (var agent in agents)
                    {
                        var match = Regex.Match(agent, @"(?<name>.+?),\s(?<adress>.+),\s(?<code>[A-Z]{2})");

                        if (match.Success)
                        {
                            biblio.Agents.Add(new PartyMember
                            {
                                Name = match.Groups["name"].Value.Trim(),
                                Address1 = match.Groups["adress"].Value.Trim(),
                                Country = match.Groups["code"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{agent} не разбился в 74");
                    }
                }
                else
                if (inid.StartsWith(I30))
                {
                    var priorities = Regex.Split(inid.Replace(I30, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    biblio.Priorities = new();

                    foreach (var priority in priorities)
                    {
                        var match = Regex.Match(priority, @"(?<date>[0-9]{2}.[0-9].{2}[0-9]{4})\s(?<code>[A-Z]{2})\s(?<number>.+)");

                        if (match.Success)
                        {
                            biblio.Priorities.Add(new Priority
                            {
                                Number = match.Groups["number"].Value.Trim(),
                                Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "-").Trim(),
                                Country = match.Groups["code"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{priority} не разбился в 30");
                    }
                }
                else
                if (inid.StartsWith(I54))
                {
                    biblio.Titles = new List<Title>
                        {
                            new Title
                            {
                                Text = inid.Replace(I54, "").Trim(),
                                Language = "SI"
                            }
                        };
                }
                else Console.WriteLine($"{inid} не обработан");

            }
            
            legalStatus.Biblio = biblio;

            return legalStatus;
        }
        internal List<string> SplitNote20 (string note)
        {
            List<string> inids = new();
            var match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<group1>\(51.+)\s(?<group2>\(21.+)");

            if (match.Success)
            {
                inids = Regex.Split(match.Groups["group2"].Value.Trim(), @"(?=\([0-9]{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

                var inid51 = Regex.Replace(match.Groups["group1"].Value.Trim(), @"\(11\)\s.+?\s", "");
                inid51 = Regex.Replace(inid51, @"\(13\)\s.+?\s", "");
                inid51 = Regex.Replace(inid51, @"\(46\)\s[0-9]{2}.[0-9]{2}.[0-9]{4}\s?", "");

                inids.Add(inid51);


                var match1 = Regex.Match(match.Groups["group1"].Value.Trim(), @"(?<inid11>\(11\)\s?[0-9]+).*(?<inid13>\(13\)\s?[A-Z]{1,2}[0-9]{1,2}).*(?<inid46>\(46\)\s?[0-9]{2}.[0-9]{2}.[0-9]{4})");

                if (match1.Success)
                {
                    inids.Add(match1.Groups["inid11"].Value.Trim());
                    inids.Add(match1.Groups["inid13"].Value.Trim());
                    inids.Add(match1.Groups["inid46"].Value.Trim());
                }
                else Console.WriteLine($"{match.Groups["group1"].Value.Trim()} -------------- Не разбился 11/13/46 айнид");
            }
            else Console.WriteLine($"{note} --------- Не разбилась запись");

            return inids;
        }
    }
}
