using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integration;

namespace Diamond_ME
{
    class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> FirstListConvertation(List<ProcessPublicationsOfEuroApp.ElementOut> elementOuts)
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

                    legalEvent.GazetteName = Path.GetFileName(ME_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*//*Setting Section Code*/
                    legalEvent.SubCode = "3";
                    legalEvent.SectionCode = "BZ";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "ME";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*30 priorities*/
                    if (record.Priority != null && record.Priority.Count > 0)
                    {
                        biblioData.Priorities = new List<Priority>();
                        for (int i = 0; i < record.Priority.Count(); i++)
                        {
                            Priority priority = new Priority
                            {
                                Number = record.Priority[i].Number,
                                Date = record.Priority[i].Date,
                                Country = record.Priority[i].Country
                            };
                            biblioData.Priorities.Add(priority);
                        }
                    }
                    /*---------------------*/
                    /*51 international classification*/
                    if (record.IPCValues != null && record.IPCValues.Count > 0)
                    {
                        biblioData.Ipcs = new List<Ipc>();
                        for (int i = 0; i < record.IPCValues.Count(); i++)
                        {
                            Ipc ipcValue = new Ipc
                            {
                                Date = record.IPCValues[i].Version,
                                Class = record.IPCValues[i].Number
                            };
                            biblioData.Ipcs.Add(ipcValue);
                        }
                    }
                    /*---------------------*/
                    /*54 Title*/
                    if (record.I54 != null)
                    {
                        Title title = new Title()
                        {
                            Text = record.I54,
                            Language = "EN"
                        };
                        biblioData.Titles.Add(title);
                    }
                    /*71 name, country code*/
                    if (record.Applicants != null && record.Applicants.Count > 0)
                    {
                        biblioData.Applicants = new List<PartyMember>();
                        for (int i = 0; i < record.Applicants.Count(); i++)
                        {
                            PartyMember applicants = new PartyMember();
                            //Translation lang = new Translation();
                            applicants.Name = record.Applicants[i].Name;
                            applicants.Address1 = record.Applicants[i].Address;
                            applicants.Country = record.Applicants[i].Country;
                            applicants.Language = "EN";
                            biblioData.Applicants.Add(applicants);
                        }
                    }
                    /*--------------*/
                    /*72 name*/
                    if (record.Inventors != null && record.Inventors.Count > 0)
                    {
                        biblioData.Inventors = new List<PartyMember>();
                        for (int i = 0; i < record.Inventors.Count(); i++)
                        {
                            PartyMember Inventors = new PartyMember();
                            Inventors.Name = record.Inventors[i].Name;
                            Inventors.Address1 = record.Inventors[i].Address;
                            Inventors.Country = record.Inventors[i].Country;
                            Inventors.Language = "EN";
                            biblioData.Inventors.Add(Inventors);
                        }
                    }
                    /*96*/
                    /*96/97 number and date*/
                    if (record.I96.Number != null || record.I97.Number != null)
                    {
                        biblioData.EuropeanPatents = new List<EuropeanPatent>();
                        EuropeanPatent euPatent = new EuropeanPatent();
                        if (record.I96.Number != null) euPatent.AppNumber = record.I96.Number;
                        if (record.I96.Date != null) euPatent.AppDate = record.I96.Date;
                        if (record.I97.Number != null) euPatent.PubNumber = record.I97.Number;
                        if (record.I97.Date != null) euPatent.PubDate = record.I97.Date;
                        biblioData.EuropeanPatents.Add(euPatent);
                    }
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
    }
}
