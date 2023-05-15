using System.Collections.Generic;
using System.IO;
using Integration;

namespace Diamond_FR
{
    class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> SubCode1Convert(List<OutElements.FirstList> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            var fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            var leDate = Methods.GetDateFromGazette(FR_main.CurrentFileName);
            if (elementOuts != null)
            {
                var leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {

                    var legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = Path.GetFileName(FR_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "1";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "PC";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "FR";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    var biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Application.Number = record.AppNumber;
                    /*Notes*/
                    legalEvent.LegalEvent = new LegalEvent { Date = leDate, Language = "FR" };
                    legalEvent.LegalEvent.Note = $"|| Numéro de l’inscription | {record.LeNoteNumber} || Nature de la demande | {record.LeNoteCountry}";
                    legalEvent.LegalEvent.Translations.Add(new NoteTranslation
                    {
                        Language = "EN",
                        Tr = $"|| Registration number | {record.LeNoteNumber} || Nature of application | {record.LeNoteCountry}",
                        Type = "note"
                    });
                    /**********************/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
        public static List<Diamond.Core.Models.LegalStatusEvent> SecondList(List<OutElements.SecondList> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            var fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            var dateFromName = Methods.GetDateFromGazette(FR_main.CurrentFileName);
            if (elementOuts != null)
            {
                var leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {

                    var legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = Path.GetFileName(FR_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "5";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "TZ";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "FR";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    var biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Application.Number = record.AppNumber;
                    biblioData.Applicants = new List<PartyMember>
                    {
                        //new PartyMember() { Name = record.OwnerName }
                    };
                    /*Notes*/
                    legalEvent.LegalEvent = new LegalEvent { Date = dateFromName };
                    /**********************/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
        public static List<Diamond.Core.Models.LegalStatusEvent> ThirdList(List<OutElements.SecondList> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            var fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            var dateFromName = Methods.GetDateFromGazette(FR_main.CurrentFileName);
            if (elementOuts != null)
            {
                var leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    var legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = Path.GetFileName(FR_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "6";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "TE";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "FR";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    var biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Application.Number = record.AppNumber;
                    biblioData.Applicants = new List<PartyMember>
                    {
                        //new PartyMember() { Name = record.OwnerName }
                    };
                    /*Notes*/
                    legalEvent.LegalEvent = new LegalEvent { Date = dateFromName };
                    /**********************/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
        public static List<Diamond.Core.Models.LegalStatusEvent> SubCode7Convert(List<OutElements.SecondList> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            var fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            var dateFromName = Methods.GetDateFromGazette(FR_main.CurrentFileName);
            if (elementOuts != null)
            {
                var leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    var legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = Path.GetFileName(FR_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "7";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "MC";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "FR";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++;
                    var biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Application.Number = record.AppNumber;
                    /*Notes*/
                    legalEvent.LegalEvent = new LegalEvent { Date = dateFromName };
                    legalEvent.LegalEvent.Note = $"|| Numéro de l’inscription | {record.RegNumber}\n|| Nature et numéro de la demande ou du titre | {record.NatureOfApplication}";
                    legalEvent.LegalEvent.Language = "FR";
                    legalEvent.LegalEvent.Translations.Add(new NoteTranslation
                    {
                        Language = "EN",
                        Tr = $"|| Registration number | {record.RegNumber}\n|| Nature of application | {record.NatureOfApplication}",
                        Type = "note"
                    });
                    /**********************/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> SubCode4Convert(List<OutElements.SecondList> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            var fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            var dateFromName = Methods.GetDateFromGazette(FR_main.CurrentFileName);
            if (elementOuts != null)
            {
                var leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    var legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = Path.GetFileName(FR_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "4";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "TC";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "FR";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++;
                    var biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Application.Number = record.AppNumber;
                    /*Notes*/
                    legalEvent.LegalEvent = new LegalEvent { Date = dateFromName };
                    legalEvent.LegalEvent.Note = $"|| Numéro de l’inscription | {record.RegNumber}\n|| Nature et numéro de la demande ou du titre | {record.NatureOfApplication}";
                    legalEvent.LegalEvent.Language = "FR";
                    legalEvent.LegalEvent.Translations.Add(new NoteTranslation
                    {
                        Language = "EN",
                        Tr = $"|| Registration number | {record.RegNumber}\n|| Nature of application | {record.NatureOfApplication}",
                        Type = "note"
                    });
                    /**********************/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
    }
}
