using System.Collections.Generic;
using System.IO;
using Integration;

namespace Diamond_EA
{
    class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub5(List<OutElements.Subcode5> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            string dateFromName = Methods.GetDateFromGazette(EA_main.CurrentFileName);
            if (elementOuts != null)
            {
                int leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = Path.GetFileName(EA_main.CurrentFileName.Replace(".txt", ".pdf").Replace(".TXT", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "5";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "FA9A";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "EA";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Application.Number = record.AppNumber;
                    biblioData.Publication.Kind = record.PubKind;
                    biblioData.Publication.Date = record.Date43;
                    //biblioData.DOfPublication = new DOfPublication { date = record.Date43 };
                    legalEvent.LegalEvent = new LegalEvent
                    {
                        Language = "RU",
                        Note = $"|| № Бюллетеня | {record.LeBulletinNumber}",
                        Date = record.LeDate,
                        Translations = new List<NoteTranslation> {
                            new NoteTranslation {
                                Language = "EN",
                                Tr = $"|| Bulletin No. | {record.LeBulletinNumber}",
                                Type = "note"
                            }
                        }
                    };
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub12(List<OutElements.Subcode12> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
       //     string dateFromName = Methods.GetDateFromGazette(EA_main.CurrentFileName);
           
            if (elementOuts != null)
            {
                int leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = Path.GetFileName(EA_main.CurrentFileName.Replace(".txt", ".pdf").Replace(".TXT", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "12";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "MM4A";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "EA";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.PubNumber;
                    biblioData.Publication.Kind = record.PubKind;
                    biblioData.DOfPublication = new DOfPublication { date_45 = record.I45Date };
                    legalEvent.LegalEvent = new LegalEvent
                    { 
                        Number = record.LeNumber, 
                        Date = record.LeDate, 
                        Note = $"|| № Бюллетеня | {record.LeBulletinNumber} " +
                        $"|| Код государства, на территории которого прекращено действие патента | {record.LeCcTerminated}" +
                        $"|| Код государства, на территории которого продолжается действие патента | {record.LeCcValid}" +
                        $"|| Дата публикации извещения | {record.NoteDate}",
                        Language = "RU",
                        Translations = new List<NoteTranslation>
                        {
                            new NoteTranslation {
                                Language = "EN",
                            Tr = $"|| Bulletin No. | {record.LeBulletinNumber} " +
                        $"|| Country code in which territory the patent is terminated | {record.LeCcTerminated}" +
                        $"|| Country code in which territory the patent is valid | {record.LeCcValid}" +
                        $"|| Publication date of notice | {record.NoteDate}",
                            Type = "note"
                            }
                        }
                    };
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
    }
}
