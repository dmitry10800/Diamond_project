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

namespace Diamond_EC_Maksim
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

            foreach (var file in directory.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            List<XElement> xElements = new();

            foreach (var tetml in files)
            {
                CurrentFileName = tetml;

                tet = XElement.Load(tetml);

                if (subCode == "3")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                          .SkipWhile(val => !val.Value.StartsWith("SOLICITUD A LA DIRECCIÓN DE PATENTES"))
                          .TakeWhile(val => !val.Value.StartsWith("OBTENCIONES VEGETALES"))
                          .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?=SOLICITUD A LA DIRECCIÓN DE PATENTES\s)", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "AF"));
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
                CountryCode = "EC",
                Id = Id++,
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                Biblio = new(),
                LegalEvent = new()
            };

            CultureInfo cultureInfo = new("ru-RU");

            if(subCode == "3")
            {
                foreach (var inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith("(12)"))
                    {
                        var match = Regex.Match(inid.Trim(), @"Patente\s(?<i12>.+)", RegexOptions.Singleline);
                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.LanguageDesignation = match.Groups["i12"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid}    ----    12");
                    }
                    else
                    if (inid.StartsWith("(21)"))
                    {
                        var match = Regex.Match(inid.Trim(), @"tud\s(?<aNum>.+)", RegexOptions.Singleline);

                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Number = match.Groups["aNum"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid}   ----    21");
                    }
                    else
                    if (inid.StartsWith("(22)"))
                    {

                        var match = Regex.Match(inid.Trim(), @"\d{2}\s?\/\s?\d{2}\s?\/\s?\d{4}", RegexOptions.Singleline);
                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Date = DateTime.Parse(match.Value.Replace(" ","").Trim(), cultureInfo).ToString(@"yyyy/MM/dd").Trim();
                        }
                        else Console.WriteLine($"{inid}   ----   22");
                    }
                    else
                    if (inid.StartsWith("(54)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"nte\s(?<title>.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "ES",
                                Text = match.Groups["title"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid}  ----   54");
                    }
                    else
                    if (inid.StartsWith("(51)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"entes\s(?<ipcs>.+)");

                        if (match.Success)
                        {
                            var ipcs = Regex.Split(match.Groups["ipcs"].Value.Trim(), @";|,|\)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (var ipc in ipcs)
                            {
                                var matchIpcs = Regex.Match(ipc.Trim(), @"(?<class>.+)\s\((?<date>.+)\).?");

                                if (matchIpcs.Success)
                                {
                                    statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
                                    {
                                        Date = matchIpcs.Groups["date"].Value.Trim(),
                                        Class = matchIpcs.Groups["class"].Value.Replace("(","").Trim()
                                    });
                                }
                                else
                                {
                                    var matchIpcs2 = Regex.Match(ipc.Trim(), @"(?<class>.+)\s?(?<date>\d{4}\.\d{2})");
                                    if (matchIpcs2.Success)
                                    {
                                        statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
                                        {
                                            Date = matchIpcs2.Groups["date"].Value.Trim(),
                                            Class = matchIpcs2.Groups["class"].Value.Replace("(", "").Trim()
                                        });
                                    }
                                    else {
                                        statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
                                        {
                                            Class = ipc
                                        });
                                    }
                                
                                } 
                            }
                        }
                    }
                    else
                    if (inid.StartsWith("(71)"))
                    {
                        var applicants = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<=\sFax\s?)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var applicant in applicants)
                        {
                            var match = Regex.Match(applicant.Trim(), @"Nombre(?<name>.+)\sDirecci.n(?<adress>.+)\sPa.s de Nacionalidad(?<country>.+)\sC.udad");
                            if (match.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = MakeCountryCode(match.Groups["country"].Value.Trim())
                                });
                                if (MakeCountryCode(match.Groups["country"].Value.Trim()) == null)
                                {
                                    Console.WriteLine($"{match.Groups["country"].Value.Trim()}");
                                }
                            }
                            else Console.WriteLine($"{applicant}  -----  71");
                        }
                    }
                    else
                    if (inid.StartsWith("(72)"))
                    {
                        var inventors = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<=\sFax\s?)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var inventor in inventors)
                        {
                            var match = Regex.Match(inventor.Trim(), @"Nombre(?<name>.+)\sDirecci.n(?<adress>.+)\sPa.s de Nacionalidad(?<country>.+)\sC.udad");
                            if (match.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = MakeCountryCode(match.Groups["country"].Value.Trim())
                                });
                                if (MakeCountryCode(match.Groups["country"].Value.Trim()) == null)
                                {
                                    Console.WriteLine($"{match.Groups["country"].Value.Trim()}");
                                }
                            }
                            else Console.WriteLine($"{inventor}  -----  72");
                        }
                    }
                    else
                    if (inid.StartsWith("(30)"))
                    {
                        var priorities = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<=\s?\d{2}\s?\/\s?\d{2}\s?\/\s?\d{4}\s?)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var priority in priorities)
                        {
                            var match = Regex.Match(priority.Trim(), @"(?<country>.+)\s(?<num>\d.+)\s(?<date>\d{2}\s?\/\s?\d{2}\s?\/\s?\d{4}\s?)");
                            if (match.Success)
                            {
                                statusEvent.Biblio.Priorities.Add(new Integration.Priority
                                {
                                    Country = MakeCountryCode(match.Groups["country"].Value.Trim()),
                                    Number = match.Groups["num"].Value.Trim(),
                                    Date = DateTime.Parse(match.Groups["date"].Value.Replace(" ", "").Trim(), cultureInfo).ToString(@"yyyy/MM/dd").Trim()
                                });
                                if (MakeCountryCode(match.Groups["country"].Value.Trim()) == null)
                                {
                                    Console.WriteLine($"{match.Groups["country"].Value.Trim()}");
                                }
                            }
                            else Console.WriteLine($"{priority}  ---- 30");
                        }
                    }
                    else
                    if (inid.StartsWith("(74)"))
                    {
                        var agents = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<=\sFax\s?)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var agent in agents)
                        {
                            var match = Regex.Match(agent.Trim(), @"Nombre(?<name>.+)\sDirecci.n(?<adress>.+)\sEmail");
                            if (match.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = "EC"
                                });

                            }
                            else Console.WriteLine($"{agent}  -----  74");
                        }
                    }
                    else
                    if (inid.StartsWith("(57)"))
                    {
                        statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                        {
                            Language = "ES",
                            Text = inid.Replace("\r", "").Replace("\n", " ").Replace("Resumen", "").Replace("(57)","").Trim()
                        });
                    }
                    else
                    if (inid.StartsWith("(Claim)"))
                    {
                        statusEvent.Biblio.Claims.Add(new DiamondProjectClasses.Claim
                        {
                            Language = "ES",
                            Text = inid.Replace("\r", "").Replace("\n", " ").Replace("REIVINDICACIONES", "").Replace("Reivindicaciones", "").Replace("(Claim)","").Trim()
                        });
                    }
                    else Console.WriteLine($"{inid}  ----  don't process");
                }
            }
           
            return statusEvent;
        }

        internal string MakeCountryCode(string country) => country switch
        {
            "Argentina" => "AR",
            "Perú" => "PE",
            "Belgica" => "BE",
            "Bélgica" => "BE",
            "Francia" => "FR",
            "Estados Unidos" => "US",
            "Australia" => "AU",
            "Nueva Zelanda" => "NZ",
            "Polonia" => "PL",
            "India" => "IN",
            "Suiza" => "CH",
            "Alemania" => "DE",
            "Singapur" => "SG",
            "Japón" => "JP",
            "Reino unido" => "UK",
            "Suecia" => "SE",
            "Italia" => "IT",
            "China" => "CN",
            "Vietnam" => "VN",
            "España" => "ES",
            "Ecuador" => "EC",
            "Colombia" => "CO",
            "Uruguay" => "UY",
            _ => null,
        };
        internal List<string> MakeInids (string note, string subCode)
        {
            List<string> inids = new();

            if(subCode == "3")
            {
                var match = Regex.Match(note.Trim(),
                    @"(?<i12>Tipo de Patente.+)\s(?<i21>No\. de\s.+)\s(?<i22>Fecha y hora de.+)\s(?<i54>T.tulo de la.+)\s(?<i51>Clasificaci.n.+)\s(?<i71>Solicitantes.+)\s(?<i72>Inventores.+)\s(?<i30>Declaraciones.+)\s(?<i74>Representante Legal.+)\s(?<i57>Resumen.+)\s(?<claim>(REIVINDICACIONES|Reivindicaciones).+)",
                    RegexOptions.Singleline);

                if (match.Success)
                {
                    inids.Add("(12) " + match.Groups["i12"].Value.Trim());
                    inids.Add("(21) " + match.Groups["i21"].Value.Trim());
                    inids.Add("(22) " + match.Groups["i22"].Value.Trim());
                    inids.Add("(54) " + match.Groups["i54"].Value.Trim());
                    inids.Add("(51) " + match.Groups["i51"].Value.Trim());
                    inids.Add("(71) " + match.Groups["i71"].Value.Trim());
                    inids.Add("(72) " + match.Groups["i72"].Value.Trim());
                    inids.Add("(30) " + match.Groups["i30"].Value.Trim());
                    inids.Add("(74) " + match.Groups["i74"].Value.Trim());
                    inids.Add("(57) " + match.Groups["i57"].Value.Trim());
                    inids.Add("(Claim) " + match.Groups["claim"].Value.Trim());
                }
                else 
                {
                    var match1 = Regex.Match(note.Trim(),
                        @"(?<i12>Tipo de Patente.+)\s(?<i21>No\. de\s.+)\s(?<i22>Fecha y hora de.+)\s(?<i54>T.tulo de la.+)\s(?<i51>Clasificaci.n.+)\s(?<i71>Solicitantes.+)\s(?<i72>Inventores.+)\s(?<i30>Declaraciones.+)\s(?<i57>Resumen.+)\s(?<claim>(REIVINDICACIONES|Reivindicaciones).+)",
                        RegexOptions.Singleline);

                    if (match1.Success)
                    {
                        inids.Add("(12) " + match1.Groups["i12"].Value.Trim());
                        inids.Add("(21) " + match1.Groups["i21"].Value.Trim());
                        inids.Add("(22) " + match1.Groups["i22"].Value.Trim());
                        inids.Add("(54) " + match1.Groups["i54"].Value.Trim());
                        inids.Add("(51) " + match1.Groups["i51"].Value.Trim());
                        inids.Add("(71) " + match1.Groups["i71"].Value.Trim());
                        inids.Add("(72) " + match1.Groups["i72"].Value.Trim());
                        inids.Add("(30) " + match1.Groups["i30"].Value.Trim());
                        inids.Add("(57) " + match1.Groups["i57"].Value.Trim());
                        inids.Add("(Claim) " + match1.Groups["claim"].Value.Trim());
                    }
                    else 
                    {
                        var match2 = Regex.Match(note.Trim(),
                            @"(?<i12>Tipo de Patente.+)\s(?<i21>No\. de\s.+)\s(?<i22>Fecha y hora de.+)\s(?<i54>T.tulo de la.+)\s(?<i51>Clasificaci.n.+)\s(?<i71>Solicitantes.+)\s(?<i72>Inventores.+)\s(?<i30>Declaraciones.+)\s(?<i57>Resumen.+)Observaciones.+",
                            RegexOptions.Singleline);

                        if (match2.Success)
                        {
                            inids.Add("(12) " + match2.Groups["i12"].Value.Trim());
                            inids.Add("(21) " + match2.Groups["i21"].Value.Trim());
                            inids.Add("(22) " + match2.Groups["i22"].Value.Trim());
                            inids.Add("(54) " + match2.Groups["i54"].Value.Trim());
                            inids.Add("(51) " + match2.Groups["i51"].Value.Trim());
                            inids.Add("(71) " + match2.Groups["i71"].Value.Trim());
                            inids.Add("(72) " + match2.Groups["i72"].Value.Trim());
                            inids.Add("(30) " + match2.Groups["i30"].Value.Trim());
                            inids.Add("(57) " + match2.Groups["i57"].Value.Trim());
                        }
                        else Console.WriteLine($"{note}");
                    }
                  
                }
              
            }

            return inids;
        }
        internal string MakeText (List<XElement> xElements, string subCode)
        {
            string text = null;

            if(subCode == "3")
            {
                foreach (var xElement in xElements)
                {
                    text += xElement.Value.Trim() + "\n";
                }
            }

            return text;
        }
        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool SendToProduction)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
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
                var result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
