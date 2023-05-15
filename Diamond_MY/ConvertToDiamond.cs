using System.Collections.Generic;
using System.IO;
using System.Linq;
using Integration;

namespace Diamond_MY
{
    class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> FirstListConvertation(List<ProcessGrants.ElementOut> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            var fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            if (elementOuts != null)
            {
                var leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    var legalEvent = new Diamond.Core.Models.LegalStatusEvent();

                    legalEvent.GazetteName = Path.GetFileName(MY_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*//*Setting Section Code*/
                    legalEvent.SubCode = "1";
                    legalEvent.SectionCode = "FG";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "MY";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    var biblioData = new Biblio();
                    biblioData.Application.Number = record.I21App;
                    biblioData.Application.Date = record.I22Date;
                    biblioData.Publication.Number = record.I11Values.Number;
                    biblioData.Publication.Kind = record.I11Values.Kind;
                    biblioData.DOfPublication = new DOfPublication { date_47 = record.I47Date };
                    /*30 priorities*/
                    if (record.I30Prio != null && record.I30Prio.Count > 0)
                    {
                        biblioData.Priorities = new List<Priority>();
                        for (var i = 0; i < record.I30Prio.Count(); i++)
                        {
                            var priority = new Priority
                            {
                                Number = record.I30Prio[i].Number,
                                Date = record.I30Prio[i].Date,
                                Country = record.I30Prio[i].Country
                            };
                            biblioData.Priorities.Add(priority);
                        }
                    }
                    /*---------------------*/
                    /*51 international classification*/
                    if (record.I51IntCl != null && record.I51IntCl.Count > 0)
                    {
                        biblioData.Ipcs = new List<Ipc>();
                        for (var i = 0; i < record.I51IntCl.Count(); i++)
                        {
                            var ipcValue = new Ipc();
                            if (record.I51IntCl[i].Date != null) ipcValue.Date = record.I51IntCl[i].Date;
                            ipcValue.Class = record.I51IntCl[i].Number;
                            if (record.I51IntCl[i].DigitClass != null) ipcValue.Edition = int.Parse(record.I51IntCl[i].DigitClass);
                            biblioData.Ipcs.Add(ipcValue);
                        }
                    }
                    /*---------------------*/
                    /*54 Title*/
                    if (record.I54Title != null)
                    {
                        var title = new Title()
                        {
                            Text = record.I54Title,
                            Language = "EN"
                        };
                        biblioData.Titles.Add(title);
                    }
                    /*54 Title*/
                    if (record.I57Absract != null)
                    {
                        var desc = new Abstract()
                        {
                            Text = record.I57Absract,
                            Language = "EN"
                        };
                        biblioData.Abstracts.Add(desc);
                    }
                    /*72 name, country code*/
                    if (record.I72Inventors != null && record.I72Inventors.Count > 0)
                    {
                        biblioData.Inventors = new List<PartyMember>();
                        for (var i = 0; i < record.I72Inventors.Count(); i++)
                        {
                            var inventors = new PartyMember();
                            inventors.Name = record.I72Inventors[i].Name;
                            inventors.Language = "EN";
                            biblioData.Inventors.Add(inventors);
                        }
                    }
                    /*--------------*/
                    /*73 name*/
                    if (record.I73Owners != null && record.I73Owners.Count > 0)
                    {
                        biblioData.Assignees = new List<PartyMember>();
                        for (var i = 0; i < record.I73Owners.Count(); i++)
                        {
                            var assignees = new PartyMember();
                            assignees.Name = record.I73Owners[i].Name;
                            assignees.Address1 = record.I73Owners[i].Address;
                            assignees.Country = record.I73Owners[i].Country;
                            assignees.Language = "EN";
                            biblioData.Assignees.Add(assignees);
                        }
                    }
                    /*74 name addres*/
                    if (record.I74Agent.Name != null)
                    {
                        biblioData.Agents = new List<PartyMember>();
                        var agents = new PartyMember();
                        agents.Name = record.I74Agent.Name;
                        agents.Address1 = record.I74Agent.Address;
                        agents.Country = "MY";
                        agents.Language = "EN";
                        biblioData.Agents.Add(agents);
                    }
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
    }
}
