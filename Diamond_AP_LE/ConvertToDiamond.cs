using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integration;

namespace Diamond_AP_LE
{
    class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub10(List<OutElements.Sub10> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            string dateFromName = Methods.GetDateFromGazette(AP_LE_main.CurrentFileName);
            if (elementOuts != null)
            {
                int leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = Path.GetFileName(AP_LE_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "10";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "AD";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "AP";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    biblioData.Application.Number = record.AppNumber;
                    legalEvent.LegalEvent = new LegalEvent { Date = record.DateFeePaid.Replace("-", "/") };
                    legalEvent.LegalEvent.Note = $"|| Valid Until | {record.ValidUntil} || Anniversary | {record.Anniversary}";
                    legalEvent.LegalEvent.Language = "EN";
                    /**********************/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub7(List<OutElements.Sub7> elementOuts)
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
                    legalEvent.GazetteName = Path.GetFileName(AP_LE_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "7";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "NZ";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "AP";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.PatNumber;
                    biblioData.Application.Number = record.AppNumber;
                    /*73 name and address*/
                    legalEvent.LegalEvent = new LegalEvent { Date = record.DateFeePaid.Replace("-", "/") };
                    legalEvent.LegalEvent.Note = $"|| Valid Until | {record.ValidUntil} || Anniversary | {record.Anniversary}";
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
