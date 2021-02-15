using Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Diamond_LV
{
    class DiamondConverter
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub4Convertor(List<Patent> patents)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();

            var id = 1;

            foreach (var record in patents)
            {
                Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();

                legalEvent.GazetteName = record.newspaperName;

                legalEvent.SubCode = "4";

                legalEvent.SectionCode = "FG";

                legalEvent.CountryCode = "LV";

                legalEvent.Id = id++;

                Biblio biblioData = new Biblio();

                if(record.i11 != null)
                {
                    biblioData.Publication.Number = record.i11;
                }

                if(record.i13 != null)
                {
                    biblioData.Publication.Kind = record.i13;
                }

                if(record.i43 != null)
                {
                    biblioData.Publication.Date = record.i43;
                }

                if(record.i51 != null)
                {
                    biblioData.Ipcs = new List<Ipc>();

                    foreach (var item in record.i51)
                    {
                        biblioData.Ipcs.Add(new Ipc
                        {
                            Class = item.Item1,
                            Date = item.Item2
                        });
                    }
                }

                if(record.i21 != null)
                {
                    biblioData.Application.Number = record.i21;
                }

                if(record.i22 != null)
                {
                    biblioData.Application.Date = record.i22;
                }

                if ( record.i45 != null)
                {
                    biblioData.DOfPublication = new DOfPublication { 
                   date_45 = record.i45
                    };
                }

                if (record.i31 != null || record.i32 !=null || record.i33 !=null)
                {
                    biblioData.Priorities = new List<Priority>();

                    for (int i = 0; i < record.i31.Count; i++)
                    {
                        biblioData.Priorities.Add(new Priority
                        {
                            Number = record.i31[i],
                            Date = record.i32[i],
                            Country = record.i33[i]
                        });
                    }
                }
                IntConvention intConvention = new IntConvention();

                if (record.i86PCTappDate != null || record.i86PCTappNumber!= null || record.i87PCTpubDate != null || record.i87PCTpubNumber != null)
                {
                    intConvention.PctApplNumber = record.i86PCTappNumber;
                    intConvention.PctApplDate = record.i86PCTappDate;
                    intConvention.PctPublDate = record.i87PCTpubDate;
                    intConvention.PctPublNumber = record.i87PCTpubNumber;
                }

                if (intConvention != null)
                {
                    biblioData.IntConvention = intConvention;
                }

                if (record.note != null)
                {
           
                    Regex regex = new Regex(@"(?<group1>.+)(?<date>\d{4}.\d{2}.\d{2})\s(?<group3>.+)");

                    Match match = regex.Match(record.note);

                        string tmpTranlation = "|| (45) | " + match.Groups["date"].Value.Trim() + " (publication after opposition)";

                    legalEvent.LegalEvent = new LegalEvent
                    {
                        Note = record.note,
                        Language = "LV",
                        Translations = new List<NoteTranslation> {
                            new NoteTranslation {Language = "EN", Tr = tmpTranlation, Type = "note" }
                            }
                    };
                }

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

                if(record.i72 != null)
                {
                    biblioData.Inventors = new List<PartyMember>();

                    foreach (var item in record.i72)
                    {
                        biblioData.Inventors.Add(new PartyMember
                        {
                            Name = item.name,
                            Country = item.country
                        });
                    }
                }

                if(record.i74 != null)
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

                if(record.i62 != null)
                {
                    biblioData.Related = new List<RelatedDocument>();

                    foreach (var item in record.i62)
                    {
                        biblioData.Related.Add(new RelatedDocument
                        {
                            Number = item,
                            Inid = "62"
                        });
                    }
                }

                if (record.i54 != null)
                {
                    biblioData.Titles = new List<Title>();

                    for (int i = 0; i < record.i54.Count; i++)
                    {
                        if (i == 0)
                        {
                            string latvianText = record.i54[i];

                            biblioData.Titles.Add(new Title
                            {
                                Text = latvianText,
                                Language = "LV"

                            });
                        }
                        else
                        {
                            string englishText = record.i54[i];

                            biblioData.Titles.Add(new Title
                            {
                                Text = englishText,
                                Language = "EN"

                            });
                        }
                    }
                }

                if (record.i57 != null)
                {
                    Abstract abst = new Abstract()
                    {
                        Text = record.i57,
                        Language = "LV"
                    };
                    biblioData.Abstracts.Add(abst);
                }


                legalEvent.Biblio = biblioData;
                legalEvents.Add(legalEvent);

            }

            return legalEvents;

        }
    }
}
