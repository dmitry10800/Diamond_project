using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Integration;

namespace Diamond_ID
{
    class ConvertToDiamond
    {
        /*Applications Filed*/
        public static List<Diamond.Core.Models.LegalStatusEvent> FirstListConvertation(List<ProcessFirstList.ElementOut> elementOuts)
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

                    legalEvent.GazetteName = Path.GetFileName(Diamond_ID_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    if (Regex.IsMatch(record.I11, @"\d{4}\/S"))
                    {
                        legalEvent.SubCode = "2";
                    }
                    else
                    {
                        legalEvent.SubCode = "1";
                    }
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "AZ";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "ID";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    var biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.I11;
                    biblioData.Publication.Kind = record.I13;
                    biblioData.Application.Number = record.I21;
                    biblioData.Application.Date = record.I22;
                    /*31,32,33*/
                    if (record.I31 != null)
                    {
                        biblioData.Priorities = new List<Priority>();
                        var prioValues = new Priority
                        {
                            Number = record.I31,
                            Date = record.I32,
                            Country = record.I33
                        };
                        biblioData.Priorities.Add(prioValues);
                    }
                    /*45 date of publication*/
                    if (record.I43 != null)
                    {
                        biblioData.Publication.Date = record.I43;
                    }
                    /*---------------------*/
                    /*51*/
                    if (record.I51C != null)
                    {
                        biblioData.Ipcs = new List<Ipc>();
                        for (var i = 0; i < record.I51C.Count(); i++)
                        {
                            var ipcValue = new Ipc();
                            ipcValue.Class = record.I51C[i];
                            try
                            {
                                if (record.I51D != null) ipcValue.Date = record.I51D[i];
                            }
                            catch (Exception)
                            {
                                ipcValue.Date = "";
                            }
                            biblioData.Ipcs.Add(ipcValue);
                        }
                    }
                    /*-------------------*/
                    /*54 Title*/
                    var title = new Title()
                    {
                        Language = "ID",
                        Text = record.I54
                    };
                    biblioData.Titles.Add(title);
                    /*--------*/
                    /*57 description*/
                    biblioData.Abstracts = new List<Abstract>();
                    var description = new Abstract()
                    {
                        Language = "ID",
                        Text = record.I57
                    };
                    biblioData.Abstracts.Add(description);
                    /*--------------*/
                    /*71 name, address, country code*/
                    biblioData.Applicants = new List<PartyMember>();
                    var applicants = new PartyMember()
                    {
                        Name = record.I71N,
                        Address1 = record.I71A,
                        Country = record.I71C
                    };
                    biblioData.Applicants.Add(applicants);
                    /*--------------*/
                    /*72 name, country code*/
                    if (record.I72C != null && record.I72N != null)
                    {
                        biblioData.Inventors = new List<PartyMember>();
                        for (var i = 0; i < record.I72N.Count(); i++)
                        {
                            var inventor = new PartyMember()
                            {
                                Name = record.I72N[i],
                                Country = record.I72C[i]
                            };
                            biblioData.Inventors.Add(inventor);
                        }
                    }
                    /*---------------------*/
                    /*74 name, address, cc*/
                    if (record.I74 != null)
                    {
                        biblioData.Agents = new List<PartyMember>();
                        var agent = new PartyMember()
                        {
                            Name = record.I74
                        };
                        biblioData.Agents.Add(agent);
                    }
                    /*--------------------*/
                    /*Notes*/
                    if (record.I51Notes != null)
                    {
                        legalEvent.LegalEvent = new LegalEvent
                        {
                            Note = "|| " + "National IPC format | " + record.I51Notes
                        };
                    }
                    /**********************/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
    }
}
