using System.Collections.Generic;
using System.IO;
using System.Linq;
using Integration;

namespace Diamond_MK
{
    class ConvertToDiamond
    {
        /*Applications Filed*/
        public static List<Diamond.Core.Models.LegalStatusEvent> NationalAndInternationalPatentsConvertation(List<ProcessNatAndInternatPatents.ElementOut> elementOuts)
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

                    legalEvent.GazetteName = Path.GetFileName(Diamond_MK_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    if (record.I13 == "Т1" || record.I13 == "Т1")
                    {
                        legalEvent.SubCode = "3";
                    }
                    else if (record.I13 == "A" || record.I13 == "А")
                    {
                        legalEvent.SubCode = "1";
                    }

                    /*Setting Section Code*/
                    legalEvent.SectionCode = "FG";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "MK";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.I11;
                    biblioData.Publication.Kind = record.I13;
                    if (record.I21 != null)
                    {
                        biblioData.Application.Number = record.I21;
                    }
                    if (record.I22 != null)
                    {
                        biblioData.Application.Date = record.I22;
                    }
                    /*30*/
                    if (record.I30N != null && record.I30D.Count() == record.I30C.Count() && record.I30D.Count() == record.I30N.Count())
                    {
                        biblioData.Priorities = new List<Priority>();
                        for (int i = 0; i < record.I30N.Count(); i++)
                        {
                            Priority priority = new Priority();
                            priority.Country = record.I30C[i];
                            priority.Date = record.I30D[i];
                            priority.Number = record.I30N[i];
                            priority.Sequence = i;
                            biblioData.Priorities.Add(priority);
                        }
                    }
                    /*---------------------*/
                    /*45*/
                    if (record.I45 != null)
                    {
                        biblioData.DOfPublication = new DOfPublication()
                        {
                            date_41 = record.I45
                        };
                    }
                    /*---------------------*/
                    /*51 classification*/
                    if (record.I51 != null)
                    {
                        biblioData.Ipcs = new List<Ipc>();
                        for (int i = 0; i < record.I51.Count(); i++)
                        {
                            Ipc ipc = new Ipc();
                            ipc.Class = record.I51[i];
                            biblioData.Ipcs.Add(ipc);
                        }
                    }
                    /*-----------------*/
                    /*54 Title*/
                    Title title = new Title()
                    {
                        Language = "MK",
                        Text = record.I54
                    };
                    biblioData.Titles.Add(title);
                    /*--------*/
                    /*57 description*/
                    if (record.I57 != null)
                    {
                        Abstract desc = new Abstract()
                        {
                            Text = record.I57,
                            Language = "MK"
                        };
                        biblioData.Abstracts.Add(desc);
                    }
                    /*--------*/
                    /*72 name, country code*/
                    if (record.I72 != null)
                    {
                        biblioData.Inventors = new List<PartyMember>();
                        for (int i = 0; i < record.I72.Count(); i++)
                        {
                            PartyMember inventor = new PartyMember()
                            {
                                Name = record.I72[i]
                            };
                            biblioData.Inventors.Add(inventor);
                        }
                    }
                    /*---------------------*/
                    /*73 name, address*/
                    if (record.I73 != null)
                    {
                        biblioData.Assignees = new List<PartyMember>();
                        PartyMember assignee = new PartyMember()
                        {
                            Name = record.I73
                        };
                        biblioData.Assignees.Add(assignee);
                    }
                    /*---------------------*/
                    /*74 name, address*/
                    if (record.I74N != null)
                    {
                        biblioData.Agents = new List<PartyMember>();
                        PartyMember agent = new PartyMember();
                        agent.Name = record.I74N;
                        if (record.I74A != null)
                        {
                            agent.Address1 = record.I74A;
                        }
                        biblioData.Agents.Add(agent);
                    }
                    /*--------------------*/
                    /*96/97 number and date*/
                    biblioData.EuropeanPatents = new List<EuropeanPatent>();
                    EuropeanPatent euPatent = new EuropeanPatent()
                    {
                        AppNumber = record.I96N,
                        AppDate = record.I96D,
                        PubNumber = record.I97N,
                        PubDate = record.I97D
                    };
                    biblioData.EuropeanPatents.Add(euPatent);
                    /*-------------------*/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
    }
}
