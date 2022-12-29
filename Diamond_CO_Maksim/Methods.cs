using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Integration;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Diamond_CO_Maksim
{
    internal class Methods
    {
        private string CurrentFileName;
        private int Id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> statusEvents = new();

            DirectoryInfo directory = new(path);

            List<string> files = new();

            foreach (var file in directory.GetFiles("*.xlsx", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            foreach (var xlsx in files)
            {
                CurrentFileName = xlsx;

                ISheet sheet;

                XSSFWorkbook OpenedDocument;
                using (FileStream file = new(xlsx, FileMode.Open, FileAccess.Read))
                {
                    OpenedDocument = new XSSFWorkbook(file);
                }

                var page = string.Empty;
                var sectionCode = string.Empty;

                if (subCode is "6")
                {
                    page = "Patente de invencion publicada";
                    sectionCode = "BZ";
                }
                else if (subCode is "7")
                {
                    page = "Modelo de utilidad publicada";
                    sectionCode = "BZ";
                } 
                else if (subCode is "8")
                {
                    page = "Patente PCT publicada";
                    sectionCode = "BZ";
                }

                if (page is null) Console.WriteLine("Wrong title for page");
                else
                {
                    sheet = OpenedDocument.GetSheet(page);

                    var startRow = 0;

                    for (var i = 0; i < sheet.LastRowNum; i++)
                    {
                        var text = string.Join(" ", sheet.GetRow(i).Cells.Where(x => x.CellType is CellType.String && !string.IsNullOrWhiteSpace(x.StringCellValue)).Select(x => x?.StringCellValue));
                        if (text.Contains("Expediente No. Tipo de trámite"))
                        {
                            startRow = ++i;
                            break;
                        }

                        if (text.Contains("Expediente No. No. De solicitud"))
                        {
                            startRow = ++i;
                            break;
                        }
                    }

                    for (var row = startRow; row <= sheet.LastRowNum; row++)
                    {
                        var isFailed = string.IsNullOrEmpty(sheet.GetRow(row)?.GetCell(1).StringCellValue);

                        if (isFailed)
                        {
                            break;
                        }

                        Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                        {
                            CountryCode = "CO",
                            SectionCode = sectionCode,
                            SubCode = subCode,
                            Id = Id++,
                            GazetteName = Path.GetFileName(CurrentFileName.Replace(".xlsx", ".pdf")),
                            Biblio = new(),
                            LegalEvent = new()
                        };

                        if (sheet.GetRow(row) != null && sheet.GetRow(row).GetCell(1) != null)
                        {
                            statusEvent.Biblio.Publication.Number = sheet.GetRow(row).GetCell(1).StringCellValue;

                            if (subCode is "6" or "7")
                            {
                                statusEvent.Biblio.Publication.LanguageDesignation = sheet.GetRow(row).GetCell(3).ToString();
                            }
                            else statusEvent.Biblio.Publication.LanguageDesignation = sheet.GetRow(row).GetCell(5).ToString();

                            if (subCode is "6" or "7")
                            {
                                statusEvent.Biblio.Titles.Add(new Title()
                                {
                                    Language = "ES",
                                    Text = sheet.GetRow(row).GetCell(4).ToString()
                                });
                            }
                            else
                            {
                                statusEvent.Biblio.Titles.Add(new Title()
                                {
                                    Language = "ES",
                                    Text = sheet.GetRow(row).GetCell(6).ToString()
                                });
                            }

                            if (subCode is "6" or "7" or "8")
                            {
                                int n;
                                if (subCode is "6" or "7") n = 6;
                                else n = 8;

                                var matchDate = Regex.Match(sheet.GetRow(row).GetCell(n).ToString(), @"(?<day>\d+)\s(?<month>\D+)\s(?<year>\d{4})");

                                if (matchDate.Success)
                                {
                                    var month = MakeMonth(matchDate.Groups["month"].Value.Trim());
                                    if (month is not null)
                                    {
                                        statusEvent.Biblio.Application.Date = matchDate.Groups["year"].Value.Trim() + "/" + month + "/" + matchDate.Groups["day"].Value.Trim();
                                    }
                                    else Console.WriteLine($"{matchDate.Groups["month"].Value.Trim()} wrong month");
                                }
                                else Console.WriteLine($"Wrong app date {sheet.GetRow(row).GetCell(n)}");
                            }

                            if (subCode is "6" or "7")
                            {
                                var memberList = Regex.Split(sheet.GetRow(row).GetCell(8).ToString(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                foreach (var member in memberList)
                                {
                                    statusEvent.Biblio.Applicants.Add(new PartyMember()
                                    {
                                        Name = member.Trim()
                                    });
                                }                             
                            }
                            else
                            {
                                var memberList = Regex.Split(sheet.GetRow(row).GetCell(11).ToString(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                foreach (var member in memberList)
                                {
                                    statusEvent.Biblio.Applicants.Add(new PartyMember()
                                    {
                                        Name = member.Trim()
                                    });
                                }
                            }

                            if (subCode is "6" or "7" or "8")
                            {
                                var n = 0;
                                if (subCode is "6" or "7") n = 9;
                                else n = 12;

                                var inventorsList = Regex.Split(sheet.GetRow(row).GetCell(n).ToString(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                foreach (var inventor in inventorsList)
                                {
                                    statusEvent.Biblio.Inventors.Add(new PartyMember()
                                    {
                                        Name = inventor.Replace("\r", "").Replace("\n", "").Trim()
                                    });
                                }
                            }

                            if (subCode is "6" or "7")
                            {
                                var memberList = Regex.Split(sheet.GetRow(row).GetCell(10).ToString(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                foreach (var member in memberList)
                                {
                                    statusEvent.Biblio.Agents.Add(new PartyMember()
                                    {
                                        Name = member.Trim()
                                    });
                                }
                            }
                            else
                            {
                                var memberList = Regex.Split(sheet.GetRow(row).GetCell(13).ToString(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                foreach (var member in memberList)
                                {
                                    statusEvent.Biblio.Agents.Add(new PartyMember()
                                    {
                                        Name = member.Trim()
                                    });
                                }
                            }
                            if (subCode is "6" or "7" or "8")
                            {
                                int countryInt, numberInt, dateInt;

                                if (subCode is "6" or "7")
                                {
                                     countryInt = 11;
                                     numberInt = 12;
                                     dateInt = 13;
                                }
                                else
                                {
                                    countryInt = 14;
                                    numberInt = 15;
                                    dateInt = 16;
                                }

                                List<string> numberList = new();
                                var countryList = Regex.Split(sheet.GetRow(row).GetCell(countryInt).ToString(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();
                                var dateList = Regex.Split(sheet.GetRow(row).GetCell(dateInt).ToString(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();
                                if (countryList.Count != 0)
                                {
                                    numberList = Regex.Split(sheet.GetRow(row).GetCell(numberInt).ToString(), @"\n").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    if (countryList.Count == dateList.Count)
                                    {
                                        string month = string.Empty, day = string.Empty, year = string.Empty;

                                        for (var i = 0; i < countryList.Count; i++)
                                        {
                                            var country = MakeCountry(countryList[i].Replace("\r", "").Replace("\n", "").Trim());
                                            
                                            var matchDate = Regex.Match(dateList[i].Replace("\r", "").Replace("\n", "").Trim(), @"(?<day>\d+)\s(?<month>\D+)\s(?<year>\d{4})");

                                            if (matchDate.Success)
                                            {
                                                month = MakeMonth(matchDate.Groups["month"].Value.Trim());

                                                if (month is not null)
                                                {
                                                    day = matchDate.Groups["day"].Value.Trim();
                                                    year = matchDate.Groups["year"].Value.Trim();
                                                }
                                                else Console.WriteLine($"{matchDate.Groups["month"].Value.Trim()} wrong month");
                                            }
                                            else Console.WriteLine($"Wrong app date {dateList[i]}");

                                            if (country is not null)
                                            {
                                                statusEvent.Biblio.Priorities.Add(new Priority()
                                                {
                                                    Number = numberList[i].Replace("\r", "").Replace("\n", "").TrimEnd(','),
                                                    Country = country,
                                                    Date = year + "/" + month + "/" + day
                                                });
                                            }
                                            else Console.WriteLine($"{countryList[i].Trim()}");
                                        }
                                    }
                                }
                            }
                            if (subCode is "6" or "7" or "8")
                            {
                                var n = 0;
                                if (subCode is "6" or "7") n = 14;
                                else n = 17;

                                var ipcsList = Regex.Split(sheet.GetRow(row).GetCell(n).ToString(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                foreach (var ipc in ipcsList)
                                {
                                    statusEvent.Biblio.Ipcs.Add(new Ipc()
                                    {
                                        Class = ipc.Replace("\r", "").Replace("\n", "").Trim()
                                    });
                                }
                            }

                            if (subCode is "8")
                            {
                                statusEvent.Biblio.IntConvention.PctApplNumber = sheet.GetRow(row).GetCell(2).StringCellValue;
                                statusEvent.Biblio.IntConvention.PctPublNumber = sheet.GetRow(row).GetCell(3).StringCellValue;

                                var matchDate = Regex.Match(sheet.GetRow(row).GetCell(9).ToString(), @"(?<day>\d+)\s(?<month>\D+)\s(?<year>\d{4})");

                                if (matchDate.Success)
                                {
                                    var month = MakeMonth(matchDate.Groups["month"].Value.Trim());
                                    if (month is not null)
                                    {
                                        statusEvent.Biblio.IntConvention.PctApplDate = matchDate.Groups["year"].Value.Trim() + "/" + month + "/" + matchDate.Groups["day"].Value.Trim();
                                    }
                                    else Console.WriteLine($"{matchDate.Groups["month"].Value.Trim()} wrong month");
                                }
                                else Console.WriteLine($"Wrong app date {sheet.GetRow(row).GetCell(9).ToString()}");
                            }
                        }

                        statusEvents.Add(statusEvent);
                    }
                }
            }
            return statusEvents;
        }
        internal string MakeMonth(string month) => month switch
        {
            "ene." => "01",
            "feb." => "02",
            "mar." => "03",
            "abr." => "04",
            "may." => "05",
            "jun." => "06",
            "jul." => "07",
            "ago." => "08",
            "sept." => "09",
            "oct." => "10",
            "nov." => "11",
            "dic." => "12",
            _ => null,
        };
        internal string MakeCountry(string country) => country switch
        {
            "ESTADOS UNIDOS DE AMÉRICA" => "US",
            "HUNGRIA" => "HU",
            "UCRANIA" => "UA",
            "BRASIL" => "BR",
            "URUGUAY" => "UY",
            "INTERNATIONAL BUREAU" => "IB",
            "SUECIA" => "SE",
            "REPUBLICA POPULAR DE CHINA" => "CN",
            "ISRAEL" => "IL",
            "REPUBLICA DOMINICANA" => "DO",
            "NORUEGA" => "NO",
            "INDIA" => "IN",
            "PERU" => "PE",
            "CHINA" => "CN",
            "ALEMANIA" => "DE",
            "PORTUGAL" => "PT",
            "JAPON" => "JP",
            "ESPAÑA" => "ES",
            "JAPÓN" => "JP",
            "FRANCIA" => "FR",
            "MEXICO" => "MX",
            "DINAMARCA" => "DK",
            "CANADA" => "CA",
            "SUIZA" => "CH",
            "ITALIA" => "IT",
            "ARGENTINA" => "AR",
            "SINGAPUR" => "SG",
            "FINLANDIA" => "FI",
            "TURQUIA" => "TR",
            "PAISES BAJOS" => "NL",
            "REINO UNIDO" => "GB",
            "FEDERACION DE RUSIA" => "RU",
            "FEDERACIÓN DE RUSIA" => "RU",
            "REPUBLICA DE COREA" => "KR",
            "REPÚBLICA DE COREA" => "KR",
            "REINO UNIDO DE LA GRAN BRETAÑA" => "GB",
            "EUROP PATENT ORGANIZATION(EPO)" => "EP",
            _ => null,
        };
        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool SendToProduction)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
                string url;
                url = SendToProduction == true ? @"https://diamond.lighthouseip.online/external-api/import/legal-event" : @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
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
