using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DiamondProjectClasses;
using Integration;

namespace Diamond_RU
{
    class ConvertToDiamondFormat
    {
        private static string DateNormalize(string date)
        {
            if (!string.IsNullOrEmpty(date))
            {
                if (Regex.IsMatch(date, @"(?<Year>\d{4})(?<Month>\d{2})(?<Day>\d{2})"))
                {
                    var pattern = new Regex(@"(?<Year>\d{4})(?<Month>\d{2})(?<Day>\d{2})");
                    var dateMatch = pattern.Match(date);
                    if (dateMatch.Success)
                        return $"{dateMatch.Groups["Year"].Value}-{dateMatch.Groups["Month"].Value}-{dateMatch.Groups["Day"].Value}";
                    else
                        Console.WriteLine("Date format doesn't match 6 digit");
                    return date;
                }

                if (Regex.IsMatch(date, @"\d{2}\/*\-*\.*\d{2}\/*\-*\.*\d{4}"))
                {
                    var pattern = new Regex(@"(?<Day>\d{2})\/*\-*\.*(?<Month>\d{2})\/*\-*\.*(?<Year>\d{4})");
                    var dateMatch = pattern.Match(date);
                    if (dateMatch.Success)
                        return $"{dateMatch.Groups["Year"].Value}-{dateMatch.Groups["Month"].Value}-{dateMatch.Groups["Day"].Value}";
                    else
                        Console.WriteLine("Date format doesn't match 6 digit");
                    return date;
                }
            }
            return null;

        }

        public static List<Diamond.Core.Models.LegalStatusEvent> ConvertSubCodeToDiamondFormat(List<RecordElements.SubCode> elementOuts, int numberSubCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            if (elementOuts != null)
            {
                int leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = RU_main.NameArchive;
                    /*Setting subcode*/
                    legalEvent.SubCode = numberSubCode.ToString();
                    /*Setting Section Code*/
                    legalEvent.SectionCode = record.B903i;
                    /*Setting Country Code*/
                    legalEvent.CountryCode = record.B190;
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    legalEvent.NewBiblio = new Biblio();
                    legalEvent.LegalEvent = new LegalEvent();
                    legalEvent.LegalEvent.Translations = new List<NoteTranslation>();
                    legalEvent.NewBiblio.Agents = new List<PartyMember>();
                    biblioData.Applicants = new List<PartyMember>();
                    legalEvent.NewBiblio.Applicants = new List<PartyMember>();
                    biblioData.Inventors = new List<PartyMember>();
                    legalEvent.NewBiblio.Inventors = new List<PartyMember>();
                    biblioData.Assignees = new List<PartyMember>();
                    legalEvent.NewBiblio.Assignees = new List<PartyMember>();
                    biblioData.Agents = new List<PartyMember>();
                    biblioData.DOfPublication = new DOfPublication();
                    legalEvent.LicenseInformation = new LicenseInformation();
                    legalEvent.LicenseInformation.LicenseeInformation = new List<LicenseeInformation>();
                    legalEvent.LicenseInformation.LicensorInformation = new List<PartyMember>();

                    /*Elements output*/
                    if (numberSubCode != 25)
                        biblioData.Publication.Number = record.B110;
                    else
                    {
                        if (record.B110j != null)
                        {
                            biblioData.Publication.Number = record.B110j;
                        }
                    }
                    biblioData.Publication.Kind = record.B130.Trim();
                    biblioData.Publication.Date = DateNormalize(record.B140);
                    biblioData.Application.Number = record.B210;
                    biblioData.Application.Date = DateNormalize(record.B220);

                    if (numberSubCode == 3 || numberSubCode == 5 || numberSubCode == 6)
                    {
                        if (record.isField73)
                        {
                            if (record.B734i.Count > 1)
                            {
                                Console.WriteLine("Патент с номером " + record.B110 + " содержит несколько полей B734i. Необходимо проверить качество заполнения данных. Необходимо заполнить вручную.");
                                continue;
                            }
                            else
                            {
                                if (record.B734i != null)
                                {
                                    string countryCode = Regex
                                        .Match(record.B734i[0], @"(?<value>\([A-Z]{2}\))", RegexOptions.IgnoreCase)?.Groups["value"]
                                        .Value;
                                    legalEvent.NewBiblio.Assignees.Add(new PartyMember { Name = record.B734i[0].Replace(countryCode, "").Trim(), Address1 = record.B980i.Address, Country = countryCode.Replace(@"(", "").Replace(@")", "").Trim() });
                                }
                            }
                        }

                        if (!record.isField73)
                        {
                            if (record.B980i != null)
                                biblioData.Agents.Add(new PartyMember { Address1 = record.B980i.Address });
                        }
                    }

                    if (record.B980i != null && numberSubCode != 3 && numberSubCode != 5 && numberSubCode != 6 && numberSubCode != 4)
                    {
                        if (record.B980i.Name != null && !string.IsNullOrEmpty(record.B980i.Address))
                            legalEvent.NewBiblio.Agents.Add(new PartyMember { Name = record.B980i.Name, Address1 = record.B980i.Address.TrimEnd(',').Trim() });
                        else
                        {
                            if (!string.IsNullOrEmpty(record.B980i.Address))
                                legalEvent.NewBiblio.Agents.Add(new PartyMember { Address1 = record.B980i.Address });
                        }
                    }

                    if (numberSubCode == 4 && record.B980i.Address != null)
                    {
                        if (record.B734i != null)
                        {
                            if (record.B734i.Count > 1)
                            {
                                Console.WriteLine("Запись " + biblioData.Publication.Number + " имеет несколько Assignee, поэтому необходимо ее проверить на корректность заполнения...");
                            }
                            foreach (var item in record.B734i)
                            {
                                legalEvent.NewBiblio.Assignees.Add(new PartyMember { Address1 = record.B980i.Address, Name = item });
                            }
                        }
                    }

                    if (record.B711 != null)
                    {
                        foreach (var item in record.B711)
                        {
                            string countryCode = Regex
                                .Match(item, @"(?<value>\([A-Z]{2}\))", RegexOptions.IgnoreCase)?.Groups["value"]
                                .Value;
                            var applicant = new PartyMember
                            {
                                Name = item.Replace(countryCode, "").Trim(),
                                Country = countryCode.Replace(@"(", "").Replace(@")", "").Trim(),
                                Language = "RU"
                            };
                            legalEvent.NewBiblio.Applicants.Add(applicant);
                        }
                    }

                    if (record.B721 != null)
                    {
                        foreach (var item in record.B721)
                        {
                            string countryCode = Regex
                                .Match(item, @"(?<value>\([A-Z]{2}\))", RegexOptions.IgnoreCase)?.Groups["value"]
                                .Value;
                            var inventor = new PartyMember
                            {
                                Name = item.Replace(countryCode, "").Trim(),
                                Country = countryCode.Replace(@"(", "").Replace(@")", "").Trim(),
                                Language = "RU"
                            };
                            legalEvent.NewBiblio.Inventors.Add(inventor);
                        }
                    }

                    if (record.B731 != null)
                    {
                        foreach (var item in record.B731)
                        {
                            string countryCode = Regex
                                .Match(item, @"(?<value>\([A-Z]{2}\))", RegexOptions.IgnoreCase)?.Groups["value"]
                                .Value;
                            biblioData.Assignees.Add(new PartyMember { Name = item.Replace(countryCode, "").Trim(), Country = countryCode.Replace(@"(", "").Replace(@")", "").Trim() });
                        }
                    }

                    //для 2 Сабкода может понадобится доработка
                    if (numberSubCode == 2)
                    {
                        if (record.B731i != null)
                        {
                            foreach (var item in record.B731i)
                            {
                                string countryCode = Regex
                                    .Match(item, @"(?<value>\([A-Z]{2}\))", RegexOptions.IgnoreCase)?.Groups["value"]
                                    .Value;
                                biblioData.Assignees = new List<PartyMember>();
                                var assignee = new PartyMember
                                {
                                    Name = item.Replace(countryCode, "").Trim(),
                                    Language = "RU",
                                    Country = countryCode.Replace(@"(", "").Replace(@")", "").Trim()
                                };

                                if (!string.IsNullOrEmpty(record.B980i.Address))
                                    assignee.Address1 = record.B980i.Address;

                                biblioData.Assignees.Add(assignee);
                            }
                        }
                    }

                    if (record.B734i != null)
                    {
                        if (legalEvent.NewBiblio.Assignees.Count == 0)
                        {
                            foreach (var item in record.B734i)
                            {
                                string countryCode = Regex
                                    .Match(item, @"(?<value>\([A-Z]{2}\))", RegexOptions.IgnoreCase)?.Groups["value"]
                                    .Value;
                                legalEvent.NewBiblio.Assignees.Add(new PartyMember { Name = item.Replace(countryCode, "").Trim(), Country = countryCode.Replace(@"(", "").Replace(@")", "").Trim() });
                            }
                        }
                    }

                    if (numberSubCode == 20)
                    {
                        if (record.B909i != null)
                        {
                            biblioData.Assignees.Add(new PartyMember { Name = record.B909i.Patent_Owner.Trim() });
                            biblioData.Titles.Add(new Title { Text = record.B909i.Title, Language = "RU" });
                        }
                    }



                    if (numberSubCode == 25 && !string.IsNullOrEmpty(record.B460))
                    {
                        biblioData.DOfPublication.date_45 = record.B460;
                    }

                    if (record.B733i != null)
                    {
                        foreach (var item in record.B733i)
                        {
                            legalEvent.LicenseInformation.LicensorInformation.Add(new PartyMember { Name = item });
                        }
                    }

                    if (record.B791 != null)
                    {
                        legalEvent.LicenseInformation.LicenseeInformation.Add(new LicenseeInformation { Name = record.B791, RegistrationNumber = record.B919i });
                    }

                    /*Notes*/
                    int[] array1 = new[] { 1, 2, 7, 8, 9, 10, 11, 12, 13 }; //выводим только номер Бюллетеня
                    int[] array2 = new[] { 3, 4, 5 };  //выводим номер гос. рег. ; дата публикации ; номер Бюллетеня
                    int[] array3 = new[] { 14, 15 }; // выводим Дата публикации, номер бюллетеня
                    int[] array4 = new[] { 17, 18, 19, 20, 21, 22, 26, 28, 31, 32 }; //Выводим  Номер бюллетеня и номер публикации
                    if (array1.Contains(numberSubCode))
                    {
                        legalEvent.LegalEvent = new LegalEvent
                        {
                            Note = $"|| Номер бюллетеня | {record.B405i}\n|| (12) | {record.Field12.versionRU}",
                            Date = DateNormalize(record.B460i),
                            Language = "RU"
                        };
                        legalEvent.LegalEvent.Translations.Add(new NoteTranslation
                        {
                            Tr = $"|| Bulletin No. | {record.B405i}\n|| (12) | {record.Field12.versionEN}",
                            Language = "EN",
                            Type = "note"
                        });
                    }
                    else if (array2.Contains(numberSubCode))
                    {
                        legalEvent.LegalEvent = new LegalEvent
                        {
                            Note = $"|| Номер государственной регистрации отчуждения исключительного права | {record.B919i} \n|| Дата публикации | {DateNormalize(record.B460i)} \n|| " +
                                   $"Номер бюллетеня | {record.B405i}\n|| (12) | {record.Field12.versionRU}",
                            Date = DateNormalize(record.B920i),
                            Language = "RU"
                        };
                        legalEvent.LegalEvent.Translations.Add(new NoteTranslation
                        {
                            Tr = $"|| Registration number of transfer of an exclusive right | {record.B919i} \n|| Bulletin publication date | {DateNormalize(record.B460i)} \n|| " +
                                 $"Bulletin No. | {record.B405i}\n|| (12) | {record.Field12.versionEN}",
                            Language = "EN",
                            Type = "note"
                        });
                    }
                    else if (array3.Contains(numberSubCode))
                    {
                        legalEvent.LegalEvent = new LegalEvent
                        {
                            Note = $"|| Дата публикации | {DateNormalize(record.B460i)} \n|| Номер бюллетеня | {record.B405i}\n|| (12) | {record.Field12.versionRU}",
                            Date = DateNormalize(record.B236),
                            Language = "RU"
                        };
                        legalEvent.LegalEvent.Translations.Add(new NoteTranslation
                        {
                            Tr = $"|| Bulletin publication date | {DateNormalize(record.B460i)} \n|| Bulletin No. | {record.B405i}\n|| (12) | {record.Field12.versionEN}",
                            Language = "EN",
                            Type = "note"
                        });
                    }
                    else if (numberSubCode == 16)
                    {
                        legalEvent.LegalEvent = new LegalEvent
                        {
                            Note = $"|| Дата, с которой заявка признана отозванной | {DateNormalize(record.B236)} \n|| Дата публикации | {DateNormalize(record.B460i)} \n|| " +
                                   $"Номер бюллетеня | {record.B405i}\n|| (12) | {record.Field12.versionRU}",
                            Date = DateNormalize(record.B238),
                            Language = "RU"
                        };
                        legalEvent.LegalEvent.Translations.Add(new NoteTranslation
                        {
                            Tr = $"|| Date of withdrawal of the application | {DateNormalize(record.B236)} \n|| Bulletin publication date | {DateNormalize(record.B460i)} \n|| " +
                                 $"Bulletin No. | {record.B405i}\n|| (12) | {record.Field12.versionEN}",
                            Language = "EN",
                            Type = "note"
                        });
                    }
                    else if (array4.Contains(numberSubCode))
                    {
                        string tempDateValue = "";
                        if (numberSubCode == 17 || numberSubCode == 18 || numberSubCode == 21 || numberSubCode == 22)
                            tempDateValue = record.B994i;
                        if (numberSubCode == 19 || numberSubCode == 20)
                            tempDateValue = record.B908i;
                        if (numberSubCode == 26 || numberSubCode == 28)
                            tempDateValue = record.B247i;
                        if (numberSubCode == 31 || numberSubCode == 32)
                            tempDateValue = record.B236;

                        legalEvent.LegalEvent = new LegalEvent
                        {
                            Note = $"|| Номер бюллетеня | {record.B405i} \n|| Дата публикации бюллетеня | {DateNormalize(record.B460i)}\n|| (12) | {record.Field12.versionRU}",
                            Date = DateNormalize(tempDateValue),
                            Language = "RU"
                        };
                        legalEvent.LegalEvent.Translations.Add(new NoteTranslation
                        {
                            Tr = $"|| Bulletin No. | {record.B405i} \n|| Bulletin publication date | {DateNormalize(record.B460i)}\n|| (12) | {record.Field12.versionEN}",
                            Language = "EN",
                            Type = "note"
                        });
                    }
                    else if (numberSubCode == 6)
                    {
                        legalEvent.LegalEvent = new LegalEvent
                        {
                            Note = $"|| Номер государственной регистрации перехода исключительного права | {record.B919i} \n|| Дата публикации | {DateNormalize(record.B460i)} \n|| " +
                                   $"Номер бюллетеня | {record.B405i}\n|| (12) | {record.Field12.versionRU}",
                            Date = DateNormalize(record.B920i),
                            Language = "RU"
                        };
                        legalEvent.LegalEvent.Translations.Add(new NoteTranslation
                        {
                            Tr = $"|| Registration number of transfer of an exclusive right | {record.B919i} \n|| Bulletin publication date | {DateNormalize(record.B460i)} \n|| " +
                                 $"Bulletin No. | {record.B405i}\n|| (12) | {record.Field12.versionEN}",
                            Language = "EN",
                            Type = "note"
                        });
                    }

                    if (numberSubCode == 25)
                    {
                        legalEvent.LegalEvent = new LegalEvent
                        {
                            Note = $"|| Номер патента (аннулированного) | {record.B110} \n|| Дата публикации бюллетеня | {DateNormalize(record.B460i)} \n|| " +
                                   $"Номер бюллетеня | {record.B405i}\n|| (12) | {record.Field12.versionRU}",
                            Date = DateNormalize(Methods.GetDateFromNameArchive()),
                            Language = "RU"
                        };
                        legalEvent.LegalEvent.Translations.Add(new NoteTranslation
                        {
                            Tr = $"|| Revoked patent number | {record.B110} \n|| Bulletin publication date | {DateNormalize(record.B460i)} \n|| " +
                                 $"Bulletin No. | {record.B405i}\n|| (12) | {record.Field12.versionEN}",
                            Language = "EN",
                            Type = "note"
                        });
                    }
                    if (numberSubCode == 24)
                    {
                        if (record.B909i.Title != null)
                        {
                            legalEvent.LegalEvent = new LegalEvent
                            {
                                Note = $"|| Возражение поступило | {DateNormalize(record.B909i.Title)} \n|| Дата публикации бюллетеня | {DateNormalize(record.B460i)} \n|| " +
                                       $"Номер бюллетеня | {record.B405i}\n|| (12) | {record.Field12.versionRU}",
                                Date = DateNormalize(record.B908i),
                                Language = "RU"
                            };
                            legalEvent.LegalEvent.Translations.Add(new NoteTranslation
                            {
                                Tr = $"|| Opposition received on | {DateNormalize(record.B909i.Title)} \n|| Bulletin publication date | {DateNormalize(record.B460i)} \n|| " +
                                     $"Bulletin No. | {record.B405i}\n|| (12) | {record.Field12.versionEN}",
                                Language = "EN",
                                Type = "note"
                            });
                        }
                    }
                    if (numberSubCode == 23)
                    {
                        legalEvent.LegalEvent = new LegalEvent
                        {
                            Note = $"|| Номер патента, выданного на идентичное изобретение | {record.B110j} \n|| Дата публикации бюллетеня | {DateNormalize(record.B460i)} \n|| " +
                                   $"Номер бюллетеня | {record.B405i}\n|| (12) | {record.Field12.versionRU}",
                            Date = DateNormalize(record.B994i),
                            Language = "RU"
                        };
                        legalEvent.LegalEvent.Translations.Add(new NoteTranslation
                        {
                            Tr = $"|| Number of patent granted for an identical invention | {record.B110j} \n|| Bulletin publication date | {DateNormalize(record.B460i)} \n|| " +
                                 $"Bulletin No. | {record.B405i}\n|| (12) | {record.Field12.versionEN}",
                            Language = "EN",
                            Type = "note"
                        });
                    }
                    if (numberSubCode == 29 || numberSubCode == 30)
                    {
                        legalEvent.LegalEvent = new LegalEvent
                        {
                            Note = $"|| Дата государственной регистрации прекращаемого представления права использования | {record.B920i} \n|| Номер государственной регистрации прекращения предоставления права использования | {record.B919ic} \n|| " +
                                   $"Дата публикации бюллетеня | {DateNormalize(record.B460i)} \n|| Номер бюллетеня | {record.B405i}\n|| (12) | {record.Field12.versionRU}",
                            Date = DateNormalize(record.B920ic),
                            Language = "RU"
                        };
                        legalEvent.LegalEvent.Translations.Add(new NoteTranslation
                        {
                            Tr = $"|| Registration date of terminated license | {record.B920i} \n|| Registration number of license termination | {record.B919ic} \n|| " +
                                 $"Bulletin publication date | {DateNormalize(record.B460i)} \n|| Bulletin No. | {record.B405i}\n|| (12) | {record.Field12.versionEN}",
                            Language = "EN",
                            Type = "note"
                        });
                    }
                    /**********************/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
    }
}
