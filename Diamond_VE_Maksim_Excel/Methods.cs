using System.Text.RegularExpressions;
using Integration;
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

                if (subCode == "26" || subCode == "64" || subCode == "65" || subCode == "34")
                {
                    for (var row = 0; row <= sheet.LastRowNum; row++)
                    {
                        var sectionCode = subCode switch
                        {
                            "26" => "FD",
                            "34" => "AZ",
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

                        statusEvent.LegalEvent.Date = DateTime.Parse(sheet.GetRow(row).GetCell(4).ToString()).ToString("yyyy/MM/dd");
                        legalStatusEvents.Add(statusEvent);
                    }
                }
                else if (subCode == "56" || subCode == "55")
                {
                    for (var row = 0; row <= sheet.LastRowNum; row++)
                    {
                        var sectionCode = subCode switch
                        {
                            "55" => "FD",
                            "56" => "FD",
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

                        var cellAssignees = sheet.GetRow(row)?.GetCell(2);
                        if (cellAssignees != null && !string.IsNullOrEmpty(cellAssignees.ToString()))
                        {

                            var listAssignees = Regex.Split(cellAssignees.ToString().Replace("\r", "").Replace("\n", " ").Trim(), ";").Where(_ => !string.IsNullOrEmpty(_));

                            foreach (var assignee in listAssignees)
                            {
                                var assigneeMatch = Regex.Match(assignee, @"(?<name>.+)\sDomicilio:(?<adress>.+)\sPaís:(?<country>.+)", RegexOptions.Singleline);

                                if (assigneeMatch.Success)
                                {
                                    var countryCode = MakeCountryCode(assigneeMatch.Groups["country"].Value.Trim());

                                    if (countryCode != null)
                                    {
                                        statusEvent.Biblio.Assignees.Add(new PartyMember()
                                        {
                                            Name = assigneeMatch.Groups["name"].Value.Trim(),
                                            Address1 = assigneeMatch.Groups["adress"].Value.Trim(),
                                            Country = countryCode
                                        });
                                    }
                                    else
                                        Console.WriteLine(assigneeMatch.Groups["country"].Value.Trim());
                                }
                            }
                        }

                        var cellAgent = sheet.GetRow(row)?.GetCell(3);
                        if (cellAgent != null && !string.IsNullOrEmpty(cellAgent.ToString()))
                        {
                            var listAgents = Regex.Split(cellAgent.ToString(), ";").Where(_ => !string.IsNullOrEmpty(_));

                            foreach (var agent in listAgents)
                            {
                                statusEvent.Biblio.Agents.Add(new PartyMember()
                                {
                                    Name = agent
                                });
                            }
                        }

                        statusEvent.LegalEvent.Date = DateTime.Parse(sheet.GetRow(row).GetCell(4).ToString()).ToString("yyyy/MM/dd");
                        legalStatusEvents.Add(statusEvent);
                    }
                }
            }
            return legalStatusEvents;
        }


        private string? MakeCountryCode(string country) => country switch
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
            "VENEZUELA" => "VE",
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
            "( SIN PAIS )" => "",
            "(SIN PAIS)" => "",
            "SIN PAIS" => "",
            "VIETNAM" => "VN",
            "COLOMBIA" => "CO",
            "ISRAEL" => "IL",
            "EMIRATOS ARABES UNIDOS" => "AE",
            "TAIWAN" => "TW",
            "ARABIA SAUDITA" => "SA",
            "ARGENTINA" => "AR",
            "BULGARIA" => "BG",
            "PANAMA" => "PA",
            "MONACO" => "MC",
            "SUDAFRICA" => "ZA",
            "BIELORRUSIA (REPUBLICA DE BELARUS)" => "BY",
            "ANDORRA" => "AD",
            "HONG KONG" => "HK",
            "MAURITANIA" => "MR",
            "PERU" => "PE",
            "BARBADOS" => "BB",
            "ANTILLAS HOLANDESAS" => "AG",
            "PORTUGAL" => "PT",
            "REPUBLICA CHECA" => "CZ",
            _ => null
        };
    }
}
