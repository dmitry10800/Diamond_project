using System.Collections.Generic;
using System.IO;
using Integration;

namespace LT
{
    class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> SubCode7(List<Sub7Elements> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            if (elementOuts != null)
            {
                int leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();

                    legalEvent.GazetteName = Path.GetFileName(LT_main.CurrentFileName.Replace("_sub7.txt", ".pdf").Replace("_sub7.TXT", ".pdf"));
                    legalEvent.SubCode = "7";
                    legalEvent.SectionCode = "MG9D/MG4D";
                    legalEvent.CountryCode = "LT";
                    legalEvent.Id = leCounter++;
                    legalEvent.Biblio = new Biblio();
                    legalEvent.Biblio.Publication.Number = record.I11;

                    legalEvent.LegalEvent = new LegalEvent { Date = record.Date };
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
    }
}
