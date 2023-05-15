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

namespace Diamond_RS_Maksim
{
    class Methods
    {
        private string CurrentFileName;

        private int id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start (string path, string subCode)
        {
            var convertedPatents = new List<Diamond.Core.Models.LegalStatusEvent>();

            var directoryInfo = new DirectoryInfo(path);

            var files = new List<string>();

            foreach (var file in directoryInfo.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            List<XElement> xElements = null;

            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;

                tet = XElement.Load(tetFile);

                if(subCode == "3")
                {
                    //xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text")
                    //    .SkipWhile(value => !value.Value.StartsWith("РЕГИСТРОВАНИ ПАТЕНТИ / Patents granted"))
                    //    .TakeWhile(value => !value.Value.StartsWith("ИСПРАВЉЕНА ПРВА СТРАНА B ДОКУМЕНТА / CORRECTED FRONT PAGE OF AN") &&
                    //    !value.Value.StartsWith("OБЈАВА ПАТЕНАТА У ИЗМЕЊЕНОМ ОБЛИКУ / PUBLICATION OF THE") &&
                    //    !value.Value.StartsWith("ИСПРАВЉЕН СПИС B ДОКУМЕНТА / COMPLETE REPRINT OF AN B DOCUMENT") &&
                    //    !value.Value.StartsWith("ПРЕСТАНАК ВАЖНОСТИ РЕГИСТРОВАНОГ ПАТЕНТА / Termination of validity of"))
                    //    .ToList();

                    xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text").ToList();

                    foreach (var note in BuildNotes(xElements))
                    {
                        convertedPatents.Add(SplitNote(note, subCode, "FG"));
                    }
                }
            }
            return convertedPatents;
        }

        internal List<string> BuildNotes(List<XElement> xElements)
        {
            string fullText = null;

            foreach (var text in xElements)
            {
                fullText += text.Value.Trim() + "\n";
            }

            var splitText = new Regex(@"(?=\(51\))", RegexOptions.Multiline);

            var notes = splitText.Split(fullText).ToList();
            notes.RemoveAt(0);

            return notes;
        }

        internal Diamond.Core.Models.LegalStatusEvent SplitNote(string note, string sub, string sectionCode)
        {
            var legalEvent = new Diamond.Core.Models.LegalStatusEvent();

            legalEvent.GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf"));

            legalEvent.CountryCode = "RS";

            legalEvent.Id = id++;

            legalEvent.SectionCode = sectionCode;

            legalEvent.SubCode = sub;

            var biblioData = new Biblio();

            biblioData.EuropeanPatents = new List<EuropeanPatent>();
           
            var europeanPatent = new EuropeanPatent();

            var intConvention = new IntConvention();

            var cultureInfo = new CultureInfo("ru-Ru");

            foreach (var record in SplitNoteIntoInid(note))
            {
                if (record.StartsWith("(51)"))
                {
                    var text = record.Replace("(51)", "").Trim();

                    biblioData.Ipcs = new List<Ipc>();

                    var ipcs = SplitIpcs(text);

                    foreach (var  ipc in ipcs)
                    {
                        biblioData.Ipcs.Add(new Ipc
                        {
                            Class = ipc.Item1,
                            Date = ipc.Item2
                        });
                    }
                }
                else
                if (record.StartsWith("(11)"))
                {
                    var text = record.Replace("(11)", "").Trim();

                    var regex = new Regex(@"(?<number>\d.+)\s?(?<kind>[A-Z]{1}\d)");

                    var match = regex.Match(text);

                    if (match.Success)
                    {
                        biblioData.Publication.Number = match.Groups["number"].Value.Trim();
                        biblioData.Publication.Kind = match.Groups["kind"].Value.Trim();
                    }
                    else Console.WriteLine("Ошибка в 11 айниде");
                }
                else
                if (record.StartsWith("(21)"))
                {
                    biblioData.Application.Number = record.Replace("(21)", "").Trim();
                }
                else
                if (record.StartsWith("(22)"))
                {
                    var text = record.Replace("(22)", "").Trim().TrimEnd('.');

                    biblioData.Application.Date = DateTime.Parse(text, cultureInfo).ToString("yyyy/MM/dd").Replace(".","/");
                }
                else
                if (record.StartsWith("(30)"))
                {
                    var text = record.Replace("(30)", "").Trim();

                    var regexSplit = new Regex(@"\n");

                    var notes = regexSplit.Split(text).ToList();

                    biblioData.Priorities = new List<Priority>();

                    foreach (var element in notes)
                    {
                        var regex = new Regex(@"(?<code>[A-Z]{2})\s(?<date>\d{2}.\d{2}.\d{4}).\s(?<number>.+)");

                        var match = regex.Match(element);

                        if (match.Success)
                        {
                            biblioData.Priorities.Add(new Priority
                            {
                                Country = match.Groups["code"].Value.Trim(),
                                Number = match.Groups["number"].Value.Trim(),
                                Date = DateTime.Parse(match.Groups["date"].Value.Trim(),cultureInfo).ToString("yyyy/MM/dd").Replace(".", "/")
                            });                           
                        }
                        else
                        {
                            Console.WriteLine($"Это не разбилось {element} в 30 поле");
                        }
                    }
                }
                else
                if (record.StartsWith("(86)"))
                {
                    var text = record.Replace("(86)", "").Trim();

                    var regex = new Regex(@"(?<code>[A-Z]{2})\s(?<date>\d{2}.\d{2}.\d{4}).\s(?<number>.+)");

                    var match = regex.Match(text);

                    if (match.Success)
                    {
                        intConvention.PctApplCountry = match.Groups["code"].Value.Trim();
                        intConvention.PctApplNumber = match.Groups["number"].Value.Trim();

                        intConvention.PctApplDate = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy/MM/dd").Replace(".", "/").Trim();
                    }
                    else
                    {
                        Console.WriteLine($"Это не разбилось {text} в 86 поле");
                    }
                }
                else
                if (record.StartsWith("(87)"))
                {
                    var text = record.Replace("(87)", "").Trim();

                    var regex = new Regex(@"(?<code>[A-Z]{2})\s(?<date>\d{2}.\d{2}.\d{4}).\s(?<number>.+)");

                    var match = regex.Match(text);

                    if (match.Success)
                    {
                        intConvention.PctPublCountry = match.Groups["code"].Value.Trim();
                        intConvention.PctPublNumber = match.Groups["number"].Value.Trim();

                        intConvention.PctPublDate = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy/MM/dd").Replace(".", "/").Trim();
                    }
                    else
                    {
                        Console.WriteLine($"Это не разбилось {text} в 87 поле");
                    }

                    biblioData.IntConvention = intConvention;
                }
                else
                if (record.StartsWith("(96)"))
                {
                    var text = record.Replace("(96)", "").Trim();

                    var regex = new Regex(@"(?<date>\d{2}.\d{2}.\d{4}).\s(?<number>.+)");

                    var match = regex.Match(text);

                    if (match.Success)
                    {
                        europeanPatent.AppDate = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy/MM/dd").Replace(".", "/").Trim();
                        europeanPatent.AppNumber = match.Groups["number"].Value.Trim();
                    }
                    else
                    {
                        Console.WriteLine($"Это не разбилось {text} в 96 поле");
                    }
                }
                else
                if (record.StartsWith("(97)"))
                {
                    var text = record.Replace("(97)", "").Trim();

                    var regex = new Regex(@"(?<date>\d{2}.\d{2}.\d{4}).\s(?<number>.+)\s(?<kind>\D)\s(?<note>.+)");

                    var match = regex.Match(text);

                    if (match.Success)
                    {
                        europeanPatent.PubKind = match.Groups["kind"].Value.Trim();
                        europeanPatent.PubNumber = match.Groups["number"].Value.Trim();
                        europeanPatent.PubDate = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy/MM/dd").Replace(".", "/").Trim();
                    }
                    else
                    {
                        Console.WriteLine($"Это не разбилось {text} в 97 поле");
                    }
                    biblioData.EuropeanPatents.Add(europeanPatent);
                }
                else
                if (record.StartsWith("(54)"))
                {
                    var text = record.Replace("(54)", "").Trim();

                    var lines = text.Split("\n").ToList();

                    var serbian = new List<string>();
                    var english = new List<string>();

                    biblioData.Titles = new List<Title>();

                    string serbianText = null, englishText = null;

                    if (lines.Count % 2 == 0)
                    {
                      
                        for (var i = 0; i < lines.Count/2; i++)
                        {
                            serbian.Add(lines[i]);
                        }

                        for(var i = lines.Count/2; i<lines.Count; i++)
                        {
                            english.Add(lines[i]);
                        }

                        foreach (var item in serbian)
                        {
                            serbianText += item + " ";
                        }

                        foreach (var item in english)
                        {
                            englishText += item + " ";
                        }

                        var serbianTitle = new Title
                        {
                            Text = serbianText,
                            Language = "RS"
                        };

                        var englishTitle = new Title
                        {
                            Text = englishText,
                            Language = "EN"
                        };

                        biblioData.Titles.Add(serbianTitle);
                        biblioData.Titles.Add(englishTitle);
                    }
                    else
                    {
                        for (var i = 0; i < (lines.Count/2) + 1; i++)
                        {
                            serbian.Add(lines[i]);
                        }

                        for (var i = (lines.Count/2)+1; i < lines.Count; i++)
                        {
                            english.Add(lines[i]);
                        }

                        foreach (var item in serbian)
                        {
                            serbianText += item + " ";
                        }

                        foreach (var item in english)
                        {
                            englishText += item + " ";
                        }

                        var serbianTitle = new Title
                        {
                            Text = serbianText,
                            Language = "RS"
                        };

                        var englishTitle = new Title
                        {
                            Text = englishText,
                            Language = "EN"
                        };

                        biblioData.Titles.Add(serbianTitle);
                        biblioData.Titles.Add(englishTitle);
                    }

                }
                else
                if (record.StartsWith("(73)"))
                {
                    var text = record.Replace("(73)", "").Replace("\r", "").Replace("\n", " ").Trim();

                    var people = text.Split(";").ToList();

                    var regex = new Regex(@"(?<name>.+?)\s(?<adress>[A-Z][a-z].+)\s(?<code>[A-Z]{2}$)");

                    biblioData.Assignees = new List<PartyMember>();

                    foreach (var item in people)
                    {
                        var match = regex.Match(text);

                        if (match.Success)
                        {
                            biblioData.Assignees.Add(new PartyMember
                            {
                                Name = match.Groups["name"].Value.Trim().TrimEnd(','),
                                Address1 = match.Groups["adress"].Value.Trim().TrimEnd(','),
                                Country = match.Groups["code"].Value.Trim()
                            });
                        }
                        else { Console.WriteLine($"Не разбился в 73 поле {text}"); }
                    }                   
                }
                else
                if (record.StartsWith("(72)"))
                {
                    var text = record.Replace("(72)", "").Replace("\r","").Replace("\n"," ").Trim();

                    var people = text.Split(@";").ToList();

                    var regex = new Regex(@"(?<name>.+?,.+?,)\s(?<adress>.+)\s(?<code>[A-Z]{2}$)");

                    biblioData.Inventors = new List<PartyMember>();

                    foreach (var item in people)
                    {
                        var match = regex.Match(item);

                        if (match.Success)
                        {
                            biblioData.Inventors.Add(new PartyMember
                            {
                                Name = match.Groups["name"].Value.Trim().TrimEnd(','),
                                Address1 = match.Groups["adress"].Value.Trim().TrimEnd(','),
                                Country = match.Groups["code"].Value.Trim()
                            });
                        }
                        else
                        {
                            Console.WriteLine($"Этот человек в 72 поле не разделился {item}");
                        }
                    }
                }
                else
                if (record.StartsWith("(74)"))
                {
                    var text = record.Replace("(74)", "").Replace("\r", "").Replace("\n", " ").Trim();

                    var regex = new Regex(@"(?<name>.+,.+,)\s(?<adress>[A-Z][a-z].+)");

                    biblioData.Agents = new List<PartyMember>();

                    var match = regex.Match("text");

                    if (match.Success)
                    {
                        biblioData.Agents.Add(new PartyMember
                        {
                            Name = match.Groups["name"].Value.Trim().TrimEnd(','),
                            Address1 = match.Groups["adress"].Value.Trim()
                        });
                    }
                    else
                    {
                        var regex1 = new Regex(@"(?<name>\D+)\s(?<adress>[A-Z][a-z].+)");

                        var match1 = regex1.Match(text);

                        if (match1.Success)
                        {
                            biblioData.Agents.Add(new PartyMember
                            {
                                Name = match1.Groups["name"].Value.Trim().TrimEnd(','),
                                Address1 = match1.Groups["adress"].Value.Trim()
                            });
                        }
                        else 
                        { 
                            Console.WriteLine($"Вот это в 74 поле не разбилось  {text}");
                        }
                    }
                }
                else
                if (record.StartsWith("(43)"))
                {
                    var text = record.Replace("(43)", "").Replace("\r","").Replace("\n","").Trim().TrimEnd('.');

                    biblioData.Publication.Date = DateTime.Parse(text, cultureInfo).ToString("yyyy/MM/dd").Replace(".", "/").Trim();

                }
                else Console.WriteLine($"Данный {record} не обработан");
            }

            legalEvent.Biblio = biblioData;

            return legalEvent;
        }

        internal List<string> SplitNoteIntoInid (string note)
        {
            var cut51field = note.Substring(note.IndexOf("(51)"), note.IndexOf("(21)")).Replace("\r","").Replace("\n", " ").Trim();

            var split51field = new Regex(@"(?<f51>.+)\s(?<f11>\(11\).\d+.[A-Z]{1}[0-9])(?<f51c>\s?.+)?");

            var match51field = split51field.Match(cut51field);

            string parseNote = null;

            if (match51field.Success)
            {
                parseNote = match51field.Groups["f51"].Value.Trim() + " " + match51field.Groups["f51c"].Value.Trim() + "\n" 
                    + match51field.Groups["f11"].Value.Trim() + "\n" 
                    + note.Substring(note.IndexOf("(21)")).Trim();
            }

            var splitRegex = new Regex(@"(?=\(\d{2}\)\s)");

            if (parseNote != null) 
            { 
                var inid = splitRegex.Split(parseNote).Where(val =>!string.IsNullOrEmpty(val)).ToList();
                return inid;
            }
            else
            {
                var inid = splitRegex.Split(note).Where(val => !string.IsNullOrEmpty(val)).ToList();
                return inid;
            }
        }

        internal List<(string, string)> SplitIpcs (string text)
        {
            var regexSplit = new Regex(@"\)");

            var records = regexSplit.Split(text).Where(val =>!string.IsNullOrEmpty(val)).ToList();

            var ipcs = new List<(string, string)>();

            foreach (var record in records)
            {
                var regex = new Regex(@"(?<number>.+)\s\((?<date>\d{4}.\d{2})");

                var match = regex.Match(record);

                if (match.Success)
                {
                    ipcs.Add((match.Groups["number"].Value.Trim(), match.Groups["date"].Value.Trim()));
                }
            }

            return ipcs;
        }

        internal  void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
                var url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                //string url = @"https://diamond.lighthouseip.online/external-api/import/legal-event";
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
