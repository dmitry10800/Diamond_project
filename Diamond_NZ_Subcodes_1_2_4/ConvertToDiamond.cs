using System.Collections.Generic;
using Integration;

namespace Diamond_NZ_Subcodes_1_2_4
{
    public class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub1Convert(List<SubCode1> elements, string gazetteName)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 0;
            foreach (var elem in elements)
            {
                Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                Biblio biblio = new Biblio();
                legalEvent.NewBiblio = new Biblio();
                legalEvent.LegalEvent = new LegalEvent();
                biblio.DOfPublication = new DOfPublication();
                biblio.Agents = new List<PartyMember>();
                biblio.Applicants = new List<PartyMember>();
                legalEvent.NewBiblio.Applicants = new List<PartyMember>();
                legalEvent.NewBiblio.Agents = new List<PartyMember>();
                legalEvent.SubCode = "1";
                legalEvent.SectionCode = "PC";
                legalEvent.CountryCode = "NZ";
                legalEvent.Id = id++;
                biblio.Publication.Number = elem.PublicationNumber;
                legalEvent.GazetteName = gazetteName;
                legalEvent.LegalEvent.Date = elem.LegalEventDate;
                if (elem.AgentInformation != null)
                {
                    biblio.Agents.Add(new PartyMember { Name = elem.AgentInformation.Name, Address1 = elem.AgentInformation.Address, Country = elem.AgentInformation.Country });
                }
                if (elem.AgentInformationNew != null)
                {
                    legalEvent.NewBiblio.Agents.Add(new PartyMember { Name = elem.AgentInformationNew.Name, Address1 = elem.AgentInformationNew.Address, Country = elem.AgentInformation.Country });
                }
                if (elem.ApplicantInformation != null)
                {
                    foreach (var applicantInformation in elem.ApplicantInformation)
                    {
                        biblio.Applicants.Add(new PartyMember { Name = applicantInformation.Name, Address1 = applicantInformation.Address, Country = applicantInformation.Country });
                    }
                }
                if (elem.ApplicantInformationNew != null)
                {
                    foreach (var applicantInformation in elem.ApplicantInformationNew)
                    {
                        legalEvent.NewBiblio.Applicants.Add(new PartyMember { Name = applicantInformation.Name, Address1 = applicantInformation.Address, Country = applicantInformation.Country });
                    }
                }
                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }
            return legalEvents;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub2Convert(List<SubCode2> elements, string gazetteName)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 0;
            foreach (var elem in elements)
            {
                Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                Biblio biblio = new Biblio();
                legalEvent.LegalEvent = new LegalEvent();
                legalEvent.SubCode = "2";
                legalEvent.SectionCode = "MM";
                legalEvent.CountryCode = "NZ";
                legalEvent.Id = id++;
                biblio.Publication.Number = elem.PublicationNumber;
                legalEvent.GazetteName = gazetteName;
                legalEvent.LegalEvent.Date = elem.LegalEventDate;
                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }
            return legalEvents;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub4Convert(List<SubCode4> elements, string gazetteName)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 0;
            foreach (var elem in elements)
            {
                Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                Biblio biblio = new Biblio();
                legalEvent.LegalEvent = new LegalEvent();
                legalEvent.SubCode = "4";
                legalEvent.SectionCode = "MK";
                legalEvent.CountryCode = "NZ";
                legalEvent.Id = id++;
                biblio.Publication.Number = elem.PublicationNumber;
                legalEvent.GazetteName = gazetteName;
                legalEvent.LegalEvent.Date = elem.LegalEventDate;
                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }
            return legalEvents;
        }
    }
}
