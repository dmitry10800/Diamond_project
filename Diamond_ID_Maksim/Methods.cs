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
using Integration;

namespace Diamond_ID_Maksim
{

    class Methods
    {
        private string CurrentFileName;
        private int Id = 1;

        private const string I11 = "(11)";
        private const string I13 = "(13)";
        private const string I51 = "(51)";
        private const string I21 = "(21)";
        private const string I22 = "(22)";
        private const string I30 = "(30)";
        private const string I43 = "(43)";
        private const string I71 = "(71)";
        private const string I72 = "(72)";
        private const string I74 = "(74)";
        private const string I54 = "(54)";
        private const string I57 = "(57)";

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

                if (subCode == "1")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                             .SkipWhile(val => !val.Value.Contains("(74) : Nama dan Alamat Konsultan Paten"))
                             .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=\(20\)\s\D)", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith(@"(20)"+'\n'+"R")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "BZ"));
                    }
                }
                else if (subCode == "2")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                             .SkipWhile(val => !val.Value.StartsWith ("(20) RI Permohonan Paten"))
                             .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=\(20\)\s\D)").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith(@"(20) R")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "BZ"));
                    }
                }
                else if (subCode is "3")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.Contains("(74) : Nama dan Alamat Konsultan Paten"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=\(20\)\nR.+)", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith(@"(20)")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "AB"));
                    }
                }
            }
            return statusEvents;
        }

        internal string MakeText (List <XElement> xElements)
        {
            string text = null;

            foreach (var xElement in xElements)
            {
                text += xElement.Value + "\n";
            }

            return text;
        }
        internal Diamond.Core.Models.LegalStatusEvent MakePatent (string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                CountryCode = "ID",
                SectionCode = sectionCode,
                SubCode = subCode,
                Id = Id++,
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                Biblio = new(),
                LegalEvent = new()
            };

            var culture = new CultureInfo("ru-Ru");

            if(subCode == "1")
            {
                foreach (var inid in MakeInids(note, subCode))
                {
                    var cleanNote = CleanInid(inid, subCode).Trim();

                    if (inid.StartsWith(I11))
                    {
                        statusEvent.Biblio.Publication.Number = cleanNote;
                    }
                    else if (inid.StartsWith(I13))
                    {
                        statusEvent.Biblio.Publication.Kind = inid.Replace("\r", "").Replace("\n", " ").Replace("(13)", "").Trim();
                    }
                    else if (inid.StartsWith(I21))
                    {
                        statusEvent.Biblio.Application.Number = cleanNote;
                    }
                    else if (inid.StartsWith(I22))
                    {
                        var date = MakeRightDate(cleanNote, subCode);

                        if (date != null)
                            statusEvent.Biblio.Application.Date = date;
                        else
                            Console.WriteLine($"{inid} --- 22");
                    }
                    else if (inid.StartsWith(I43))
                    {
                        var date = MakeRightDate(cleanNote, subCode);
                        if (date != null)
                            statusEvent.Biblio.Publication.Date = date;
                        else
                            Console.WriteLine($"{inid} --- 43");
                    }
                    else if (inid.StartsWith(I71))
                    {
                        var name = string.Empty;
                        var adress = string.Empty;
                        var lines = Regex.Split(cleanNote.Trim(), @"\n").Where(_ => !string.IsNullOrEmpty(_)).ToList();
                        if (lines.Count % 2 == 0)
                        {
                            for (int i = 0; i < lines.Count/2; i++)
                            {
                                name += lines[i].Trim() + " ";
                            }

                            for (int i = lines.Count / 2; i < lines.Count; i++)
                            {
                                adress += lines[i].Trim() + " ";
                            }

                            var adressMatch = Regex.Match(adress.Trim(), @"(?<adress>.+)\s(?<country>.+)");
                            if (adressMatch.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new PartyMember()
                                {
                                    Name = name.Trim(),
                                    Address1 = adressMatch.Groups["adress"].Value.Trim(),
                                    Country = MakeCountry(adressMatch.Groups["country"].Value.Trim())
                                });
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 1; i++)
                            {
                                name = lines[i].Trim();
                            }

                            for (int i = 1; i < lines.Count; i++)
                            {
                                adress += lines[i].Trim() + " ";
                            }

                            var adressMatch = Regex.Match(adress.Trim(), @"(?<adress>.+)\s(?<country>.+)");
                            if (adressMatch.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new PartyMember()
                                {
                                    Name = name.Trim(),
                                    Address1 = adressMatch.Groups["adress"].Value.Trim(),
                                    Country = MakeCountry(adressMatch.Groups["country"].Value.Trim())
                                });
                            }
                        }
                    }
                }
                /*
                 *
(51) I.P.C : F 26B 21/10,F 26B 21/08

(30)
Data Prioritas :
(31) Nomor
(32) Tanggal
(33) Negara
2014072.9
08 September
2020
GB


(72) Nama Inventor :
DUCHEYNE, Wouter,BE
(74) Nama dan Alamat Konsultan Paten :
Maulitta Pramulasari S.Pd
Mirandah Asia Indonesia Sudirman Plaza, Plaza Marein
Lantai 10E Jalan Jenderal Sudirman Kavling 76-78
(54)
Judul
Invensi :
UNIT UNTUK PENGERINGAN SORPSI LANGSUNG DAN METODENYA
(57)
Abstrak :
Invensi ini berhubungan dengan bidang unit pengering industri. Invensi ini menyediakan suatu unit untuk pengeringan dan metode
untuk pengeringan sorpsi langsung dari zat seperti, tetapi tidak terbatas pada, pelet, bahan konstruksi, kayu, gas, pulp dan kertas,
pengeringan bahan kimia dari reaksi sintesis atau asam anorganik dan organik produksi, makanan.
                 */






                foreach (var inid in MakeInids(note))
                {
                    if (inid.StartsWith("(11)"))
                    {
                        statusEvent.Biblio.Publication.Number = inid.Replace("\r", "").Replace("\n", " ").Replace("(11) No Pengumuman :", "").Trim();
                    }
                    else
                    if (inid.StartsWith("(13)"))
                    {
                        statusEvent.Biblio.Publication.Kind = inid.Replace("\r", "").Replace("\n", " ").Replace("(13)", "").Trim();
                    }
                    else
                    if (inid.StartsWith("(21)"))
                    {
                        statusEvent.Biblio.Application.Number = inid.Replace("\r", "").Replace("\n", " ").Replace("(21) No. Permohonan Paten :", "").Trim();
                    }
                    else
                    if (inid.StartsWith("(22)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(22) Tanggal Penerimaan Permohonan Paten :", "").Trim(), @"(?<date>\d{2}).(?<month>\D{3}).(?<year>\d{2})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Date = "20" + match.Groups["year"].Value.Trim() + "/" + MakeMonth(match.Groups["month"].Value.Trim()) + "/" + match.Groups["date"].Value.Trim();
                        }
                        else
                        {
                            if (MakeMonth(match.Groups["month"].Value.Trim()) == null) Console.WriteLine(match.Groups["month"].Value.Trim());

                            statusEvent.Biblio.Application.Date = DateTime.Parse(inid.Replace("\r", "").Replace("\n", " ").Replace("(22) Tanggal Penerimaan Permohonan Paten :", "").Replace("Data Prioritas :","").Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                    }
                    else
                    if (inid.StartsWith("(51)"))
                    {
                        var ipcs = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(51) I.P.C :", "").Trim(), @"(?<=\.\d{2}\)?)").Where(val => !string.IsNullOrEmpty(val)).Where(val => !val.StartsWith(")")).ToList();

                        foreach (var ipc in ipcs)
                        {
                            var match = Regex.Match(ipc.Trim(), @"(?<class>.+)\s\(?(?<edi>\d{4}.\d{2})\)?");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
                                {
                                    Class = match.Groups["class"].Value.Trim(),
                                    Date = match.Groups["edi"].Value.Trim()
                                });
                            }
                            else
                            {
                                statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
                                {
                                    Class = match.Groups["class"].Value.Trim()
                                });
                            }
                        }
                    }
                    else
                    if (inid.StartsWith("(33)")|| inid.StartsWith("(30)"))
                    {
                        var priorities = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(33) Negara", "").Replace("(30)", "").Trim(), @"(?=\d{2}\/?\d+)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var priotity in priorities)
                        {
                            var match = Regex.Match(priotity.Trim(), @"(?<num>.+)\s(?<day>\d{2}).(?<month>\D+).(?<year>\d{2})\s(?<code>.+)");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Priorities.Add(new Integration.Priority
                                {
                                    Number = match.Groups["num"].Value.Trim(),
                                    Date = "20" + match.Groups["year"].Value.Trim() + "/" + MakeMonth(match.Groups["month"].Value.Trim()) + "/" + match.Groups["day"].Value.Trim(),
                                    Country = MakeCountry(match.Groups["code"].Value.Trim())
                                });
                            }
                            else
                            {
                                if (MakeMonth(match.Groups["month"].Value.Trim()) == null) Console.WriteLine("========================================================" + match.Groups["month"].Value.Trim());
                                if (MakeCountry(match.Groups["code"].Value.Trim()) == null) Console.WriteLine("========================================================" + match.Groups["code"].Value.Trim());

                                Console.WriteLine($"{priotity}");
                            }
                        }
                    }
                    else
                    if (inid.StartsWith("(74)"))
                    {
                        var match = Regex.Match(inid, @"(?<i71>Nama dan Alamat yang.+)\n(?<i72>Nama Inventor.+)\n(?<i74>Nama dan Alamat.+)", RegexOptions.Singleline);

                        if (match.Success)
                        {
                            var match71 = Regex.Match(match.Groups["i71"].Value.Replace("(74)", "").Replace("Nama dan Alamat yang mengajukan Permohonan Paten :", "").Trim(), @"(?<name>.+)\n(?<adress>.+),?\s(?<country>[A-Z].+)");
                            if (match71.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Name = match71.Groups["name"].Value.Trim(),
                                    Address1 = match71.Groups["adress"].Value.Trim(),
                                    Country = MakeCountry(match71.Groups["country"].Value.Trim())
                                });
                            }
                            else Console.WriteLine($"{match.Groups["i71"].Value}");

                            if (MakeCountry(match71.Groups["country"].Value.Trim()) == null) Console.WriteLine(" ======================================================== " + $"{match71.Groups["country"].Value.Trim()}");

                            var inventors = Regex.Split(match.Groups["i72"].Value.Replace("Nama Inventor :", "").Trim(), @"\n").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (var inventor in inventors)
                            {
                                var match72 = Regex.Match(inventor.Trim(), @"(?<name>.+),\s(?<country>[A-Z]{2})");

                                if (match72.Success)
                                {
                                    statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                                    {
                                        Name = match72.Groups["name"].Value.Trim(),
                                        Country = match72.Groups["country"].Value.Trim()
                                    });
                                }
                                else Console.WriteLine($"{inventor} -- 72");
                            }

                            var match74 = Regex.Match(match.Groups["i74"].Value.Trim(), @"(?<name>.+?)\n(?<adress>.+)");

                            if (match74.Success)
                            {
                                statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                                {
                                    Name = match74.Groups["name"].Value.Trim(),
                                    Address1 = match74.Groups["adress"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{match.Groups["i74"].Value.Trim()} -- 74");
                        }
                        else 
                        {
                            var match1 = Regex.Match(inid, @"(?<i72>Nama Inventor.+)\n(?<i74>Nama dan Alamat.+)", RegexOptions.Singleline);
                            if (match1.Success)
                            {
                                var inventors = Regex.Split(match.Groups["i72"].Value.Replace("Nama Inventor :", "").Trim(), @"\n").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                foreach (var inventor in inventors)
                                {
                                    var match72 = Regex.Match(inventor.Trim(), @"(?<name>.+),\s(?<country>[A-Z]{2})");

                                    if (match72.Success)
                                    {
                                        statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                                        {
                                            Name = match72.Groups["name"].Value.Trim(),
                                            Country = match72.Groups["country"].Value.Trim()
                                        });
                                    }
                                    else Console.WriteLine($"{inventor} -- 72");
                                }

                                var match74 = Regex.Match(match.Groups["i74"].Value.Trim(), @"(?<name>.+?)\n(?<adress>.+)");

                                if (match74.Success)
                                {
                                    statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                                    {
                                        Name = match74.Groups["name"].Value.Trim(),
                                        Address1 = match74.Groups["adress"].Value.Trim()
                                    });
                                }
                                else Console.WriteLine($"{match.Groups["i74"].Value.Trim()} -- 74");
                            }
                            else Console.WriteLine($"{inid}");
                        } 
                    }
                    else
                    if (inid.StartsWith("(54)"))
                    {
                        statusEvent.Biblio.Titles.Add(new Integration.Title
                        {
                            Language = "ID",
                            Text = inid.Replace("\r", "").Replace("\n", " ").Replace("(54) Judul Invensi : ", "").Trim()
                        });
                    }
                    else
                    if (inid.StartsWith("(57)"))
                    {
                        statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                        {
                            Language = "ID",
                            Text = inid.Replace("\r", "").Replace("\n", " ").Replace("(57) Abstrak :", "").Trim()
                        });
                    }
                    else
                    if (inid.StartsWith("(43)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(43) Tanggal Pengumuman Paten :", ""), @"\d{2}.\d{2}.\d{4}");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Date = DateTime.Parse(match.Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} -- 43");
                        
                    }
                    else Console.WriteLine($"{inid}");
                    //if(inid.StartsWith(""))
                    //if(inid.StartsWith(""))
                    //if(inid.StartsWith(""))
                }
            }
            else if(subCode == "2")
            {
                foreach (var inid in MakeInids(note))
                {
                    if (inid.StartsWith("(20)"))
                    {

                    }
                    else
                    if (inid.StartsWith("(11)"))
                    {
                        statusEvent.Biblio.Publication.Number = inid.Replace("\r", "").Replace("\n", " ").Replace("(11) No Pengumuman :", "").Trim();
                    }
                    else
                    if (inid.StartsWith("(13)"))
                    {
                        statusEvent.Biblio.Publication.Kind = inid.Replace("\r", "").Replace("\n", " ").Replace("(13)", "").Trim();
                    }
                    else
                    if (inid.StartsWith("(21)"))
                    {
                        statusEvent.Biblio.Application.Number = inid.Replace("\r", "").Replace("\n", " ").Replace("(21)","").Replace("No. Permohonan Paten :", "").Trim();
                    }
                    else
                    if (inid.StartsWith("(22)"))
                    {
                      statusEvent.Biblio.Application.Date = DateTime.Parse(inid.Replace("\r", "").Replace("\n", " ").Replace("(22)", "").Replace("Tanggal Penerimaan Permohonan Paten :", "").Replace("Data Prioritas :", "").Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();                        
                    }
                    else
                    if (inid.StartsWith("(43)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(43) Tanggal Pengumuman Paten :", ""), @"\d{2}.\d{2}.\d{4}");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Date = DateTime.Parse(match.Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} -- 43");

                    }
                    else
                    if (inid.StartsWith("(54)"))
                    {
                        statusEvent.Biblio.Titles.Add(new Integration.Title
                        {
                            Language = "ID",
                            Text = inid.Replace("\r", "").Replace("\n", " ").Replace("(54) Judul Invensi : ", "").Trim()
                        });
                    }
                    else
                    if (inid.StartsWith("(57)"))
                    {
                        statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                        {
                            Language = "ID",
                            Text = inid.Replace("\r", "").Replace("\n", " ").Replace("(57) Abstrak :", "").Trim()
                        });
                    }
                    else
                    if (inid.StartsWith("(74)"))
                    {
                        var match = Regex.Match(inid, @"(?<i71>Nama dan Alamat yang.+)\n(?<i72>Nama Inventor.+)\n(?<i74>Nama dan Alamat.+)", RegexOptions.Singleline);

                        if (match.Success)
                        {
                            var match71 = Regex.Match(match.Groups["i71"].Value.Replace("(74)", "").Replace("Nama dan Alamat yang mengajukan Permohonan Paten :", "").Trim(), @"(?<name>.+)\n(?<adress>.+),?\s(?<country>[A-Z].+)");
                            if (match71.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Name = match71.Groups["name"].Value.Trim(),
                                    Address1 = match71.Groups["adress"].Value.Trim(),
                                    Country = MakeCountry(match71.Groups["country"].Value.Trim())
                                });
                            }
                            else Console.WriteLine($"{match.Groups["i71"].Value}");

                            if (MakeCountry(match71.Groups["country"].Value.Trim()) == null) Console.WriteLine(" ======================================================== " + $"{match71.Groups["country"].Value.Trim()}");

                            var inventors = Regex.Split(match.Groups["i72"].Value.Replace("Nama Inventor :", "").Trim(), @"\n").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (var inventor in inventors)
                            {
                                var match72 = Regex.Match(inventor.Trim(), @"(?<name>.+),\s(?<country>[A-Z]{2})");

                                if (match72.Success)
                                {
                                    statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                                    {
                                        Name = match72.Groups["name"].Value.Trim(),
                                        Country = match72.Groups["country"].Value.Trim()
                                    });
                                }
                                else Console.WriteLine($"{inventor} -- 72");
                            }

                            var match74 = Regex.Match(match.Groups["i74"].Value.Replace("Nama dan Alamat Konsultan Paten :","").Trim(), @"(?<name>.+?)\n(?<adress>.+)");

                            if (match74.Success)
                            {
                                statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                                {
                                    Name = match74.Groups["name"].Value.Trim(),
                                    Address1 = match74.Groups["adress"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{match.Groups["i74"].Value.Trim()} -- 74");
                        }
                    }
                    else Console.WriteLine($"{inid}");



                }
            }
            else if (subCode is "3")
            {
                foreach (var inid in MakeInids(note.Trim()))
                {
                    if (inid.StartsWith("(11)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @":(?<num>.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Number = match.Groups["num"].Value.Trim();
                        }
                        else Console.WriteLine($"11 --- {inid}");
                    }
                    else if (inid.StartsWith("(13)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"\)(?<kind>.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                        }
                        else Console.WriteLine($"13 --- {inid}");
                    }
                    else if (inid.StartsWith("(51)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @":(?<ipcs>.+)");

                        if (match.Success)
                        {
                            var ipcs = Regex.Split(match.Groups["ipcs"].Value.Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (var ipc in ipcs)
                            {
                                var ipcMatch = Regex.Match(ipc.Trim(), @"(?<class>.+)\s(?<num>\d+\/\d+)");

                                if (ipcMatch.Success)
                                {
                                    statusEvent.Biblio.Ipcs.Add(new Ipc()
                                    {
                                        Class = ipcMatch.Groups["class"].Value.Replace(" ","") + " " + ipcMatch.Groups["num"].Value.Trim()
                                    });
                                }
                                else Console.WriteLine($"ipc --- {ipc}");
                            }
                        }
                        else Console.WriteLine($"51 --- {inid}");
                    }
                    else if (inid.StartsWith("(21)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @":(?<num>.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Number = match.Groups["num"].Value.Trim();
                        }
                        else Console.WriteLine($"21 --- {inid}");
                    }
                    else if (inid.StartsWith("(22)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @":(?<date>.+)");

                        if (match.Success)
                        {
                            var dateMatch = Regex.Match(match.Groups["date"].Value.Trim(), @"(?<day>\d+)\s(?<month>\D+)\s(?<year>\d{4})");

                            if (dateMatch.Success)
                            {
                                var month = MakeMonth(dateMatch.Groups["month"].Value.Trim());
                                if (month is not null)
                                {
                                    statusEvent.Biblio.Application.Date = dateMatch.Groups["year"].Value.Trim() + "/" + month + "/" + dateMatch.Groups["day"].Value.Trim();
                                }
                                else Console.WriteLine($"====== {dateMatch.Groups["month"].Value.Trim()}");
                            }
                            else Console.WriteLine($"date --- {match.Groups["date"].Value.Trim()}");
                        }
                        else Console.WriteLine($"22 --- {inid}");
                    }
                    else if (inid.StartsWith("(33)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"Negara(?<prio>.+)");

                        if (match.Success)
                        {
                            var priorities = Regex.Split(match.Groups["prio"].Value.Trim(), @"(?<=[A-Z]{2})").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (var priority in priorities)
                            {
                                var prioMatch = Regex.Match(priority.Trim(), @"(?<num>.+)\s(?<day>\d+)\s(?<month>\D+)\s(?<year>\d{4})\s(?<code>\D{2})");

                                if (prioMatch.Success)
                                {
                                    var month = MakeMonth(prioMatch.Groups["month"].Value.Trim());

                                    if (month is not null)
                                    {
                                        statusEvent.Biblio.Priorities.Add(new Priority()
                                        {
                                            Number = prioMatch.Groups["num"].Value.Trim(),
                                            Country = prioMatch.Groups["code"].Value.Trim(),
                                            Date = prioMatch.Groups["year"].Value.Trim() + "/" + month + "/" + prioMatch.Groups["day"].Value.Trim()
                                    });
                                    }
                                    else Console.WriteLine($"===== {prioMatch.Groups["month"].Value.Trim()}");

                                }
                                else Console.WriteLine($" priority ---- {priority}");
                            }
                        }
                        else Console.WriteLine($"33 --- {inid}");
                    }
                    else if (inid.StartsWith("(43)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @":(?<date>.+)");

                        if (match.Success)
                        {
                            var dateMatch = Regex.Match(match.Groups["date"].Value.Trim(), @"(?<day>\d+)\s(?<month>\D+)\s(?<year>\d{4})");

                            if (dateMatch.Success)
                            {
                                var month = MakeMonth(dateMatch.Groups["month"].Value.Trim());
                                if (month is not null)
                                {
                                    statusEvent.Biblio.Publication.Date = dateMatch.Groups["year"].Value.Trim() + "/" + month + "/" + dateMatch.Groups["day"].Value.Trim();
                                }
                                else Console.WriteLine($"====== {dateMatch.Groups["month"].Value.Trim()}");
                            }
                            else Console.WriteLine($"date --- {match.Groups["date"].Value.Trim()}");
                        }
                        else Console.WriteLine($"43 --- {inid}");
                    }
                    else if (inid.StartsWith("(71)"))
                    {
                        var match = Regex.Match(inid.Trim(), @":\n(?<name>.+)\n(?<adress>.+\n?.+\s)(?<country>.+)");

                        if (match.Success)
                        {
                            var country = MakeCountry(match.Groups["country"].Value.Trim());

                            if (country is not null)
                            {
                                statusEvent.Biblio.Applicants.Add(new PartyMember()
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = country
                                });
                            }
                            else Console.WriteLine($"===== {match.Groups["country"].Value.Trim()}");
                        }
                        else Console.WriteLine($"71 --- {inid}");
                    }
                    else if (inid.StartsWith("(72)"))
                    {
                        var match = Regex.Match(inid.Trim(), @":(?<inventors>.+)", RegexOptions.Singleline);

                        if (match.Success)
                        {
                            var inventors = Regex.Split(match.Groups["inventors"].Value.Trim(), "\n").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (var inventor in inventors)
                            {
                                var inventorMatch = Regex.Match(inventor.Trim(), @"(?<name>.+),(?<code>[A-Z]{2})");

                                if (inventorMatch.Success)
                                {
                                    statusEvent.Biblio.Inventors.Add(new PartyMember()
                                    {
                                        Name = inventorMatch.Groups["name"].Value.Trim(),
                                        Country = inventorMatch.Groups["code"].Value.Trim()
                                    });
                                }
                            }
                        }
                        else Console.WriteLine($"72 --- {inid}");
                    }
                    else if (inid.StartsWith("(74)"))
                    {
                        var match = Regex.Match(inid.Trim(), @":\n(?<name>.+?)\n(?<adress>.+)", RegexOptions.Singleline);

                        if (match.Success)
                        {
                            statusEvent.Biblio.Agents.Add(new PartyMember()
                            {
                                Name = match.Groups["name"].Value.Trim(),
                                Address1 = match.Groups["adress"].Value.Trim(),
                                Country = "ID"
                            });
                        }
                        else Console.WriteLine($"74 --- {inid}");
                    }
                    else if (inid.StartsWith("(54)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @":(?<title>.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Titles.Add(new Title()
                            {
                                Language = "ID",
                                Text = match.Groups["title"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"54 --- {inid}");
                    }
                    else if (inid.StartsWith("(57)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @":(?<abstract>.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Abstracts.Add(new Abstract()
                            {
                                Language = "ID",
                                Text = match.Groups["abstract"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"57 --- {inid}");
                    }
                }
            }
            return statusEvent;
        }

        private List<string> MakeInids(string note, string subcode)
        {
            var inids = new List<string>();

            if (subcode == "1")
            {
                var match = Regex.Match(note, @"(?<FirstPart>\(20.+)(?<Inid30>\(30.+)(?<SecondPart>\(43.+)(?<Inid57>\(57.+)", RegexOptions.Singleline);

                if (match.Success)
                {
                    inids = Regex.Split(match.Groups["FirstPart"].Value + match.Groups["SecondPart"].Value, @"(?=\(\d{2}\).+)", RegexOptions.Singleline).Where(_ => !string.IsNullOrEmpty(_)).ToList();

                    inids.Add(match.Groups["Inid30"].Value.Trim());
                    inids.Add(match.Groups["Inid57"].Value.Trim());
                }
            }
            return inids;
        }
        private string CleanInid(string inid, string subCode)
        {
            var cleanInid = string.Empty;
            var match = Regex.Match(inid.Trim(), @"\d{2}\).+?:(?<inid>.+)", RegexOptions.Singleline);
            if (match.Success)
            {
                cleanInid = match.Groups["inid"].Value.Trim();
            }
            return cleanInid;
        }

        private string MakeRightDate(string inid, string subCode)
        {
            var date = string.Empty;
            if (subCode == "1")
            {
                var match = Regex.Match(inid.Trim(), @"(?<day>\d{2})(?<month>.+)(?<year>\d{4})");
                if (match.Success)
                {
                    date = match.Groups["year"].Value.Trim() + "/" + MakeMonth(match.Groups["month"].Value.Trim()) + "/" +
                           match.Groups["day"].Value.Trim();
                }
            }
            return date;
        }









        internal List<string> MakeInids (string note)
        {
            var inids = Regex.Split(note.Substring(0, note.IndexOf("(57)")).Trim(), @"(?=\(\d{2}\)\s?)").Where(val => !string.IsNullOrEmpty(val)).ToList();

            inids.Add(note.Substring(note.IndexOf("(57)")).Trim());

            return inids;
        }
        internal string MakeMonth(string month) => month switch
        {
            "JAN" => "01",
            "Januari" => "01",
            "FEB" => "02",
            "Februari" => "02",
            "MAR" => "03",
            "Maret" => "03",
            "APR" => "04",
            "April" => "04",
            "MAY" => "05",
            "Mei" => "05",
            "JUN" => "06",
            "Juni" => "06",
            "JUL" => "07",
            "Juli" => "07",
            "AUG" => "08",
            "Agustus" => "08",
            "SEP" => "09",
            "September" => "09",
            "OCT" => "10",
            "Oktober" => "10",
            "NOV" => "11",
            "November" => "11",
            "DEC" => "12",
            "Desember" => "12",
            _ => null
        };
        internal string MakeCountry(string country) => country switch
        {
            "France" => "FR",
            "Finland" => "FI",
            "Cimahi" => "ID",
            "Guangdong" => "CN",
            "Japan" => "JP",
            "Federation" => "RU",
            "America" => "US",
            "Bandung" => "ID",
            "Bogor" => "ID",
            "Netherlands" => "NL",
            "INDONESIA" => "ID",
            "Taiwan" => "TW",
            "Padang" => "ID",
            "JAPAN" => "JP",
            "Cirebon" => "ID",
            "Indonesia" => "ID",
            "Australia" => "AU",
            "Germany" => "DE",
            "Zealand" => "NZ",
            "Denmark" => "DK",
            "China" => "CN",
            "States of America" => "US",
            "MAKASSAR" => "ID",
            "Singapore" => "SG",
            "Republic of Korea" => "KR",
            "KINGDOM" => "UK",
            "Sweden" => "SE",
            "United States of America" => "US",
            "AUSTRALIA" => "AU",
            "THAILAND" => "TH",
            "Austria" => "AT",
            "SINGAPORE" => "SG",
            "DEUTSCHLAND" => "DE",
            "USA" => "US",
            "Belgium" => "BE",
            "Korea" => "KR",
            "Beijing" => "CN",
            "Kingdom" => "UK",
            "States" => "US",
            "STATES OF AMERICA" => "US",
            "NETHERLANDS" => "NL",
            "SPAIN" => "ES",
            "Depok" => "ID",
            "Banjarmasi" => "ID",
            "Italy" => "IT",
            "India" => "IN",
            "Colombia" => "CO",
            "Slovenia" => "SI",
            "Zurich" => "CH",
            "Switzerland" => "CH",
            "Canada" => "CA",
            "United Kingdom" => "UK",
            "Zhejiang" => "CN",
            "UNITED STATES OF AMERICA" => "US",
            "Spain" => "ES",
            "Makassar" => "ID",
            "INDIA" => "IN",
            "Mejayan" => "ID",
            "Manis" => "ID",
            "Surakarta" => "ID",
            "Semarang 5" => "ID",
            "MANADO" => "ID",
            "Manado" => "ID",
            "Malang" => "ID",
            "Dayeuhkolot" => "ID",
            "Sumedang" => "ID",
            "Russia" => "RU",
            "Timur" => "ID",
            "Jatiwarna" => "ID",
            "Jakarta" => "ID",
            "Surabaya" => "ID",
            _ => null
        };
        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool SendToProduction)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
                string url;
                url = SendToProduction == true ? @"https://diamond.lighthouseip.online/external-api/import/legal-event" : @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                StringContent content = new(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                _ = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
