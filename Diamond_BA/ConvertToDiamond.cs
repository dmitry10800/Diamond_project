using System.Collections.Generic;
using System.IO;
using System.Linq;
using Integration;

namespace Diamond_BA
{
    class ConvertToDiamond
    {
        /*Applications Filed*/
        public static List<Diamond.Core.Models.LegalStatusEvent> FirstlistConvertation(List<ProcessFirstList.ElementOut> elementOuts)
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

                    legalEvent.GazetteName = Path.GetFileName(Diamond_BA_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "1";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "BA";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "BA";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Application.Number = record.I21;
                    if (record.I26 != null) biblioData.Publication.Authority = record.I26;
                    if (record.I22 != null) biblioData.Application.Date = record.I22;
                    /*30*/
                    if (record.I31 != null)
                    {
                        biblioData.Priorities = new List<Priority>();
                        for (int i = 0; i < record.I31.Count(); i++)
                        {
                            Priority priority = new Priority();
                            priority.Number = record.I31[i];
                            priority.Date = record.I32[i];
                            priority.Country = record.I33[i];
                            //priority.Sequence = i;
                            biblioData.Priorities.Add(priority);
                        }
                    }
                    /*---------------------*/
                    /*43*/
                    if (record.I43 != null) biblioData.DOfPublication = new DOfPublication() { date = record.I43 };
                    /*---------------------*/
                    /*51 classification*/
                    if (record.I51Class != null)
                    {
                        biblioData.Ipcs = new List<Ipc>();
                        for (int i = 0; i < record.I51Class.Count(); i++)
                        {
                            Ipc ipc = new Ipc();
                            //ipc.Class = record.I51Class[i];
                            if (record.I51Class != null && record.I51Class[i] != null) ipc.Class = record.I51Class[i];
                            if (record.I51Version != null && record.I51Version[i] != null) ipc.Date = record.I51Version[i];
                            biblioData.Ipcs.Add(ipc);
                        }
                    }
                    /*-----------------*/
                    /*54 Title*/
                    Title title = new Title()
                    {
                        Text = record.I54,
                        Language = "HR"
                    };
                    biblioData.Titles.Add(title);
                    /*--------*/
                    /*57 Title*/
                    if (record.I57 != null)
                    {
                        Abstract desc = new Abstract()
                        {
                            Text = record.I57,
                            Language = "HR"
                        };
                        biblioData.Abstracts.Add(desc);
                    }
                    /*--------*/
                    /*71 name, country code*/
                    if (record.I71N != null)
                    {
                        biblioData.Applicants = new List<PartyMember>();
                        for (int i = 0; i < record.I71N.Count(); i++)
                        {
                            PartyMember applicants = new PartyMember();
                            applicants.Name = record.I71N[i];
                            if (record.I71C[i] != null) applicants.Country = record.I71C[i];
                            biblioData.Applicants.Add(applicants);
                        }
                    }
                    /*---------------------*/
                    /*72 name, country code*/
                    if (record.I72N != null)
                    {
                        biblioData.Inventors = new List<PartyMember>();
                        for (int i = 0; i < record.I72N.Count(); i++)
                        {
                            PartyMember inventor = new PartyMember();
                            inventor.Name = record.I72N[i];
                            if (record.I72A[i] != null) inventor.Address1 = record.I72A[i];
                            if (record.I72C[i] != null) inventor.Country = record.I72C[i];
                            biblioData.Inventors.Add(inventor);
                        }
                    }
                    /*---------------------*/
                    /*73 name, address*/
                    if (record.I73N != null)
                    {
                        biblioData.Assignees = new List<PartyMember>();
                        for (int i = 0; i < record.I73N.Count(); i++)
                        {
                            PartyMember assignee = new PartyMember();
                            assignee.Name = record.I73N[i];
                            //if (record.I73A[i] != null) assignee.Address1 = record.I73A[i];
                            if (record.I73C[i] != null) assignee.Country = record.I73C[i];
                            biblioData.Assignees.Add(assignee);
                        }
                    }
                    /*---------------------*/
                    /*74 name, address, cc*/
                    if (record.I74N != null)
                    {
                        biblioData.Agents = new List<PartyMember>();
                        for (int i = 0; i < record.I74N.Count(); i++)
                        {
                            PartyMember agent = new PartyMember();
                            agent.Name = record.I74N[i];
                            //if (record.I74A[i] != null) agent.Address1 = record.I74A[i];
                            if (record.I74C != null && record.I74C[i] != null) agent.Country = record.I74C[i];
                            biblioData.Agents.Add(agent);
                        }
                    }
                    /*--------------------*/
                    /*75 name, address, cc*/
                    if (record.I75N != null)
                    {
                        biblioData.InvOrApps = new List<PartyMember>();
                        for (int i = 0; i < record.I75N.Count(); i++)
                        {
                            PartyMember invOrApps = new PartyMember();
                            invOrApps.Name = record.I75N[i];
                            if (record.I75A[i] != null) invOrApps.Address1 = record.I75A[i];
                            if (record.I75C[i] != null) invOrApps.Country = record.I75C[i];
                            biblioData.InvOrApps.Add(invOrApps);
                        }
                    }
                    /*--------------------*/
                    /*84*/
                    IntConvention intConvention = new IntConvention();
                    /*86*/
                    if (record.I86N != null) intConvention.PctApplNumber = record.I86N;
                    if (record.I86D != null) intConvention.PctApplDate = record.I86D;
                    if (record.I87N != null) intConvention.PctPublNumber = record.I87N;
                    if (record.I87D != null) intConvention.PctPublDate = record.I87D;
                    /*--------------------*/
                    if (intConvention != null)
                    {
                        biblioData.IntConvention = intConvention;
                    }
                    /*-------------------**/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }

        /*Publication of European Patents Entered in Patent Register*/
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
                    legalEvent.GazetteName = Path.GetFileName(Diamond_BA_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "4";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "BA";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "BA";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.I11;
                    biblioData.Application.Number = record.I21;
                    if (record.I26 != null) biblioData.Publication.Authority = record.I26;
                    if (record.I22 != null) biblioData.Application.Date = record.I22;
                    /*30*/
                    if (record.I31 != null)
                    {
                        biblioData.Priorities = new List<Priority>();
                        for (int i = 0; i < record.I31.Count(); i++)
                        {
                            Priority priority = new Priority();
                            priority.Number = record.I31[i];
                            priority.Date = record.I32[i];
                            priority.Country = record.I33[i];
                            //priority.Sequence = i;
                            biblioData.Priorities.Add(priority);
                        }
                    }
                    /*---------------------*/
                    /*51 classification*/
                    if (record.I51Class != null)
                    {
                        biblioData.Ipcs = new List<Ipc>();
                        for (int i = 0; i < record.I51Class.Count(); i++)
                        {
                            Ipc ipc = new Ipc();
                            ipc.Class = record.I51Class[i];
                            if (record.I51Version != null && record.I51Version[i] != null) ipc.Date = record.I51Version[i];
                            biblioData.Ipcs.Add(ipc);
                        }
                    }
                    /*-----------------*/
                    /*54 Title*/
                    Title title = new Title()
                    {
                        Text = record.I54,
                        Language = "HR"
                    };
                    biblioData.Titles.Add(title);
                    /*--------*/
                    /*57 Title*/
                    if (record.I57 != null)
                    {
                        Abstract desc = new Abstract()
                        {
                            Text = record.I57,
                            Language = "HR"
                        };
                        biblioData.Abstracts.Add(desc);
                    }
                    /*--------*/
                    /*72 name, country code*/
                    if (record.I72N != null)
                    {
                        biblioData.Inventors = new List<PartyMember>();
                        for (int i = 0; i < record.I72N.Count(); i++)
                        {
                            PartyMember inventor = new PartyMember();
                            inventor.Name = record.I72N[i];
                            if (record.I72A[i] != null)
                                inventor.Address1 = record.I72A[i];
                            if (record.I72C[i] != null)
                                inventor.Country = record.I72C[i];
                            biblioData.Inventors.Add(inventor);
                        }
                    }
                    /*---------------------*/
                    /*73 name, address*/
                    if (record.I73N != null)
                    {
                        biblioData.Assignees = new List<PartyMember>();
                        for (int i = 0; i < record.I73N.Count(); i++)
                        {
                            PartyMember assignee = new PartyMember();
                            assignee.Name = record.I73N[i];
                            //if (record.I73A[i] != null) assignee.Address1 = record.I73A[i];
                            if (record.I73C[i] != null) assignee.Country = record.I73C[i];
                            biblioData.Assignees.Add(assignee);
                        }
                    }
                    /*---------------------*/
                    /*74 name, address, cc*/
                    if (record.I74N != null)
                    {
                        biblioData.Agents = new List<PartyMember>();
                        for (int i = 0; i < record.I74N.Count(); i++)
                        {
                            PartyMember agent = new PartyMember();
                            agent.Name = record.I74N[i];
                            //if (record.I74A[i] != null) agent.Address1 = record.I74A[i];
                            if (record.I74C != null && record.I74C[i] != null) agent.Country = record.I74C[i];
                            biblioData.Agents.Add(agent);
                        }
                    }
                    /*--------------------*/
                    /*96/97 number and date*/
                    if (record.I96D != null || record.I97D != null || record.I96N != null)
                    {
                        biblioData.EuropeanPatents = new List<EuropeanPatent>();
                        EuropeanPatent euPatent = new EuropeanPatent();
                        if (record.I96N != null) euPatent.AppNumber = record.I96N;
                        if (record.I96D != null) euPatent.AppDate = record.I96D;
                        if (record.I97D != null) euPatent.PubDate = record.I97D;
                        biblioData.EuropeanPatents.Add(euPatent);
                    }
                    /*--------------------*/
                    if (!string.IsNullOrEmpty(record.I99))
                    {
                        legalEvent.LegalEvent = new LegalEvent
                        {
                            Note = $"|| Broj ostalih patentnih zahtjeva | {record.I99}",
                            Language = "HR",
                            Translations = new List<NoteTranslation> {
                            new NoteTranslation {Language = "EN", Tr = $"|| The number of other claims | {record.I99}", Type = "note" }
                        }
                        };
                    }
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }

        /*Consensual patent (Granted Consensual Patent)*/
        public static List<Diamond.Core.Models.LegalStatusEvent> ThirdListConvertation(List<ProcessThirdList.ElementOut> elementOuts)
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
                    legalEvent.GazetteName = Path.GetFileName(Diamond_BA_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "5";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "FF";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "BA";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.I11;
                    biblioData.Application.Number = record.I21;
                    if (record.I26 != null) biblioData.Publication.Authority = record.I26;
                    if (record.I22 != null) biblioData.Application.Date = record.I22;
                    /*51 classification*/
                    if (record.I51Class != null)
                    {
                        biblioData.Ipcs = new List<Ipc>();
                        for (int i = 0; i < record.I51Class.Count(); i++)
                        {
                            Ipc ipc = new Ipc();
                            ipc.Class = record.I51Class[i];
                            if (record.I51Version != null && record.I51Version[i] != null) ipc.Date = record.I51Version[i];
                            biblioData.Ipcs.Add(ipc);
                        }
                    }
                    /*-----------------*/
                    /*54 Title*/
                    Title title = new Title()
                    {
                        Text = record.I54,
                        Language = "HR"
                    };
                    biblioData.Titles.Add(title);
                    /*--------*/
                    /*57 Title*/
                    if (record.I57 != null)
                    {
                        Abstract desc = new Abstract()
                        {
                            Text = record.I57,
                            Language = "HR"
                        };
                        biblioData.Abstracts.Add(desc);
                    }
                    /*--------*/
                    /*72 name, country code*/
                    if (record.I75N != null)
                    {
                        biblioData.InvOrApps = new List<PartyMember>();
                        for (int i = 0; i < record.I75N.Count(); i++)
                        {
                            PartyMember invOrApps = new PartyMember();
                            invOrApps.Name = record.I75N[i];
                            if (record.I75A[i] != null) invOrApps.Address1 = record.I75A[i];
                            if (record.I75C[i] != null) invOrApps.Country = record.I75C[i];
                            biblioData.InvOrApps.Add(invOrApps);
                        }
                    }
                    /*---------------------*/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }

        /**Patent Desicions (Article 44)*/
        public static List<Diamond.Core.Models.LegalStatusEvent> FourthListConvertation(List<ProcessFourthList.ElementOut> elementOuts)
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

                    legalEvent.GazetteName = Path.GetFileName(Diamond_BA_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "3";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "FF";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "BA";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.I11;
                    biblioData.Application.Number = record.I21;
                    if (record.I22 != null) biblioData.Application.Date = record.I22;
                    /*30*/
                    if (record.I31 != null)
                    {
                        biblioData.Priorities = new List<Priority>();
                        for (int i = 0; i < record.I31.Count(); i++)
                        {
                            Priority priority = new Priority();
                            priority.Number = record.I31[i];
                            priority.Date = record.I32[i];
                            priority.Country = record.I33[i];
                            //priority.Sequence = i;
                            biblioData.Priorities.Add(priority);
                        }
                    }
                    /*---------------------*/
                    /*45*/
                    if (record.I45 != null) biblioData.DOfPublication = new DOfPublication() { date_45 = record.I45 };
                    /*---------------------*/
                    /*51 classification*/
                    if (record.I51Class != null)
                    {
                        biblioData.Ipcs = new List<Ipc>();
                        for (int i = 0; i < record.I51Class.Count(); i++)
                        {
                            Ipc ipc = new Ipc();
                            //ipc.Class = record.I51Class[i];
                            if (record.I51Class != null && record.I51Class[i] != null) ipc.Class = record.I51Class[i];
                            if (record.I51Version != null && record.I51Version[i] != null) ipc.Date = record.I51Version[i];
                            biblioData.Ipcs.Add(ipc);
                        }
                    }
                    /*-----------------*/
                    /*54 Title*/
                    Title title = new Title()
                    {
                        Text = record.I54,
                        Language = "HR"
                    };
                    biblioData.Titles.Add(title);
                    /*--------*/
                    /*57 Title*/
                    if (record.I57 != null)
                    {
                        Abstract desc = new Abstract()
                        {
                            Text = record.I57,
                            Language = "HR"
                        };
                        biblioData.Abstracts.Add(desc);
                    }
                    /*--------*/
                    /*72 name, country code*/
                    if (record.I72N != null)
                    {
                        biblioData.Inventors = new List<PartyMember>();
                        for (int i = 0; i < record.I72N.Count(); i++)
                        {
                            PartyMember inventor = new PartyMember();
                            inventor.Name = record.I72N[i];
                            if (record.I72A[i] != null) inventor.Address1 = record.I72A[i];
                            if (record.I72C[i] != null) inventor.Country = record.I72C[i];
                            biblioData.Inventors.Add(inventor);
                        }
                    }
                    /*---------------------*/
                    /*73 name, address*/
                    if (record.I73N != null)
                    {
                        biblioData.Assignees = new List<PartyMember>();
                        for (int i = 0; i < record.I73N.Count(); i++)
                        {
                            PartyMember assignee = new PartyMember();
                            assignee.Name = record.I73N[i];
                            //if (record.I73A[i] != null) assignee.Address1 = record.I73A[i];
                            if (record.I73C[i] != null) assignee.Country = record.I73C[i];
                            biblioData.Assignees.Add(assignee);
                        }
                    }
                    /*---------------------*/
                    /*84*/
                    IntConvention intConvention = new IntConvention();
                    /*86*/
                    if (record.I86N != null) intConvention.PctApplNumber = record.I86N;
                    if (record.I86D != null) intConvention.PctApplDate = record.I86D;
                    if (record.I87N != null) intConvention.PctPublNumber = record.I87N;
                    if (record.I87D != null) intConvention.PctPublDate = record.I87D;
                    //if (record.I87K != null) intConvention.PctKind = record.I87K;
                    /*--------------------*/
                    if (intConvention != null)
                    {
                        biblioData.IntConvention = intConvention;
                    }
                    /*-------------------**/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
    }
}
