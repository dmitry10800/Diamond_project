using System.Collections.Generic;
using System.IO;
using System.Linq;
using Integration;

namespace Diamond_IE
{
    class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> FirstList(List<OutElements.FirstList> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            string dateFromName = Methods.GetDateFromGazette(IE_diam.CurrentFileName);
            if (elementOuts != null)
            {
                int leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {

                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = Path.GetFileName(IE_diam.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    if (record.PatNumber.StartsWith("S"))
                    {
                        legalEvent.SubCode = "6";
                    }
                    else
                    {
                        legalEvent.SubCode = "1";
                    }
                    legalEvent.SectionCode = "MM";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "IE";
                    legalEvent.Id = leCounter++;
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.PatNumber;
                    if (record.IpcVersion != null)
                    {
                        biblioData.Ipcs = new List<Ipc>();
                        for (int i = 0; i < record.IpcClass.Count(); i++)
                        {
                            Ipc ipcValue = new Ipc
                            {
                                Date = record.IpcVersion,
                                Class = record.IpcClass[i]
                            };
                            biblioData.Ipcs.Add(ipcValue);
                        }
                    }
                    /*-------------------*/
                    /*54 Title*/
                    Title title = new Title()
                    {
                        Language = "EN",
                        Text = record.Title
                    };
                    biblioData.Titles.Add(title);
                    /*--------------*/
                    /*71 name, address, country code*/
                    biblioData.Assignees = new List<PartyMember>();
                    PartyMember assignee = new PartyMember()
                    {
                        Name = record.AppName
                    };
                    biblioData.Assignees.Add(assignee);
                    /*LE*/
                    legalEvent.LegalEvent = new LegalEvent { Number = "", Date = dateFromName };
                    /**********************/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
    }
}
