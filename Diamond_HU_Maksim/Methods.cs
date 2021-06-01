using Integration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_HU_Maksim
{
    class Methods
    {
        private readonly string I11 = "( 11 )";
        private readonly string I21 = "( 21 )";
        private readonly string I96 = "( 96 )";
        private readonly string I97 = "( 97 )";
        private readonly string I73 = "( 73 )";
        private readonly string I72 = "( 72 )";
        private readonly string I74 = "( 74 )";
        private readonly string I54 = "( 54 )";
        private readonly string I51 = "( 51 )";
        private readonly string I13 = "( 13 )";
        private readonly string I30 = "( 30 )";

        private string CurrentFileName;
        private int id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start (string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalStatusEvents = new List<Diamond.Core.Models.LegalStatusEvent>();

            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            List<string> files = new List<string>();

            foreach (FileInfo file in directoryInfo.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            List<XElement> xElements = new List<XElement>();

            foreach (string tetFile in files)
            {
                CurrentFileName = tetFile;

                tet = XElement.Load(tetFile);

                if(subCode == "3")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Európai szabadalmak szövege fordításának benyújtása"))
                        .TakeWhile(val => !val.Value.StartsWith("Felszólalási eljárásban módosított európai szabadalom szövege fordításának benyújtása"))
                        .TakeWhile(val => !val.Value.StartsWith("Európai szabadalom igénypontokon kívüli szövegének magyar nyelvű fordítása"))
                        .ToList();

                    foreach (string note in BuildNotes(xElements))
                    {
                        legalStatusEvents.Add(SplitNote(note, subCode, "AG"));
                    }
                }
            }
            return legalStatusEvents;
        }

        internal string BuildText(List<XElement> xElements)
        {
            string fullText = null;

            foreach (XElement text in xElements)
            {
                fullText += text.Value.Trim() + "\n";
            }
            return fullText;
        }

        internal List<string> BuildNotes (List<XElement> xElements)
        {
           string text = BuildText(xElements);

            List<string> notes = Regex.Split(text, @"(?=\(\s?11\s?\))", RegexOptions.Multiline).ToList();
            notes.RemoveAt(0);

            return notes;
        }

        internal Diamond.Core.Models.LegalStatusEvent SplitNote(string note, string sub, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent legalStatus = new Diamond.Core.Models.LegalStatusEvent();

            legalStatus.GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf"));

            legalStatus.CountryCode = "HU";

            legalStatus.SectionCode = sectionCode;

            legalStatus.SubCode = sub;

            legalStatus.Id = id++;

            Biblio biblioData = new Biblio();

            EuropeanPatent europeanPatent = new EuropeanPatent();

            biblioData.EuropeanPatents = new List<EuropeanPatent>();

            foreach (string inid in SplitNoteToInid(note))
            {
                if (inid.StartsWith(I11))
                {
                    biblioData.Publication.Number = inid.Replace(I11, "").Replace("\r", "").Replace("\n", "").Trim();
                }
                else
                if (inid.StartsWith(I21))
                {
                    biblioData.Application.Number = inid.Replace(I21, "").Replace("\r", "").Replace("\n", "").Trim();
                }
                else
                if (inid.StartsWith(I96))
                {
                    string text = inid.Replace(I96, "").Replace("\r", "").Replace("\n", " ").Trim();

                    Match match = Regex.Match(text, @"(?<number>.+)\s(?<date>\d{4}.\d{2}.\d{2})");

                    if (match.Success)
                    {
                        europeanPatent.AppNumber = match.Groups["number"].Value.Trim();
                        europeanPatent.AppDate = match.Groups["date"].Value.Trim();
                    }
                    else Console.WriteLine($"in 96 field this {text} don't matched");
                }
                else
                if (inid.StartsWith(I97))
                {
                    string text = inid.Replace(I97, "").Replace("\r", "").Replace("\n", " ").Trim();
                    List<string> inids97 = Regex.Split(text, @"(?<=\d{4}.\d{2}.\d{2}.)", RegexOptions.Multiline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                    for (int i = 0; i < inids97.Count; i++)
                    {
                        if (i == 0)
                        {
                            Match match = Regex.Match(inids97[i], @"(?<number>.+)\s(?<kind>[A-Z]{1}\d+)\s(?<date>\d{4}.\d{2}.\d{2})");
                            if (match.Success)
                            {
                                europeanPatent.PubNumber = match.Groups["number"].Value.Trim();
                                europeanPatent.PubKind = match.Groups["kind"].Value.Trim();
                                europeanPatent.PubDate = match.Groups["date"].Value.Trim();
                            }

                            biblioData.EuropeanPatents.Add(europeanPatent);
                        }
                        else
                        {
                            EuropeanPatent europeanPatent1 = new EuropeanPatent();

                            Match match = Regex.Match(inids97[i], @"(?<number>.+)\s(?<kind>[A-Z]{1}\d+)\s(?<date>\d{4}.\d{2}.\d{2})");
                            if (match.Success)
                            {
                                europeanPatent1.PubNumber = match.Groups["number"].Value.Trim();
                                europeanPatent1.PubKind = match.Groups["kind"].Value.Trim();
                                europeanPatent1.PubDate = match.Groups["date"].Value.Trim();
                            }

                            biblioData.EuropeanPatents.Add(europeanPatent1);
                        }
                    }
                }
                else
                if (inid.StartsWith(I73))
                {
                    string text = inid.Replace(I73, "").Replace("\r", "").Replace("\n", " ").Trim();

                    Match match = Regex.Match(text, @"(?<name>.+?),(?<adress>.+)\s\((?<code>[A-Z]{2})");

                    biblioData.Assignees = new List<PartyMember>();

                    if (match.Success)
                    {
                        biblioData.Assignees.Add(new PartyMember
                        {
                            Name = match.Groups["name"].Value.Trim(),
                            Address1 = match.Groups["adress"].Value.Trim(),
                            Country = match.Groups["code"].Value.Trim()
                        });
                    }
                    else Console.WriteLine($"in 73 field this {text} don't matched");
                }
                else
                if (inid.StartsWith(I72))
                {
                    string text = inid.Replace(I72, "").Trim();

                    List<string> inventors = Regex.Split(text, "\n").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    biblioData.Inventors = new List<PartyMember>();

                    foreach (string inventor in inventors)
                    {
                        biblioData.Inventors.Add(new PartyMember
                        {
                            Name = inventor
                        });
                    }
                }
                else
                if (inid.StartsWith(I74))
                {
                    string text = inid.Replace(I74, "").Replace("\r", "").Replace("\n", " ").Trim();

                    Match match = Regex.Match(text, @"(?<name>.+?),\s(?<adress>.+)\s\((?<code>[A-Z]{2})");

                    biblioData.Agents = new List<PartyMember>();

                    if (match.Success)
                    {
                        biblioData.Agents.Add(new PartyMember
                        {
                            Name = match.Groups["name"].Value.Trim(),
                            Address1 = match.Groups["adress"].Value.TrimEnd(','),
                            Country = match.Groups["code"].Value.Trim()
                        });
                    }
                    else Console.WriteLine($"in 74 field this {text} don't matched");
                }
                else
                if (inid.StartsWith(I54))
                {
                    string text = inid.Replace(I54, "").Replace("\r", "").Replace("\n", " ").Trim();

                    biblioData.Titles = new List<Title>
                    {
                        new Title
                        {
                            Text = text,
                            Language = "HU"
                        }
                    };
                }
                else
                if (inid.StartsWith(I51))
                {
                    string text = inid.Replace(I51, "").Replace("\r", "").Replace("\n", " ").Trim();

                    List<string> ipcs = Regex.Split(text, @"(?<=\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    biblioData.Ipcs = new List<Ipc>();

                    foreach (string ipc in ipcs)
                    {
                        Match match = Regex.Match(ipc, @"(?<class>.+)\s?\((?<date>.+)\)");

                        if (match.Success)
                        {
                            biblioData.Ipcs.Add(new Ipc
                            {
                                Class = match.Groups["class"].Value.Trim(),
                                Date = match.Groups["date"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"in 51 field this {ipc} don't matched");
                    }
                }
                else
                if (inid.StartsWith(I13))
                {
                    biblioData.Publication.Kind = inid.Replace(I13, "").Replace("\r", "").Replace("\n", "").Trim();
                }
                else
                if (inid.StartsWith(I30))
                {
                    string text = inid.Replace(I30, "").Replace("\r", "").Replace("\n", " ").Trim();

                    List<string> field30 = Regex.Split(text, @"(?<=\b[A-Z]{2}\b)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    biblioData.Priorities = new List<Priority>();

                    foreach (string item in field30)
                    {
                        Match match = Regex.Match(item, @"(?<number>.+)\s?(?<date>\d{4}.\d{2}.\d{2}).?\s(?<code>[A-Z]{2})");

                        if (match.Success)
                        {
                            biblioData.Priorities.Add(new Priority
                            {
                                Number = match.Groups["number"].Value.Trim(),
                                Date = match.Groups["date"].Value.Trim(),
                                Country = match.Groups["code"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"in 30 field this {item} don't matched in ------------ {biblioData.Application.Number}");
                    }
                }
                else Console.WriteLine($"This inid {inid} not procesed ");
            }

            legalStatus.Biblio = biblioData;

            return legalStatus;
        }

        internal List<string> SplitNoteToInid (string note) => Regex.Split(note, @"(?=\(\s?[0-9]{2}\s?\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
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
