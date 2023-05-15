using System.Collections.Generic;
using Integration;

namespace Diamond_BG_Subcode_21
{
    public class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub21Convert(List<Subcode21> elements, string gazetteName)
        {
            var legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 0;
            foreach (var elem in elements)
            {
                var legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                var biblio = new Biblio();
                legalEvent.LegalEvent = new LegalEvent();
                legalEvent.SubCode = "21";
                legalEvent.SectionCode = "MM";
                legalEvent.CountryCode = "BG";
                legalEvent.Id = id++;
                biblio.Publication.Number = elem.PublicationNumber;
                if (elem.PublicationKind != null)
                {
                    biblio.Publication.Kind = elem.PublicationKind;
                }
                legalEvent.GazetteName = gazetteName;
                legalEvent.LegalEvent.Date = elem.LegalStatusEvents.EventDate;
                //legalEvent.LegalEvent.Number = elem.LegalStatusEvents.PatentNumber;
                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }
            return legalEvents;
        }
    }
}
