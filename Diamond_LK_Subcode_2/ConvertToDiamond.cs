using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integration;

namespace Diamond_LK_Subcode_2
{
    class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub2Convert(List<Elements> elements)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 0;
            foreach (var elem in elements)
            {
                Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                Biblio biblio = new Biblio();
                biblio.Assignees = new List<PartyMember>();
                legalEvent.SubCode = "2";
                legalEvent.SectionCode = "FG";
                legalEvent.CountryCode = "LK";
                legalEvent.Id = id++;
                biblio.Assignees.Add(new PartyMember { Name = elem.AssigneeName });
                biblio.Publication.Number = elem.PubNumber;
                biblio.Application.Date = elem.AppDate;
                biblio.Titles.Add(new Title { Text = elem.Title, Language = "EN" });
                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }

            return legalEvents;
        }
    }
}
