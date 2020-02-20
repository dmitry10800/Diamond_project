using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Integration;

namespace Diamond_GC
{
    class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> FirstListConvertation(List<ProcessGrantedPatents.ElementOut> elementOuts)
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

                    legalEvent.GazetteName = Path.GetFileName(Diamond_GC_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*//*Setting Section Code*/
                    legalEvent.SubCode = "3";
                    legalEvent.SectionCode = "FG4A / BA4A";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "GC";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Application.Number = record.I21;
                    biblioData.Publication.Number = record.I11;
                    biblioData.Publication.Language = record.I12; ///<summary>not displayed in UI</summary>
                    if (record.I22 != null) biblioData.Application.Date = record.I22.Trim();
                    /*30*/
                    if (record.I31Number != null && record.I31Number.Count() == record.I32Date.Count() && record.I31Number.Count() == record.I33State.Count())
                    {
                        biblioData.Priorities = new List<Priority>();
                        for (int i = 0; i < record.I31Number.Count(); i++)
                        {
                            Priority priority = new Priority();
                            priority.Number = record.I31Number[i];
                            priority.Date = record.I32Date[i];
                            priority.Country = record.I33State[i];
                            biblioData.Priorities.Add(priority);
                        }
                    }
                    /*---------------------*/
                    /*45*/
                    if (record.I45 != null)
                    {
                        biblioData.DOfPublication = new DOfPublication { date_45 = record.I45 };
                    }
                    /*51 international classification*/
                    if (record.I51ClasNumber != null && record.I51ClasNumber.Count() > 0)
                    {
                        biblioData.Ipcs = new List<Ipc>();
                        for (int i = 0; i < record.I51ClasNumber.Count(); i++)
                        {
                            Ipc ipcValue = new Ipc();
                            ipcValue.Class = record.I51ClasNumber[i].Replace("//", "/");
                            if (record.I51VersionYear != null) ipcValue.Date = record.I51VersionYear;
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
                    /*---------------------*/
                    /*56*/
                    //if (record.I56 != null)
                    //{
                    //    biblioData.NonPatentCitations = new List<NonPatentCitation>();
                    //    NonPatentCitation citation = new NonPatentCitation { Text = record.I56 };
                    //    biblioData.NonPatentCitations.Add(citation);
                    //}
                    /*57*/
                    if (record.I57 != null)
                    {
                        biblioData.Abstracts = new List<Abstract>();
                        Abstract desc = new Abstract { Text = record.I57, Language = "EN" };
                        biblioData.Abstracts.Add(desc);
                    }
                    /*72 name*/
                    if (record.I72N != null)
                    {
                        biblioData.Inventors = new List<PartyMember>();
                        for (int i = 0; i < record.I72N.Count(); i++)
                        {
                            PartyMember Inventors = new PartyMember();
                            if (record.I72N != null && record.I72N[i] != null)
                                Inventors.Name = record.I72N[i];
                            biblioData.Inventors.Add(Inventors);
                        }
                    }
                    /*---------------------*/
                    /*73 Name, Addr, Country*/
                    if (record.I73N != null && record.I73N.Count() > 0)
                    {
                        biblioData.Assignees = new List<PartyMember>();
                        for (int i = 0; i < record.I73N.Count(); i++)
                        {
                            PartyMember member = new PartyMember();
                            member.Name = record.I73N[i];
                            if (record.I73A != null && record.I73A[i] != null) member.Address1 = record.I73A[i];
                            if (record.I73C != null && record.I73C[i] != null) member.Country = record.I73C[i];
                            biblioData.Assignees.Add(member);
                        }
                    }
                    /*----------------------*/
                    /*74 name and address*/
                    if (record.I74 != null)
                    {
                        biblioData.Agents = new List<PartyMember>();
                        PartyMember Agents = new PartyMember();
                        Agents.Name = record.I74;
                        biblioData.Agents.Add(Agents);
                    }
                    /*---------------------*/
                    /*Note field filing*/
                    if (record.INotes != null && record.INotes.Count > 0)
                    {
                        legalEvent.LegalEvent = new LegalEvent();
                        legalEvent.LegalEvent.Note = Regex.Replace("|| " + string.Join(" || ", record.INotes.Select(x => x.Replace(":", " | "))), @"\s+", " ");

                    }
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }

        /*Leval Event Convertation*/
        public static List<Diamond.Core.Models.LegalStatusEvent> LegalEventConvertation(List<ProcessLegalEvents.ElementOut> elementOuts, string subcode)
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

                    legalEvent.GazetteName = Path.GetFileName(Diamond_GC_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    if (subcode == "sub14")
                    {
                        /*Setting subcode*//*Setting Section Code*/
                        legalEvent.SubCode = "14";
                        legalEvent.SectionCode = "FD";
                    }
                    else if (subcode == "sub26")
                    {
                        /*Setting subcode*//*Setting Section Code*/
                        legalEvent.SubCode = "26";
                        legalEvent.SectionCode = "FC";
                    }
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "GC";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    legalEvent.LegalEvent = new LegalEvent();
                    /*Elements output*/
                    biblioData.Application.Number = record.I21;
                    biblioData.Application.Date = record.I22;
                    legalEvent.LegalEvent.Number = record.I21;
                    legalEvent.LegalEvent.Date = record.EventDate;
                    legalEvent.LegalEvent.Language = "EN";
                    biblioData.Titles.Add(new Title { Text = record.Title, Language = "EN" });
                    /*Note field filing*/
                    if (record.Note != null)
                    {
                        legalEvent.LegalEvent.Note = /*"|| Decision No | " +*/ record.Note;
                    }
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub06LegalEventConvertation(List<ProcessLegalEvents.Sub06v2ElementOut> elementOuts, string subcode)
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

                    legalEvent.GazetteName = Path.GetFileName(Diamond_GC_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    legalEvent.SubCode = "6";
                    legalEvent.SectionCode = "MM";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "GC";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    legalEvent.LegalEvent = new LegalEvent();
                    /*Elements output*/
                    biblioData.Titles.Add(new Title { Language = "EN", Text = record.I54 });
                    biblioData.Publication.Number = record.I11;
                    legalEvent.LegalEvent.Language = "EN";
                    legalEvent.LegalEvent.Date = record.DecisionDate;
                    legalEvent.LegalEvent.Note = "|| Decision No | " + record.DecisionNo;
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
    }
}
