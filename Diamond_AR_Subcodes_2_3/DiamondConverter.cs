using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using Integration;

namespace Diamond_AR_Subcodes_2_3
{
    class DiamondConverter
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub2And3(List<Elements> elements)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 1;
            var gazetteName = AR_main.currentFileName.Replace(".tetml", ".pdf");
            var gazetteDate = gazetteName.Split('_').Skip(1).First();
            gazetteDate = gazetteDate.Substring(0, 4) + "-" + gazetteDate.Substring(4, 2) + "-" + gazetteDate.Substring(6);
            foreach (var rec in elements)
            {
                if (rec.Is2 == true)
                {
                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = AR_main.currentFileName.Replace(".tetml", ".pdf");
                    legalEvent.SubCode = "2";
                    legalEvent.SectionCode = "FG";
                    legalEvent.CountryCode = "AR";
                    legalEvent.Id = id++;
                    Biblio biblio = new Biblio();
                    biblio.Priorities = new List<Integration.Priority>();
                    biblio.Ipcs = new List<Integration.Ipc>();
                    biblio.Agents = new List<PartyMember>();
                    biblio.Inventors = new List<PartyMember>();

                    biblio.Publication = new Publication
                    {
                        Number = rec.PubNumber,
                        Kind = rec.Kind,
                        LanguageDesignation = rec.PlainLang
                    };

                    if (rec.Date45 == null && rec.Date47 == null)
                        rec.Date45 = gazetteDate;

                    biblio.DOfPublication = new DOfPublication
                    {
                        date_45 = rec.Date45,
                        date_47 = rec.Date47
                    };

                    biblio.Application = new Application
                    {
                        Number = rec.AppNumber,
                        Date = rec.AppDate,
                        EffectiveDate = rec.EffectiveDate
                    };

                    legalEvent.LegalEvent = new LegalEvent
                    {
                        Note = rec.Note,
                        Language = "ES",
                        Translations = new List<NoteTranslation>
                        {
                            new NoteTranslation
                            {
                                Tr = rec.Tranlation,
                                Language = "EN",
                                Type = "INID"
                            }
                        }
                    };


                    if (rec.Priorities != null)
                    {
                        foreach (var priority in rec.Priorities)
                        {
                            biblio.Priorities.Add(new Integration.Priority
                            {
                                Country = priority.Country,
                                Number = priority.Number,
                                Date = priority.Date
                            });
                        }
                    }

                    if (rec.Ipcs != null)
                    {
                        foreach (var ipc in rec.Ipcs)
                        {
                            biblio.Ipcs.Add(new Integration.Ipc
                            {
                                Class = ipc.Classification,
                                Date = rec.IpcVersion
                            });
                        }
                    }

                    biblio.Titles = new List<Title>
                    {
                        new Title
                        {
                            Text = rec.Title,
                            Language = "ES"
                        }
                    };

                    biblio.Abstracts = new List<Abstract>
                    {
                        new Abstract
                        {
                            Language = "ES",
                            Text = rec.Abstract
                        }
                    };


                    if (rec.Agents != null)
                    {
                        foreach (var agent in rec.Agents)
                        {
                            biblio.Agents.Add(new PartyMember
                            {
                                Name = agent.Name
                            });
                        }
                    }

                    if (rec.Inventors != null)
                    {
                        foreach (var inventor in rec.Inventors)
                        {
                            biblio.Inventors.Add(new PartyMember
                            {
                                Name = inventor.Name
                            });
                        }
                    }

                    legalEvent.Biblio = biblio;
                    legalEvents.Add(legalEvent);
                }
                else
                {
                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = AR_main.currentFileName.Replace(".tetml", ".pdf");
                    legalEvent.SubCode = "3";
                    legalEvent.SectionCode = "FG";
                    legalEvent.CountryCode = "AR";
                    legalEvent.Id = id++;
                    Biblio biblio = new Biblio();
                    biblio.Priorities = new List<Integration.Priority>();
                    biblio.Ipcs = new List<Integration.Ipc>();
                    biblio.Agents = new List<PartyMember>();
                    biblio.Inventors = new List<PartyMember>();

                    legalEvent.LegalEvent = new LegalEvent
                    {
                        Note = rec.Note,
                        Language = "ES",
                        Translations = new List<NoteTranslation>
                        {
                            new NoteTranslation
                            {
                                Language = "EN",
                                Tr = rec.Tranlation,
                                Type = "INID"
                            }
                        }
                    };

                    biblio.Publication = new Publication
                    {
                        Number = rec.PubNumber,
                        Kind = rec.Kind,
                        LanguageDesignation = rec.PlainLang
                    };

                    biblio.Application = new Application
                    {
                        Number = rec.AppNumber,
                        Date = rec.AppDate,
                        EffectiveDate = rec.EffectiveDate
                    };

                    if (rec?.Priorities != null)
                    {
                        foreach (var priority in rec.Priorities)
                        {
                            biblio.Priorities.Add(new Integration.Priority
                            {
                                Country = priority.Country,
                                Number = priority.Number,
                                Date = priority.Date
                            });
                        }
                    }

                    if (rec.Date45 == null && rec.Date47 == null)
                        rec.Date45 = gazetteDate;

                    biblio.DOfPublication = new DOfPublication
                    {
                        date_47 = rec.Date47,
                        date_45 = rec.Date45
                    };

                    if (rec?.Ipcs != null)
                    {
                        foreach (var ipc in rec.Ipcs)
                        {
                            biblio.Ipcs.Add(new Integration.Ipc
                            {
                                Class = ipc.Classification,
                                Date = rec.IpcVersion
                            });
                        }
                    }

                    biblio.Titles = new List<Title>
                    {
                        new Title
                        {
                            Text = rec.Title,
                            Language = "ES"
                        }
                    };

                    biblio.Abstracts = new List<Abstract>
                    {
                        new Abstract
                        {
                            Text = rec.Abstract,
                            Language = "ES"
                        }
                    };

                    if (rec.Agents != null)
                    {
                        foreach (var agent in rec.Agents)
                        {
                            biblio.Agents.Add(new PartyMember
                            {
                                Name = agent.Name
                            });
                        }
                    }

                    if (rec.Inventors != null)
                    {
                        foreach (var inventor in rec.Inventors)
                        {
                            biblio.Inventors.Add(new PartyMember
                            {
                                Name = inventor.Name
                            });
                        }
                    }

                    legalEvent.Biblio = biblio;
                    legalEvents.Add(legalEvent);
                }

            }

            return legalEvents;
        }
    }
}
