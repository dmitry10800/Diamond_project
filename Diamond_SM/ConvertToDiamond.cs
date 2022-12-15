using System.Collections.Generic;
using System.IO;
using System.Linq;
using Integration;

namespace Diamond_SM
{
    class ConvertToDiamond
    {
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

                    legalEvent.GazetteName = Path.GetFileName(Diamond_SM_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*//*Setting Section Code*/
                    legalEvent.SubCode = "3";
                    legalEvent.SectionCode = "FG";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "SM";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    biblioData.Application.Number = record.I21Number;
                    biblioData.Application.Date = record.I22Date;
                    /*30 priorities*/
                    if (record.I30Prio != null && record.I30Prio.Count > 0)
                    {
                        biblioData.Priorities = new List<Priority>();
                        for (int i = 0; i < record.I30Prio.Count(); i++)
                        {
                            Priority priority = new Priority
                            {
                                Number = record.I30Prio[i].Number,
                                Date = record.I30Prio[i].Date,
                                Country = record.I30Prio[i].Country
                            };
                            biblioData.Priorities.Add(priority);
                        }
                    }
                    /*---------------------*/
                    /*54 Title*/
                    if (record.I54Title != null)
                    {
                        Title title = new Title()
                        {
                            Text = record.I54Title,
                            Language = "IT"
                        };
                        biblioData.Titles.Add(title);
                    }
                    /*54 Title*/
                    if (record.I57Absract != null)
                    {
                        Abstract desc = new Abstract()
                        {
                            //Text = record.I57Absract,
                            Language = "IT"
                        };
                        biblioData.Abstracts.Add(desc);
                    }
                    /*72 name, country code*/
                    if (record.I72Inventors != null && record.I72Inventors.Count > 0)
                    {
                        biblioData.Inventors = new List<PartyMember>();
                        for (int i = 0; i < record.I72Inventors.Count(); i++)
                        {
                            PartyMember inventors = new PartyMember();
                            inventors.Name = record.I72Inventors[i].Name;
                            inventors.Language = "EN";
                            biblioData.Inventors.Add(inventors);
                        }
                    }
                    /*--------------*/
                    /*73 name*/
                    if (record.I73Applicants != null && record.I73Applicants.Count > 0)
                    {
                        biblioData.Assignees = new List<PartyMember>();
                        for (int i = 0; i < record.I73Applicants.Count(); i++)
                        {
                            PartyMember assignees = new PartyMember();
                            assignees.Name = record.I73Applicants[i].Name;
                            assignees.Address1 = record.I73Applicants[i].Address;
                            assignees.Country = record.I73Applicants[i].Country;
                            assignees.Language = "EN";
                            biblioData.Assignees.Add(assignees);
                        }
                    }
                    /*74 name addres*/
                    if (record.I74Agent.Name != null)
                    {
                        biblioData.Agents = new List<PartyMember>();
                        PartyMember agents = new PartyMember();
                        if (record.I74Agent.Name != "")
                        {
                            agents.Name = record.I74Agent.Name;
                            agents.Country = "SM";
                            agents.Language = "EN";
                        }
                        if (record.I74Agent.Address != null && record.I74Agent.Address != "")
                            agents.Address1 = record.I74Agent.Address;

                        biblioData.Agents.Add(agents);
                    }

                    /*96/97 number and date*/
                    if (record.I96Number != null || record.I97 != null)
                    {
                        biblioData.EuropeanPatents = new List<EuropeanPatent>();
                        EuropeanPatent euPatent = new EuropeanPatent();
                        if (record.I96Number != null) euPatent.AppNumber = record.I96Number;
                        if (record.I96Date != null) euPatent.AppDate = record.I96Date;
                        if (record.I97 != null) euPatent.PubNumber = record.I97;
                        biblioData.EuropeanPatents.Add(euPatent);
                    }

                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
        public static List<Diamond.Core.Models.LegalStatusEvent> SecondListConvertation(ProcessSecondtList.ElementOut elementOuts)
        {
            /*list of record for whole gazette chapter*/
            List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            if (elementOuts != null)
            {
                int leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts.RenewalsNumbers)
                {
                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();

                    legalEvent.GazetteName = Path.GetFileName(Diamond_SM_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*//*Setting Section Code*/
                    legalEvent.SubCode = "4";
                    legalEvent.SectionCode = "ND";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "SM";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    biblioData.Application.Number = record;
                    legalEvent.LegalEvent = new LegalEvent
                    {
                        Date = elementOuts.GazetteDate
                    };
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
    }
}
