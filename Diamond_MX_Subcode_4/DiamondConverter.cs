using System.Collections.Generic;
using Integration;

namespace Diamond_MX_Subcode_4
{
    class DiamondConverter
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub4Convert(List<Elements> elements)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 1;
            foreach (var rec in elements)
            {
                Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                legalEvent.GazetteName = MX_main.currentFileName.Replace(".tetml", ".pdf");
                legalEvent.SubCode = "4";
                legalEvent.SectionCode = "TC";
                legalEvent.CountryCode = "MX";
                legalEvent.Id = id++;
                Biblio biblio = new Biblio();
                legalEvent.NewBiblio = new Biblio();
                legalEvent.NewBiblio.Assignees = new List<PartyMember>();
                legalEvent.LegalEvent = new LegalEvent();
                biblio.Publication.Number = rec.PubNumber;
                biblio.Application.Number = rec.AppNumber;
                legalEvent.NewBiblio.Assignees.Add(new PartyMember { Name = rec.Owner.Name });
                legalEvent.LegalEvent.Date = rec.EventDate;

                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }

            return legalEvents;
        }
    }
}
