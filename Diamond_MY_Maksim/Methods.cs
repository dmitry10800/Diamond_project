using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_MY_Maksim
{
    internal class Methods
    {
        private string CurrentFileName;
        private int Id = 1;

        public List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
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
                       .TakeWhile(val => !val.Value.StartsWith("LAPSED"))
                       .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode), @"((?=\(12\)\nM)|(?=\(12\)\sM))").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (string note in notes)
                    {
                       statusEvents.Add(MakePatent(note, subCode, "FG"));
                    }
                }
            }
            return statusEvents;
        }

        internal string MakeText(List<XElement> xElement, string subCode)
        {
            string text = "";

            if (subCode == "1")
            {
                foreach (XElement element in xElement)
                {
                    text += element.Value + "\n";
                }

                return text;
            }

            return null;
        }

        internal Diamond.Core.Models.LegalStatusEvent MakePatent(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                CountryCode = "MY",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = Id++,
                LegalEvent = new(),
                Biblio = new()
                {
                    DOfPublication = new()
                }
            };

            CultureInfo culture = new("ru-RU");

            if(subCode == "1")
            {
                Match inid1113 = Regex.Match(note.Trim(), @"-(?<inid11>\d+)-(?<inid13>[A-Z])", RegexOptions.Singleline);

                if (inid1113.Success)
                {
                    statusEvent.Biblio.Publication.Number = inid1113.Groups["inid11"].Value.Trim();
                    statusEvent.Biblio.Publication.Kind = inid1113.Groups["inid13"].Value.Trim();
                }
                else
                {
                    Console.WriteLine($"11 Wrong ------ {note.Trim()}");
                }

                Match inid51 = Regex.Match(note.Trim(), @"(?<inid51>Classification.+?)\(", RegexOptions.Singleline);

                if (inid51.Success)
                {
                    List<string> ipcs = Regex.Split(inid51.Groups["inid51"].Value.Trim(), @"\n").Where(val => !string.IsNullOrEmpty(val) && new Regex(@"\D\d{2}\D\s\d+\/\d+").Match(val).Success).ToList();

                    foreach (string ipc in ipcs)
                    {
                        statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
                        {
                            Class = ipc.Trim()
                        });
                    }
                }
                else
                {
                    Console.WriteLine($"51 Wrong ------ {note.Trim()}");
                }

                Match inid47 = Regex.Match(note.Trim(), @"(?<inid47>Date\s?of\s?Pub.+?\d+\s\D+\s\d{4})", RegexOptions.Singleline);

                if (inid47.Success)
                {
                    Match match = Regex.Match(inid47.Groups["inid47"].Value.Trim(), @"(?<day>\d+)\s(?<month>\D+)\s(?<year>\d{4})");

                    if (match.Success)
                    {
                        statusEvent.Biblio.DOfPublication.date_47 = match.Groups["year"].Value.Trim() + "/" + MakeMonth(match.Groups["month"].Value.Trim()) + "/" + match.Groups["day"].Value.Trim();
                    }
                }
                else
                {
                    Console.WriteLine($"47 ----- {note.Trim()}");
                }

                Match inid56 = Regex.Match(note.Trim(), @"(?<inid56>Prior\sArt.+?)\(57", RegexOptions.Singleline);

                if (inid56.Success)
                {
                    List<string> patentCitations = Regex.Split(inid56.Groups["inid56"].Value.Trim(), @"\n")
                        .Where(val => !string.IsNullOrEmpty(val) && new Regex(@"[A-Z]{2}\s?\d+.\d+.?\d+\s?[A-Z]\d?").Match(val).Success).ToList();

                    foreach (string patentCitation in patentCitations)
                    {
                        Match match = Regex.Match(patentCitation.Trim(), @"(?<code>[A-Z]{2})\s?(?<num>\d+.\d+.?\d+)\s?(?<kind>[A-Z]\d?)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.PatentCitations.Add(new Integration.PatentCitation
                            {
                                Kind = match.Groups["kind"].Value.Trim(),
                                Number = match.Groups["num"].Value.Trim(),
                                Applicant = match.Groups["code"].Value.Trim()
                            });
                        }
                    }



                }
                else
                {
                    Console.WriteLine($"56 Wrong ----- {note.Trim()}");
                }

                Match inid54 = Regex.Match(note.Trim(), @"(?<inid54>Title.+)\(57", RegexOptions.Singleline);

                if (inid54.Success)
                {
                    statusEvent.Biblio.Titles.Add(new Integration.Title
                    {
                        Language = "EN",
                        Text = inid54.Groups["inid54"].Value.Replace("\r", "").Replace("\n", " ").Replace("Title", "").Replace(":", "").Trim()
                    });
                }
                else
                {
                    Console.WriteLine($"54 Wrong ----- {note.Trim()}");
                }

                Match inid57 = Regex.Match(note.Trim(), @"(?<inid57>Abstract\s:.+)", RegexOptions.Singleline);

                if (inid57.Success)
                {
                    statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                    {
                        Language = "EN",
                        Text = inid57.Groups["inid57"].Value.Replace("\r", "").Replace("\n", " ").Replace("Abstract", "").Replace(":", "").Trim()
                    });
                }
                else
                {
                    Console.WriteLine($"57 Wrong ---- {note.Trim()}");
                }

            }

            return statusEvent;
        }

        internal string ? MakeMonth(string month) => month  switch
        {
            "January" => 01.ToString(),
            "February" => 02.ToString(),
            "March" => 03.ToString(),
            "April" => 04.ToString(),
            "May" => 05.ToString(),
            "June" => 06.ToString(),
            "July" => 07.ToString(),
            "August" => 08.ToString(),
            "September" => 09.ToString(),
            "October" => 10.ToString(),
            "November" => 11.ToString(),
            "December" => 12.ToString(),
            _ => null
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
