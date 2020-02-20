using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integration;

namespace Diamond_AU
{
    class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> FirstList(List<OutElements.FirstList> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            string dateFromName = Methods.GetDateFromGazette(AU_main.CurrentFileName);
            if (elementOuts != null)
            {
                int leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = Path.GetFileName(AU_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "2";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "PC";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "AU";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Application.Number = record.AppNumber;
                    /*73 name and address*/
                    biblioData.Assignees = new List<PartyMember>();
                    legalEvent.NewBiblio = new Biblio();
                    legalEvent.NewBiblio.Assignees = new List<PartyMember>();
                    /*Notes*/
                    legalEvent.LegalEvent = new LegalEvent { Date = dateFromName.Replace("-", "/") };
                    /**********************/
                    if (record.NameOld.Contains(";") && record.NameNew.Contains(";"))
                    {
                        var splNamesOld = record.NameOld.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        var splNamesNew = record.NameNew.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        if (splNamesNew.Count == splNamesOld.Count)
                        {
                            for (int i = 0; i < splNamesNew.Count; i++)
                            {
                                biblioData.Assignees.Add(new PartyMember { Name = splNamesOld[i].Trim() });
                                legalEvent.NewBiblio.Assignees.Add(new PartyMember { Name = splNamesNew[i].Trim() });
                            }
                        }
                    }
                    else
                    {
                        biblioData.Assignees = new List<PartyMember>
                            {
                                new PartyMember { Name = record.NameOld }
                            };
                        /*New Biblio test*/
                        legalEvent.NewBiblio = new Biblio
                        {
                            Assignees = new List<PartyMember>
                            {
                                new PartyMember { Name = record.NameNew }
                            }
                        };
                        /*New Biblio endTest*/
                    }
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
    }
}
