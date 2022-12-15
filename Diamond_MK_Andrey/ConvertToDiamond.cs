using System.Collections.Generic;
using System.IO;
using Integration;

namespace Diamond_MK_Andrey
{
    class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> ConvertSub3(List<Subcodes.ElementsOut> elements)
        {
            List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();

            var id = 1;

            foreach (var rec in elements)
            {
                Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();

                legalEvent.GazetteName = Path.GetFileName(MK_Processing._currentFileName.Replace(".tetml", ".pdf"));
                legalEvent.SubCode = "3";
                legalEvent.SectionCode = "FG";
                legalEvent.CountryCode = "MK";
                legalEvent.Id = id++;
                Biblio biblio = new Biblio();
                biblio.Assignees = new List<PartyMember>();
                biblio.Priorities = new List<Priority>();
                biblio.Inventors = new List<PartyMember>();
                biblio.Ipcs = new List<Ipc>();
                biblio.Agents = new List<PartyMember>();

                biblio.Publication = new Publication
                {
                    Number = rec.PubNumber,
                    Kind = rec.Kind
                };

                biblio.Application = new Application
                {
                    Number = rec.AppNumber,
                    Date = rec.AppDate
                };

                biblio.DOfPublication = new DOfPublication
                {
                    date_45 = rec.Date
                };

                if (rec.Priorities != null && rec.Priorities.Count > 0)
                {
                    foreach (var priority in rec.Priorities)
                    {
                        biblio.Priorities.Add(new Priority
                        {
                            Country = priority.Country,
                            Number = priority.Number,
                            Date = priority.Date
                        });
                    }
                }

                if (rec.Assignees != null && rec.Assignees.Count > 0)
                {
                    foreach (var assignee in rec.Assignees)
                    {
                        biblio.Assignees.Add(new PartyMember
                        {
                            Name = assignee.Name,
                            Address1 = assignee.Address,
                            Country = assignee.Country
                        });
                    }
                }

                if (rec.Agent != null)
                {
                    biblio.Agents.Add(new PartyMember
                    {
                        Name = rec.Agent.Name,
                        Address1 = rec.Agent.Address,
                        Country = rec.Agent.Country
                    });
                }

                if (rec.Inventors != null && rec.Inventors.Count > 0)
                {
                    foreach (var inventor in rec.Inventors)
                    {
                        biblio.Inventors.Add(new PartyMember
                        {
                            Name = inventor.Name
                        });
                    }
                }

                biblio.Titles = new List<Title>
                {
                    new Title
                    {
                        Text = rec.Title,
                        Language = "MK"
                    }
                };

                biblio.Abstracts = new List<Abstract>
                {
                    new Abstract
                    {
                        Text = rec.Abstract,
                        Language = "MK"
                    }
                };

                if (rec.Ipc != null && rec.Ipc.Count > 0)
                {
                    foreach (var ipc in rec.Ipc)
                    {
                        biblio.Ipcs.Add(new Ipc
                        {
                            Class = ipc
                        });
                    }
                }

                biblio.EuropeanPatents = new List<EuropeanPatent>
                {
                    new EuropeanPatent
                    {
                        AppDate = rec.Related96.AppDate,
                        AppNumber = rec.Related96.AppNumber,
                        PubDate = rec.Related97.PubDate,
                        PubNumber = rec.Related97.PubNumber
                    }
                };

                legalEvent.Biblio = biblio;
                fullGazetteInfo.Add(legalEvent);
            }

            return fullGazetteInfo;
        }
    }
}
