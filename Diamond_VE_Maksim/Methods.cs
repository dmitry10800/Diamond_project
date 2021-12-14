﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_VE_Maksim
{
    class Methods
    {
        private string CurrentFileName;
        private int Id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> statusEvents = new();

            DirectoryInfo directory = new(path);

            List<string> files = new();

            foreach (FileInfo file in directory.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            List<XElement> xElements = new();

            foreach (string tetml in files)
            {
                CurrentFileName = tetml;

                tet = XElement.Load(tetml);

                if (subCode == "24")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                          .SkipWhile(val => !val.Value.StartsWith("SOLICITUDES EXTRANJERAS DE PATENTE DE INVENCIÓN" + "\n" + "NEGADAS DE OFICIO"))
                          .TakeWhile(val => !val.Value.StartsWith("Total de Solicitudes"))
                          .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=\d{4}\-\d+)").Where(val => !string.IsNullOrEmpty(val) && new Regex(@"^\d+.*").Match(val).Success).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FC"));
                    }
                }
            }
            return statusEvents;
        }

        internal Diamond.Core.Models.LegalStatusEvent MakePatent (string note, string subCode , string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                Id = Id++,
                CountryCode = "VE",
                SectionCode = sectionCode,
                SubCode = subCode,
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                Biblio = new(),
                LegalEvent = new()
            };

            if(subCode == "24")
            {
                Match match = Regex.Match(note.Trim(), @"(?<aNum>\d{4}\-\d+)\s(?<title>.+)Tramitante:(?<f74>.+?)\n(?<f71>.+)COMENTARIO:(?<note>.+)",RegexOptions.Singleline);
                if (match.Success)
                {
                    statusEvent.Biblio.Application.Number = match.Groups["aNum"].Value.Trim();

                    statusEvent.Biblio.Titles.Add(new Integration.Title
                    {
                        Text = match.Groups["title"].Value.Trim(),
                        Language = "ES"
                    });

                    statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                    {
                        Name = match.Groups["f74"].Value.Trim()
                    });

                    List<string> applicants = Regex.Split(match.Groups["f71"].Value.Replace("\r","").Replace("\n"," ").Trim(), @"(?<=s:.+,\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (string applicant in applicants)
                    {
                        Match match1 = Regex.Match(applicant.Trim().TrimEnd(','), @"(?<name>.+)Dom.+:(?<country>.+)", RegexOptions.Singleline);

                        if (match1.Success)
                        {
                            statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                            {
                                Name = match1.Groups["name"].Value.Trim(),
                                Country = MakeCountryCode(match1.Groups["country"].Value.Trim())
                            });
                        }
                        else Console.WriteLine($"{applicant} --- 71 /0  -----  { match.Groups["aNum"].Value.Trim()}");

                        if (MakeCountryCode(match1.Groups["country"].Value.Trim()) == null) Console.WriteLine($"{match1.Groups["country"].Value.Trim()}");
                    }

                    statusEvent.LegalEvent.Note = match.Groups["note"].Value.Trim();
                    statusEvent.LegalEvent.Language = "ES";
                    //Integration.NoteTranslation noteTranslation = new()
                    //{
                    //    Language = "EN",
                    //    Type = "note",
                    //    Tr = "THIS APPLICATION CONTRAVES ARTICLES 8 (MORE THAN AN INVENT), 14.1 (USE OF UNPAIRED TERMS) AND 59.2A (MEMORY WITH LACK OF CLARITY), AND IS UNCURRENT IN THE PROHIBITIONS PROVIDED IN ARTICLE 15.7 (CONTRAVES THE METROLOGY LAW)."
                    //};

                    //statusEvent.LegalEvent.Translations.Add(noteTranslation);
                }
                else 
                {
                    Match match1 = Regex.Match(note.Trim(), @"(?<aNum>\d{4}\-\d+)\sTramitante:(?<f74>.+?)\n(?<title>.+)\.\s(?<f71>.+)\sCOMENTARIO:(?<note>.+)", RegexOptions.Singleline);

                    if (match1.Success)
                    {
                        statusEvent.Biblio.Application.Number = match1.Groups["aNum"].Value.Trim();

                        statusEvent.Biblio.Titles.Add(new Integration.Title
                        {
                            Text = match1.Groups["title"].Value.Trim(),
                            Language = "ES"
                        });

                        statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                        {
                            Name = match1.Groups["f74"].Value.Trim()
                        });

                        List<string> applicants = Regex.Split(match1.Groups["f71"].Value.Trim(), @"(?<=s:.+,\s)", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string applicant in applicants)
                        {
                            Match match2 = Regex.Match(applicant.Trim().TrimEnd(','), @"(?<name>.+)Dom.+:(?<country>.+)", RegexOptions.Singleline);

                            if (match2.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Name = match2.Groups["name"].Value.Trim(),
                                    Country = MakeCountryCode(match2.Groups["country"].Value.Trim())
                                });
                            }
                            else Console.WriteLine($"{applicant} --- 71 /1 -----  { match1.Groups["aNum"].Value.Trim()}");

                            if (MakeCountryCode(match2.Groups["country"].Value.Trim()) == null) Console.WriteLine($"{match2.Groups["country"].Value.Trim()}");
                        }

                        statusEvent.LegalEvent.Note = match1.Groups["note"].Value.Trim();
                        statusEvent.LegalEvent.Language = "ES";
                    }
                    else
                    {
                        Match match2 = Regex.Match(note.Trim(), @"(?<aNum>\d{4}\-\d+)\s(?<title>.+)\.\s(?<f71>.+)\sTramitante:(?<f74>.+?)\nCOMENTARIO:(?<note>.+)", RegexOptions.Singleline);

                        if (match2.Success)
                        {
                            statusEvent.Biblio.Application.Number = match2.Groups["aNum"].Value.Trim();

                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Text = match2.Groups["title"].Value.Trim(),
                                Language = "ES"
                            });

                            statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                            {
                                Name = match2.Groups["f74"].Value.Trim()
                            });

                            List<string> applicants = Regex.Split(match2.Groups["f71"].Value.Trim(), @"(?<=s:.+,\s)", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (string applicant in applicants)
                            {
                                Match match3 = Regex.Match(applicant.Trim().TrimEnd(','), @"(?<name>.+)Dom.+:(?<country>.+)", RegexOptions.Singleline);

                                if (match3.Success)
                                {
                                    statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                    {
                                        Name = match3.Groups["name"].Value.Trim(),
                                        Country = MakeCountryCode(match3.Groups["country"].Value.Trim())
                                    });
                                }
                                else Console.WriteLine($"{applicant} --- 71 /2-----  { match2.Groups["aNum"].Value.Trim()}");

                                if (MakeCountryCode(match3.Groups["country"].Value.Trim()) == null) Console.WriteLine($"{match3.Groups["country"].Value.Trim()}");
                            }

                            statusEvent.LegalEvent.Note = match2.Groups["note"].Value.Trim();
                            statusEvent.LegalEvent.Language = "ES";
                        }
                        else
                        {
                            Match match3 = Regex.Match(note.Trim(), @"(?<aNum>\d{4}\-\d+)\sTramitante:(?<f74>.+?)\n(?<title>.+?\n.+?)\.?\s(?<f71>.+)\sCOMENTARIO:(?<note>.+)", RegexOptions.Singleline);

                            if (match3.Success)
                            {
                                statusEvent.Biblio.Application.Number = match3.Groups["aNum"].Value.Trim();

                                statusEvent.Biblio.Titles.Add(new Integration.Title
                                {
                                    Text = match3.Groups["title"].Value.Trim(),
                                    Language = "ES"
                                });

                                statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                                {
                                    Name = match3.Groups["f74"].Value.Trim()
                                });

                                List<string> applicants = Regex.Split(match3.Groups["f71"].Value.Trim(), @"(?<=s:.+,\s)", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                                foreach (string applicant in applicants)
                                {
                                    Match match4 = Regex.Match(applicant.Trim().TrimEnd(','), @"(?<name>.+)Dom.+:(?<country>.+)", RegexOptions.Singleline);

                                    if (match4.Success)
                                    {
                                        statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                        {
                                            Name = match4.Groups["name"].Value.Trim(),
                                            Country = MakeCountryCode(match4.Groups["country"].Value.Trim())
                                        });
                                    }
                                    else Console.WriteLine($"{applicant} --- 71/3 -----  { match3.Groups["aNum"].Value.Trim()}");

                                    if (MakeCountryCode(match4.Groups["country"].Value.Trim()) == null) Console.WriteLine($"{match4.Groups["country"].Value.Trim()}");
                                }

                                statusEvent.LegalEvent.Note = match3.Groups["note"].Value.Trim();
                                statusEvent.LegalEvent.Language = "ES";
                            }
                            else Console.WriteLine($"{ note}");
                        } 
                    }
                    
                }
               
            }
            return statusEvent;
        }
        internal string MakeCountryCode(string country) => country switch
        {
            "PAISES BAJOS ( HOLANDA )" => "NL",
            "ESTADOS UNIDOS DE AMÉRICA" => "US",
            "ESPAÑA" => "ES",
            "JAPON" => "JP",
            "FRANCIA" => "FR",
            "REINO UNIDO" => "UK",
            "GRECIA" => "GR",
            "ALEMANIA" => "DE",
            "SUIZA" => "CH",
            "ITALIA" => "IT",
            _ => null
        };
        internal string MakeText(List<XElement> xElements, string subCOde)
        {
            string text = null;

            if(subCOde == "24")
            {
                foreach (XElement xElement in xElements)
                {
                    text += xElement.Value + "\n";
                }
            }

            return text;
        }
        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool SendToProduction)
        {
            foreach (var rec in events)
            {
                string tmpValue = JsonConvert.SerializeObject(rec);
                string url;
                if (SendToProduction == true)
                {
                    url = @"https://diamond.lighthouseip.online/external-api/import/legal-event";  // продакшен
                }
                else
                {
                    url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";     // стейдж
                }
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
