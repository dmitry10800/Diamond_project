using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integration;

namespace Diamond_BE
{
    class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub4Convert(List<Elements> elements)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 1;
            foreach (var elem in elements)
            {
                Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                legalEvent.GazetteName = BE_main.currentFileName.Replace(".tetml", ".pdf");
                legalEvent.SubCode = "4";
                legalEvent.SectionCode = "MZ";
                legalEvent.CountryCode = "BE";
                legalEvent.Id = id++;
                Biblio biblio = new Biblio();
                biblio.Assignees = new List<PartyMember>();
                biblio.Titles = new List<Title>();
                biblio.DOfPublication = new DOfPublication();
                legalEvent.LegalEvent = new LegalEvent();
                biblio.Agents = new List<PartyMember>();
                biblio.Publication.Number = elem.PubNumber;
                //biblio.Publication.LanguageDesignation = elem.PubLang;
                biblio.Application.Number = elem.AppNumber;
                biblio.DOfPublication.date_47 = elem.Date47;
                biblio.DOfPublication.date_45 = elem.Date45;
                legalEvent.LegalEvent.Date = elem.EventDate;
                biblio.Titles.Add(new Title { Text = elem.Title, Language = "EN" });

                if (elem.Agent != null)
                    biblio.Agents.Add(new PartyMember { Name = elem.Agent.Name });

                if (elem.Owner != null)
                    biblio.Assignees.Add(new PartyMember { Name = elem.Owner.Name, Address1 = elem.Owner.Address, Country = elem.Owner.Country });

                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }

            return legalEvents;
        }
    }
}
