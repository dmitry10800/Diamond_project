using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integration;

namespace Diamond_IL
{
    class ConvertToDiamond
    {
        /*Applications Filed*/
        /*Applications published under section 16A - Subcode 1 (data from IL_xxaabb_xxA)*/
        /*Applications accepted under section 26 - Subcode 3 (data from IL_xxaabb_xx)*/
        public static List<Diamond.Core.Models.LegalStatusEvent> FirstListConvertation(List<Process_Apps_INID.ElementOut> elementOuts)
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

                    legalEvent.GazetteName = Path.GetFileName(Diamond_IL_main.CurrentFileName.Replace("_APPA.txt", ".pdf").Replace("_APPB.txt", ".pdf").Replace(".txt", ".pdf"));
                    /*Setting subcode*//*Setting Section Code*/
                    if (Diamond_IL_main.CurrentFileName.Contains("_APPA.txt"))
                    {
                        legalEvent.SubCode = "3";
                        legalEvent.SectionCode = "BA";
                    }
                    else
                    if (Diamond_IL_main.CurrentFileName.Contains("_APPB.txt"))
                    {
                        legalEvent.SubCode = "1";
                        legalEvent.SectionCode = "AZ";
                    }
                    else
                    {
                        Console.WriteLine("Section code not specified!");
                        Console.ReadKey();
                    }
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "IL";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.I21;
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
                    /*51 international classification*/
                    if (record.I51Version != null && record.I51Number != null && record.I51Number.Count() > 0)
                    {
                        biblioData.Ipcs = new List<Ipc>();
                        for (int i = 0; i < record.I51Number.Count(); i++)
                        {
                            Ipc ipcValue = new Ipc
                            {
                                Date = record.I51Version,
                                Class = record.I51Number[i].Replace("//", "/")
                            };
                            biblioData.Ipcs.Add(ipcValue);
                        }
                    }
                    /*---------------------*/
                    /*54 Title*/
                    if (record.I54Hebrew != null)
                    {
                        Title title = new Title()
                        {
                            Text = record.I54Hebrew,
                            Language = "HE"
                        };
                        biblioData.Titles.Add(title);
                    }
                    if (record.I54Eng != null)
                    {
                        Title title = new Title()
                        {
                            Text = record.I54Eng,
                            Language = "EN"
                        };
                        biblioData.Titles.Add(title);
                    }
                    /*62*/
                    if (record.I62 != null)
                    {
                        List<RelatedDocument> citations = new List<RelatedDocument>();
                        RelatedDocument document = new RelatedDocument { Number = record.I62, Type = "62" };
                        citations.Add(document);
                        biblioData.Related = citations;
                    }
                    /*71 name, country code*/
                    if (record.I71NameIL != null)
                    {
                        biblioData.Applicants = new List<PartyMember>();
                        for (int i = 0; i < record.I71NameIL.Count(); i++)
                        {
                            PartyMember applicants = new PartyMember();
                            List<Translation> lang = new List<Translation>();
                            //Translation lang = new Translation();
                            if (record.I71NameIL != null && record.I71NameIL[i] != null) applicants.Name = record.I71NameIL[i].Replace("\n", " ");
                            if (record.I71AddrIL != null && record.I71AddrIL[i] != null) applicants.Address1 = record.I71AddrIL[i];
                            if (record.I71CountryIL != null && record.I71CountryIL[i] != null) applicants.Country = record.I71CountryIL[i];
                            else if (record.I71CountryENG != null && record.I71CountryENG[i] != null) applicants.Country = record.I71CountryENG[i];
                            applicants.Language = "HE";
                            if (record.I71NameENG != null && record.I71NameIL.Count() == record.I71NameENG.Count())
                            {
                                Translation translation = new Translation
                                {
                                    Language = "EN",
                                    Type = "71",
                                    TrName = record.I71NameENG[i].Replace("\n", " ")
                                };
                                lang.Add(translation);
                            }
                            applicants.Translations = lang;
                            biblioData.Applicants.Add(applicants);
                        }
                    }
                    else if (record.I71NameENG != null)
                    {
                        biblioData.Applicants = new List<PartyMember>();
                        for (int i = 0; i < record.I71NameENG.Count(); i++)
                        {
                            PartyMember applicants = new PartyMember();
                            if (record.I71NameENG != null && record.I71NameENG[i] != null) applicants.Name = record.I71NameENG[i];
                            if (record.I71CountryENG != null && record.I71CountryENG[i] != null) applicants.Country = record.I71CountryENG[i];
                            applicants.Language = "EN";
                            biblioData.Applicants.Add(applicants);
                        }
                    }
                    /*--------------*/
                    /*72 name*/
                    if (record.I72IL != null)
                    {
                        biblioData.Inventors = new List<PartyMember>();
                        for (int i = 0; i < record.I72IL.Count(); i++)
                        {
                            PartyMember Inventors = new PartyMember();
                            List<Translation> lang = new List<Translation>();
                            if (record.I72IL != null && record.I72IL[i] != null)
                                Inventors.Name = record.I72IL[i];
                            Inventors.Language = "HE";
                            if (record.I71NameENG != null && record.I72IL.Count() == record.I72Eng.Count())
                            {
                                Translation translation = new Translation
                                {
                                    Language = "EN",
                                    Type = "72",
                                    TrName = record.I72Eng[i]
                                };
                                lang.Add(translation);
                            }
                            Inventors.Translations = lang;
                            biblioData.Inventors.Add(Inventors);
                        }
                    }
                    else if (record.I72Eng != null && record.I72Eng.Count() > 0)
                    {
                        biblioData.Inventors = new List<PartyMember>();
                        for (int i = 0; i < record.I72Eng.Count(); i++)
                        {
                            PartyMember Inventors = new PartyMember();
                            if (record.I72Eng != null && record.I72Eng[i] != null) Inventors.Name = record.I72Eng[i];
                            Inventors.Language = "EN";
                            biblioData.Inventors.Add(Inventors);
                        }
                    }
                    /*---------------------*/
                    /*74 name and address*/
                    if (record.I74HebName != null)
                    {
                        biblioData.Agents = new List<PartyMember>();
                        PartyMember Agents = new PartyMember();
                        List<Translation> lang = new List<Translation>();
                        Agents.Name = record.I74HebName;
                        if (record.I74HebAddr != null)
                        {
                            Agents.Address1 = record.I74HebAddr;
                            Agents.Country = "IL";
                        }
                        Agents.Language = "HE";
                        if (record.I74EngName != null)
                        {
                            Translation translation = new Translation
                            {
                                Language = "EN",
                                Type = "74",
                                TrName = record.I74EngName

                            };
                            if (record.I74EngAddr != null) translation.TrAddress1 = record.I74EngAddr;
                            lang.Add(translation);
                        }
                        Agents.Translations = lang;
                        biblioData.Agents.Add(Agents);
                    }
                    else if (record.I74EngName != null)
                    {
                        biblioData.Agents = new List<PartyMember>();
                        PartyMember Agents = new PartyMember();
                        Agents.Name = record.I74EngName;
                        if (record.I74EngAddr != null) Agents.Address1 = record.I74EngAddr;
                        Agents.Language = "EN";
                        biblioData.Agents.Add(Agents);
                    }
                    /*---------------------*/
                    /*87*/
                    if (record.I87 != null)
                    {
                        IntConvention intConvention = new IntConvention { PctPublNumber = record.I87 };
                        biblioData.IntConvention = intConvention;
                    }
                    /*---------------------*/
                    /*Note field filing*/
                    if (record.INoteHeb != null)
                    {
                        legalEvent.LegalEvent = new LegalEvent();
                        legalEvent.LegalEvent.Note = record.INoteHeb;
                        legalEvent.LegalEvent.Language = "HE";
                        if (record.INoteEng != null)
                        {
                            NoteTranslation noteTransl = new NoteTranslation();
                            noteTransl.Tr = record.INoteEng;
                            noteTransl.Language = "EN";
                            noteTransl.Type = "";
                            legalEvent.LegalEvent.Translations.Add(noteTransl);
                        }
                    }
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
        public static List<Diamond.Core.Models.LegalStatusEvent> FourthListConvertation(List<ProcessNumbers.ElementOut> elementOuts)
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
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "IL";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.I11;
                    if (Diamond_IL_main.CurrentFileName.EndsWith("_REG_NUM.txt"))
                    {
                        legalEvent.GazetteName = Path.GetFileName(Diamond_IL_main.CurrentFileName.Replace("_REG_NUM.txt", ".pdf").Replace(".txt", ".pdf"));
                        /*Setting subcode*/
                        legalEvent.SubCode = "4";
                        /*Setting Section Code*/
                        legalEvent.SectionCode = "FZ";
                        biblioData.DOfPublication = new DOfPublication { date_45 = record.I45 };
                    }
                    if (Diamond_IL_main.CurrentFileName.EndsWith("_REN_NUM.txt"))
                    {
                        legalEvent.GazetteName = Path.GetFileName(Diamond_IL_main.CurrentFileName.Replace("_REN_NUM.txt", ".pdf").Replace(".txt", ".pdf"));
                        /*Setting subcode*/
                        legalEvent.SubCode = "5";
                        /*Setting Section Code*/
                        legalEvent.SectionCode = "NZ";
                        legalEvent.LegalEvent = new LegalEvent { Date = record.I45 };
                    }
                    if (Diamond_IL_main.CurrentFileName.EndsWith("_NONPAY_NUM.txt"))
                    {
                        legalEvent.GazetteName = Path.GetFileName(Diamond_IL_main.CurrentFileName.Replace("_NONPAY_NUM.txt", ".pdf").Replace(".txt", ".pdf"));
                        /*Setting subcode*/
                        legalEvent.SubCode = "6";
                        /*Setting Section Code*/
                        legalEvent.SectionCode = "MM";
                        legalEvent.LegalEvent = new LegalEvent { Date = record.I45 };
                    }
                    if (Diamond_IL_main.CurrentFileName.EndsWith("_REN20_NUM.txt"))
                    {
                        legalEvent.GazetteName = Path.GetFileName(Diamond_IL_main.CurrentFileName.Replace("_REN20_NUM.txt", ".pdf").Replace(".txt", ".pdf"));
                        /*Setting subcode*/
                        legalEvent.SubCode = "7";
                        /*Setting Section Code*/
                        legalEvent.SectionCode = "ND";
                        legalEvent.LegalEvent = new LegalEvent { Date = record.I45 };
                    }
                    if (Diamond_IL_main.CurrentFileName.EndsWith("_EXP_NUM.txt"))
                    {
                        legalEvent.GazetteName = Path.GetFileName(Diamond_IL_main.CurrentFileName.Replace("_EXP_NUM.txt", ".pdf").Replace(".txt", ".pdf"));
                        /*Setting subcode*/
                        legalEvent.SubCode = "8";
                        /*Setting Section Code*/
                        legalEvent.SectionCode = "MK";
                        legalEvent.LegalEvent = new LegalEvent { Date = record.I45 };
                    }
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }

        /*Applications without INIDs - Subcode 2*/
        public static List<Diamond.Core.Models.LegalStatusEvent> SecondListConvertation(List<ProcessAppNoInids.ElementOut> elementOuts)
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

                    legalEvent.GazetteName = Path.GetFileName(Diamond_IL_main.CurrentFileName.Replace("_APPNOINIDS.txt", ".pdf").Replace(".txt", ".pdf"));
                    /*Setting subcode*//*Setting Section Code*/
                    legalEvent.SubCode = "2";
                    legalEvent.SectionCode = "AA";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "IL";
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
                    /*51 international classification*/
                    if (record.I51Version != null && record.I51Number != null && record.I51Number.Count() > 0)
                    {
                        biblioData.Ipcs = new List<Ipc>();
                        Ipc ipcValue = new Ipc
                        {
                            Date = record.I51Version,
                            Class = record.I51Number.Replace("//", "/")
                        };
                        biblioData.Ipcs.Add(ipcValue);
                    }
                    /*---------------------*/
                    /*54 Title*/
                    if (record.I54Hebrew != null)
                    {
                        Title title = new Title()
                        {
                            Text = record.I54Hebrew,
                            Language = "HE"
                        };
                        biblioData.Titles.Add(title);
                    }
                    if (record.I54Eng != null)
                    {
                        Title title = new Title()
                        {
                            Text = record.I54Eng,
                            Language = "EN"
                        };
                        biblioData.Titles.Add(title);
                    }
                    /*71 name, country code*/
                    if (record.I71NameIL != null)
                    {
                        biblioData.Applicants = new List<PartyMember>();
                        PartyMember applicants = new PartyMember();
                        List<Translation> lang = new List<Translation>();
                        //Translation lang = new Translation();
                        if (record.I71NameIL != null)
                            applicants.Name = record.I71NameIL;
                        applicants.Language = "HE";
                        if (record.I71NameENG != null)
                        {
                            Translation translation = new Translation
                            {
                                Language = "EN",
                                Type = "71",
                                TrName = record.I71NameENG
                            };
                            lang.Add(translation);
                        }
                        applicants.Translations = lang;
                        biblioData.Applicants.Add(applicants);
                    }
                    else if (record.I71NameENG != null)
                    {
                        biblioData.Applicants = new List<PartyMember>();
                        PartyMember applicants = new PartyMember();
                        if (record.I71NameENG != null)
                            applicants.Name = record.I71NameENG;
                        applicants.Language = "EN";
                        biblioData.Applicants.Add(applicants);
                    }
                    /*--------------*/
                    /*86 and 87*/
                    if (record.I86 != null || record.I87 != null)
                    {
                        IntConvention intConvention = new IntConvention();
                        if (record.I86 != null) intConvention.PctApplNumber = record.I86;
                        if (record.I87 != null) intConvention.PctPublNumber = record.I87;
                        biblioData.IntConvention = intConvention;
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
