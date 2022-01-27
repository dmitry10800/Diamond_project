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
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_PA_Maksim
{
    class Methods
    {

        private string CurrentFileName;
        private int Id = 1;

        private readonly string I51 = "(51)";
        private readonly string I12 = "(12)";
        private readonly string I21 = "(21)";
        private readonly string I22 = "(22)";
        private readonly string I54 = "(54)";
        private readonly string I30 = "(30)";
        private readonly string I71 = "(71)";
        private readonly string I72 = "(72)";
        private readonly string I74 = "(74)";
        private readonly string I57 = "(57)";

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

                if (subCode == "1")
                {

                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                         .SkipWhile(val => !val.Value.StartsWith("MINISTERIO DE COMERCIO E INDUSTRIAS"))
                         .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements), @"(?=\(12\)\s)").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(12) PATENTE DE INVENCION")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "AZ"));
                    }
                }
                else
                if(subCode == "2")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                          .SkipWhile(val => !val.Value.StartsWith("INFORME SOBRE EL ESTADO DE LA TECNICA"))
                          .TakeWhile(val => !val.Value.StartsWith("PUBLIQUESE LAS SOLICITUDES DE PATENTES DE INVENCIÓN, MODELO INDUSTRIAL,"))
                          .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements), @"(?=\(21\)\s)").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(21)")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "EC"));
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
                CountryCode = "PA",
                SectionCode = sectionCode,
                Id = Id++,
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                Biblio = new(),
                LegalEvent = new()
            };

            CultureInfo culture = new("RU-ru");
            if(subCode == "1") 
            {
                foreach (string inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith(I21))
                    {
                        statusEvent.Biblio.Application.Number = inid.Replace("(21) Solicitud N?: ", "").Trim();
                    }
                    else if (inid.StartsWith(I22))
                    {
                        Match match = Regex.Match(inid.Replace("(22) Fecha de Solicitud : ", ""), @"(?<day>\d{2}).(?<month>.+).(?<year>\d{2})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Date = "20" + match.Groups["year"].Value.Trim() + "/" + MakeMonth(match.Groups["month"].Value.Trim()) + "/" + match.Groups["day"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} -- 22");
                    }
                    else if (inid.StartsWith(I71))
                    {

                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(71) Titular(es): ", "").Trim(), @"(?<name>.+?),\s(?<adress>.+),\s(?<code>.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                            {
                                Name = match.Groups["name"].Value.Trim(),
                                Address1 = match.Groups["adress"].Value.Trim(),
                                Country = MakeCountry(match.Groups["code"].Value.Trim())
                            });

                            if (MakeCountry(match.Groups["code"].Value.Trim()) == null)
                            {
                                Console.WriteLine($"{match.Groups["code"].Value.Trim()}");
                            }

                        }
                        else Console.WriteLine($"{inid} -- 71");
                    }
                    else if (inid.StartsWith(I72))
                    {
                        List<string> inventors = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(72) Inventor(es): ", "").Trim(), @"(?<=\),)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string inventor in inventors)
                        {
                            Match match = Regex.Match(inventor.Trim(), @"(?<name>.+)\(");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{inventor} -- 72");
                        }
                    }
                    else if (inid.StartsWith(I74))
                    {
                        statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                        {
                            Name = inid.Replace("(74) Apoderado: ", "").Trim()
                        });
                    }
                    else if (inid.StartsWith(I30))
                    {
                        List<string> priorities = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(30) Numero(s) prioridad: ", "").Trim(), @"(?<=[a-z||á],)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string priotity in priorities)
                        {
                            Match match = Regex.Match(priotity.TrimEnd(',').Trim(), @"(?<num>.+?)\s(?<country>.+)");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Priorities.Add(new Integration.Priority
                                {
                                    Number = match.Groups["num"].Value.Trim(),
                                    Country = MakeCountry(match.Groups["country"].Value.Trim())
                                });

                                if (MakeCountry(match.Groups["country"].Value.Trim()) == null)
                                {
                                    Console.WriteLine($"{match.Groups["country"].Value.Trim()}");
                                }

                            }
                            else Console.WriteLine($"{priotity} --30");
                        }
                    }
                    else if (inid.StartsWith(I54))
                    {
                        statusEvent.Biblio.Titles.Add(new Integration.Title
                        {
                            Language = "ES",
                            Text = inid.Replace("(54) Titulo: ", "").Trim()
                        });
                    }
                    else if (inid.StartsWith(I57))
                    {
                        statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                        {
                            Language = "ES",
                            Text = inid.Replace("(57) Resumen", "").Replace("\r", "").Replace("\n", " ").Trim()
                        });
                    }
                    else if (inid.StartsWith(I51))
                    {
                        List<string> ipcs = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(51) Clasificacion Internacional de Patentes ", "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string ipc in ipcs)
                        {
                            statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
                            {
                                Class = ipc.Trim()
                            });
                        }
                    }
                    else if (inid.StartsWith(I12))
                    {
                        statusEvent.Biblio.Publication.LanguageDesignation = inid.Replace("(12)", "").Replace("_", "").Replace("\r", "").Replace("\n", " ").Trim();
                    }
                }
            }
            else
            if(subCode == "2")
            {
                foreach (string inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith(I21))
                    {
                        statusEvent.Biblio.Application.Number = inid.Replace("(21) Solicitud N?: ", "").Trim();
                    }
                    else
                    if (inid.StartsWith(I22))
                    {
                        Match match = Regex.Match(inid.Replace("(22) Fecha de Solicitud: ", ""), @"(?<day>\d{2}).(?<month>.+).(?<year>\d{2})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Date = "20" + match.Groups["year"].Value.Trim() + "/" + MakeMonth(match.Groups["month"].Value.Trim()) + "/" + match.Groups["day"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} -- 22");
                    }
                    else
                    if (inid.StartsWith(I30))
                    {
                        List<string> priorities = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(30) Numero(s) prioridad: ", "").Trim(), @"(?<=[a-z||á],)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string priotity in priorities)
                        {
                            Match match = Regex.Match(priotity.TrimEnd(',').Trim(), @"(?<num>.+?)\s(?<country>.+)");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Priorities.Add(new Integration.Priority
                                {
                                    Number = match.Groups["num"].Value.Trim(),
                                    Country = MakeCountry(match.Groups["country"].Value.Trim())
                                });

                                if (MakeCountry(match.Groups["country"].Value.Trim()) == null)
                                {
                                    Console.WriteLine($"{match.Groups["country"].Value.Trim()}");
                                }

                            }
                            else Console.WriteLine($"{priotity} --30");
                        }
                    }
                    else
                    if (inid.StartsWith(I71))
                    {
                        List<string> applicants = Regex.Split(inid.Replace("(71) Titular(es): ", "").Trim(), @"(?<=[a-z]$)",RegexOptions.Multiline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string applicant in applicants)
                        {
                            Match match = Regex.Match(applicant, @"(?<name>.+?),\s(?<adress>.+),\s(?<code>.+)");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = MakeCountry(match.Groups["code"].Value.Trim())
                                });

                                if (MakeCountry(match.Groups["code"].Value.Trim()) == null)
                                {
                                    Console.WriteLine($"{match.Groups["code"].Value.Trim()}");
                                }

                            }
                            else Console.WriteLine($"{inid} -- 71");
                        }                      
                    }
                    else
                    if (inid.StartsWith(I74))
                    {
                        statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                        {
                            Name = inid.Replace("(74) Apoderado: ", "").Trim()
                        });
                    }
                    else
                    if (inid.StartsWith(I54))
                    {
                        statusEvent.Biblio.Titles.Add(new Integration.Title
                        {
                            Language = "ES",
                            Text = inid.Replace("(54) Titulo: ", "").Trim()
                        });
                    }
                    else
                    if (inid.StartsWith(I51))
                    {
                        List<string> ipcs = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(51) Clasificacion Internacional de Patentes ", "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string ipc in ipcs)
                        {
                            statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
                            {
                                Class = ipc.Trim()
                            });
                        }
                    }
                    else
                    if (inid.StartsWith("(note)"))
                    {
                 //       Match match = Regex.Match()
                    }

                }
            }

            return statusEvent;
        }
        internal List<string> MakeInids(string note, string subCode)
        {
            List<string> inids = new();

            if(subCode == "1")
            {
                inids = Regex.Split(note.Substring(0, note.IndexOf(@"(57)")).Trim(), @"(?=\(\d{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();
                inids.Add(note.Substring(note.IndexOf("(57)")).Trim());
            }
            else
            if(subCode == "2")
            {
                string field51 = note.Substring(note.IndexOf("(51)"));
                inids = Regex.Split(note.Substring(0, note.IndexOf("(51)")), @"(?=\(\d{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();
                Match match = Regex.Match(field51, @"(?<f51>.+)\n(?<note>Ba.+)",RegexOptions.Singleline);
                if (match.Success)
                {
                    inids.Add(match.Groups["f51"].Value.Trim());
                    inids.Add("(note) " + match.Groups["note"].Value.Trim());
                }
            }
            return inids;
        }
        internal string MakeText(List<XElement> xElements)
        {
            string text = null;

            foreach (XElement xElement in xElements)
            {
                text += xElement.Value + " ";
            }
            return text;
        }
        internal string MakeMonth(string month) => month switch
        {
            "ENE" => "01",
            "FEB" => "02",
            "MAR" => "03",
            "ABR" => "04",
            "MAY" => "05",
            "JUN" => "06",
            "JUL" => "07",
            "AGO" => "08",
            "SEP" => "09",
            "OCT" => "10",
            "NOV" => "11",
            "DIC" => "12",
            "de enero de" => "01",
            "de mayo de" => "05",
            _ => null
        };
        internal string MakeCountry(string country) => country switch
        {
            "Paises Bajos" => "NL",
            "Estados Unidos de América" => "US",
            "El Salvador" => "SV",
            "Bélgica" => "BE",
            "Hong Kong" => "HK",
            "Rusia" => "RU",
            "España" => "ES",
            "Reino Unido" => "UK",
            "Suecia" => "SE",
            "Suiza" => "CH",
            "India" => "IN",
            "Los Paises Bajos" => "NL",
            "Francia" => "FR",
            "Australia" => "AU",
            "Alemania" => "DE",
            "México" => "MX",
            "Canadá" => "CA",
            "REINO UNIDO" => "UK",
            "Oficina Europea de Patentes (OEP)" => "OEP",
            "Israel" => "IL",
            "Singapur" => "SG",
            "Irlanda" => "IE",
            "China" => "CN",
            "Los Países Bajos" => "NL",
            "Reino Unido de Gran Bretaña" => "GB",
            
            _ => null,
        };
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
