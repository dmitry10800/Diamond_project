using System.Collections.Generic;
using Integration;

namespace Diamond_RS
{
    class DiamondConverter
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub3Convert(List<Elements> elements)
        {
            var legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 1;
            foreach (var rec in elements)
            {
                var legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                legalEvent.GazetteName = RS_main.currentFileName.Replace(".tetml", ".pdf");
                legalEvent.SubCode = "3";
                legalEvent.SectionCode = "FG";
                legalEvent.CountryCode = "RS";
                legalEvent.Id = id++;
                var biblio = new Biblio();
                biblio.Ipcs = new List<Ipc>();
                biblio.Assignees = new List<Integration.PartyMember>();
                biblio.Inventors = new List<Integration.PartyMember>();
                biblio.Agents = new List<Integration.PartyMember>();

                biblio.Application = new Application
                {
                    Number = rec.AppNumber,
                    Date = rec.AppDate
                };

                biblio.Publication = new Integration.Publication
                {
                    Number = rec.Publication.PubNumber,
                    Kind = rec.Publication.Kind
                };

                biblio.Priorities = new List<Integration.Priority>
                {
                    new Integration.Priority
                    {
                        Number = rec.Priority.Number,
                        Country = rec.Priority.Country,
                        Date = rec.Priority.Date
                    }
                };

                biblio.IntConvention = new IntConvention
                {
                    PctApplDate = rec.Pct.AppDate,
                    PctApplNumber = rec.Pct.AppNumber,
                    PctPublDate = rec.Pct.PubDate,
                    PctPublNumber = rec.Pct.PubNumber
                };

                if (rec.Ipcs != null)
                {
                    foreach (var ipc in rec.Ipcs)
                    {
                        biblio.Ipcs.Add(new Ipc
                        {
                            Class = ipc.Classification
                        });
                    }
                }

                biblio.EuropeanPatents = new List<EuropeanPatent>
                {
                    new EuropeanPatent
                    {
                        AppNumber = rec.EuropeanPatents.AppNumber,
                        AppDate = rec.EuropeanPatents.AppDate,
                        PubNumber = rec.EuropeanPatents.PubNumber,
                        PubDate = rec.EuropeanPatents.PubDate
                    }
                };

                biblio.Titles = new List<Integration.Title>
                {
                    new Integration.Title
                    {
                        Text = rec.Title.Text,
                        Language = rec.Title.Language
                    }
                };

                if (rec.GranteeAssigneeOwner != null)
                {
                    foreach (var assignee in rec.GranteeAssigneeOwner)
                    {
                        biblio.Assignees.Add(new Integration.PartyMember
                        {
                            Name = assignee.Name,
                            Address1 = assignee.Address,
                            Country = assignee.Country
                        });
                    }
                }

                if (rec.Inventors != null)
                {
                    foreach (var inventor in rec.Inventors)
                    {
                        biblio.Inventors.Add(new Integration.PartyMember
                        {
                            Name = inventor.Name,
                            Address1 = inventor.Address,
                            Country = inventor.Country
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
                            Address1 = agent.Address
                        });
                    }
                }

                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }

            return legalEvents;
        }
    }
}
