using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integration;

namespace Diamond_NZ
{
    class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> FirstList(List<OutElements.Subcode2Elements> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            string dateFromName = Methods.GetDateFromGazette(NZ_main.CurrentFileName);
            if (elementOuts != null)
            {
                int leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {

                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = Path.GetFileName(NZ_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "2";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "MM";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "NZ";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.PatNumber;
                    /*Notes*/
                    legalEvent.LegalEvent = new LegalEvent { Date = dateFromName };
                    /**********************/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
    }
}
