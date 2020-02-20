using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integration;

namespace Diamond_US
{
    class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub1(List<OutElements.Sub1> elementOuts)
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
                    legalEvent.GazetteName = Path.GetFileName(US_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "1";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "MM";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "US";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    biblioData.Application.Number = record.AppNumber;
                    biblioData.Publication.Number = record.PubNumber;
                    biblioData.DOfPublication = new DOfPublication { date_45 = record.IssueDate };
                    legalEvent.LegalEvent = new LegalEvent { Number = record.PubNumber, Date = record.EventDate };
                    legalEvent.LegalEvent.Language = "EN";
                    /**********************/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
    }
}
