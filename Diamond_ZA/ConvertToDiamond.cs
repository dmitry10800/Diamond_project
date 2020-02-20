using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integration;

namespace Diamond_ZA
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

                    legalEvent.GazetteName = Path.GetFileName(Diamond_ZA_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "1";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "BA";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "ZA";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Application.Number = record.I21;
                    if (record.I22 != null) biblioData.Application.Date = record.I22;
                    /*30*/
                    if (record.I31 != null && record.I31.Count() == record.I32.Count() && record.I31.Count() == record.I33.Count())
                    {
                        biblioData.Priorities = new List<Priority>();
                        for (int i = 0; i < record.I31.Count(); i++)
                        {
                            Priority priority = new Priority();
                            priority.Number = record.I31[i];
                            priority.Date = record.I32[i];
                            priority.Country = record.I33[i];
                            biblioData.Priorities.Add(priority);
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
                    /*71 name, address, country code*/
                    if (record.I71Name != null)
                    {
                        biblioData.Applicants = new List<PartyMember>();
                        for (int i = 0; i < record.I71Name.Count(); i++)
                        {
                            PartyMember applicants = new PartyMember();
                            if (record.I71Name != null && record.I71Name[i] != null) applicants.Name = record.I71Name[i];
                            if (record.I71Adress != null && record.I71Adress[i] != null) applicants.Address1 = record.I71Adress[i];
                            if (record.I71CountryCode != null && record.I71CountryCode[i] != null) applicants.Country = record.I71CountryCode[i];
                            biblioData.Applicants.Add(applicants);
                        }
                    }
                    /*--------------*/
                    /*72 name, country code*/
                    if (record.I72 != null)
                    {
                        biblioData.Inventors = new List<PartyMember>();
                        for (int i = 0; i < record.I72.Count(); i++)
                        {
                            PartyMember inventor = new PartyMember();
                            inventor.Name = record.I72[i];
                            biblioData.Inventors.Add(inventor);
                        }
                    }
                    /*---------------------*/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }

        /*Complete Specifications*/
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
                    legalEvent.GazetteName = Path.GetFileName(Diamond_ZA_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "3";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "FZ";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "ZA";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Application.Number = record.I21;
                    biblioData.Application.Date = record.I22;

                    /*30*/
                    if (record.I31 != null && record.I32 != null && record.I33 != null)
                    {
                        biblioData.Priorities = new List<Priority>();
                        for (int i = 0; i < record.I31.Count(); i++)
                        {
                            Priority priority = new Priority();
                            if (record.I33[i] != null) priority.Country = record.I33[i];
                            if (record.I32[i] != null) priority.Date = record.I32[i];
                            if (record.I31[i] != null) priority.Number = record.I31[i];
                            biblioData.Priorities.Add(priority);
                        }
                    }
                    /*43 date of publication*/
                    if (record.I43 != null)
                    {
                        //biblioData.DOfPublication = new DOfPublication() { date_43 = record.I43 };
                        if (record.I43 != null)
                        {
                            biblioData.Publication.Date = record.I43;
                        }
                        //biblioData.DOfPublication.date_43 = record.I43;
                    }
                    /*---------------------*/
                    /*51*/
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
                    /*---------------------*/
                    /*54 Title*/
                    if (record.I54 != null)
                    {
                        Title title = new Title()
                        {
                            Language = "EN",
                            Text = record.I54
                        };
                        biblioData.Titles.Add(title);
                    }
                    /*--------*/
                    /*57 description*/
                    if (record.I00 != null)
                    {
                        biblioData.Abstracts = new List<Abstract>();
                        Abstract description = new Abstract()
                        {
                            Language = "EN",
                            Text = record.I00
                        };
                        biblioData.Abstracts.Add(description);
                    }
                    /*--------------*/
                    /*71 name, address, country code*/
                    biblioData.Applicants = new List<PartyMember>();
                    PartyMember applicants = new PartyMember()
                    {
                        Name = record.I71
                    };
                    biblioData.Applicants.Add(applicants);
                    /*--------------*/
                    /*72 name, country code*/
                    if (record.I72 != null)
                    {
                        biblioData.Inventors = new List<PartyMember>();
                        PartyMember inventor = new PartyMember()
                        {
                            Name = record.I72
                        };
                        biblioData.Inventors.Add(inventor);
                    }
                    /*---------------------*/

                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
    }
}
