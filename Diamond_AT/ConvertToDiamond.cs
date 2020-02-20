using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integration;

namespace Diamond_AT
{
    class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> FirstSubcode(List<Sub1Elements> elementOuts)
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

                    legalEvent.GazetteName = Path.GetFileName(Diamond_AT_main.CurrentFileName.Replace("_sub1.txt", ".pdf").Replace("_sub1.TXT", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "1";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "PC4A/PC4B";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "AT";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    legalEvent.NewBiblio = new Biblio();
                    legalEvent.NewBiblio.Assignees = new List<PartyMember>();
                    /*Elements output*/
                    biblioData.Publication.Number = record.I11;

                    if (!string.IsNullOrEmpty(record.I21))
                        biblioData.Application.Number = record.I21;

                    if (!string.IsNullOrEmpty(record.I51))
                        biblioData.Ipcs = new List<Ipc> { new Ipc { Class = record.I51 } };

                    foreach (var item in record.I73new)
                    {
                        legalEvent.NewBiblio.Assignees.Add(new PartyMember
                        {
                            Name = item.Name,
                            Address1 = item.Address,
                            Country = item.Country
                        });
                    }

                    legalEvent.LegalEvent = new LegalEvent
                    {
                        Date = Methods.GetDate(Diamond_AT_main.CurrentFileName)
                    };
                    /*-------------------*/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
        public static List<Diamond.Core.Models.LegalStatusEvent> SecondSubcode(List<string> elementOuts)
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

                    legalEvent.GazetteName = Path.GetFileName(Diamond_AT_main.CurrentFileName.Replace("_sub2.txt", ".pdf").Replace("_sub2.TXT", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "2";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "MK4A/MK4B";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "AT";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++;
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record;
                    legalEvent.LegalEvent = new LegalEvent();
                    legalEvent.LegalEvent.Date = Methods.GetDate(Diamond_AT_main.CurrentFileName);
                    /*-------------------*/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> FourthSubcode(List<string> elementOuts)
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

                    legalEvent.GazetteName = Path.GetFileName(Diamond_AT_main.CurrentFileName.Replace("_sub4.txt", ".pdf").Replace("_sub4.TXT", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "4";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "MK4A/MK4B";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "AT";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++;
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record;
                    legalEvent.LegalEvent = new LegalEvent();
                    legalEvent.LegalEvent.Date = Methods.GetDate(Diamond_AT_main.CurrentFileName);
                    /*-------------------*/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> FifthSubcode(List<string> elementOuts)
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

                    legalEvent.GazetteName = Path.GetFileName(Diamond_AT_main.CurrentFileName.Replace("_sub5.txt", ".pdf").Replace("_sub5.TXT", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "5";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "MK4A/MK4B";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "AT";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++;
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record;
                    legalEvent.LegalEvent = new LegalEvent();
                    legalEvent.LegalEvent.Date = Methods.GetDate(Diamond_AT_main.CurrentFileName);
                    /*-------------------*/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
    }
}
