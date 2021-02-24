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

namespace Diamond_MK_Maksim
{
    class Methods
    {
        string CurrentFileName;

        int id = 1;

        public List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = new List<Diamond.Core.Models.LegalStatusEvent>();

            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            List<string> files = new List<string>();

            foreach (FileInfo file in directoryInfo.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            List<XElement> xElements = null;

            foreach (string  tetFile in files)
            {
                CurrentFileName = tetFile;

                tet = XElement.Load(tetFile);

                xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text").ToList();

                if (subCode == "3")
                {
                    foreach (string note in BuildNotes(xElements))
                    {
                     convertedPatents.Add(SplitNote(note, subCode, "FG"));
                    }
                }

            }

            return convertedPatents;
        }


        List<string> BuildNotes(List<XElement> xElements)
        {
            string fullText = null;

            foreach (XElement xElement in xElements)
            {
                fullText += xElement.Value + "\n";
            }

            Regex splitText = new Regex(@"(?=\(51\)\s)");

            List<string> notes = splitText.Split(fullText).Where(val => !string.IsNullOrEmpty(val)).ToList();

            return notes;
        }

        Diamond.Core.Models.LegalStatusEvent SplitNote(string note, string sub, string sectionCode)
        {
            string formatText = note.Replace("\r", "").Replace("\n", " ");

            string field57 = formatText.Substring(formatText.IndexOf("(57) ")).Replace("(57) ","").Trim();

            string textWithOutField57 = formatText.Remove(formatText.IndexOf("(57) "));

            Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();

            legalEvent.GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf"));

            legalEvent.SubCode = sub;

            legalEvent.SectionCode = sectionCode;

            legalEvent.CountryCode = "MK";

            legalEvent.Id = id++;

            Biblio biblioData = new Biblio();

            Abstract abs = new Abstract()
            {
                Text = field57,
                Language = "MK"
            };

            biblioData.Abstracts.Add(abs);

            biblioData.EuropeanPatents = new List<EuropeanPatent>();

            EuropeanPatent europeanPatent = new EuropeanPatent();

            foreach (string record in SplitByInid(textWithOutField57))
            {
               
                if (record.Contains("(51)"))
                {
                    string text = record.Replace("(51)", "").Trim();

                    biblioData.Ipcs = new List<Ipc>();

                    List<string> ipcs = text.Split(",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (string ipc in ipcs)
                    {
                        string tmp = ipc.Trim();

                        Regex regex = new Regex(@"(?<gr1>^\D{1})\s?(?<gr2>.+)");

                        Match match = regex.Match(tmp);

                        if (match.Success)
                        {
                            string temp = match.Groups["gr1"].Value.Trim() + match.Groups["gr2"].Value.Trim();
                            biblioData.Ipcs.Add(new Ipc
                            {
                                Class = temp
                            });
                        }                     
                    }
                }
                else
                if (record.Contains("(11)"))
                {
                    string text = record.Replace("(11)", "").Trim();

                    biblioData.Publication.Number = text;
                }
                else
                if (record.Contains("(13)"))
                {
                    string text = record.Replace("(13)", "").Trim();

                    biblioData.Publication.Kind = text;
                }
                else
                if (record.Contains("(21)"))
                {
                    string text = record.Replace("(21)", "").Trim();

                    biblioData.Application.Number = text;
                }
                else
                if (record.Contains("(22)"))
                {
                    string text = record.Replace("(22)", "").Trim();

                    CultureInfo cultureInfo = new CultureInfo("ru-RU");

                    biblioData.Application.Date = DateTime.Parse(text, cultureInfo).ToString("yyyy/MM/dd").Replace(".", "/");
                }
                else
                if (record.Contains("(45"))
                {
                    string text = record.Replace("(45)", "").Trim();

                    CultureInfo cultureInfo = new CultureInfo("ru-RU");

                    biblioData.DOfPublication = new DOfPublication
                    {
                        date_45 = DateTime.Parse(text, cultureInfo).ToString("yyyy/MM/dd").Replace(".", "/")
                    };
                }
                else
                if (record.Contains("(30)"))
                {
                    string text = record.Replace("(30)", "").Trim();

                    Regex splitRegex = new Regex(@";|and");

                    List<string> elements = splitRegex.Split(text).Where(val => !string.IsNullOrEmpty(val)).ToList();

                    Regex regex = new Regex(@"(?<code>^\D{2})\s?(?<number>\d.+)\s(?<date>\d{2}\/\d{2}\/\d{4})");

                    biblioData.Priorities = new List<Priority>();

                    foreach (string element in elements)
                    {
                        Match match = regex.Match(element);

                        if (match.Success)
                        {
                            CultureInfo cultureInfo = new CultureInfo("ru-RU");

                            biblioData.Priorities.Add(new Priority
                            {
                                Number = match.Groups["number"].Value.Trim(),
                                Date = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy/MM/dd").Replace(".", "/"),
                                Country = match.Groups["code"].Value.Trim()
                            });
                        }                      
                        else
                        {
                            Regex regex1 = new Regex(@"(?<number>\d.+)\s(?<date>\d{2}\/\d{2}\/\d{4})\s(?<code>\D{2})");

                            Match match1 = regex1.Match(element);

                            if (match1.Success)
                            {
                                CultureInfo cultureInfo = new CultureInfo("ru-RU");

                                biblioData.Priorities.Add(new Priority
                                {
                                    Number = match1.Groups["number"].Value.Trim(),
                                    Date = DateTime.Parse(match1.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy/MM/dd").Replace(".", "/"),
                                    Country = match1.Groups["code"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"в 30-ом поле не разбился {element}");
                        }                       
                    }
                }
                else
                if (record.Contains("(96)"))
                {
                    string text = record.Replace("(96)", "").Trim();
                   
                    Regex regex = new Regex(@"(?<date>\d{2}\/\d{2}\/\d{4})\s(?<number>.+)");

                    Match match = regex.Match(text);

                    if (match.Success) {

                        CultureInfo cultureInfo = new CultureInfo("ru-RU");

                        europeanPatent.AppNumber = match.Groups["number"].Value.Trim();

                        europeanPatent.AppDate = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy/MM/dd").Replace(".", "/");
                    }
                }
                else
                if (record.Contains("(97)"))
                {
                    string text = record.Replace("(97)", "").Trim();

                    Regex regex = new Regex(@"(?<date>\d{2}\/\d{2}\/\d{4})\s(?<number>.+)");

                    Match match = regex.Match(text);

                    if (match.Success)
                    {
                        CultureInfo cultureInfo = new CultureInfo("ru-RU");

                        europeanPatent.PubNumber = match.Groups["number"].Value.Trim();

                        europeanPatent.PubDate = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy/MM/dd").Replace(".", "/");

                        biblioData.EuropeanPatents.Add(europeanPatent);
                    }
                }
                else
                if (record.Contains("(73)"))
                {
                    string text = record.Replace("(73)", "").Trim();

                    Regex splitRegex = new Regex(@"(?<=[A-Z]{2};)");

                    List<string> elements = splitRegex.Split(text).Where(val => !string.IsNullOrEmpty(val)).ToList();

                    Regex regex = new Regex(@"(?<adress>.+)(?<code>\D{2})");

                    biblioData.Assignees = new List<PartyMember>();

                    foreach (string element in elements)
                    {
                        Match match = regex.Match(element);

                        if (match.Success)
                        {

                            biblioData.Assignees.Add(new PartyMember
                            {
                                Address1 = match.Groups["adress"].Value.Trim().TrimEnd(','),
                                Country = match.Groups["code"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"Assignees не получился тут {element}");
                    }
                }
                else
                if (record.Contains("(74)"))
                {
                    string text = record.Replace("(74)", "").Trim();

                    Regex regex = new Regex(@"(?<name>.+)\s(?<adress>б?ул\..+)");

                    Match match = regex.Match(text);

                    biblioData.Agents = new List<PartyMember>();

                    if (match.Success)
                    {
                        biblioData.Agents.Add(new PartyMember
                        {
                            Name = match.Groups["name"].Value.Trim(),
                            Address1 = match.Groups["adress"].Value.Trim(),
                            Country = "MK"
                        });
                    }
                    else
                    {
                        Regex regex1 = new Regex(@"(?<name>[А-Я].+?)\s(?<adress>Никола.+)");

                        Match match1 = regex1.Match(text);

                        if (match1.Success)
                        {
                            biblioData.Agents.Add(new PartyMember
                            {
                                Name = match1.Groups["name"].Value.Trim(),
                                Address1 = match1.Groups["adress"].Value.Trim(),
                                Country = "MK"
                            });
                        }
                        else
                        {
                            Regex regex2 = new Regex(@"(?<name>.+?)\s(?<adress>[А-Я][а-я].+)");

                            Match match2 = regex2.Match(text);

                            if (match2.Success)
                            {
                                biblioData.Agents.Add(new PartyMember
                                {
                                    Name = match2.Groups["name"].Value.Trim(),
                                    Address1 = match2.Groups["adress"].Value.Trim(),
                                    Country = "MK"
                                });
                            }
                            else Console.WriteLine($"не разбился {text}");
                        }
                    }
                }
                else
                if (record.Contains("(72)"))
                {
                    string text = record.Replace("(72)", "").Trim();

                    Regex splitRegex = new Regex(@";|and");

                    List<string> names = splitRegex.Split(text).Where(val => !string.IsNullOrEmpty(val)).ToList();

                    biblioData.Inventors = new List<PartyMember>();

                    foreach (var item in names)
                    {
                        biblioData.Inventors.Add(new PartyMember
                        {
                            Name = item,
                        });
                    }
                }
                else
                if (record.Contains("(54)"))
                {
                    string text = record.Replace("(54)", "").Trim();

                    biblioData.Titles = new List<Title>
                    {
                        new Title
                        {
                            Text = text,
                            Language = "MK"
                        }
                    };
                }

                else Console.WriteLine($"Не обработан вот такой айнид {record}");
            }

            legalEvent.Biblio = biblioData;

            return legalEvent;
        }

        List<string> SplitByInid(string formateText)
        {
            Regex splitByInid = new Regex(@"(?=\(\d{2}\)\s)");

            List<string> inids = splitByInid.Split(formateText).Where(val => !string.IsNullOrEmpty(val)).ToList();

            return inids;
        }

        public void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                string tmpValue = JsonConvert.SerializeObject(rec);
                string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                //string url = @"https://diamond.lighthouseip.online/external-api/import/legal-event";
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
