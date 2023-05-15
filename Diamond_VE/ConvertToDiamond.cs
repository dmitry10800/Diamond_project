using System.Collections.Generic;
using System.IO;
using System.Linq;
using Integration;

namespace Diamond_VE
{
    class ConvertToDiamond
    {
        /*Applications Filed*/
        public static List<Diamond.Core.Models.LegalStatusEvent> FirstlistConvertation(List<ProcessFirstList.ElementOut> elementOuts)
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

                    legalEvent.GazetteName = Path.GetFileName(Diamond_VE_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "1";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "BA";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "VE";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    var biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.I11;
                    biblioData.Application.Number = record.I21;
                    if (record.I22 != null) biblioData.Application.Date = record.I22;
                    /*30*/
                    if (record.I31 != null)
                    {
                        biblioData.Priorities = new List<Priority>();
                        for (var i = 0; i < record.I31.Count(); i++)
                        {
                            var priority = new Priority();
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
                        for (var i = 0; i < record.I51Class.Count(); i++)
                        {
                            var ipc = new Ipc();
                            //ipc.Class = record.I51Class[i];
                            if (record.I51Class != null && record.I51Class[i] != null) ipc.Class = record.I51Class[i];
                            if (record.I51Version != null && record.I51Version[i] != null) ipc.Date = record.I51Version[i];
                            biblioData.Ipcs.Add(ipc);
                        }
                    }
                    /*-----------------*/
                    /*54 Title*/
                    var title = new Title()
                    {
                        Text = record.I54,
                        Language = "ES"
                    };
                    biblioData.Titles.Add(title);
                    /*--------*/
                    /*57 Title*/
                    if (record.I57 != null)
                    {
                        var desc = new Abstract()
                        {
                            Text = record.I57,
                            Language = "ES"
                        };
                        biblioData.Abstracts.Add(desc);
                    }
                    /*--------*/
                    /*72 name, country code*/
                    if (record.I72N != null)
                    {
                        biblioData.Inventors = new List<PartyMember>();
                        for (var i = 0; i < record.I72N.Count(); i++)
                        {
                            var inventor = new PartyMember();
                            inventor.Name = record.I72N[i];
                            biblioData.Inventors.Add(inventor);
                        }
                    }
                    /*---------------------*/
                    /*73 name, address*/
                    if (record.I73N != null)
                    {
                        biblioData.Assignees = new List<PartyMember>();
                        for (var i = 0; i < record.I73N.Count(); i++)
                        {
                            var assignee = new PartyMember();
                            assignee.Name = record.I73N[i];
                            assignee.Address1 = record.I73A[i];
                            assignee.Country = record.I73C[i];
                            biblioData.Assignees.Add(assignee);
                        }
                    }
                    /*---------------------*/
                    /*74 name, address, cc*/
                    if (record.I74N != null)
                    {
                        biblioData.Agents = new List<PartyMember>();
                        var agent = new PartyMember();
                        agent.Name = record.I74N;
                        biblioData.Agents.Add(agent);
                    }
                    /*--------------------*/
                    ///*84*/
                    //IntConvention intConvention = new IntConvention();
                    ///*86*/
                    //if (record.I86N != null) intConvention.PctApplNumber = record.I86N;
                    //if (record.I86D != null) intConvention.PctApplDate = record.I86D;
                    //if (record.I87N != null) intConvention.PctPublNumber = record.I87N;
                    //if (record.I87D != null) intConvention.PctPublDate = record.I87D;
                    ///*--------------------*/
                    //if (intConvention != null)
                    //{
                    //    biblioData.IntConvention = intConvention;
                    //}
                    ///*--------------------*/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
    }
}
