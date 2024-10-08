﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Diamond.Core.Models;
using Integration;

namespace Diamond_PA_Maksim
{
    class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        private const string I51 = "(51)";
        private const string I12 = "(12)";
        private const string I21 = "(21)";
        private const string I22 = "(22)";
        private const string I54 = "(54)";
        private const string I30 = "(30)";
        private const string I71 = "(71)";
        private const string I72 = "(72)";
        private const string I74 = "(74)";
        private const string I57 = "(57)";

        internal List<LegalStatusEvent> Start(string path, string subCode)
        {
            var statusEvents = new List<LegalStatusEvent>();

            var directory = new DirectoryInfo(path);

            var files = new List<string>();

            foreach (var file in directory.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            var xElements = new List<XElement>();

            foreach (var tetml in files)
            {
                _currentFileName = tetml;

                var tet = XElement.Load(tetml);

                if (subCode == "1")
                {

                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                         .SkipWhile(val => !val.Value.StartsWith("MINISTERIO DE COMERCIO E INDUSTRIAS"))
                         .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=\(12\)\s)").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(12) PATENTE DE INVENCION")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "AZ"));
                    }
                }
                
                if(subCode == "2")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                          .SkipWhile(val => !val.Value.StartsWith("INFORME SOBRE EL ESTADO DE LA TECNICA"))
                          .TakeWhile(val => !val.Value.StartsWith("PUBLIQUESE LAS SOLICITUDES DE PATENTES DE INVENCIÓN, MODELO INDUSTRIAL,"))
                          .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=\(21\)\s)").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(21)")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "EC"));
                    }
                }
            }

            return statusEvents;
        }

        internal LegalStatusEvent MakePatent(string note, string subCode, string sectionCode)
        {
            var statusEvent = new LegalStatusEvent()
            {
                SubCode = subCode,
                CountryCode = "PA",
                SectionCode = sectionCode,
                Id = _id++,
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                Biblio = new Biblio(),
                LegalEvent = new LegalEvent()
            };

            var culture = new CultureInfo("RU-ru");
            if(subCode == "1") 
            {
                foreach (var inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith(I21))
                    {
                        statusEvent.Biblio.Application.Number = inid.Replace("(21) Solicitud N?: ", "").Trim();
                    }
                    else if (inid.StartsWith(I22))
                    {
                        var match = Regex.Match(inid.Replace("(22) Fecha de Solicitud : ", ""), @"(?<day>\d{2}).(?<month>.+).(?<year>\d{2})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Date = "20" + match.Groups["year"].Value.Trim() + "/" + MakeMonth(match.Groups["month"].Value.Trim()) + "/" + match.Groups["day"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} -- 22");
                    }
                    else if (inid.StartsWith(I71))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(71) Titular(es): ", "").Trim(), @"(?<name>.+?),\s(?<adress>.+),\s(?<code>.+)");

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
                        var inventors = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(72) Inventor(es): ", "").Trim(), @"(?<=\),)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var inventor in inventors)
                        {
                            var match = Regex.Match(inventor.Trim(), @"(?<name>.+)\(");

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
                        var priorities = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(30) Numero(s) prioridad: ", "").Trim(), @"(?<=[a-z||á],)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var priotity in priorities)
                        {
                            var match = Regex.Match(priotity.TrimEnd(',').Trim(), @"(?<num>.+?)\s(?<country>.+)");

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
                        var match = Regex.Match(inid.Trim(), @"(?<field54>.+)\sMINISTERIO", RegexOptions.Singleline);

                        if (match.Success)
                        {
                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "ES",
                                Text = match.Groups["field54"].Value.Replace("(54) Titulo: ", "").Trim()
                            });
                        }
                        else
                        {
                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "ES",
                                Text = inid.Replace("(54) Titulo: ", "").Trim()
                            });
                        }
                    }
                    else if (inid.StartsWith(I57))
                    {
                        var match = Regex.Match(inid.Trim(), @"(?<field57>.+)\sMINISTERIO", RegexOptions.Singleline);

                        if (match.Success)
                        {
                            statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                            {
                                Language = "ES",
                                Text = match.Groups["field57"].Value.Replace("(57) Resumen", "").Replace("\r", "").Replace("\n", " ").Trim()
                            });
                        }
                        else
                        {
                            statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                            {
                                Language = "ES",
                                Text = inid.Replace("(57) Resumen", "").Replace("\r", "").Replace("\n", " ").Trim()
                            });
                        }
                    }
                    else if (inid.StartsWith(I51))
                    {
                        var ipcs = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(51) Clasificacion Internacional de Patentes ", "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var ipc in ipcs)
                        {
                            var ipcsMatch = Regex.Match(ipc.Trim(), @"(?<first>.+)\s(?<second>.+)");
                            if (ipcsMatch.Success)
                            {
                                statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
                                {
                                    Class = ipcsMatch.Groups["first"].Value.Replace("/","").Trim() + " " + ipcsMatch.Groups["second"].Value.Trim()
                                });
                            }
                        }
                    }
                    else if (inid.StartsWith(I12))
                    {
                        statusEvent.Biblio.Publication.LanguageDesignation = inid.Replace("(12)", "").Replace("_", "").Replace("\r", "").Replace("\n", " ").Trim();
                    }
                }
            }
            
            if(subCode == "2")
            {
                foreach (var inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith(I21))
                    {
                        statusEvent.Biblio.Application.Number = inid.Replace("(21) Solicitud N?: ", "").Trim();
                    }
                    
                    if (inid.StartsWith(I22))
                    {
                        var match = Regex.Match(inid.Replace("(22) Fecha de Solicitud: ", ""), @"(?<day>\d+).(?<month>.+).(?<year>\d{4})");

                        if (match.Success)
                        {
                            if (match.Groups["day"].Value.Trim().Length == 1)
                            {
                                statusEvent.Biblio.Application.Date = match.Groups["year"].Value.Trim() + "/" + MakeMonth(match.Groups["month"].Value.Trim()) + "/" + "0" + match.Groups["day"].Value.Trim();
                            }
                            else
                            {
                                statusEvent.Biblio.Application.Date = match.Groups["year"].Value.Trim() + "/" + MakeMonth(match.Groups["month"].Value.Trim()) + "/" + match.Groups["day"].Value.Trim();
                            }
                        }
                        else Console.WriteLine($"{inid} -- 22");
                    }
                    
                    if (inid.StartsWith(I30))
                    {
                        var priorities = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(30) Numero(s) prioridad: ", "").Trim(), @"(?<=[a-z||á],)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var priotity in priorities)
                        {
                            var match = Regex.Match(priotity.TrimEnd(',').Trim(), @"(?<num>.+?)\s(?<country>.+)");

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
                    
                    if (inid.StartsWith(I71))
                    {
                        var applicants = Regex.Split(inid.Replace("(71) Titular(es): ", "").Trim(), @"(?<=[a-z]$)",RegexOptions.Multiline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var applicant in applicants)
                        {
                            var match = Regex.Match(applicant, @"(?<name>.+?),\s(?<adress>.+),\s(?<code>.+)");

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
                    
                    if (inid.StartsWith(I74))
                    {
                        statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                        {
                            Name = inid.Replace("(74) Apoderado: ", "").Trim()
                        });
                    }
                    
                    if (inid.StartsWith(I54))
                    {
                        statusEvent.Biblio.Titles.Add(new Integration.Title
                        {
                            Language = "ES",
                            Text = inid.Replace("(54) Titulo: ", "").Trim()
                        });
                    }
                    
                    if (inid.StartsWith(I51))
                    {
                        var ipcs = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(51) Clasificacion Internacional de Patentes ", "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var ipc in ipcs)
                        {
                            var ipcsMatch = Regex.Match(ipc.Trim(), @"(?<first>.+)\s(?<second>.+)");
                            if (ipcsMatch.Success)
                            {
                                statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
                                {
                                    Class = ipcsMatch.Groups["first"].Value.Replace("/", "").Trim() + " " + ipcsMatch.Groups["second"].Value.Trim()
                                });
                            }
                        }
                    }
                    if (inid.StartsWith("(note)"))
                    {
                        var matchPctPubNum = Regex.Match(inid, @"EUROPEA DE PATENTES CON EL N°\s(?<num>.+)\.");
                        if (matchPctPubNum.Success)
                        {
                            statusEvent.Biblio.IntConvention.PctPublNumber = matchPctPubNum.Groups["num"].Value.Trim();
                        }

                        var matchPctPublDate =
                            Regex.Match(inid, @"Fecha de realizacion del informe:\s(?<date>\d+.+\d{4})");

                        if (matchPctPublDate.Success)
                        {
                            var dateMatch = Regex.Match(matchPctPublDate.Groups["date"].Value.Trim(),
                                @"(?<day>\d+)\s(?<month>.+)\s(?<year>\d{4})");


                            statusEvent.Biblio.IntConvention.PctSearchDate = dateMatch.Groups["year"].Value.Trim() + "/"
                                + MakeMonth(dateMatch.Groups["month"].Value.Trim()) + "/"
                                + dateMatch.Groups["day"].Value.Trim();
                        }

                        var pubNumMatch = Regex.Match(inid, @"^PATENTES CON EL N°\s(?<num>.+)\.");
                        if (pubNumMatch.Success)
                        {
                            statusEvent.Biblio.Publication.Number = pubNumMatch.Groups["num"].Value.Trim();
                        }
                    }
                }
            }

            return statusEvent;
        }
        internal List<string> MakeInids(string note, string subCode)
        {
            var inids = new List<string>();

            if(subCode == "1")
            {
                inids = Regex.Split(note.Substring(0, note.IndexOf(@"(57)")).Trim(), @"(?=\(\d{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();
                inids.Add(note.Substring(note.IndexOf("(57)")).Trim());
            }
            
            if(subCode == "2")
            {
                var field51 = note.Substring(note.IndexOf("(51)"));
                inids = Regex.Split(note.Substring(0, note.IndexOf("(51)")), @"(?=\(\d{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();
                var match = Regex.Match(field51, @"(?<f51>.+)\n(?<note>Ba.+)",RegexOptions.Singleline);
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
            var text = string.Empty;

            foreach (var xElement in xElements)
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
            "de septiembre de" => "09",
            "de febrero de" => "02",
            "de marzo de" => "03",
            "de junio de" => "06",
            "de julio de" => "07",
            "de agosto de" => "08",
            "de noviembre de" => "11",
            "de abril de" => "04",
            "de octubre de" => "10",
            "de diciembre de" => "12",
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
            "UNIÓN EUROPEA" => "EU",
            "República de Corea" => "KR",
            "Corea" => "KR",
            "La Federación Rusa" => "RU",
            "La Republica de Panama" => "PA",
            "Colombia" => "CO",
            "Corea del Sur" => "KR",
            "Italia" => "IT",
            "IRLANDA" => "IE",
            "Estados Unidos de America" => "US",
            "La República de Panamá" => "PA",
            _ => null,
        };
    }
}
