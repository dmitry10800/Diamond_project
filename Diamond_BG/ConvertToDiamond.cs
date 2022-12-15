using System.Collections.Generic;
using System.IO;
using System.Linq;
using Integration;

namespace Diamond_BG
{
    class ConvertToDiamond
    {
        /*Applications Filed*/
        public static List<Diamond.Core.Models.LegalStatusEvent> GrantedEuropeanPatentsConvertation(List<ProcessGrantedEuropeanPatents.ElementOut> elementOuts)
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

                    legalEvent.GazetteName = Path.GetFileName(Diamond_BG_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "5";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "FG";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "BG";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.I11;
                    biblioData.Publication.Kind = record.I13;
                    biblioData.Application.EffectiveDate = record.I24; // is 24 inid - publication date?
                    /*30*/
                    if (record.I31 != null && record.I31.Count() == record.I32.Count() && record.I31.Count() == record.I33.Count())
                    {
                        biblioData.Priorities = new List<Priority>();
                        for (int i = 0; i < record.I31.Count(); i++)
                        {
                            Priority priority = new Priority();
                            priority.Country = record.I33[i];
                            priority.Date = record.I32[i];
                            priority.Number = record.I31[i];
                            priority.Sequence = i;
                            biblioData.Priorities.Add(priority);
                        }
                    }
                    /*---------------------*/
                    /*51 classification*/
                    if (record.I51D != null && record.I51N != null && record.I51N.Count() == record.I51D.Count())
                    {
                        biblioData.Ipcs = new List<Ipc>();
                        for (int i = 0; i < record.I51N.Count(); i++)
                        {
                            Ipc ipc = new Ipc();
                            ipc.Class = record.I51N[i];
                            ipc.Date = record.I51D[i];
                            biblioData.Ipcs.Add(ipc);
                        }
                    }
                    /*-----------------*/
                    /*54 Title*/
                    Title title = new Title()
                    {
                        Language = "BG",
                        Text = record.I54
                    };
                    biblioData.Titles.Add(title);
                    /*--------*/
                    /*57 Title*/
                    if (record.I57 != null)
                    {
                        Abstract desc = new Abstract()
                        {
                            Text = record.I57,
                            Language = "BG"
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
                    if (record.I73N != null && record.I73A != null && record.I73C != null)
                    {
                        biblioData.Assignees = new List<PartyMember>();
                        for (int i = 0; i < record.I73N.Count(); i++)
                        {
                            PartyMember assignee = new PartyMember()
                            {
                                Name = record.I73N[i],
                                Address1 = record.I73A[i],
                                Country = record.I73C[i]
                            };
                            biblioData.Assignees.Add(assignee);
                        }
                    }
                    /*---------------------*/
                    /*74 name, address, cc*/
                    if (record.I74N != null && record.I74A != null && record.I74C != null)
                    {
                        biblioData.Agents = new List<PartyMember>();
                        for (int i = 0; i < record.I74N.Count(); i++)
                        {
                            PartyMember agent = new PartyMember()
                            {
                                Name = record.I74N[i],
                                Address1 = record.I74A[i],
                                Country = record.I74C[i]
                            };
                            biblioData.Agents.Add(agent);
                        }
                    }
                    /*--------------------*/
                    /*84*/
                    IntConvention intConvention = new IntConvention();
                    /*86*/
                    if (record.I86 != null)
                    {
                        intConvention.PctApplNumber = record.I86;
                    }
                    if (record.I87N != null && record.I87D != null)
                    {
                        intConvention.PctPublNumber = record.I87N;
                        intConvention.PctPublDate = record.I87D;
                    }
                    /*--------------------*/
                    if (intConvention != null)
                    {
                        biblioData.IntConvention = intConvention;
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
                    /*-------------------**/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
        /*Applications Filed*/
        public static List<Diamond.Core.Models.LegalStatusEvent> ApplEuropeanPatentsConvertation(List<ProcessEuropeanPatentApplications.ElementOut> elementOuts)
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

                    legalEvent.GazetteName = Path.GetFileName(Diamond_BG_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "4";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "BA";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "BG";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.I11;
                    biblioData.Publication.Kind = record.I13;
                    biblioData.Application.EffectiveDate = record.I24; // is 24 inid - publication date?
                    /*30*/
                    if (record.I31 != null && record.I31.Count() == record.I32.Count() && record.I31.Count() == record.I33.Count())
                    {
                        biblioData.Priorities = new List<Priority>();
                        for (int i = 0; i < record.I31.Count(); i++)
                        {
                            Priority priority = new Priority();
                            priority.Country = record.I33[i];
                            priority.Date = record.I32[i];
                            priority.Number = record.I31[i];
                            priority.Sequence = i;
                            biblioData.Priorities.Add(priority);
                        }
                    }
                    /*---------------------*/
                    /*51 classification*/
                    if (record.I51D != null && record.I51N != null && record.I51N.Count() == record.I51D.Count())
                    {
                        biblioData.Ipcs = new List<Ipc>();
                        for (int i = 0; i < record.I51N.Count(); i++)
                        {
                            Ipc ipc = new Ipc();
                            ipc.Class = record.I51N[i];
                            ipc.Date = record.I51D[i];
                            biblioData.Ipcs.Add(ipc);
                        }
                    }
                    /*-----------------*/
                    /*54 Title*/
                    Title title = new Title()
                    {
                        Language = "BG",
                        Text = record.I54
                    };
                    biblioData.Titles.Add(title);
                    /*--------*/
                    /*57 Title*/
                    if (record.I57 != null)
                    {
                        Abstract desc = new Abstract()
                        {
                            Text = record.I57,
                            Language = "BG"
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
                    if (record.I73N != null && record.I73A != null && record.I73C != null)
                    {
                        biblioData.Assignees = new List<PartyMember>();
                        for (int i = 0; i < record.I73N.Count(); i++)
                        {
                            PartyMember assignee = new PartyMember()
                            {
                                Name = record.I73N[i],
                                Address1 = record.I73A[i],
                                Country = record.I73C[i]
                            };
                            biblioData.Assignees.Add(assignee);
                        }
                    }
                    /*---------------------*/
                    /*74 name, address, cc*/
                    if (record.I74N != null && record.I74A != null && record.I74C != null)
                    {
                        biblioData.Agents = new List<PartyMember>();
                        for (int i = 0; i < record.I74N.Count(); i++)
                        {
                            PartyMember agent = new PartyMember()
                            {
                                Name = record.I74N[i],
                                Address1 = record.I74A[i],
                                Country = record.I74C[i]
                            };
                            biblioData.Agents.Add(agent);
                        }
                    }
                    /*--------------------*/
                    /*84*/
                    IntConvention intConvention = new IntConvention();
                    /*86*/
                    if (record.I86 != null)
                    {
                        intConvention.PctApplNumber = record.I86;
                    }
                    if (record.I87N != null && record.I87D != null)
                    {
                        intConvention.PctPublNumber = record.I87N;
                        intConvention.PctPublDate = record.I87D;
                    }
                    /*--------------------*/
                    if (intConvention != null)
                    {
                        biblioData.IntConvention = intConvention;
                    }
                    /*--------------------*/
                    /*96/97 number and date*/
                    if (record.I96N != null || record.I96D != null || record.I97N != null || record.I96D != null)
                    {
                        biblioData.EuropeanPatents = new List<EuropeanPatent>();
                        EuropeanPatent euPatent = new EuropeanPatent()
                        {
                            AppNumber = record.I96N,
                            AppDate = record.I96D,
                            PubNumber = record.I97N,
                            PubDate = record.I97D
                        };
                        biblioData.EuropeanPatents.Add(euPatent);
                    }

                    /*-------------------**/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
        /*Published Applications For Inventions*/
        public static List<Diamond.Core.Models.LegalStatusEvent> PublishedAppForInventionConvertation(List<ProcessPublishedApplicationsForInventions.ElementOut> elementOuts)
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

                    legalEvent.GazetteName = Path.GetFileName(Diamond_BG_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "1";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "BA";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "BG";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Application.Number = record.I21;
                    biblioData.Application.Date = record.I22;
                    /*51 classification*/
                    if (record.I51D != null && record.I51N != null && record.I51N.Count() == record.I51D.Count())
                    {
                        biblioData.Ipcs = new List<Ipc>();
                        for (int i = 0; i < record.I51N.Count(); i++)
                        {
                            Ipc ipc = new Ipc();
                            ipc.Class = record.I51N[i];
                            ipc.Date = record.I51D[i];
                            biblioData.Ipcs.Add(ipc);
                        }
                    }
                    /*-----------------*/
                    /*54 Title*/
                    Title title = new Title()
                    {
                        Language = "BG",
                        Text = record.I54
                    };
                    biblioData.Titles.Add(title);
                    /*--------*/
                    /*57 Title*/
                    if (record.I57 != null)
                    {
                        Abstract desc = new Abstract()
                        {
                            Text = record.I57,
                            Language = "BG"
                        };
                        biblioData.Abstracts.Add(desc);
                    }
                    /*--------*/
                    /*71 name, address*/
                    if (record.I71N != null)
                    {
                        biblioData.Applicants = new List<PartyMember>();
                        for (int i = 0; i < record.I71N.Count(); i++)
                        {
                            PartyMember applicant = new PartyMember()
                            {
                                Name = record.I71N[i],
                                Address1 = record.I71A[i]
                            };
                            biblioData.Applicants.Add(applicant);
                        }
                    }
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
                    /*74 name, address, cc*/
                    if (record.I74N != null)
                    {
                        biblioData.Agents = new List<PartyMember>();
                        for (int i = 0; i < record.I74N.Count(); i++)
                        {
                            PartyMember agent = new PartyMember();
                            agent.Name = record.I74N[i];
                            if (record.I74A != null && record.I74A[i] != null) agent.Address1 = record.I74A[i];
                            if (record.I74C != null && record.I74C[i] != null) agent.Country = record.I74C[i];
                            biblioData.Agents.Add(agent);
                        }
                    }
                    /*-------------------*/
                    /*86*/
                    IntConvention intConvention = new IntConvention();
                    if (record.I86N != null && record.I86D != null)
                    {
                        intConvention.PctApplNumber = record.I86N;
                        intConvention.PctApplDate = record.I86D;
                    }
                    /*--------------------*/
                    /*87*/
                    if (record.I87N != null && record.I87D != null)
                    {
                        intConvention.PctPublNumber = record.I87N;
                        intConvention.PctPublDate = record.I87D;
                    }
                    /*86/87*/
                    if (intConvention != null)
                    {
                        biblioData.IntConvention = intConvention;
                    }
                    /*--------------------*/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
        /*Applications Filed*/
        public static List<Diamond.Core.Models.LegalStatusEvent> GrantedPatentsForInventionsConvertation(List<ProcessGrantedPatentsForInventions.ElementOut> elementOuts)
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

                    legalEvent.GazetteName = Path.GetFileName(Diamond_BG_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "3";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "FG";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "BG";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.I11;
                    biblioData.Publication.Kind = record.I13;
                    //biblioData.Publication.Date = record.I24; //is 24 inid publication date?
                    biblioData.Application.Number = record.I21;
                    biblioData.Application.Date = record.I22;
                    if (record.I24 != null)
                        biblioData.Application.EffectiveDate = record.I24;
                    /*30*/
                    if (record.I31 != null && record.I31.Count() == record.I32.Count() && record.I31.Count() == record.I33.Count())
                    {
                        biblioData.Priorities = new List<Priority>();
                        for (int i = 0; i < record.I31.Count(); i++)
                        {
                            Priority priority = new Priority();
                            priority.Country = record.I33[i];
                            priority.Date = record.I32[i];
                            priority.Number = record.I31[i];
                            biblioData.Priorities.Add(priority);
                        }
                    }
                    /*---------------------*/
                    /*41 date*/
                    if (record.I41 != null)
                    {
                        biblioData.DOfPublication = new DOfPublication() { date_41 = record.I41 };
                    }
                    /*51 classification*/
                    if (record.I51D != null && record.I51N != null && record.I51N.Count() == record.I51D.Count())
                    {
                        biblioData.Ipcs = new List<Ipc>();
                        for (int i = 0; i < record.I51N.Count(); i++)
                        {
                            Ipc ipc = new Ipc();
                            ipc.Class = record.I51N[i];
                            ipc.Date = record.I51D[i];
                            biblioData.Ipcs.Add(ipc);
                        }
                    }
                    /*-----------------*/
                    /*54 Title*/
                    Title title = new Title()
                    {
                        Language = "BG",
                        Text = record.I54
                    };
                    biblioData.Titles.Add(title);
                    /*--------*/
                    /*57 Title*/
                    if (record.I57 != null)
                    {
                        Abstract desc = new Abstract()
                        {
                            Text = record.I57,
                            Language = "BG"
                        };
                        biblioData.Abstracts.Add(desc);
                    }
                    /*--------*/
                    /*73 name, addressб сс*/
                    if (record.I73N != null)
                    {
                        biblioData.Assignees = new List<PartyMember>();
                        for (int i = 0; i < record.I73N.Count(); i++)
                        {
                            PartyMember assignee = new PartyMember();
                            assignee.Name = record.I73N[i];
                            assignee.Address1 = record.I73A[i];
                            if (record.I73C != null)
                            {
                                assignee.Country = record.I73C[i];
                            }
                            biblioData.Assignees.Add(assignee);
                        }
                    }
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
                    /*74 name, address, cc*/
                    if (record.I74N != null)
                    {
                        biblioData.Agents = new List<PartyMember>();
                        for (int i = 0; i < record.I74N.Count(); i++)
                        {
                            PartyMember agent = new PartyMember();
                            agent.Name = record.I74N[i];
                            agent.Address1 = record.I74A[i];
                            if (record.I74C[i] != null)
                            {
                                agent.Country = record.I74C[i];
                            }
                            biblioData.Agents.Add(agent);
                        }
                    }
                    /*-------------------*/
                    /*86, 87*/
                    IntConvention intConvention = new IntConvention();
                    if (record.I86Number != null)
                    {
                        intConvention.PctApplNumber = record.I86Number;
                        if (record.I86Number != null) intConvention.PctApplDate = record.I86Date;
                    }
                    if (record.I87Number != null)
                    {
                        intConvention.PctPublNumber = record.I87Number;
                        if (record.I87Date != null) intConvention.PctPublDate = record.I87Date;
                    }
                    /*--------------------*/
                    if (intConvention != null)
                    {
                        biblioData.IntConvention = intConvention;
                    }
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
        /*Applications Filed*/
        public static List<Diamond.Core.Models.LegalStatusEvent> GrantedCertificatesForRegUMConvertation(List<ProcessGrantedCertificatesForRegUtilityModels.ElementOut> elementOuts)
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

                    legalEvent.GazetteName = Path.GetFileName(Diamond_BG_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "7";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "FG";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "BG";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.I11;
                    biblioData.Publication.Kind = record.I13;
                    biblioData.Application.Number = record.I21;
                    biblioData.Application.Date = record.I22;
                    if (record.I24 != null)
                        biblioData.Application.EffectiveDate = record.I24;
                    /*30*/
                    if (record.I31N != null && record.I31N.Count() == record.I32D.Count() && record.I31N.Count() == record.I33C.Count())
                    {
                        biblioData.Priorities = new List<Priority>();
                        for (int i = 0; i < record.I31N.Count(); i++)
                        {
                            Priority priority = new Priority();
                            priority.Country = record.I33C[i];
                            priority.Date = record.I32D[i];
                            priority.Number = record.I31N[i];
                            biblioData.Priorities.Add(priority);
                        }
                    }
                    /*---------------------*/
                    /*51 classification*/
                    if (record.I51D != null && record.I51N != null && record.I51N.Count() == record.I51D.Count())
                    {
                        biblioData.Ipcs = new List<Ipc>();
                        for (int i = 0; i < record.I51N.Count(); i++)
                        {
                            Ipc ipc = new Ipc();
                            ipc.Class = record.I51N[i];
                            ipc.Date = record.I51D[i];
                            biblioData.Ipcs.Add(ipc);
                        }
                    }
                    /*-----------------*/
                    /*54 Title*/
                    Title title = new Title()
                    {
                        Language = "BG",
                        Text = record.I54
                    };
                    biblioData.Titles.Add(title);
                    /*--------*/
                    /*57 Title*/
                    if (record.I57 != null)
                    {
                        Abstract desc = new Abstract()
                        {
                            Text = record.I57,
                            Language = "BG"
                        };
                        biblioData.Abstracts.Add(desc);
                    }
                    /*--------*/
                    /*67*/
                    if (record.I67N != null)
                    {
                        biblioData.Related = new List<RelatedDocument>();
                        RelatedDocument document = new RelatedDocument();
                        document.Number = record.I67N;
                        if (record.I67D != null)
                        {
                            document.Date = record.I67D;
                        }
                    }
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
                    if (record.I73N != null)
                    {
                        biblioData.Assignees = new List<PartyMember>();
                        for (int i = 0; i < record.I73N.Count(); i++)
                        {
                            PartyMember assignee = new PartyMember();
                            assignee.Name = record.I73N[i];
                            if (record.I73A != null && record.I73A[i] != null) assignee.Address1 = record.I73A[i];
                            if (record.I73C != null && record.I73C[i] != null) assignee.Country = record.I73C[i];
                            else assignee.Country = "BG";
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
                            if (record.I74A != null && record.I74A[i] != null) agent.Address1 = record.I74A[i];
                            if (record.I74C != null && record.I74C[i] != null) agent.Country = record.I74C[i];
                            biblioData.Agents.Add(agent);
                        }
                    }
                    /*--------------------*/
                    /*84*/
                    IntConvention intConvention = new IntConvention();
                    /*86*/
                    if (record.I86N != null && record.I86D != null)
                    {
                        intConvention.PctApplNumber = record.I86N;
                        intConvention.PctApplDate = record.I86D;
                    }
                    if (record.I87N != null && record.I87D != null)
                    {
                        intConvention.PctPublNumber = record.I87N;
                        intConvention.PctPublDate = record.I87D;
                    }
                    /*--------------------*/
                    if (intConvention != null)
                    {
                        biblioData.IntConvention = intConvention;
                    }
                    /*Notes*/
                    if (record.INote != null)
                    {
                        legalEvent.LegalEvent = new LegalEvent();
                        legalEvent.LegalEvent.Note = "|| " + record.INote;
                        legalEvent.LegalEvent.Language = "BG";

                        NoteTranslation noteTransl = new NoteTranslation();
                        noteTransl.Tr = record.INote;
                        noteTransl.Language = "EN";
                        noteTransl.Type = "";
                        legalEvent.LegalEvent.Translations.Add(noteTransl);
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
