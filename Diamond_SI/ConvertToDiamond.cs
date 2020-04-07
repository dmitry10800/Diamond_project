using Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diamond_SI
{
    public class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub3Convert(List<Subcode3> elements, string gazetteName)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 0;
            foreach (var elem in elements)
            {
                Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                Biblio biblio = new Biblio();
                legalEvent.LegalEvent = new LegalEvent();
                biblio.DOfPublication = new DOfPublication();
                legalEvent.SubCode = "3";
                legalEvent.SectionCode = "MK";
                legalEvent.CountryCode = "SI";
                legalEvent.Id = id++;
                biblio.Publication.Number = elem.PublicationNumber;
                legalEvent.GazetteName = gazetteName;
                legalEvent.LegalEvent.Date = elem.LegalEventDate;
                biblio.DOfPublication.date_45 = elem.DateField45;
                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }
            return legalEvents;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub4Convert(List<Subcode4> elements, string gazetteName)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 0;
            foreach (var elem in elements)
            {
                Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                Biblio biblio = new Biblio();
                legalEvent.LegalEvent = new LegalEvent();
                biblio.DOfPublication = new DOfPublication();
                legalEvent.SubCode = "4";
                legalEvent.SectionCode = "MK";
                legalEvent.CountryCode = "SI";
                legalEvent.Id = id++;
                biblio.Publication.Number = elem.PublicationNumber;
                legalEvent.GazetteName = gazetteName;
                legalEvent.LegalEvent.Date = elem.LegalEventDate;
                biblio.DOfPublication.date_46 = elem.DateField46;
                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }
            return legalEvents;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub20Convert(List<Subcode20> elements, string gazetteName)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 0;
            foreach (var elem in elements)
            {
                Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                Biblio biblio = new Biblio();
                biblio.DOfPublication = new DOfPublication();
                biblio.Titles = new List<Integration.Title>();
                biblio.IntConvention = new IntConvention();
                biblio.EuropeanPatents = new List<EuropeanPatent>();
                biblio.Priorities = new List<Priority>();
                biblio.Inventors = new List<PartyMember>();
                biblio.Agents = new List<PartyMember>();
                biblio.Assignees = new List<PartyMember>();
                biblio.Ipcs = new List<Ipc>();
                legalEvent.SubCode = "20";
                legalEvent.SectionCode = "BA";
                legalEvent.CountryCode = "SI";
                legalEvent.Id = id++;
                biblio.Publication.Number = elem.PublicationNumber;
                biblio.Publication.Kind = elem.PublicationKind;
                biblio.Application.Number = elem.ApplicationNumber;
                biblio.Application.Date = elem.ApplicationDate;
                if(elem.Title != null)
                    biblio.Titles.Add(new Integration.Title {Text = elem.Title?.Text, Language = elem.Title?.Language });
                foreach(var classification in elem.Classification)
                {
                    biblio.Ipcs.Add(new Ipc { Class = classification });
                }
                biblio.DOfPublication.date_46 = elem.DateField46;

                if(elem.Field_86.Count > 0 && elem.Field_87.Count > 0)
                {
                    biblio.IntConvention.PctApplNumber = elem.Field_86[0].Number;
                    biblio.IntConvention.PctApplDate = elem.Field_86[0].Date;
                    biblio.IntConvention.PctPublNumber = elem.Field_87[0].Number;
                    biblio.IntConvention.PctPublDate = elem.Field_87[0].Date;
                }

                int maxSizeList = elem.Field_96.Count;
                if (elem.Field_97.Count > maxSizeList)
                {
                    maxSizeList = elem.Field_97.Count;
                    elem.Field_96 = Methods.NormalizeList(elem.Field_96, maxSizeList);
                }
                else
                {
                    elem.Field_97 = Methods.NormalizeList(elem.Field_97, maxSizeList);
                }

                EuropeanPatent euPatent = null;

                for (int i = 0; i < maxSizeList; i++)
                {
                    euPatent = new EuropeanPatent()
                    {
                        AppNumber = elem.Field_96[i].Number,
                        AppDate = elem.Field_96[i].Date,
                        PubNumber = elem.Field_97[i].Number,
                        PubDate = elem.Field_97[i].Date
                    };
                    biblio.EuropeanPatents.Add(euPatent);
                }

                foreach(var priority in elem.PriorityInformation)
                {
                    if (priority != null)
                    {
                        biblio.Priorities.Add(new Priority { Number = priority.PriorityNumber, Date = priority.PriorityDate, Country = priority.PriorityCountryCode });
                    }
                }

                if(elem.InventorInformation != null)
                {
                    foreach(var inventor in elem.InventorInformation)
                    {
                        biblio.Inventors.Add(new PartyMember {Name = inventor.Name, Address1 = inventor.Address, Country = inventor.Country });
                    }
                }

                if(elem.Grantee_Assignee_OwnerInformation != null)
                {
                    foreach(var assign in elem.Grantee_Assignee_OwnerInformation)
                    {
                        biblio.Assignees.Add(new PartyMember {Name = assign.Name, Address1 = assign.Address, Country = assign.Country });
                    }
                }

                if(elem.AgentInformation != null)
                {
                    foreach(var agent in elem.AgentInformation)
                    {
                        biblio.Agents.Add(new PartyMember {Name = agent.Name, Address1 = agent.Address, Country = agent.Country  });
                    }
                }
                
                legalEvent.GazetteName = gazetteName;
                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }
            return legalEvents;
        }
    }
}
