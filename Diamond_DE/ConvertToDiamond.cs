using System.Collections.Generic;
using System.IO;
using Integration;

namespace Diamond_DE
{
    class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub1(List<OutElements.Sub1> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            string dateFromName = Methods.GetDateFromGazette(DE_main.CurrentFileName);
            if (elementOuts != null)
            {
                int leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = Path.GetFileName(DE_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "1";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "NZ";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "DE";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Application.Number = record.AppNumber;
                    biblioData.Publication.Date = record.DateI43;
                    /*73 name and address*/
                    biblioData.Assignees = new List<PartyMember>();
                    legalEvent.NewBiblio = new Biblio();
                    legalEvent.NewBiblio.Applicants = new List<PartyMember>();
                    legalEvent.NewBiblio.Assignees = new List<PartyMember>();
                    legalEvent.NewBiblio.Agents = new List<PartyMember>();
                    /*Notes*/
                    legalEvent.LegalEvent = new LegalEvent { Date = dateFromName.Replace("-", "/") };
                    /**********************/
                    foreach (var item in record.New71Applicant)
                    {
                        legalEvent.NewBiblio.Applicants.Add(new PartyMember
                        {
                            Name = item.Name,
                            Address1 = item.Address,
                            Country = item.Country
                        });
                    }
                    foreach (var item in record.New74Agent)
                    {
                        legalEvent.NewBiblio.Agents.Add(new PartyMember
                        {
                            Name = item.Name,
                            Address1 = item.Address,
                            Country = item.Country
                        });
                    }
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub3(List<OutElements.Sub3> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            string dateFromName = Methods.GetDateFromGazette(DE_main.CurrentFileName);
            if (elementOuts != null)
            {
                int leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = Path.GetFileName(DE_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "3";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "TZ";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "DE";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Application.Number = record.AppNumber;
                    biblioData.DOfPublication = new DOfPublication() { date_45 = record.DateI45.Replace("-", "/") };
                    /*73 name and address*/
                    biblioData.Assignees = new List<PartyMember>();
                    legalEvent.NewBiblio = new Biblio();
                    legalEvent.NewBiblio.Applicants = new List<PartyMember>();
                    legalEvent.NewBiblio.Assignees = new List<PartyMember>();
                    legalEvent.NewBiblio.Agents = new List<PartyMember>();
                    /*Notes*/
                    legalEvent.LegalEvent = new LegalEvent { Date = dateFromName.Replace("-", "/") };

                    /**********************/
                    foreach (var item in record.New73Assignee)
                    {
                        legalEvent.NewBiblio.Assignees.Add(new PartyMember
                        {
                            Name = item.Name,
                            Address1 = item.Address,
                            Country = item.Country
                        });
                    }
                    foreach (var item in record.New74Agent)
                    {
                        legalEvent.NewBiblio.Agents.Add(new PartyMember
                        {
                            Name = item.Name,
                            Address1 = item.Address,
                            Country = item.Country
                        });
                    }
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub6(List<OutElements.Sub6> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            string dateFromName = Methods.GetDateFromGazette(DE_main.CurrentFileName);
            if (elementOuts != null)
            {
                int leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = Path.GetFileName(DE_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "6";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "FC";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "DE";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Application.Number = record.AppNumber;
                    biblioData.Publication.Date = record.DateI43.Replace("-", "/");
                    /*73 name and address*/
                    biblioData.Ipcs = new List<Ipc> { new Ipc { Class = record.IpcClass } };
                    /*Notes*/
                    legalEvent.LegalEvent = new LegalEvent { Date = dateFromName.Replace("-", "/") };
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
            string dateFromName = Methods.GetDateFromGazette(DE_main.CurrentFileName);
            if (elementOuts != null)
            {
                int leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = Path.GetFileName(DE_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "7";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "NC";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "DE";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Application.Number = record.AppNumber;
                    biblioData.DOfPublication = new DOfPublication() { date_45 = record.DateI45.Replace("-", "/") };
                    /*73 name and address*/
                    biblioData.Ipcs = new List<Ipc> { new Ipc { Class = record.IpcClass } };
                    /*Notes*/
                    legalEvent.LegalEvent = new LegalEvent { Date = dateFromName.Replace("-", "/") };
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
    }
}
