using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integration;

namespace Diamond_PH
{
    class DiamondConverter
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub7Convert(List<Elements> elements)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 1;

            foreach (var elem in elements)
            {
                Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                legalEvent.GazetteName = PH_main.currentFileName.Replace(".txt", ".pdf");
                legalEvent.SubCode = "7";
                legalEvent.SectionCode = "MK";
                legalEvent.CountryCode = "PH";
                legalEvent.Id = id++;
                Biblio biblio = new Biblio();
                biblio.Assignees = new List<PartyMember>();
                biblio.Titles = new List<Title>();
                legalEvent.LegalEvent = new LegalEvent();
                biblio.Application.Number = elem.AppNumber;
                biblio.Application.Date = elem.AppDate;
                biblio.Assignees.Add(new PartyMember { Name = elem.Owner.Name, Country = "PH" });
                biblio.Titles.Add(new Title { Text = elem.Title, Language = "EN" });
                legalEvent.LegalEvent.Date = elem.EventDate;

                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }

            return legalEvents;
        }
    }
}
