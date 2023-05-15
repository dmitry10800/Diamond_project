using System.Collections.Generic;
using Integration;

namespace Diamond_LK_Subcode_2
{
    class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub2Convert(List<Elements> elements)
        {
            var legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 0;
            foreach (var elem in elements)
            {
                var legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                var biblio = new Biblio();
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
