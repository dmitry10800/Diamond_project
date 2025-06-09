using Integration;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Dimond_EE
{
    class DiamondConverter
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub5Convertor(List<Patent> patents)
        {
            var legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();

            var id = 1;

            foreach (var record in patents)
            {
                var legalEvent = new Diamond.Core.Models.LegalStatusEvent();

                legalEvent.GazetteName = record.newspaperName;

                legalEvent.SubCode = "5";

                legalEvent.SectionCode = "FG4A";

                legalEvent.CountryCode = "EE";

                legalEvent.Id = id++;

                var biblioData = new Biblio();

                if (record.i11 != null)
                    biblioData.Publication.Number = record.i11;

                if (record.i13 != null)
                    biblioData.Publication.Kind = record.i13;

                if (record.i51 != null)
                {
                    biblioData.Ipcs = new List<Ipc>();

                    foreach (var item in record.i51)
                    {
                        biblioData.Ipcs.Add(new Ipc
                        {
                            Class = item.Item1,
                            Date = item.Item2.Replace(".", "/") + "/01"
                        });
                    }
                }

                if (record.i30 != null)
                {
                    biblioData.Priorities = new List<Priority>();

                    foreach (var item in record.i30)
                    {
                        biblioData.Priorities.Add(new Priority
                        {
                            Date = item.date,
                            Number = item.number,
                            Country = item.kind
                        });
                    }
                }

                if (record.i96appDate != null || record.i96appNumber != null || record.i97 != null)
                {
                    biblioData.EuropeanPatents = new List<EuropeanPatent>();
                    foreach (var item in record.i97)
                    {
                        biblioData.EuropeanPatents.Add(new EuropeanPatent
                        {
                            PubNumber = item.number,
                            PubDate = item.date,
                            PubCountry = item.kind,
                            AppNumber = record.i96appNumber,
                            AppDate = record.i96appDate,
                        });
                    }
                }

                var title = new Title(){
                    Text = record.i54,
                    Language = "ET"
                };
                biblioData.Titles.Add(title);


                    if(record.i73 != null)
                    {
                        biblioData.Assignees = new List<PartyMember>();

                        foreach (var item in record.i73)
                        {
                            biblioData.Assignees.Add(new PartyMember
                            {
                                Name = item.name,
                                Address1 = item.adress,
                                Country = item.country
                            });
                        }
                    }

                if (record.i72 != null)
                {
                    biblioData.Inventors = new List<PartyMember>();

                    foreach (var item in record.i72)
                    {
                        biblioData.Inventors.Add(new PartyMember
                        {
                            Name = item.name,
                            Address1 = item.adress,
                            Country = item.country
                        });
                    }
                }

                if (record.i74 != null)
                {
                    biblioData.Agents = new List<PartyMember>();

                    foreach (var item in record.i74)
                    {
                        biblioData.Agents.Add(new PartyMember
                        {
                            Name = item.name,
                            Address1 = item.adress,
                            Country = item.country
                        });
                    }
                }

                if (record.note != null)
                {
                    var forNote = new List<string>();

                    var textWithDate = record.note[0];

                    var inidAndNumber = record.note[1];

                    var regex = new Regex(@"(?<text>.+)(?<date>\d{2}\.\d{2}\.\d{4})");

                    var regex1 = new Regex(@"(?<inid>\(\d{2}\))(?<number>.+)");

                    var match = regex.Match(textWithDate);

                    var match1 = regex1.Match(inidAndNumber);

                    if (match1.Success)
                    {
                        var inid = match1.Groups["inid"].Value.Trim();

                        var number = match1.Groups["number"].Value.Trim();

                        forNote.Add(inid);

                        forNote.Add(number);
                    }

                    if (match.Success)
                    {
                        var text = match.Groups["text"].Value.Trim();

                        var date = match.Groups["date"].Value.Trim();

                        var ruCulture = new System.Globalization.CultureInfo("ru-RU");

                        var formatDate = DateTime.Parse(date, ruCulture.DateTimeFormat).ToString("yyyy.MM.dd").Replace(".","/");

                        forNote.Add(text);

                        forNote.Add(date);

                        forNote.Add(formatDate);

                    }


                    var count = forNote.Count;
                    Console.WriteLine();

                    var tmpNote = "|| "+ forNote[0] + " | " + forNote[1] + "\n" + "|| "+ forNote[2] + " | " + forNote[4] ;
                    
                    var tmpTranlation = "|| " + forNote[0] + " | Registration number " + forNote[1] + "\n" + "|| Date of filing of the translation of the specification | "  + forNote[4]; ;

                   
                    if (!string.IsNullOrEmpty(tmpNote))
                    {
                        legalEvent.LegalEvent = new LegalEvent
                        {
                            Note = tmpNote,
                            
                            Language = "ET",
                            Translations = new List<NoteTranslation>
                            {
                                new NoteTranslation 
                                {
                                    Language = "EN",
                                    Tr = tmpTranlation,
                                    Type = "INID"
                                }
                            }
                        };
                    }
                }

                legalEvent.Biblio = biblioData;
                legalEvents.Add(legalEvent);

            }

            return legalEvents;
        }
    }
}
