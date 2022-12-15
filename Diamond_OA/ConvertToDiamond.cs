using System.Collections.Generic;
using System.IO;
using System.Linq;
using Integration;

namespace Diamond_OA
{
    class ConvertToDiamond
    {
        /*Patent of invention*/
        public static List<Diamond.Core.Models.LegalStatusEvent> FirstListConvertation(List<ProcessFirstList.ElementOut> elementOuts)
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

                    legalEvent.GazetteName = Path.GetFileName(Diamond_OA_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "1";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "FG";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "OA";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.I11;
                    biblioData.Application.Number = record.I21;
                    biblioData.Application.Date = record.I22;
                    /*31,32,33*/
                    if (record.I30N != null)
                    {
                        biblioData.Priorities = new List<Priority>();
                        for (int i = 0; i < record.I30N.Count(); i++)
                        {
                            Priority prioValues = new Priority
                            {
                                Number = record.I30N[i],
                                Date = record.I30D[i],
                                Country = record.I30C[i]
                            };
                            biblioData.Priorities.Add(prioValues);
                        }
                    }
                    /*51*/
                    if (record.I51C != null)
                    {
                        biblioData.Ipcs = new List<Ipc>();
                        for (int i = 0; i < record.I51C.Count(); i++)
                        {
                            Ipc ipcValue = new Ipc
                            {
                                Class = record.I51C[i],
                                Date = record.I51D[i]
                            };
                            biblioData.Ipcs.Add(ipcValue);
                        }
                    }
                    /*-------------------*/
                    /*54 Title*/
                    Title title = new Title()
                    {
                        //Language = "FR",
                        Text = record.I54
                    };
                    biblioData.Titles.Add(title);
                    /*--------*/
                    /*57 description*/
                    biblioData.Abstracts = new List<Abstract>();
                    Abstract description = new Abstract()
                    {
                        //Language = "FR",
                        Text = record.I57
                    };
                    biblioData.Abstracts.Add(description);
                    /*--------------*/
                    /*72 name, country code*/
                    if (record.I72N != null)
                    {
                        biblioData.Inventors = new List<PartyMember>();
                        for (int i = 0; i < record.I72N.Count(); i++)
                        {
                            PartyMember inventor = new PartyMember();
                            inventor.Name = record.I72N[i];
                            if (record.I72C != null)
                            {
                                inventor.Country = record.I72C[i];
                            }
                            biblioData.Inventors.Add(inventor);
                        }
                    }
                    /*---------------------*/
                    /*73 name, country code*/
                    if (record.I73N != null)
                    {
                        biblioData.Assignees = new List<PartyMember>();
                        for (int i = 0; i < record.I73N.Count(); i++)
                        {
                            PartyMember assignee = new PartyMember();
                            assignee.Name = record.I73N[i];
                            if (record.I73C != null)
                            {
                                assignee.Country = record.I73C[i];
                            }
                            if (record.I73A != null)
                            {
                                assignee.Address1 = record.I73A[i];
                            }
                            biblioData.Assignees.Add(assignee);
                        }
                    }
                    /*---------------------*/
                    /*74 name, address, cc*/
                    if (record.I74N != null)
                    {
                        biblioData.Agents = new List<PartyMember>();
                        PartyMember agent = new PartyMember();
                        agent.Name = record.I74N;
                        if (record.I74A != null)
                        {
                            agent.Address1 = record.I74A;
                        }
                        if (record.I74C != null)
                        {
                            agent.Country = record.I74C;
                        }
                        biblioData.Agents.Add(agent);
                    }
                    /*--------------------*/
                    /*86*/
                    if (record.I86 != null)
                    {
                        IntConvention intConvention = new IntConvention();
                        intConvention.PctApplNumber = record.I86;
                        biblioData.IntConvention = intConvention;
                    }
                    /*--------------------*/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }

        /*Utility models*/
        public static List<Diamond.Core.Models.LegalStatusEvent> SecondListConvertation(List<ProcessSecondList.ElementOut> elementOuts)
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

                    legalEvent.GazetteName = Path.GetFileName(Diamond_OA_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "2";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "FG";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "OA";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.I11;
                    biblioData.Application.Number = record.I21;
                    biblioData.Application.Date = record.I22;
                    /*31,32,33*/
                    if (record.I30N != null)
                    {
                        biblioData.Priorities = new List<Priority>();
                        for (int i = 0; i < record.I30N.Count(); i++)
                        {
                            Priority prioValues = new Priority
                            {
                                Number = record.I30N[i],
                                Date = record.I30D[i],
                                Country = record.I30C[i]
                            };
                            biblioData.Priorities.Add(prioValues);
                        }
                    }
                    /*51*/
                    if (record.I51C != null)
                    {
                        biblioData.Ipcs = new List<Ipc>();
                        for (int i = 0; i < record.I51C.Count(); i++)
                        {
                            Ipc ipcValue = new Ipc
                            {
                                Class = record.I51C[i],
                                Date = record.I51D[i]
                            };
                            biblioData.Ipcs.Add(ipcValue);
                        }
                    }
                    /*-------------------*/
                    /*54 Title*/
                    Title title = new Title()
                    {
                        //Language = "FR",
                        Text = record.I54
                    };
                    biblioData.Titles.Add(title);
                    /*--------*/
                    /*57 description*/
                    biblioData.Abstracts = new List<Abstract>();
                    Abstract description = new Abstract()
                    {
                        //Language = "FR",
                        Text = record.I57
                    };
                    biblioData.Abstracts.Add(description);
                    /*--------------*/
                    /*72 name, country code*/
                    if (record.I72N != null)
                    {
                        biblioData.Inventors = new List<PartyMember>();
                        for (int i = 0; i < record.I72N.Count(); i++)
                        {
                            PartyMember inventor = new PartyMember();
                            inventor.Name = record.I72N[i];
                            if (record.I72C != null)
                            {
                                inventor.Country = record.I72C[i];
                            }
                            biblioData.Inventors.Add(inventor);
                        }
                    }
                    /*---------------------*/
                    /*73 name, country code*/
                    if (record.I73N != null)
                    {
                        biblioData.Assignees = new List<PartyMember>();
                        for (int i = 0; i < record.I73N.Count(); i++)
                        {
                            PartyMember assignee = new PartyMember();
                            assignee.Name = record.I73N[i];
                            if (record.I73C != null)
                            {
                                assignee.Country = record.I73C[i];
                            }
                            if (record.I73A != null)
                            {
                                assignee.Address1 = record.I73A[i];
                            }
                            biblioData.Assignees.Add(assignee);
                        }
                    }
                    /*---------------------*/
                    /*74 name, address, cc*/
                    if (record.I74N != null)
                    {
                        biblioData.Agents = new List<PartyMember>();
                        PartyMember agent = new PartyMember();
                        agent.Name = record.I74N;
                        if (record.I74A != null)
                        {
                            agent.Address1 = record.I74A;
                        }
                        if (record.I74C != null)
                        {
                            agent.Country = record.I74C;
                        }
                        biblioData.Agents.Add(agent);
                    }
                    /*--------------------*/
                    /*86*/
                    if (record.I86 != null)
                    {
                        IntConvention intConvention = new IntConvention();
                        intConvention.PctApplNumber = record.I86;
                        biblioData.IntConvention = intConvention;
                    }
                    /*--------------------*/

                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
    }
}
