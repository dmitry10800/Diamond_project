using System;
using System.Collections.Generic;
using System.Text;
using Integration;

namespace Diamond_VN
{
    class DiamondConverter
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub12Convert(List<Applications> elements)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 1;
            foreach (var rec in elements)
            {
                Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                legalEvent.GazetteName = VN_main.currentFileName.Replace(".tetml", ".pdf");
                legalEvent.SubCode = "12";
                legalEvent.SectionCode = "AA";
                legalEvent.CountryCode = "VN";
                legalEvent.Id = id++;
                Biblio biblio = new Biblio();
                biblio.Priorities = new List<Integration.Priority>();
                biblio.Ipcs = new List<Ipc>();
                biblio.Agents = new List<Integration.PartyMember>();
                biblio.Applicants = new List<Integration.PartyMember>();
                biblio.Inventors = new List<Integration.PartyMember>();
                biblio.InvOrApps = new List<Integration.PartyMember>();

                biblio.DOfPublication = new DOfPublication
                {
                    date_43 = rec.Date43
                };

                biblio.Publication = new Publication
                {
                    Number = rec.PubNumber,
                    Date = rec.Date43
                };

                biblio.Application = new Application
                {
                    Date = rec.AppDate,
                    Number = rec.AppNumber
                };

                biblio.IntConvention = new IntConvention
                {
                    PctApplDate = rec.Pct86?.AppDate,
                    PctApplNumber = rec.Pct86?.AppNumber,
                    PctPublDate = rec.Pct87?.PubDate,
                    PctPublNumber = rec.Pct87?.PubNumber,
                    PctNationalDate = rec.Date85
                };

                if (rec.Priorities != null)
                {
                    foreach (var priority in rec?.Priorities)
                    {
                        biblio.Priorities.Add(new Integration.Priority
                        {
                            Number = priority.Number,
                            Date = priority.Date,
                            Country = priority.Country
                        });
                    }
                }

                if (rec.ClassificationIpcs != null)
                {
                    foreach (var ipc in rec.ClassificationIpcs)
                    {
                        biblio.Ipcs.Add(new Ipc
                        {
                            Edition = ipc.Edition,
                            Class = ipc.Classification
                        });
                    }
                }

                if (rec.Agents != null)
                {
                    foreach (var agent in rec.Agents)
                    {
                        if (!string.IsNullOrEmpty(agent.TransName))
                        {
                            biblio.Agents.Add(new Integration.PartyMember
                            {
                                Name = agent.Name,
                                Language = agent.Language,
                                Translations = new List<Translation>
                            {
                                new Translation
                                {
                                    Language = agent.TransCountry,
                                    TrName = agent.TransName
                                }
                            },
                                Country = agent.Country
                            });
                        }
                        else
                        {
                            biblio.Agents.Add(new Integration.PartyMember
                            {
                                Name = agent.Name,
                                Country = agent.Country,
                                Language = agent.Language
                            });
                        }

                    }
                }

                if (rec.InvOrApp != null)
                {
                    foreach (var invOrApp in rec.InvOrApp)
                    {
                        if (!string.IsNullOrEmpty(invOrApp.TransName))
                        {
                            biblio.InvOrApps.Add(new Integration.PartyMember
                            {
                                Address1 = invOrApp.Address,
                                Name = invOrApp.Name,
                                Country = invOrApp.Country,
                                Language = invOrApp.Language,
                                Translations = new List<Translation>
                                {
                                    new Translation
                                    {
                                    TrName = invOrApp.TransName,
                                    Language = invOrApp.TransCountry
                                    }
                                }
                            });
                        }
                        else
                        {
                            biblio.InvOrApps.Add(new Integration.PartyMember
                            {
                                Address1 = invOrApp.Address,
                                Name = invOrApp.Name,
                                Country = invOrApp.Country,
                                Language = invOrApp.Language
                            });
                        }

                    }
                }

                if (rec.Applicants != null)
                {
                    foreach (var applicant in rec?.Applicants)
                    {
                        if (!string.IsNullOrEmpty(applicant.TransCountry))
                        {
                            biblio.Applicants.Add(new Integration.PartyMember
                            {
                                Name = applicant.Name,
                                Address1 = applicant.Address,
                                Country = applicant.Country,
                                Language = applicant.Language,
                                Translations = new List<Translation>
                                {
                                    new Translation
                                    {
                                        Language = applicant.TransCountry,
                                        TrName = applicant.TransName
                                    }
                                }
                            });
                        }
                        else
                        {
                            biblio.Applicants.Add(new Integration.PartyMember
                            {
                                Name = applicant.Name,
                                Address1 = applicant.Address,
                                Country = applicant.Country,
                                Language = applicant.Language
                            });
                        }
                    }
                }

                if (rec.Inventors != null)
                {
                    foreach (var inventor in rec?.Inventors)
                    {
                        if (!string.IsNullOrEmpty(inventor.TransName))
                        {
                            if (!string.IsNullOrEmpty(inventor.Address))
                            {
                                biblio.Inventors.Add(new Integration.PartyMember
                                {
                                    Name = inventor.Name,
                                    Country = inventor.Country,
                                    Address1 = inventor.Address,
                                    Language = inventor.Language,
                                    Translations = new List<Translation>
                                    {
                                        new Translation
                                        {
                                            TrName = inventor.TransName,
                                            Language = inventor.TransCountry
                                        }
                                    }
                                });
                            }
                            else
                            {
                                biblio.Inventors.Add(new Integration.PartyMember
                                {
                                    Name = inventor.Name,
                                    Country = inventor.Country,
                                    Language = inventor.Language,
                                    Translations = new List<Translation>
                                    {
                                        new Translation
                                        {
                                            TrName = inventor.TransName,
                                            Language = inventor.TransCountry
                                        }
                                    }
                                });
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(inventor.Address))
                            {
                                biblio.Inventors.Add(new Integration.PartyMember
                                {
                                    Name = inventor.Name,
                                    Country = inventor.Country,
                                    Address1 = inventor.Address,
                                    Language = inventor.Language
                                });
                            }
                            else
                            {
                                biblio.Inventors.Add(new Integration.PartyMember
                                {
                                    Name = inventor.Name,
                                    Country = inventor.Country,
                                    Language = inventor.Language
                                });
                            }
                        }

                    }
                }

                biblio.Titles = new List<Title>
                {
                    new Title
                    {
                        Text = rec.Title,
                        Language = "VI"
                    }
                };

                biblio.Abstracts = new List<Integration.Abstract>
                {
                    new Integration.Abstract
                    {
                        Text = rec.Abstract.Text,
                        Language = rec.Abstract.Language
                    }
                };

                legalEvent.LegalEvent = new LegalEvent
                {
                    Note = rec.Note,
                    Language = "VI",
                    Translations = new List<NoteTranslation>
                    {
                        new NoteTranslation
                        {
                            Tr = rec.Translation,
                            Language = "EN"
                        }
                    }
                };


                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }

            return legalEvents;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub13Convert(List<Applications> elements)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 1;
            foreach (var rec in elements)
            {
                Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                legalEvent.GazetteName = VN_main.currentFileName.Replace(".tetml", ".pdf");
                legalEvent.SubCode = "13";
                legalEvent.SectionCode = "AA";
                legalEvent.CountryCode = "VN";
                legalEvent.Id = id++;
                Biblio biblio = new Biblio();
                biblio.Priorities = new List<Integration.Priority>();
                biblio.Ipcs = new List<Ipc>();
                biblio.Agents = new List<Integration.PartyMember>();
                biblio.InvOrApps = new List<Integration.PartyMember>();
                biblio.Inventors = new List<Integration.PartyMember>();
                biblio.Applicants = new List<Integration.PartyMember>();
                biblio.Publication.Number = rec.PubNumber;
                biblio.Application.Number = rec.AppNumber;
                biblio.Application.Date = rec.AppDate;
                biblio.Publication.Date = rec.Date43;

                biblio.DOfPublication = new DOfPublication
                {
                    date_43 = rec.PubDate
                };

                biblio.IntConvention = new IntConvention
                {
                    PctApplDate = rec.Pct86?.AppDate,
                    PctApplNumber = rec.Pct86?.AppNumber,
                    PctPublDate = rec.Pct87?.PubDate,
                    PctPublNumber = rec.Pct87?.PubNumber,
                    PctNationalDate = rec.Date85
                };

                if (rec.Priorities != null)
                {
                    foreach (var priority in rec?.Priorities)
                    {
                        biblio.Priorities.Add(new Integration.Priority
                        {
                            Number = priority.Number,
                            Date = priority.Date,
                            Country = priority.Country
                        });
                    }
                }

                if (rec.ClassificationIpcs != null)
                {
                    foreach (var ipc in rec.ClassificationIpcs)
                    {
                        biblio.Ipcs.Add(new Ipc
                        {
                            Edition = ipc.Edition,
                            Class = ipc.Classification
                        });
                    }
                }

                if (rec.Agents != null)
                {
                    foreach (var agent in rec.Agents)
                    {
                        if (!string.IsNullOrEmpty(agent.TransCountry))
                        {
                            biblio.Agents.Add(new Integration.PartyMember
                            {
                                Name = agent.Name,
                                Language = agent.Language,
                                Country = agent.Country,
                                Translations = new List<Translation>
                                {
                                    new Translation
                                    {
                                        Language = agent.TransCountry,
                                        TrName = agent.TransName
                                    }
                                }
                            });
                        }
                        else
                        {
                            biblio.Agents.Add(new Integration.PartyMember
                            {
                                Name = agent.Name,
                                Language = agent.Language,
                                Country = agent.Country
                            });
                        }
                    }
                }

                if (rec.Inventors != null)
                {
                    foreach (var inventor in rec.Inventors)
                    {
                        if (!string.IsNullOrEmpty(inventor.TransName))
                        {
                            biblio.Inventors.Add(new Integration.PartyMember
                            {
                                Name = inventor.Name,
                                Country = inventor.Country,
                                Language = inventor.Language,
                                Translations = new List<Translation>
                                {
                                    new Translation
                                    {
                                        TrName = inventor.TransName,
                                        Language = inventor.TransCountry
                                    }
                                }
                            });
                        }
                        else
                        {
                            biblio.Inventors.Add(new Integration.PartyMember
                            {
                                Name = inventor.Name,
                                Country = inventor.Country,
                                Language = inventor.Language
                            });
                        }
                    }
                }

                if (rec.Applicants != null)
                {
                    foreach (var applicant in rec.Applicants)
                    {
                        if (!string.IsNullOrEmpty(applicant.TransCountry))
                        {
                            biblio.Applicants.Add(new Integration.PartyMember
                            {
                                Name = applicant.Name,
                                Address1 = applicant.Address,
                                Country = applicant.Country,
                                Language = applicant.Language,
                                Translations = new List<Translation>
                                {
                                    new Translation
                                    {
                                        Language = applicant.TransCountry,
                                        TrName = applicant.TransName
                                    }
                                }
                            });
                        }
                        else
                        {
                            biblio.Applicants.Add(new Integration.PartyMember
                            {
                                Name = applicant.Name,
                                Address1 = applicant.Address,
                                Country = applicant.Country,
                                Language = applicant.Language
                            });
                        }
                    }
                }

                if (rec.InvOrApp != null)
                {
                    foreach (var applicant in rec?.InvOrApp)
                    {
                        if (!string.IsNullOrEmpty(applicant.TransName))
                        {
                            biblio.InvOrApps.Add(new Integration.PartyMember
                            {
                                Name = applicant.Name,
                                Address1 = applicant.Address,
                                Country = applicant.Country,
                                Language = applicant.Language,
                                Translations = new List<Translation>
                                {
                                    new Translation
                                    {
                                        Language = applicant.TransCountry,
                                        TrName = applicant.TransName
                                    }
                                }
                            });
                        }
                        else
                        {
                            biblio.InvOrApps.Add(new Integration.PartyMember
                            {
                                Name = applicant.Name,
                                Address1 = applicant.Address,
                                Country = applicant.Country,
                                Language = applicant.Language
                            });
                        }
                    }
                }

                biblio.Titles = new List<Title>
                {
                    new Title
                    {
                        Text = rec.Title,
                        Language = "VI"
                    }
                };

                biblio.Abstracts = new List<Integration.Abstract>
                {
                    new Integration.Abstract
                    {
                        Text = rec.Abstract.Text,
                        Language = rec.Abstract.Language
                    }
                };

                legalEvent.LegalEvent = new LegalEvent
                {
                    Note = rec.Note,
                    Language = "VI",
                    Translations = new List<NoteTranslation>
                    {
                        new NoteTranslation
                        {
                            Tr = rec.Translation,
                            Language = "EN"
                        }
                    }
                };

                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }

            return legalEvents;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub14Convert(List<Applications> elements)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 1;
            foreach (var rec in elements)
            {
                Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                legalEvent.GazetteName = VN_main.currentFileName.Replace(".tetml", ".pdf");
                legalEvent.SubCode = "14";
                legalEvent.SectionCode = "FG";
                legalEvent.CountryCode = "VN";
                legalEvent.Id = id++;
                Biblio biblio = new Biblio();
                biblio.Priorities = new List<Integration.Priority>();
                biblio.Ipcs = new List<Ipc>();
                biblio.Agents = new List<Integration.PartyMember>();
                biblio.InvAppGrants = new List<Integration.PartyMember>();
                biblio.Inventors = new List<Integration.PartyMember>();
                biblio.Assignees = new List<Integration.PartyMember>();
                biblio.Related = new List<RelatedDocument>();

                biblio.IntConvention = new IntConvention
                {
                    PctApplDate = rec.Pct86?.AppDate,
                    PctApplNumber = rec.Pct86?.AppNumber,
                    PctPublDate = rec.Pct87?.PubDate,
                    PctPublNumber = rec.Pct87?.PubNumber,
                    PctNationalDate = rec.Date85
                };

                biblio.Application = new Application
                {
                    Number = rec.AppNumber,
                    Date = rec.AppDate
                };


                biblio.Publication = new Publication
                {
                    Number = rec.PubNumber,
                    Date = rec.PubDate
                };

                biblio.DOfPublication = new DOfPublication
                {
                    date_43 = rec.Date43,
                    date_45 = rec.Date45
                };

                if (rec.GrantAssigOwner != null)
                {
                    foreach (var assignee in rec.GrantAssigOwner)
                    {
                        if (!string.IsNullOrEmpty(assignee.TransName))
                        {
                            biblio.Assignees.Add(new Integration.PartyMember
                            {
                                Address1 = assignee.Address,
                                Name = assignee.Name,
                                Country = assignee.Country,
                                Language = assignee.Language,
                                Translations = new List<Translation>
                                {
                                    new Translation
                                    {
                                        TrName = assignee.TransName,
                                        Language = assignee.TransCountry
                                    }
                                }
                            });
                        }
                        else
                        {
                            biblio.Assignees.Add(new Integration.PartyMember
                            {
                                Address1 = assignee.Address,
                                Name = assignee.Name,
                                Country = assignee.Country,
                                Language = assignee.Language
                            });
                        }
                    }
                }

                if (rec.Priorities != null)
                {
                    foreach (var priority in rec?.Priorities)
                    {
                        biblio.Priorities.Add(new Integration.Priority
                        {
                            Number = priority.Number,
                            Date = priority.Date,
                            Country = priority.Country
                        });
                    }
                }

                if (rec.ClassificationIpcs != null)
                {
                    foreach (var ipc in rec.ClassificationIpcs)
                    {
                        biblio.Ipcs.Add(new Ipc
                        {
                            Edition = ipc.Edition,
                            Class = ipc.Classification
                        });
                    }
                }

                if (rec.Agents != null)
                {
                    foreach (var agent in rec.Agents)
                    {
                        if (!string.IsNullOrEmpty(agent.TransCountry))
                        {
                            biblio.Agents.Add(new Integration.PartyMember
                            {
                                Name = agent.Name,
                                Language = agent.Language,
                                Country = agent.Country,
                                Translations = new List<Translation>
                                {
                                    new Translation
                                    {
                                        Language = agent.TransCountry,
                                        TrName = agent.TransName
                                    }
                                }
                            });
                        }
                        else
                        {
                            biblio.Agents.Add(new Integration.PartyMember
                            {
                                Name = agent.Name,
                                Language = agent.Language,
                                Country = agent.Country
                            });
                        }
                    }
                }

                if (rec.Related != null)
                {
                    foreach (var relate in rec.Related)
                    {
                        biblio.Related.Add(new RelatedDocument
                        {
                            Number = relate.Number,
                            Type = relate.Inid
                        });
                    }
                }

                if (rec.InvAppGrant != null)
                {
                    foreach (var applicant in rec?.InvAppGrant)
                    {
                        if (!string.IsNullOrEmpty(applicant.TransName))
                        {
                            biblio.InvAppGrants.Add(new Integration.PartyMember
                            {
                                Address1 = applicant.Address,
                                Name = applicant.Name,
                                Country = applicant.Country,
                                Language = applicant.Language,
                                Translations = new List<Translation>
                                {
                                    new Translation
                                    {
                                        TrName = applicant.TransName,
                                        Language = applicant.TransCountry
                                    }
                                }
                            });
                        }
                        else
                        {
                            biblio.InvAppGrants.Add(new Integration.PartyMember
                            {
                                Address1 = applicant.Address,
                                Name = applicant.Name,
                                Country = applicant.Country,
                                Language = applicant.Language
                            });
                        }
                    }
                }
                else
                {
                    if (rec.Inventors != null)
                    {
                        foreach (var inventor in rec?.Inventors)
                        {
                            if (!string.IsNullOrEmpty(inventor.TransName))
                            {
                                if (!string.IsNullOrEmpty(inventor.Address))
                                {
                                    biblio.Inventors.Add(new Integration.PartyMember
                                    {
                                        Name = inventor.Name,
                                        Country = inventor.Country,
                                        Address1 = inventor.Address,
                                        Language = inventor.Language,
                                        Translations = new List<Translation>
                                    {
                                        new Translation
                                        {
                                            TrName = inventor.TransName,
                                            Language = inventor.TransCountry
                                        }
                                    }
                                    });
                                }
                                else
                                {
                                    biblio.Inventors.Add(new Integration.PartyMember
                                    {
                                        Name = inventor.Name,
                                        Country = inventor.Country,
                                        Language = inventor.Language,
                                        Translations = new List<Translation>
                                    {
                                        new Translation
                                        {
                                            TrName = inventor.TransName,
                                            Language = inventor.TransCountry
                                        }
                                    }
                                    });
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(inventor.Address))
                                {
                                    biblio.Inventors.Add(new Integration.PartyMember
                                    {
                                        Name = inventor.Name,
                                        Country = inventor.Country,
                                        Address1 = inventor.Address,
                                        Language = inventor.Language
                                    });
                                }
                                else
                                {
                                    biblio.Inventors.Add(new Integration.PartyMember
                                    {
                                        Name = inventor.Name,
                                        Country = inventor.Country,
                                        Language = inventor.Language
                                    });
                                }
                            }
                        }
                    }
                }


                biblio.Titles = new List<Title>
                {
                    new Title
                    {
                        Text = rec.Title,
                        Language = "VI"
                    }
                };

                biblio.Abstracts = new List<Integration.Abstract>
                {
                    new Integration.Abstract
                    {
                        Text = rec.Abstract.Text,
                        Language = rec.Abstract.Language
                    }
                };

                legalEvent.LegalEvent = new LegalEvent
                {
                    Note = rec.Note.Trim(),
                    Language = "VI",
                    Translations = new List<NoteTranslation>
                    {
                        new NoteTranslation
                        {
                            Tr = rec.Translation,
                            Language = "EN"
                        }
                    }
                };


                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }

            return legalEvents;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub15Convert(List<Applications> elements)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 1;
            foreach (var rec in elements)
            {
                Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                legalEvent.GazetteName = VN_main.currentFileName.Replace(".tetml", ".pdf");
                legalEvent.SubCode = "15";
                legalEvent.SectionCode = "FG";
                legalEvent.CountryCode = "VN";
                legalEvent.Id = id++;
                Biblio biblio = new Biblio();
                biblio.Priorities = new List<Integration.Priority>();
                biblio.Ipcs = new List<Ipc>();
                biblio.Agents = new List<Integration.PartyMember>();
                biblio.InvAppGrants = new List<Integration.PartyMember>();
                biblio.Inventors = new List<Integration.PartyMember>();
                biblio.Assignees = new List<Integration.PartyMember>();
                biblio.Related = new List<RelatedDocument>();

                biblio.Application = new Application
                {
                    Number = rec.AppNumber,
                    Date = rec.AppDate
                };

                biblio.Application = new Application
                {
                    Number = rec.AppNumber,
                    Date = rec.AppDate
                };

                biblio.Publication = new Publication
                {
                    Number = rec.PubNumber,
                    Date = rec.Date43
                };

                if (biblio.Publication.Number.Contains("2-0002288"))
                {

                }

                biblio.DOfPublication = new DOfPublication
                {
                    date_43 = rec.Date43,
                    date_45 = rec.Date45
                };


                biblio.IntConvention = new IntConvention
                {
                    PctApplDate = rec.Pct86?.AppDate,
                    PctApplNumber = rec.Pct86?.AppNumber,
                    PctPublDate = rec.Pct87?.PubDate,
                    PctPublNumber = rec.Pct87?.PubNumber
                };

                if (rec.Priorities != null)
                {
                    foreach (var priority in rec?.Priorities)
                    {
                        biblio.Priorities.Add(new Integration.Priority
                        {
                            Number = priority.Number,
                            Date = priority.Date,
                            Country = priority.Country
                        });
                    }
                }

                if (rec.ClassificationIpcs != null)
                {
                    foreach (var ipc in rec.ClassificationIpcs)
                    {
                        biblio.Ipcs.Add(new Ipc
                        {
                            Edition = ipc.Edition,
                            Class = ipc.Classification
                        });
                    }
                }

                if (rec.Agents != null)
                {
                    foreach (var agent in rec.Agents)
                    {
                        biblio.Agents.Add(new Integration.PartyMember
                        {
                            Name = agent.Name,
                            Language = agent.Language,
                            Translations = new List<Translation>
                            {
                                new Translation
                                {
                                    Language = agent.TransCountry,
                                    TrName = agent.TransName
                                }
                            }
                        });
                    }
                }

                if (rec.Related != null)
                {
                    foreach (var relate in rec.Related)
                    {
                        biblio.Related.Add(new RelatedDocument
                        {
                            Number = relate.Number,
                            Source = relate.Inid
                        });
                    }
                }

                if (rec.InvAppGrant != null)
                {
                    foreach (var applicant in rec?.InvAppGrant)
                    {
                        if (!string.IsNullOrEmpty(applicant.TransCountry))
                        {
                            biblio.InvAppGrants.Add(new Integration.PartyMember
                            {
                                Address1 = applicant.Address,
                                Name = applicant.Name,
                                Country = applicant.Country,
                                Language = applicant.Language,
                                Translations = new List<Translation>
                                {
                                    new Translation
                                    {
                                        TrName = applicant.TransName,
                                        Language = applicant.TransCountry
                                    }
                                }
                            });
                        }
                        else
                        {
                            biblio.InvAppGrants.Add(new Integration.PartyMember
                            {
                                Address1 = applicant.Address,
                                Name = applicant.Name,
                                Country = applicant.Country,
                                Language = applicant.Language
                            });
                        }
                    }
                }

                if (rec.Inventors != null)
                {
                    foreach (var inventor in rec?.Inventors)
                    {
                        if (!string.IsNullOrEmpty(inventor.Address))
                        {
                            if (!string.IsNullOrEmpty(inventor.TransCountry))
                            {
                                biblio.Inventors.Add(new Integration.PartyMember
                                {
                                    Name = inventor.Name,
                                    Country = inventor.Country,
                                    Address1 = inventor.Address,
                                    Language = inventor.Language,
                                    Translations = new List<Translation>
                                    {
                                        new Translation
                                        {
                                            TrName = inventor.TransName,
                                            Language = inventor.TransCountry
                                        }
                                    }
                                });
                            }
                            else
                            {
                                biblio.Inventors.Add(new Integration.PartyMember
                                {
                                    Name = inventor.Name,
                                    Country = inventor.Country,
                                    Address1 = inventor.Address,
                                    Language = inventor.Language
                                });
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(inventor.TransCountry))
                            {
                                biblio.Inventors.Add(new Integration.PartyMember
                                {
                                    Name = inventor.Name,
                                    Country = inventor.Country,
                                    Language = inventor.Language,
                                    Translations = new List<Translation>
                                    {
                                        new Translation
                                        {
                                            TrName = inventor.TransName,
                                            Language = inventor.TransCountry
                                        }
                                    }
                                });
                            }
                            else
                            {
                                biblio.Inventors.Add(new Integration.PartyMember
                                {
                                    Name = inventor.Name,
                                    Country = inventor.Country,
                                    Language = inventor.Language
                                });
                            }
                        }
                    }
                }

                if (rec.GrantAssigOwner != null)
                {
                    foreach (var assignee in rec.GrantAssigOwner)
                    {
                        if (!string.IsNullOrEmpty(assignee.TransCountry))
                        {
                            biblio.Assignees.Add(new Integration.PartyMember
                            {
                                Address1 = assignee.Address,
                                Name = assignee.Name,
                                Country = assignee.Country,
                                Language = assignee.Language,
                                Translations = new List<Translation>
                                {
                                    new Translation
                                    {
                                        TrName = assignee.TransName,
                                        Language = assignee.TransCountry
                                    }
                                }
                            });
                        }
                        else
                        {
                            biblio.Assignees.Add(new Integration.PartyMember
                            {
                                Address1 = assignee.Address,
                                Name = assignee.Name,
                                Country = assignee.Country,
                                Language = assignee.Language
                            });
                        }
                    }
                }

                biblio.Titles = new List<Title>
                {
                    new Title
                    {
                        Text = rec.Title,
                        Language = "VI"
                    }
                };

                biblio.Abstracts = new List<Integration.Abstract>
                {
                    new Integration.Abstract
                    {
                        Text = rec.Abstract.Text,
                        Language = rec.Abstract.Language
                    }
                };

                legalEvent.LegalEvent = new LegalEvent
                {
                    Note = rec.Note.Replace("(15)", "").Trim(),
                    Language = "VI",
                    Translations = new List<NoteTranslation>
                    {
                        new NoteTranslation
                        {
                            Tr = rec.Translation,
                            Language = "EN"
                        }
                    }
                };


                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }

            return legalEvents;
        }
    }
}
