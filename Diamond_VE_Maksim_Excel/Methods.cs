using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Integration;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;


namespace Diamond_VE_Maksim_Excel
{
    internal class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            var legalStatusEvents = new List<Diamond.Core.Models.LegalStatusEvent>();

            var directory = new DirectoryInfo(path);

            var files = directory.GetFiles("*.xlsx", SearchOption.AllDirectories).Select(file => file.FullName);

            foreach (var xlsxFile in files)
            {
                _currentFileName = xlsxFile;

                ISheet sheet;

                XSSFWorkbook OpenedDocument;

                using (FileStream file = new(xlsxFile, FileMode.Open, FileAccess.Read))
                {
                    OpenedDocument = new XSSFWorkbook(file);
                }

                sheet = OpenedDocument.GetSheet("Sheet1") ?? OpenedDocument.GetSheet("Лист1");

                if (subCode == "26" || subCode == "64" || subCode == "65")
                {
                    for (var row = 0; row <= sheet.LastRowNum; row++)
                    {
                        var sectionCode = subCode switch
                        {
                            "26" => "FD",
                            "64" => "FD",
                            "65" => "FC",
                            _ => null
                        };

                        Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                        {
                            CountryCode = "VE",
                            SectionCode = sectionCode,
                            SubCode = subCode,
                            Id = _id++,
                            GazetteName = Path.GetFileName(_currentFileName.Replace(".xlsx", ".pdf")),
                            Biblio = new Biblio(),
                            LegalEvent = new LegalEvent()
                        };

                        statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(0).ToString();

                        statusEvent.Biblio.Titles.Add(new Title()
                        {
                            Text = sheet.GetRow(row).GetCell(1).ToString(),
                            Language = "ES"
                        });


                        var listApplicant = Regex.Split(sheet.GetRow(row).GetCell(2).ToString().Replace("\r", "").Replace("\n", " ").Trim(), ";").Where(_ => !string.IsNullOrEmpty(_));

                        foreach (var applicant in listApplicant)
                        {
                            var applicantMatch = Regex.Match(applicant, @"(?<name>.+)\sDomicilio:(?<adress>.+)\sPaís:(?<country>.+)", RegexOptions.Singleline);

                            if (applicantMatch.Success)
                            {
                                var countryCode = MakeCountryCode(applicantMatch.Groups["country"].Value.Trim());

                                if (countryCode != null)
                                {
                                    statusEvent.Biblio.Applicants.Add(new PartyMember()
                                    {
                                        Name = applicantMatch.Groups["name"].Value.Trim(),
                                        Address1 = applicantMatch.Groups["adress"].Value.Trim(),
                                        Country = countryCode
                                    });
                                }
                                else
                                    Console.WriteLine(applicantMatch.Groups["country"].Value.Trim());
                            }
                        }

                        var listAgents = Regex.Split(sheet.GetRow(row).GetCell(3).ToString(), ";").Where(_ => !string.IsNullOrEmpty(_));

                        foreach (var agent in listAgents)
                        {
                            statusEvent.Biblio.Agents.Add(new PartyMember()
                            {
                                Name = agent
                            });
                        }
                        
                        var match = Regex.Match(_currentFileName, @"_(?<date>\d{8})_");
                        if (match.Success)
                        {
                            statusEvent.LegalEvent.Date = match.Groups["date"].Value.Insert(4, "/").Insert(7, "/").Trim();
                        }

                        legalStatusEvents.Add(statusEvent);
                    }
                }
            }
            return legalStatusEvents;
        }


        private string? MakeCountryCode (string country) => country switch
        {
            "ESTADOS UNIDOS DE AMÉRICA" => "US",
            "JAPON" => "JP",
            "ITALIA" => "IT",
            "ESPAÑA" => "ES",
            "CANADA" => "CA",
            "BRASIL" => "BR",
            "FRANCIA" => "FR",
            "ALEMANIA" => "DE",
            "BAHAMAS" => "BS",
            "BELGICA" => "BE",
            "SUIZA" => "CH",
            "MÉXICO" => "MX",
            "BERMUDA" => "BM",
            "SUECIA" => "SE",
            "PAISES BAJOS" => "NL",
            "AUSTRALIA" => "AU",
            "REINO UNIDO" => "GB",
            "INDIA" => "IN",
            "RUSIA" => "RU",
            "URUGUAY" => "UY",
            "FINLANDIA" => "FI",
            "LUXEMBURGO" => "LU",
            "REPUBLICA DE COREA" => "KR",
            "SINGAPUR" => "SG",
            "CUBA" => "CU",
            "IRLANDA" => "IE",
            "AUSTRIA" => "AT",
            "REPUBLICA POPULAR DEMOCRATICA DE COREA" => "KP",
            "CHINA" => "CN",
            "CHILE" => "CL",
            "DINAMARCA" => "DK",
            "TURQUIA" => "TR",
            "NORUEGA" => "NO",
            _ => null
        };

        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool SendToProduction)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
                var url = SendToProduction == true ? @"https://diamond.lighthouseip.online/external-api/import/legal-event" : @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                HttpClient httpClient = new();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                StringContent content = new(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                _ = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
