using System;
using System.Collections.Generic;
using System.IO;
using Integration;

namespace Diamond_NL
{
    class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> SubCombo(List<OutElements.SubCombo> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            var fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            var dateFromName = Methods.GetDateFromGazette(NL_main.CurrentFileName);
            if (elementOuts != null)
            {
                var leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    var legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = Path.GetFileName(NL_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    legalEvent.LegalEvent = new LegalEvent { /*Number = record.LePatNumber,*/ Date = record.DateI24 };
                    if (record.LeNoteValue == "Patent Expired")
                    {
                        legalEvent.SubCode = "13";
                        legalEvent.SectionCode = "MK";
                        legalEvent.LegalEvent.Note = "|| Legal Event Text | Patent Expired";
                    }
                    else if (record.LeNoteValue == "SPC got Invalid due to parent patent lapse")
                    {
                        legalEvent.SubCode = "16";
                        legalEvent.SectionCode = "MM";
                        legalEvent.LegalEvent.Note = "|| Legal Event Text | SPC got Invalid due to parent patent lapse";
                    }
                    else if (record.LeNoteValue == "EPV Ceased Effect")
                    {
                        legalEvent.SubCode = "17";
                        legalEvent.SectionCode = "MP";
                        legalEvent.LegalEvent.Note = "|| Legal Event Text | EPV Ceased Effect";
                    }
                    else if (record.LeNoteValue == "EPV lapsed")
                    {
                        legalEvent.SubCode = "18";
                        legalEvent.SectionCode = "MM";
                        legalEvent.LegalEvent.Note = "|| Legal Event Text | EPV lapsed";
                    }
                    else if (record.LeNoteValue == "European Patent got Revoked")
                    {
                        legalEvent.SubCode = "19";
                        legalEvent.SectionCode = "MF";
                        legalEvent.LegalEvent.Note = "|| Legal Event Text | European Patent got Revoked";
                    }
                    else if (record.LeNoteValue == "SPC Expired")
                    {
                        legalEvent.SubCode = "20";
                        legalEvent.SectionCode = "MK";
                        legalEvent.LegalEvent.Note = "|| Legal Event Text | SPC Expired";
                    }
                    else if (record.LeNoteValue == "NP lapsed")
                    {
                        legalEvent.SubCode = "21";
                        legalEvent.SectionCode = "MM";
                        legalEvent.LegalEvent.Note = "|| Legal Event Text | NP lapsed";
                    }
                    else
                    {
                        Console.WriteLine("Subcode not recognized!");
                        continue;
                    }
                    legalEvent.LegalEvent.Language = "EN";

                    /*Setting Country Code*/
                    legalEvent.CountryCode = "DE";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    var biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.PublNumber;
                    //biblioData.Application.EffectiveDate = record.DateI24;
                    /**********************/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub26(List<OutElements.Sub26> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            var fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            var dateFromName = Methods.GetDateFromGazette(NL_main.CurrentFileName);
            if (elementOuts != null)
            {
                var leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    var legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = Path.GetFileName(NL_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    legalEvent.LegalEvent = new LegalEvent { Date = dateFromName.Replace("-", "/") };
                    /*Setting subcode*/
                    if (record.LeNoteValue == "Change of owner(s) name")
                    {
                        legalEvent.SubCode = "24";
                        legalEvent.SectionCode = "PD";
                        legalEvent.LegalEvent.Note = "|| Legal Event Text | Change of owner(s) name";
                    }
                    else if (record.LeNoteValue == "Assignment")
                    {
                        legalEvent.SubCode = "25";
                        legalEvent.SectionCode = "PC";
                        legalEvent.LegalEvent.Note = "|| Legal Event Text | Assignment";
                    }
                    else if (record.LeNoteValue == "Merge")
                    {
                        legalEvent.SubCode = "26";
                        legalEvent.SectionCode = "PD";
                        legalEvent.LegalEvent.Note = "|| Legal Event Text | Merge";
                    }
                    else if (record.LeNoteValue == "Change of Legal Entity")
                    {
                        legalEvent.SubCode = "27";
                        legalEvent.SectionCode = "PD";
                        legalEvent.LegalEvent.Note = "|| Legal Event Text | Change of Legal Entity";
                    }
                    else if (record.LeNoteValue == "Demerger")
                    {
                        legalEvent.SubCode = "28";
                        legalEvent.SectionCode = "PD";
                        legalEvent.LegalEvent.Note = "|| Legal Event Text | Demerger";
                    }
                    else
                    {
                        Console.WriteLine("Subcode not recognized!");
                        continue;
                    }
                    legalEvent.LegalEvent.Language = "EN";
                    legalEvent.CountryCode = "NL";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    var biblioData = new Biblio();
                    legalEvent.NewBiblio = new Biblio();
                    legalEvent.NewBiblio.Assignees = new List<PartyMember>();
                    biblioData.Publication.Number = record.PublNumber;
                    biblioData.Application.EffectiveDate = record.DateI24;
                    if (record.IntClass.Count > 0)
                    {
                        biblioData.Ipcs = new List<Ipc>();
                        foreach (var item in record.IntClass)
                        {
                            biblioData.Ipcs.Add(new Ipc { Class = item.Trim() });
                        }

                    }
                    /*Notes*/
                    /**********************/
                    if (record.New71Applicant != null && record.New71Applicant != "")
                    {
                        biblioData.Applicants = new List<PartyMember>
                        {
                            new PartyMember
                            {
                                Name = record.New71Applicant
                            }
                        };
                    }
                    if (record.New73Assignee != null)
                    {
                        legalEvent.NewBiblio.Assignees.Add(new PartyMember
                        {
                            Name = record.New73Assignee
                        });
                    }
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
    }
}
