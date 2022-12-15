namespace Diamond_MA
{
    class ConvertToDiamond
    {
        //public static List<Diamond.Core.Models.LegalStatusEvent> FirstList(List<OutElements.FirstList> elementOuts)
        //{
        //    /*list of record for whole gazette chapter*/
        //    List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
        //    string dateFromName = Methods.GetDateFromGazette(SG_main.CurrentFileName);
        //    if (elementOuts != null)
        //    {
        //        int leCounter = 1;
        //        /*Create a new event to fill*/
        //        foreach (var record in elementOuts)
        //        {

        //            Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
        //            legalEvent.GazetteName = Path.GetFileName(SG_main.CurrentFileName.Replace(".tetml", ".pdf"));
        //            /*Setting subcode*/
        //            legalEvent.SubCode = "2";
        //            /*Setting Section Code*/
        //            legalEvent.SectionCode = "FG";
        //            /*Setting Country Code*/
        //            legalEvent.CountryCode = "SG";
        //            /*Setting File Name*/
        //            legalEvent.Id = leCounter++; // creating uniq identifier
        //            Biblio biblioData = new Biblio();
        //            /*Elements output*/
        //            biblioData.Application.Number = record.AppNumber;
        //            /*Notes*/
        //            legalEvent.LegalEvent = new LegalEvent { Number = record.LePatNumber, Date = dateFromName };
        //            /**********************/
        //            legalEvent.Biblio = biblioData;
        //            fullGazetteInfo.Add(legalEvent);
        //        }
        //    }
        //    return fullGazetteInfo;
        //}
        //public static List<Diamond.Core.Models.LegalStatusEvent> SecondList(List<OutElements.SecondList> elementOuts)
        //{
        //    /*list of record for whole gazette chapter*/
        //    List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
        //    string dateFromName = Methods.GetDateFromGazette(SG_main.CurrentFileName);
        //    if (elementOuts != null)
        //    {
        //        int leCounter = 1;
        //        /*Create a new event to fill*/
        //        foreach (var record in elementOuts)
        //        {

        //            Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
        //            legalEvent.GazetteName = Path.GetFileName(SG_main.CurrentFileName.Replace(".tetml", ".pdf"));
        //            /*Setting subcode*/
        //            legalEvent.SubCode = "5";
        //            /*Setting Section Code*/
        //            legalEvent.SectionCode = "ND";
        //            /*Setting Country Code*/
        //            legalEvent.CountryCode = "SG";
        //            /*Setting File Name*/
        //            legalEvent.Id = leCounter++; // creating uniq identifier
        //            Biblio biblioData = new Biblio();
        //            /*Elements output*/
        //            biblioData.Application.Number = record.AppNumber;
        //            biblioData.Applicants = new List<PartyMember>
        //            {
        //                new PartyMember() { Name = record.OwnerName }
        //            };
        //            /*Notes*/
        //            legalEvent.LegalEvent = new LegalEvent { Number = record.LePatNumber, Date = dateFromName };
        //            /**********************/
        //            legalEvent.Biblio = biblioData;
        //            fullGazetteInfo.Add(legalEvent);
        //        }
        //    }
        //    return fullGazetteInfo;
        //}
        //public static List<Diamond.Core.Models.LegalStatusEvent> ThirdList(List<OutElements.SecondList> elementOuts)
        //{
        //    /*list of record for whole gazette chapter*/
        //    List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
        //    string dateFromName = Methods.GetDateFromGazette(SG_main.CurrentFileName);
        //    if (elementOuts != null)
        //    {
        //        int leCounter = 1;
        //        /*Create a new event to fill*/
        //        foreach (var record in elementOuts)
        //        {
        //            Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
        //            legalEvent.GazetteName = Path.GetFileName(SG_main.CurrentFileName.Replace(".tetml", ".pdf"));
        //            /*Setting subcode*/
        //            legalEvent.SubCode = "6";
        //            /*Setting Section Code*/
        //            legalEvent.SectionCode = "MM";
        //            /*Setting Country Code*/
        //            legalEvent.CountryCode = "SG";
        //            /*Setting File Name*/
        //            legalEvent.Id = leCounter++; // creating uniq identifier
        //            Biblio biblioData = new Biblio();
        //            /*Elements output*/
        //            biblioData.Application.Number = record.AppNumber;
        //            biblioData.Applicants = new List<PartyMember>
        //            {
        //                new PartyMember() { Name = record.OwnerName }
        //            };
        //            /*Notes*/
        //            legalEvent.LegalEvent = new LegalEvent { Number = record.LePatNumber, Date = dateFromName };
        //            /**********************/
        //            legalEvent.Biblio = biblioData;
        //            fullGazetteInfo.Add(legalEvent);
        //        }
        //    }
        //    return fullGazetteInfo;
        //}
        //public static List<Diamond.Core.Models.LegalStatusEvent> FourthList(List<OutElements.SecondList> elementOuts)
        //{
        //    /*list of record for whole gazette chapter*/
        //    List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
        //    string dateFromName = Methods.GetDateFromGazette(SG_main.CurrentFileName);
        //    if (elementOuts != null)
        //    {
        //        int leCounter = 1;
        //        /*Create a new event to fill*/
        //        foreach (var record in elementOuts)
        //        {
        //            Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
        //            legalEvent.GazetteName = Path.GetFileName(SG_main.CurrentFileName.Replace(".tetml", ".pdf"));
        //            /*Setting subcode*/
        //            legalEvent.SubCode = "7";
        //            /*Setting Section Code*/
        //            legalEvent.SectionCode = "MK";
        //            /*Setting Country Code*/
        //            legalEvent.CountryCode = "SG";
        //            /*Setting File Name*/
        //            legalEvent.Id = leCounter++; // creating uniq identifier
        //            Biblio biblioData = new Biblio();
        //            /*Elements output*/
        //            biblioData.Application.Number = record.AppNumber;
        //            biblioData.Applicants = new List<PartyMember>
        //            {
        //                new PartyMember() { Name = record.OwnerName }
        //            };
        //            /*Notes*/
        //            legalEvent.LegalEvent = new LegalEvent { Number = record.LePatNumber, Date = dateFromName };
        //            /**********************/
        //            legalEvent.Biblio = biblioData;
        //            fullGazetteInfo.Add(legalEvent);
        //        }
        //    }
        //    return fullGazetteInfo;
        //}
    }
}
