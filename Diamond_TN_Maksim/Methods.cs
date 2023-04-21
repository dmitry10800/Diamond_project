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
using Diamond.Core.Models;
using Integration;

namespace Diamond_TN_Maksim
{
    internal class Methods
    {
        private string _currentFileName;
        private int _id = 1;
        private string I21 = "[21]";
        private string I22 = "[22]";
        private string I54 = "[54]";
        private string I71 = "[71]";
        private string I72 = "[72]";

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            var patents = new List<Diamond.Core.Models.LegalStatusEvent>();

            var directory = new DirectoryInfo(path);

            var files = new List<string>();

            foreach (var file in directory.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            var xElements = new List<XElement>();

            foreach (var tetml in files)
            {
                _currentFileName = tetml;

                tet = XElement.Load(tetml);

                if (subCode == "2")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Requêtes en validation des"))
                        .ToList();

                    var notes = Regex.Split(FullText(xElements,subCode), @"(?=\[21\].+)").Where(_ => !string.IsNullOrEmpty(_) && _.StartsWith("[21]")).ToList();

                    foreach (var note in notes)
                    {
                       patents.Add(MakeNote(note, subCode, "AG"));
                    }
                }
            }
            return patents;
        }
        private string FullText(List<XElement> xElements, string subCode)
        {
            var sb = new StringBuilder();

            if (subCode == "2")
            {
                foreach (var xElement in xElements)
                {
                    sb = sb.Append(xElement.Value + " ");
                }
            }
            return sb.ToString();
        }

        private Diamond.Core.Models.LegalStatusEvent MakeNote(string note, string subCode, string sectionCode)
        {
            var patent = new Diamond.Core.Models.LegalStatusEvent()
            {
                SectionCode = sectionCode,
                SubCode = subCode,
                CountryCode = "TN",
                Id = _id++,
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                Biblio = new(),
            };

            var culture = new CultureInfo("ru-RU");

            if (subCode == "2")
            {
                note = CleanNote(note);
                foreach (var inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith(I21))
                    {
                        patent.Biblio.Application.Number = CleanInid(inid, subCode);
                    }
                    if (inid.StartsWith(I22))
                    {
                        patent.Biblio.Application.Date = DateTime.Parse(CleanInid(inid, subCode),culture).ToString(@"yyyy/MM/dd").Replace(".", "/");
                    }
                    if (inid.StartsWith(I54))
                    {
                        patent.Biblio.Titles.Add(new Title()
                        {
                            Language = "FR",
                            Text = CleanInid(inid, subCode)
                        });
                    }
                    if (inid.StartsWith(I71))
                    {
                        var applicants = Regex.Split(CleanInid(inid, subCode), @";").Where(_ => !string.IsNullOrEmpty(_));

                        foreach (var applicant in applicants)
                        {
                            patent.Biblio.Applicants.Add(new PartyMember()
                            {
                                Name = applicant.Trim()
                            });
                        }
                    }
                    if (inid.StartsWith(I72))
                    {
                        var inventors = Regex.Split(CleanInid(inid, subCode), @";").Where(_ => !string.IsNullOrEmpty(_));

                        foreach (var inventor in inventors)
                        {
                            patent.Biblio.Inventors.Add(new PartyMember()
                            {
                                Name = inventor.Trim()
                            });
                        }
                    }
                }
                var match = Regex.Match(Path.GetFileName(_currentFileName.Replace(".tetml", "").Replace("TN_", "")), @"\d{8}");
                if (match.Success)
                {
                    patent.Biblio.Publication.Date = match.Value.Insert(4, "/").Insert(7, "/");
                }
            }

            return patent;
        }

        private List<string> MakeInids(string note, string subCode)
        {
            var inids = new List<string>();

            if (subCode == "2")
            {
                inids = Regex.Split(note, @"(?=\[\d{2}\].+)").Where(_ => !string.IsNullOrEmpty(_)).ToList();
            }
            return inids;
        }
        private string CleanNote(string note)
        {
            return note.Replace("\r", "").Replace("\n", " ");
        }
        private string CleanInid(string inid, string subCode)
        {
            var inidResult = string.Empty;
            if (subCode == "2")
            {
                var match = Regex.Match(inid, @"\[\d{2}\].+:(?<Note>.+)");
                if (match.Success)
                {
                    inidResult = match.Groups["Note"].Value.Trim();
                }
                else Console.WriteLine(inidResult + " wrong inid");
            }
            return inidResult;
        }
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
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
