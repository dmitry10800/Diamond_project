using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integration;

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
    }
}
