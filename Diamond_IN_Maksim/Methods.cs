using Integration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_IN_Maksim
{
    internal class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            var statusEvents = new List<Diamond.Core.Models.LegalStatusEvent>();

            var directory = new DirectoryInfo(path);

            var files = new List<string>();

            foreach (var file in directory.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            var xElements = new List<XElement>();

            foreach (var tetml in files)
            {
                _currentFileName = tetml;

                tet = XElement.Load(tetml);

                if (subCode == "1")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            //.SkipWhile(val => !val.Value.StartsWith("Early Publication:"))
                            .TakeWhile(val => !val.Value.StartsWith("Publication After 18 Months:"))
                            .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?=\(12\)\sPA)")
                        .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(12) PA")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "BZ"));
                    }
                }
                if(subCode == "10")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("WEEKLY ISSUED FER (DELHI)"))
                            .TakeWhile(val => !val.Value.StartsWith("PUBLICATION UNDER SECTION 57 AND UNDER RULE 81(3)"))
                            .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?>(\d{1,3}\sKOLKATA)|(\d{1,3}\sDELHI)|(\d{1,3}\sMUMBAI)|(\d{1,3}\sCHENNAI))")
                        .Where(val => !string.IsNullOrEmpty(val) && new Regex(@"\d").Match(val).Success).ToList();

                    var works = new List<string>();
                    for (var i = 0; i < notes.Count; i+=2)
                    {
                        works.Add(notes[i] + " " + notes[i+1]);
                    }

                    foreach (var note in works)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "EC"));
                    }
                }
                if (subCode == "26")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text").ToList();

                    var text = MakeText(xElements, subCode);

                    var notes = Regex.Split(text, @"(?=\(12\)\s*PATENT\b)", RegexOptions.IgnoreCase)
                        .Where(val => !string.IsNullOrWhiteSpace(val))
                        .Where(val => Regex.IsMatch(val, @"^\s*\(12\)\s*PATENT\b", RegexOptions.IgnoreCase))
                        .ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "AA"));
                    }
                }
            }

            return statusEvents;
        }

        internal Diamond.Core.Models.LegalStatusEvent MakePatent(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                CountryCode = "IN",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = _id++,
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                Biblio = new Biblio(),
                LegalEvent = new LegalEvent()
            };

            var culture = new CultureInfo("ru-RU");
            if(subCode == "1" || subCode == "26")
            {
                var priority = new Priority();
                foreach (var inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith("(12)"))
                    {
                        statusEvent.Biblio.Publication.LanguageDesignation = inid.Replace("(12)", "").Trim();
                    }
                    if (inid.StartsWith("(21)"))
                    {
                        if (subCode == "26")
                        {
                            var match = Regex.Match(inid.Trim(), @"No\.\s?(?<num>.+)\s(?<kind>\D)");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Application.Number = match.Groups["num"].Value.Trim();
                                statusEvent.Biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                            }
                            else Console.WriteLine($"{inid}  --- 21");
                        }
                        else
                        {
                            var match = Regex.Match(inid.Trim(), @"(?<num>\d+)\s(?<kind>\D)");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Application.Number = match.Groups["num"].Value.Trim();
                                statusEvent.Biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                            }
                            else Console.WriteLine($"{inid}  --- 21");
                        }
                        
                    }
                    if (inid.StartsWith("(22)"))
                    {
                        var match = Regex.Match(inid.Trim(), @"(?<date>\d{2}.\d{2}.\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} --- 22");
                    }
                    if (inid.StartsWith("(43)"))
                    {
                        var match = Regex.Match(inid.Trim(), @"(?<date>\d{2}.\d{2}.\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} --- 43");
                    }
                    if (inid.StartsWith("(54)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"invention :\s?(?<title>.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "EN",
                                Text = match.Groups["title"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid} --- 54");
                    }
                    if (inid.StartsWith("(71)"))
                    {
                       var applicants = Regex.Split(inid.Replace("(71)Name of Applicant :","").Trim(),
                            @"(?>\d+\))", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var applicant in applicants)
                        {
                            if (subCode != "26")
                            {
                                var match = Regex.Match(applicant.Trim(), @"(?<name>.+)\sAddr.+cant\s?:(?<adress>.+),(?<code>.+?)[\s--]", RegexOptions.Singleline);

                                if (match.Success)
                                {
                                    var countryCode = MakeCountryCode(match.Groups["code"].Value.Trim().TrimEnd('.'));

                                    if (countryCode == null)
                                    {
                                        Console.WriteLine($"<=================== {match.Groups["code"].Value}");
                                    }
                                    else
                                    {
                                        statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                        {
                                            Name = match.Groups["name"].Value.Trim(),
                                            Address1 = match.Groups["adress"].Value.Trim(),
                                            Country = countryCode
                                        });
                                    }
                                }
                                else
                                {
                                    var match1 = Regex.Match(applicant.Trim(), @"(?<name>.+)\sName\sof\sApplicant.+", RegexOptions.Singleline);

                                    if (match1.Success)
                                    {
                                        statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                        {
                                            Name = match1.Groups["name"].Value.Trim()
                                        });
                                    }
                                    else
                                    {
                                        statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                        {
                                            Name = applicant.Trim()
                                        });
                                    }
                                }
                            }
                            else
                            {
                                var match = Regex.Match(applicant.Trim(), @"(?<name>.+)\sAddr.+cant\s?:(?<adress>.+),(?<code>.+?)", RegexOptions.Singleline);

                                if (match.Success)
                                {
                                    var countryCode = MakeCountryCode(match.Groups["code"].Value.Trim().TrimEnd('.'));

                                    if (countryCode == null)
                                    {
                                        Console.WriteLine($"<=================== {match.Groups["code"].Value}");
                                    }
                                    else
                                    {
                                        statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                        {
                                            Name = match.Groups["name"].Value.Trim(),
                                            Address1 = match.Groups["adress"].Value.Trim(),
                                            Country = countryCode
                                        });
                                    }
                                }
                                else
                                {
                                    var match1 = Regex.Match(applicant.Trim(), @"(?<name>.+)\sName\sof\sApplicant.+", RegexOptions.Singleline);

                                    if (match1.Success)
                                    {
                                        statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                        {
                                            Name = match1.Groups["name"].Value.Trim()
                                        });
                                    }
                                    else
                                    {
                                        statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                        {
                                            Name = applicant.Trim()
                                        });
                                    }
                                }
                            }
                        }
                    }
                    if (inid.StartsWith("(72)"))
                    {
                        var inventors = Regex.Split(inid.Replace("(72)Name of Inventor :", "").Trim(),
                            @"(?>\d+\))", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var inventor in inventors)
                        {
                            var match = Regex.Match(inventor.Trim(), @"(?<name>.+)\sAddr.+cant\s?:(?<adress>.+),(?<code>.+?)[\s--]", RegexOptions.Singleline);

                            if (match.Success)
                            {
                                var countryCode = MakeCountryCode(match.Groups["code"].Value.Trim().TrimEnd('.'));

                                if (countryCode == null)
                                {
                                    Console.WriteLine($"<=================== {match.Groups["code"].Value}");
                                }

                                statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = countryCode
                                });
                            }
                            else
                            {
                                var match1 = Regex.Match(inventor.Trim(), @"(?<name>.+)\sName\sof\sApplicant.+", RegexOptions.Singleline);

                                if (match1.Success)
                                {
                                    statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                                    {
                                        Name = match1.Groups["name"].Value.Trim()
                                    });
                                }
                                else
                                {
                                    statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                                    {
                                        Name = inventor.Trim()
                                    });
                                }
                            }
                        }
                    }
                    if (inid.StartsWith("(57)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"act\s?:(?<text>.+?)\s(?<note>No\..+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                            {
                                Language = "EN",
                                Text = match.Groups["text"].Value.Trim()
                            });

                            var match1 = Regex.Match(match.Groups["note"].Value.Trim(), @"(?<a1>No\..+?)\s?:\s?(?<p>\d+)\s(?<a2>No\..+)\s?:\s?(?<c>\d+)");

                            if (match1.Success)
                            {
                                statusEvent.LegalEvent.Note = "|| " + match1.Groups["a1"].Value.Trim() + " | " + match1.Groups["p"].Value.Trim() + " || " + match1.Groups["a2"].Value.Trim()
                                    + " | " + match1.Groups["c"].Value.Trim();
                                statusEvent.LegalEvent.Language = "EN";
                            }
                            else Console.WriteLine($"{match.Groups["note"].Value.Trim()} --- note57");
                        }
                        else {
                            statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                            {
                                Language = "EN",
                                Text = inid.Replace("\r", "").Replace("\n", " ").Replace("(57)","").Replace("Abstract :","").Trim()
                            });
                        } 
                    }
                    if (inid.StartsWith("(51)"))
                    {
                        var ipcsData = Regex.Match(inid.Trim(), @".+:(?<ipcs>.+)", RegexOptions.Singleline).Groups["ipcs"].Value.Replace("classification","").Trim();
                        var ipcs = Regex.Split(ipcsData, @",", RegexOptions.Singleline).Where(_ => !string.IsNullOrEmpty(_)).ToList();

                        foreach (var ipc in ipcs)
                        {
                            var typeOne = Regex.Match(ipc.Trim(), @"(?<fPart>\D\d{2}\D)(?<sPart>\d{4})(?<tPart>.+)");
                            
                            if (typeOne.Success)
                            {
                                var fPart = typeOne.Groups["fPart"].Value.Trim();
                                var sPart = typeOne.Groups["sPart"].Value.Trim().TrimStart('0');
                                var tPart = typeOne.Groups["tPart"].Value.TrimEnd('0');
                                if (tPart.Length == 0)
                                {
                                    tPart = "00";
                                }

                                statusEvent.Biblio.Ipcs.Add(new Ipc()
                                {
                                    Class = fPart + " " + sPart + "/" + tPart 
                                });
                            }
                            else
                            {
                                var typeTwo = Regex.Match(ipc.Trim(), @"(?<fPart>\D\d{2}\D)\s(?<sPart>\d{2})(?<tPart>\d+)");

                                if (typeTwo.Success)
                                {
                                    var tPart = typeOne.Groups["tPart"].Value.TrimEnd('0');
                                    if (tPart.Length == 0)
                                    {
                                        tPart = "00";
                                    }

                                    statusEvent.Biblio.Ipcs.Add(new Ipc()
                                    {
                                        Class = typeTwo.Groups["fPart"].Value.Trim()+ " " + typeTwo.Groups["sPart"].Value.Trim().TrimStart('0') + "/" + tPart
                                    });
                                }
                                else
                                {
                                    var typeThree = Regex.Match(ipc.Trim(), @"(?<fPart>\D\d+\D)0(?<sPart>\d{2})(?<tPart>.+)");
                                    if (typeThree.Success)
                                    {
                                        var fPart = typeThree.Groups["fPart"].Value.Trim();
                                        var sPart = typeThree.Groups["sPart"].Value.Trim().TrimStart('0');
                                        var tPart = typeOne.Groups["tPart"].Value.TrimEnd('0');
                                        if (tPart.Length == 0)
                                        {
                                            tPart = "00";
                                        }

                                        statusEvent.Biblio.Ipcs.Add(new Ipc()
                                        {
                                            Class = fPart + " " + sPart + "/" + tPart
                                        });
                                    }
                                    else
                                    {
                                        var typeFour = Regex.Match(ipc.Trim(), @"(?<fPart>\D\d{2}\D)(?<sPart>\d+\/\d+)");
                                        if (typeFour.Success)
                                        {
                                            statusEvent.Biblio.Ipcs.Add(new Ipc()
                                            {
                                                Class = typeFour.Groups["fPart"].Value.Trim() + " " + typeFour.Groups["sPart"].Value.Trim()
                                            });
                                        }
                                        else
                                        {
                                            statusEvent.Biblio.Ipcs.Add(new Ipc()
                                            {
                                                Class = ipc
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (inid.StartsWith("(31)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"No\s?:(?<num>.+)");
                        if (match.Success)
                        {
                            priority.Number = match.Groups["num"].Value.Trim();
                        }
                    }

                    if (inid.StartsWith("(32)"))
                    {
                        var match = Regex.Match(inid.Trim(), @"(?<date>\d{2}.\d{2}.\d{4})");

                        if (match.Success)
                        {
                            priority.Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                    }

                    if (inid.StartsWith("(33)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"\s?:(?<country>.+)");
                        if (match.Success)
                        {
                            var code = MakeCountryCode(match.Groups["country"].Value.Trim());
                            if (code != null)
                            {
                                priority.Country = code;
                            }
                            else
                            {
                                Console.WriteLine($"{match.Groups["country"].Value.Trim()}");
                            }
                        }
                    }
                }

                if (subCode == "26")
                {
                    statusEvent.Biblio.Priorities.Add(priority);
                }
            }
            if(subCode == "10")
            {
                var match = Regex.Match(note.Trim(), @"\d{1,3}?\s(?<city>\D.+?)\s\s?(?<aNum>\d.+)\s(?<eDate>\d{2}\/\d{2}\/\d{4}).+?(?<f74>[A-Z].+)\s(?<note>.+@.+)");

                if (match.Success)
                {
                    statusEvent.Biblio.Application.Number = match.Groups["aNum"].Value.Trim();

                    statusEvent.LegalEvent.Note = "|| LOCATION | " + match.Groups["city"].Value.Trim() + " || EMAIL | " + match.Groups["note"].Value.Trim();

                    statusEvent.LegalEvent.Date = DateTime.Parse(match.Groups["eDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                    statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                    {
                        Address1 = match.Groups["f74"].Value.Trim()
                    });
                }
                else
                {
                    Console.WriteLine($"{note}");
                }
            }

            return statusEvent;
        }

        private string MakeCountryCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }

            code = code.Trim().TrimEnd('.', ',').Trim().ToUpperInvariant();

            return code switch
            {
                "TAMILNADU" => "IN",
                "INDIA" => "IN",
                "HARYANA" => "IN",
                "GURUGRAM" => "IN",
                "TIRUNELVELI" => "IN",
                "UTTAR" => "IN",
                "CHINA" => "CN",
                "DELHI" => "IN",
                "MAHARASHTRA" => "IN",
                "DEHRADUN-" => "IN",
                "CHHATTISGARH-" => "IN",
                "UTTARAKHAND" => "IN",
                "GAJRAULA" => "IN",
                "AMROHA(U.P)" => "IN",
                "THIRUVALLUR" => "IN",
                "MATHURA" => "IN",
                "JHARKHAND" => "IN",
                "TULJAPUR" => "IN",
                "ANDHRA" => "IN",
                "BANGALORE" => "IN",
                "TAMIL" => "IN",
                "GAUTAM" => "IN",
                "JAIPUR" => "IN",
                "GURGAON" => "IN",
                "RUSSIAN" => "RU",
                "RAJASTHAN" => "IN",
                "GUANGDONG" => "CN",
                "MEERUT" => "IN",
                "KURUKSHETRA" => "IN",
                "GHAZIABAD" => "IN",
                "DENMARK" => "DK",
                "MUMBAI" => "IN",
                "AMRVATI" => "IN",
                "KANPUR" => "IN",
                "VADODARA" => "IN",
                "SAITAMA" => "JP",
                "UDUPI" => "IN",
                "MANGALORE" => "IN",
                "GERMANY" => "DE",
                "ODISHA" => "IN",
                "FINLAND" => "FI",
                "ITANAGAR" => "IN",
                "COIMBATORE" => "IN",
                "KARNATAKA" => "IN",
                "NOIDA" => "IN",
                "INDORE" => "IN",
                "KOMPALL" => "IN",
                "KOMPALLY" => "IN",
                "HYDERABAD" => "IN",
                "CHENNAI" => "IN",
                "BENGALURU" => "IN",
                "TELANGANA" => "IN",
                "IRELAND" => "IE",
                "JAPAN" => "JP",
                "TOKYO" => "JP",
                "OSAKA" => "JP",
                "UTAH" => "US",
                "USA" => "US",
                "CALIFORNIA" => "US",
                "WASHINGTON" => "US",
                "NC" => "US",
                "MASSACHUSETTS" => "US",
                "FLORIDA" => "US",
                "PENNSYLVANIA" => "US",
                "ONTARIO" => "CA",
                "COLORADO" => "US",
                "SEOUL" => "KR",
                "SWITZERLAND" => "CH",
                "SAUDI" => "SA",
                "FRANCE" => "FR",
                "U.K" => "GB",
                // Australia
                "AUSTRALIA" => "AU",

                // Spain
                "SPAIN" => "ES",

                // Austria
                "AUSTRIA" => "AT",

                // Sweden
                "SWEDEN" => "SE",

                // Hungary
                "HUNGARY" => "HU",

                // Netherlands
                "NETHERLANDS" => "NL",
                "NL" => "NL",

                // Cuba
                "CUBA" => "CU",

                // Algeria
                "ALGERIA" => "DZ",

                // Italy
                "ITALY" => "IT",

                // South Korea
                "REPUBLIC OF KOREA" => "KR",
                "KOREA" => "KR",

                // Singapore
                "SINGAPORE" => "SG",

                // Taiwan
                "TAIWAN" => "TW",

                // New Zealand
                "NEW ZEALAND" => "NZ",

                // Israel
                "ISRAEL" => "IL",

                // Argentina
                "ARGENTINA" => "AR",

                // Estonia
                "ESTONIA" => "EE",

                // Brazil
                "BRAZIL" => "BR",

                // Hong Kong
                "HONGKONG(CHINA)" => "HK",
                "HONG KONG" => "HK",
                "HONGKONG" => "HK",

                // Additional abbreviations
                "GB" => "GB",
                "G.B" => "GB",
                "DE" => "DE",
                "CH" => "CH",
                "BR" => "BR",
                "P.R.CHINA" => "CN",
                "U.S.A" => "US",
                _ => null
            };
        }
        private List<string> MakeInids(string note, string subCode)
        {
            var inids = new List<string>();

            if (subCode == "26")
            {
                var inids26Match = Regex.Match(note, @"(?<part1>.+)(?<inid57>\(57\).+)(?<part3>\(71\).+)", RegexOptions.Singleline);
                if (inids26Match.Success)
                {
                    inids = Regex.Split(inids26Match.Groups["part1"].Value.Trim(), @"(?=\(\d{2}\).+)", RegexOptions.Singleline).Where(_ => !string.IsNullOrEmpty(_)).ToList();
                    inids.Add(inids26Match.Groups["inid57"].Value.Trim());
                    
                    var inidsPart3 = Regex.Split(inids26Match.Groups["part3"].Value.Trim(), @"(?=\(\d{2}\).+)", RegexOptions.Singleline).Where(_ => !string.IsNullOrEmpty(_)).ToList();
                    inids.AddRange(inidsPart3);
                    return inids;
                }
                else
                {
                    var inidsMatch26 = Regex.Match(note, @"(?<part1>.+)(?<inid57>\(57\).+)", RegexOptions.Singleline);

                    if (inidsMatch26.Success)
                    {
                        inids = Regex
                            .Split(inidsMatch26.Groups["part1"].Value.Trim(), @"(?=\(\d{2}\).+)",
                                RegexOptions.Singleline).Where(_ => !string.IsNullOrEmpty(_)).ToList();
                        inids.Add(inidsMatch26.Groups["inid57"].Value.Trim());
                        return inids;
                    }
                    else
                    {
                        Console.WriteLine($"{inids26Match} - wrong split regex");
                    }
                }
            }
            var inidsMatch = Regex.Match(note, @"(?<part1>.+)(?<inid57>\(57\).+)", RegexOptions.Singleline);

            if (inidsMatch.Success)
            {
                inids = Regex.Split(inidsMatch.Groups["part1"].Value.Trim(), @"(?=\(\d{2}\).+)", RegexOptions.Singleline).Where(_ => !string.IsNullOrEmpty(_)).ToList();
                inids.Add(inidsMatch.Groups["inid57"].Value.Trim());
            }
            return inids;
        }
        private string MakeText(List<XElement> xElements, string subCode)
        {
            string text = null;
            if(subCode == "1")
            {
                foreach (var xElement in xElements)
                {
                    text += xElement.Value + "\n";
                }

                return text;
            }
            if (subCode == "10")
            {
                foreach (var xElement in xElements)
                {
                    text += xElement.Value + " ";
                }

                return text.Replace("\r", "").Replace("\n", " ").Trim();
            }
            if (subCode == "26")
            {
                return string.Join(" ", xElements.Select(x => x.Value))
                    .Replace("\r", "")
                    .Replace("\n", " ")
                    .Trim();
            }
            return null;
        }
    }
}
