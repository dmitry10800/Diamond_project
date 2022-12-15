using Integration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace IS_Diamond_Maksim
{
    class Methods
    {
        private string CurrentFileName;
        private int id = 1;

        private string I11 = "(11)";
        private string I51 = "(51)";
        private string I54 = "(54)";
        private string I73 = "(73)";
        private string I74 = "(74)";
        private string I30 = "(30)";
        private string I80 = "(80)";
        private string I86 = "(86)";


        internal List<Diamond.Core.Models.LegalStatusEvent> Start (string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = new();

            DirectoryInfo directory = new(path);

            List<string> files = new();

            foreach (FileInfo file in directory.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            List<XElement> xElements = null;

            foreach (string tetml in files)
            {
                CurrentFileName = tetml;

                tet = XElement.Load(tetml);

                if(subCode == "3")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Evrópsk einkaleyfi sem öðlast hafa"))
                             .TakeWhile(val => !val.Value.StartsWith("Breytt útgáfa evrópskra einkaleyfa í gildi á slandi eftir takmörkun (T4)") && !val.Value.StartsWith("Breytt útgáfa evrópskra einkaleyfa í"))
                             .ToList();

                    List<string> notes = Regex.Split(BuildText(xElements), @"(?=\(11\))").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(11)")).ToList();

                    foreach (string note in notes)
                    {
                        convertedPatents.Add(MakeConvertPatent(note, subCode, "FG"));
                    }
                }
                else if (subCode is "9")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName is "Text")
                        .SkipWhile(val => !val.Value.StartsWith("IS/EP einkaleyfi staðfest hér á landi sem fallin eru úr gildi skv. 2. mgr. 81. gr. laga nr. 17/1991 um"))
                        .TakeWhile(val => !val.Value.StartsWith("Einkaleyfisumsóknir afskrifaðar skv. 2. mgr. 15. gr. laga nr. 17/1991 um einkaleyfi:"))
                        .ToList();

                    List<string> notes = Regex.Split(BuildText(xElements), @",").Where(val => !string.IsNullOrEmpty(val)).Where(val => new Regex(@"\D{2}\d+").Match(val).Success).ToList();

                    foreach (string note in notes)
                    {
                        convertedPatents.Add(MakeConvertPatent(note, subCode, "NG"));
                    }
                }
            }

            return convertedPatents;
        }

        internal string BuildText(List<XElement> xElement)
        {
            string text = null;

            foreach (XElement note in xElement)
            {
                text += note.Value.Trim() + " ";
            }

            return text;
        }

        internal Diamond.Core.Models.LegalStatusEvent MakeConvertPatent(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent legalStatusEvent = new()
            {
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                SubCode = subCode,
                SectionCode = sectionCode,
                CountryCode = "IS",
                Id = id++,
                Biblio = new(),
                LegalEvent = new()
            };

            Biblio biblio = new();

            CultureInfo cultureInfo = new("Ru-ru");

            if (subCode is "3")
            {
                foreach (string inid in SplitNote(note))
                {
                    if (inid.StartsWith(I11))
                    {
                        Match match = Regex.Match(inid.Replace(I11, "").Trim(), @"(?<number>.+)\s(?<kind>[A-Z]{1}[0-9]{1})");

                        if (match.Success)
                        {
                            biblio.Publication.Number = match.Groups["number"].Value.Trim();
                            biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else
                    if (inid.StartsWith(I51))
                    {
                        biblio.Ipcs = new List<Ipc>();

                        List<string> ipcs = Regex.Split(inid.Replace(I51, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string ipc in ipcs)
                        {
                            biblio.Ipcs.Add(new Ipc
                            {
                                Class = ipc
                            });
                        }
                    }
                    else
                    if (inid.StartsWith(I54))
                    {
                        biblio.Titles = new();

                        biblio.Titles.Add(new Title
                        {
                            Language = "IS",
                            Text = inid.Replace(I54, "").Trim()
                        });
                    }
                    else
                    if (inid.StartsWith(I73))
                    {
                        List<string> owners = Regex.Split(inid.Replace(I73, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        biblio.Assignees = new();

                        foreach (string owner in owners)
                        {
                            Match match = Regex.Match(owner, @"(?<name>.+?),\s(?<adress>.+),\s(?<country>.+)");

                            if (match.Success)
                            {
                                biblio.Assignees.Add(new PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = MakeCountryCode(match.Groups["country"].Value.Trim())
                                });

                                if (MakeCountryCode(match.Groups["country"].Value.Trim()) == null)
                                {
                                    Console.WriteLine(match.Groups["country"].Value.Trim());
                                }
                            }
                            else Console.WriteLine($"{owner}");
                        }
                    }
                    else
                    if (inid.StartsWith(I74))
                    {
                        Match match = Regex.Match(inid.Replace(I74, ""), @"(?<name>.+?),\s(?<adress>.+),?\s(?<country>.+)");

                        biblio.Agents = new();

                        if (match.Success)
                        {
                            biblio.Agents.Add(new PartyMember
                            {
                                Name = match.Groups["name"].Value.Trim(),
                                Address1 = match.Groups["adress"].Value.TrimEnd(','),
                                Country = MakeCountryCode(match.Groups["country"].Value.Trim())
                            });

                            if (MakeCountryCode(match.Groups["country"].Value.Trim()) == null)
                            {
                                Console.WriteLine(match.Groups["country"].Value.Trim());
                            }
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else
                    if (inid.StartsWith(I30))
                    {
                        List<string> priorites = Regex.Split(inid.Replace(I30, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        biblio.Priorities = new List<Priority>();

                        foreach (string priority in priorites)
                        {
                            Match match = Regex.Match(priority, @"(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4}),?\s(?<code>[A-Z]{2}),?\s(?<number>.+)");
                            if (match.Success)
                            {
                                biblio.Priorities.Add(new Priority
                                {
                                    Date = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy/MM/dd").Replace(".", "/").Trim(),
                                    Country = match.Groups["code"].Value.Trim(),
                                    Number = match.Groups["number"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{priority}");
                        }
                    }
                    else
                    if (inid.StartsWith(I80))
                    {
                        Match match = Regex.Match(inid.Replace(I80, ""), @"[0-9]{2}.[0-9]{2}.[0-9]{4}");

                        if (match.Success)
                        {
                            legalStatusEvent.LegalEvent = new LegalEvent
                            {
                                Note = "|| (80) Dagsetning tilkynningar um veitingu EP einkaleyfis | " + DateTime.Parse(match.Value.Trim(), cultureInfo).ToString("yyyy/MM/dd").Replace(".", "/").Trim(),
                                Language = "IS",
                                Translations = new List<NoteTranslation>
                            {
                                new NoteTranslation
                                {
                                    Language = "EN",
                                    Tr = "|| (80) Date of notification of EP patent grant | " + DateTime.Parse(match.Value.Trim(), cultureInfo).ToString("yyyy/MM/dd").Replace(".", "/").Trim(),
                                    Type = "note"
                                }
                            }
                            };
                        }
                    }
                    else
                    if (inid.StartsWith(I86))
                    {
                        Match match = Regex.Match(inid.Replace(I86, ""), @"(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4}),?\s(?<number>.+)");
                        if (match.Success)
                        {
                            biblio.IntConvention = new IntConvention
                            {
                                PctApplNumber = match.Groups["number"].Value.Trim(),
                                PctApplDate = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy/MM/dd").Replace(".", "/").Trim()
                            };
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else Console.WriteLine($"{inid}");
                }

                legalStatusEvent.Biblio = biblio;

            }
            else if (subCode is "9")
            {
                Match match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"[A-Z]{2}\d+");

                if (match.Success)
                {
                    legalStatusEvent.Biblio.Publication.Number = match.Value.Trim();

                    Match match1 = Regex.Match(Path.GetFileName(CurrentFileName.Replace(".tetml", "")), @"\d{8}");

                    if (match1.Success)
                    {
                        legalStatusEvent.LegalEvent.Date = match1.Value.Insert(4, "/").Insert(7, "/").Trim();
                    }
                }
            }

            return legalStatusEvent;
        }

        internal List<string> SplitNote(string note) => Regex.Split(note.Replace("\r","").Replace("\n", " ").Trim(), @"(?=\([0-9]{2}\))").Where(val =>!string.IsNullOrEmpty(val)).ToList();

        internal string MakeCountryCode(string country)
        {
            string code = country switch
            {
                "Astraliu" => "AU",
                "Ástralíu" => "AU",
                "Reykjavik" => "IS",
                "Reykjavík" => "IS",
                "Hollandi" => "NL",
                "Frakklandi" => "FR",
                "Bandarikjunum" => "US",
                "Bandaríkjunum" => "US",
                "Noregi" => "NO",
                "Svíþjóð" => "SE",
                "Svi?jo?" => "SE",
                "Kina" => "CN",
                "Kína" => "CN",
                "Þýskalandi" => "DE",
                "?yskalandi" => "DE",
                "Luxemborg" => "LU",
                "Lúxemborg" => "LU",
                "Belgiu" => "BE",
                "Belgíu" => "BE",
                "Bahamaeyjum" => "BS",
                "Sviss" => "CH",
                "Portugal" => "PT",
                "Portúgal" => "PT",
                "Danmorku" => "DK",
                "Danmörku" => "DK",
                "Israel" => "IL",
                "Ísrael" => "IL",
                "Bretlandi" => "UK",
                "Italiu" => "IT",
                "Ítalíu" => "IT",
                "Kypur" => "CY",
                "Kýpur" => "CY",
                "Islandi" => "IS",
                "Íslandi" => "IS",
                "Spani" => "ES",
                "Spáni" => "ES",
                "-" => "",
                "Irlandi" => "IE",
                "Írlandi" => "IE",
                "Indlandi" => "IN",
                "Singapur" => "SG",
                "Singapúr" => "SG",
                "Kolumbiu" => "CO",
                "Kólumbíu" => "CO",
                "Japan" => "JP",
                "Finnlandi" => "FI",
                "Nyja Sjalandi" => "NZ",
                "Nýja Sjálandi" => "NZ",
                "Austurríki" => "AT",
                "Suður-Kóreu" => "KR",
                "Suður Kóreu" => "KR",
                "Víetnam" => "VN",
                "Kanada" => "CA",
                "Sameinudu arabísku furstadæmunum" => "AE",
                "Sameinuðu arabísku furstadæmunum" => "AE",
                "Tékklandi" => "CZ",
                "Ungverjalandi" => "HU",
                "Grikklandi" => "GR",
                "Tyrklandi" => "TR",

                _ => null
            };

            return code;
        }

        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool SendToProduction)
        {
            foreach (var rec in events)
            {
                string tmpValue = JsonConvert.SerializeObject(rec);
                string url;
                url = SendToProduction == true ? @"https://diamond.lighthouseip.online/external-api/import/legal-event" : @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                HttpClient httpClient = new();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                StringContent content = new(tmpValue.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = httpClient.PostAsync("", content).Result;
                string answer = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
