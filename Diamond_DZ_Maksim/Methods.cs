﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Integration;

namespace Diamond_DZ_Maksim
{
    internal class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        private const string I11 = "(11)";
        private const string I21 = "(21)";
        private const string I22 = "(22)";
        private const string I24 = "(24)";
        private const string I30 = "(30)";
        private const string I51 = "(51)";
        private const string I54 = "(54)";
        private const string I73 = "(73)";
        private const string I74 = "(74)";
        private const string I57 = "(57)";
        private const string I86 = "(86)";

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> statusEvents = new();

            DirectoryInfo directory = new(path);

            List<string> files = new();

            foreach (var file in directory.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            List<XElement> xElements = new();

            foreach (var tetml in files)
            {
                _currentFileName = tetml;

                tet = XElement.Load(tetml);

                if(subCode == "1")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                         .SkipWhile(val => !val.Value.Contains("(54) Titre de l'invention"))
                         .TakeWhile(val => !val.Value.StartsWith("BON DE SOUSCRIPTION"))
                         .ToList();
                        
                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?=\(11\)\s\d)").Where(val => !string.IsNullOrEmpty(val) && new Regex(@"\(11\)\s\d+").Match(val).Success).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "BA"));
                    }
                }
            }
            return statusEvents;
        }
        internal Diamond.Core.Models.LegalStatusEvent MakePatent(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                SubCode = subCode,
                SectionCode = sectionCode,
                CountryCode = "DZ",
                Id = _id++,
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                Biblio = new(),
                LegalEvent = new()
            };

            CultureInfo cultureInfo = new("ru-Ru");

            if(subCode == "1")
            {
                foreach (var inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith(I11))
                    {
                        statusEvent.Biblio.Publication.Number = inid.Replace("\r", "").Replace("\n", " ").Replace(I11, "").Trim();
                    }
                    else if (inid.StartsWith(I22))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace(I22, "").Trim(), @"(?<day>\d{2})\s(?<month>.+)\s(?<year>\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Date = match.Groups["year"].Value.Trim() + "/" + MakeMonth(match.Groups["month"].Value.Trim()) + "/" + match.Groups["day"].Value.Trim();

                            if(MakeMonth(match.Groups["month"].Value.Trim()) == null)
                            {
                                Console.WriteLine(match.Groups["month"].Value.Trim());
                            }
                        }
                        else Console.WriteLine($"{inid}  --- 22");
                    }
                    else if (inid.StartsWith(I21))
                    {
                        statusEvent.Biblio.Application.Number = inid.Replace("\r", "").Replace("\n", " ").Replace(I21, "").Trim();
                        statusEvent.Biblio.IntConvention.PctApplNumber = inid.Replace("\r", "").Replace("\n", " ").Replace(I21, "").Trim();
                    }
                    else if (inid.StartsWith(I30))
                    {
                        var priorities = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace(I30, "").Trim(), @"(?<=\d{2}\.\d{2}\.\d{4})").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var priority in priorities)
                        {
                            var match = Regex.Match(priority.Trim(), @"(?<code>[A-Z]{2})\s?(?<num>.+)du\s?(?<date>\d{2}\.\d{2}\.\d{4})");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Priorities.Add(new Integration.Priority
                                {
                                    Country = match.Groups["code"].Value.Trim(),
                                    Number = match.Groups["num"].Value.TrimStart(',').Trim(),
                                    Date = DateTime.Parse(match.Groups["date"].Value.Trim(),cultureInfo).ToString(@"yyyy.MM.dd").Replace(".","/").Trim()
                                });
                            }
                            else 
                            {
                                var match1 = Regex.Match(priority.Trim(), @"(?<code>[A-Z]{2})\s?(?<num>.+)\s(?<date>\d{2}\.\d{2}\.\d{4})");

                                if (match1.Success)
                                {
                                    statusEvent.Biblio.Priorities.Add(new Integration.Priority
                                    {
                                        Country = match1.Groups["code"].Value.Trim(),
                                        Number = match1.Groups["num"].Value.Trim(),
                                        Date = DateTime.Parse(match1.Groups["date"].Value.Trim(), cultureInfo).ToString(@"yyyy.MM.dd").Replace(".", "/").Trim()
                                    });
                                }
                                else   Console.WriteLine($"{priority} ---  {statusEvent.Biblio.Publication.Number}  ------  30");
                            }
                          
                        }
                    }
                    else if (inid.StartsWith(I74))
                    {
                        statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                        {
                            Name = inid.Replace("\r", "").Replace("\n", " ").Replace(I74, "").Trim()
                        });
                    }
                    else if (inid.StartsWith(I73))
                    {
                        var assignees = Regex.Split(inid.Replace(I73, "").Trim(), @"(?=\d{1,4}-\s)", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var assigny in assignees)
                        {
                            var match = Regex.Match(assigny.Trim(), @"(?<name>(.+\n)?.+)\n(?<adress>(.+\n)?(.+\n)?.+)\n(?<country>.+)\.?");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
                                {
                                    Country = MakeCountry(match.Groups["country"].Value.TrimEnd('.').Trim()),
                                    Name = match.Groups["name"].Value.Replace("\r", "").Replace("\n", " ").TrimEnd('.').Trim(),
                                    Address1 = match.Groups["adress"].Value.Replace("\r", "").Replace("\n", " ").Trim()
                                });

                                if (MakeCountry(match.Groups["country"].Value.TrimEnd('.').Trim()) == null)
                                {
                                    Console.WriteLine(match.Groups["country"].Value.TrimEnd('.').Trim() + $" -------  {statusEvent.Biblio.Publication.Number}");
                                }
                            }
                            else Console.WriteLine($"{assigny}  ---- 73");
                        }
                    }
                    else if (inid.StartsWith(I54))
                    {
                        statusEvent.Biblio.Titles.Add(new Integration.Title
                        {
                            Language = "FR",
                            Text = inid.Replace("\r", "").Replace("\n", " ").Replace(I54, "").Trim()
                        });
                    }
                    else if (inid.StartsWith(I57))
                    {
                        statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                        {
                            Language = "FR",
                            Text = inid.Replace("\r", "").Replace("\n", " ").Replace(I57, "").Replace("_","").Trim()
                        });
                    }
                    else if (inid.StartsWith(I86))
                    {
                        var match = Regex.Match(inid.Replace(I86, "").Trim(),
                            @"(?<day>\d{2})\s?(?<month>\D+)\s(?<year>\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.IntConvention.PctApplDate = match.Groups["year"].Value.Trim() + "/" +
                                                                           MakeMonth(match.Groups["month"].Value
                                                                               .Trim()) + "/" +
                                                                           match.Groups["day"].Value.Trim();

                            if (MakeMonth(match.Groups["month"].Value.Trim()) == null)
                            {
                                Console.WriteLine(match.Groups["month"].Value.Trim());
                            }
                        }

                        var match1 = Regex.Match(inid.Replace(I86, "").Trim(), @"\D+\/.+");

                        if (match1.Success)
                        {
                            statusEvent.Biblio.Application.Number = match1.Value.Trim();
                            statusEvent.Biblio.IntConvention.PctApplNumber = match1.Value.Trim();
                        }
                    }
                    else if (inid.StartsWith(I24))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace(I24, "").Trim(), @"(?<day>\d{2})\s(?<month>.+)\s(?<year>\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.EffectiveDate = match.Groups["year"].Value.Trim() + "/" + MakeMonth(match.Groups["month"].Value.Trim()) + "/" + match.Groups["day"].Value.Trim();

                            if (MakeMonth(match.Groups["month"].Value.Trim()) == null)
                            {
                                Console.WriteLine(match.Groups["month"].Value.Trim());
                            }
                        }
                        else Console.WriteLine($"{inid}  --- 24");
                    }
                    else if (inid.StartsWith(I51))
                    {
                        var ipcs = Regex.Split(inid.Replace(I51, "").Trim(), @"(?<=\d{1,3}\/\d{1,3}\s)", RegexOptions.Multiline)
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var ipc in ipcs)
                        {
                            statusEvent.Biblio.Ipcs.Add(new Ipc()
                            {
                                Class = ipc.Trim().TrimStart('-').Trim()
                            });
                        }
                    }
                    else Console.WriteLine($"{inid} - don't process");
                }
            }
            return statusEvent;
        }
        internal string MakeCountry(string country) => country switch
        {
            "ETATS-UNIS D'AMERIQUE" => "US",
            "ÉTATS-UNIS D’AMÉRIQUE" => "US",
            "ITALIE" => "IT",
            "FRANCE" => "FR",
            "ESPAGNE" => "ES",
            "IRLANDE" => "IE",
            "TURQUIE" => "TR",
            "DANEMARK" => "DK",
            "COLOMBIE" => "CO",
            "SUISSE" => "CH",
            "ALLEMAGNE" => "DE",
            "AUTRICHE" => "AT",
            "TUNISIE" => "TN",
            "GRANDE BRETAGNE" => "UK",
            "ALGERIE" => "DZ",
            "ALGÉRIE" => "DZ",
            "JAPON" => "JP",
            "AUSTRALIE" => "AU",
            "INDE" => "IN",
            "LES PAYS-BAS" => "NL",
            "SUEDE" => "SE",
            "SUÈDE" => "SE",
            "SUÉDE" => "SE",
            "NORVÈGE" => "NO",
            "CHINE" => "CN",
            "PAYS-BAS" => "NL",
            "CANADA" => "CA",
            "BELGIQUE" => "BE",
            "ILES VIERGES BRITANIQUES" => "VG",
            "ILE VIERGE BRITANIQUE" => "VG",
            "ÎLES VIERGES BRITANIQUES" => "VG",
            "ILES VIERGES BRITANNIQUES" => "VG",
            "ÎLE VIERGE BRITANIQUE" => "VG",
            "ÎLES VIÈRGES BRITANNIQUES" => "VG",
            "ARABIE SAOUDITE" => "SA",
            "FINLANDE" => "FI",
            "HONGRIE" => "HU",
            "HOLLANDE" => "NL",
            "NORVEGE" => "NO",
            "FEDERATION DE RUSSIE" => "RU",
            "FÉDÉRATION DE RUSSIE" => "RU",
            "LUXEMBOURG" => "LU",
            "GIBRALTAR" => "GI",
            "VENEZUELA" => "VE",
            "GRANDE-BRETAGNE" => "UK",
            "ROYAUME-UNI" =>"UK",
            "Royaume-Uni" => "UK",
            "EMIRATS ARABES UNIS" => "AE",
            "ETATS-UNIS-D'AMERIQUE" => "US",
            "ÉTATS-UNIS-D’AMÉRIQUE" => "US",
            "ÉTAT-UNIS D’AMÉRIQUE" => "US",
            "GRECE" => "GR",
            "GRÈCE" => "GR",
            "CHANNEL ISLANDS" => "GB",
            "THAILANDE" => "TH",
            "CROATIE" => "HR",
            "CORÉE" => "KR",
            "SERBIE" => "RS",
            "Alger" => "DZ",
            "DANMARK" => "DK",
            "FINLAND" => "FI",
            "IRELAND" => "IE",
            "BERMUDES" => "BM",
            "SINGAPOUR" => "SG",
            "JAPAN" => "JP",
            "CUBA" => "CU",
            "MAROC" => "MA",
            "JORDANIE" => "JO",
            "PUERTO RICO" => "PR",
            "SINGAPOURE" => "SG",
            "SYRIE" => "SY",
            "Luxembourg" => "LU",
            "AFRIQUE DU SUD" => "ZA",
            "RÉPUBLIQUE DE MOLDOVA" => "MD",
            "ÉTAT-UNIS D'AMÉRIQUE" => "US",
            "SAINT-VINCENT ET LES GRENADINES" => "VC",
            _ => null,
        };
        internal string MakeMonth(string month) => month switch
        {
            "Janvier" => "01",
            "Février" => "02",
            "Mars" => "03",
            "Avril" => "04",
            "Mai" => "05",
            "Juin" => "06",
            "Juillet" => "07",
            "Août" => "08",
            "Septembre" => "09",
            "Septemebre" => "09",
            "Octobre" => "10",
            "Novembre" => "11",
            "Décembre" => "12",
            _ => null,
        };
        internal List<string> MakeInids(string note, string subCode)
        {
            List<string> inids = new();

            if(subCode == "1")
            {
                inids = Regex.Split(note.Substring(0, note.IndexOf("(57)")).Trim(), @"(?=\(\d{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();
                inids.Add(note.Substring(note.IndexOf("(57)")));
            }

            return inids;
        }
        internal string MakeText(List<XElement> xElements, string subCode)
        {
            string text = null;

            if(subCode == "1")
            {
                foreach (var xElement in xElements)
                {
                    text += xElement.Value + "\n";
                }
            }

            return text;
        }
    }
}
