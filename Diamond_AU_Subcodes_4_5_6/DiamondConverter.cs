using System.Collections.Generic;
using Integration;

namespace Diamond_AU_Subcodes_4_5_6
{
    class DiamondConverter
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub4Convert(List<Elements.SubCode4> elements)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 1;
            foreach (var rec in elements)
            {
                Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                legalEvent.GazetteName = AU_main.currentFileName.Replace(".tetml", ".pdf");
                legalEvent.SubCode = "4";
                legalEvent.SectionCode = "FA";
                legalEvent.CountryCode = "AU";
                legalEvent.Id = id++;
                Biblio biblio = new Biblio();
                legalEvent.LegalEvent = new LegalEvent { Date = rec.EventDate };
                biblio.Application.Number = rec.AppNumber;
                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }


            return legalEvents;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub5Convert(List<Elements.SubCode5> elements)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 1;
            foreach (var rec in elements)
            {
                Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                legalEvent.GazetteName = AU_main.currentFileName.Replace(".tetml", ".pdf");
                legalEvent.SubCode = "5";
                legalEvent.SectionCode = "MK";
                legalEvent.CountryCode = "AU";
                legalEvent.Id = id++;
                Biblio biblio = new Biblio();
                legalEvent.LegalEvent = new LegalEvent { Date = rec.EventDate };
                biblio.Application.Number = rec.AppNumber;
                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }


            return legalEvents;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub6Convert(List<Elements.SubCode6> elements)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 1;
            foreach (var rec in elements)
            {
                Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                legalEvent.GazetteName = AU_main.currentFileName.Replace(".tetml", ".pdf");
                legalEvent.SubCode = "6";
                legalEvent.SectionCode = "FD";
                legalEvent.CountryCode = "AU";
                legalEvent.Id = id++;
                Biblio biblio = new Biblio();
                legalEvent.LegalEvent = new LegalEvent { Date = rec.EventDate };
                biblio.Application.Number = rec.AppNumber;
                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }


            return legalEvents;
        }
    }
}
