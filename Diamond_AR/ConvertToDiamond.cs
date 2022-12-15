using System.Collections.Generic;
using System.IO;
using System.Linq;
using Integration;

namespace Diamond_AR
{
    class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> ApplicationsConvertation(List<ProcessApplications.ElementOut> elementOuts)
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

                    legalEvent.GazetteName = Path.GetFileName(Diamond_AR_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*//*Setting Section Code*/
                    legalEvent.SubCode = "1";
                    legalEvent.SectionCode = "AZ";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "AR";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.I10;
                    biblioData.Publication.Kind = record.I13;
                    biblioData.Application.Number = record.I21;
                    if (record.I22 != null) biblioData.Application.Date = record.I22.Trim();
                    /*30*/
                    if (record.I30N != null && record.I30N.Count() == record.I30D.Count() && record.I30N.Count() == record.I30C.Count())
                    {
                        biblioData.Priorities = new List<Priority>();
                        for (int i = 0; i < record.I30N.Count(); i++)
                        {
                            Priority priority = new Priority();
                            priority.Number = record.I30N[i];
                            priority.Date = record.I30D[i];
                            priority.Country = record.I30C[i];
                            biblioData.Priorities.Add(priority);
                        }
                    }
                    /*---------------------*/
                    /*41*/
                    if (record.I41 != null)
                    {
                        biblioData.DOfPublication = new DOfPublication { date_41 = record.I41 };
                    }
                    /*51 international classification*/
                    if (record.I51 != null && record.I51.Count() > 0)
                    {
                        biblioData.Ipcs = new List<Ipc>();
                        for (int i = 0; i < record.I51.Count(); i++)
                        {
                            Ipc ipcValue = new Ipc();
                            ipcValue.Class = record.I51[i];
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
                            Language = "ES"
                        };
                        biblioData.Titles.Add(title);
                    }
                    /*---------------------*/
                    /*57*/
                    if (record.I57 != null)
                    {
                        biblioData.Abstracts = new List<Abstract>();
                        Abstract desc = new Abstract { Text = record.I57, Language = "ES" };
                        biblioData.Abstracts.Add(desc);
                    }
                    /*62*/
                    if (record.I62 != null)
                    {
                        List<RelatedDocument> relatedDocuments = new List<RelatedDocument>();
                        RelatedDocument document = new RelatedDocument { Number = record.I62, Source = "62" };
                        document.Number = record.I62;
                        document.Source = "62";
                        if (record.I62Kind != null) document.Type = record.I62Kind;
                        relatedDocuments.Add(document);
                        biblioData.Related = relatedDocuments;
                    }
                    /*71 Name, Addr, Country*/
                    if (record.I71N != null && record.I71N.Count() > 0)
                    {
                        biblioData.Applicants = new List<PartyMember>();
                        for (int i = 0; i < record.I71N.Count(); i++)
                        {
                            PartyMember member = new PartyMember();
                            member.Name = record.I71N[i];
                            if (record.I71A != null && record.I71A[i] != null) member.Address1 = record.I71A[i];
                            if (record.I71C != null && record.I71C[i] != null) member.Country = record.I71C[i];
                            biblioData.Applicants.Add(member);
                        }
                    }
                    /*----------------------*/
                    /*72 name*/
                    if (record.I72 != null)
                    {
                        biblioData.Inventors = new List<PartyMember>();
                        for (int i = 0; i < record.I72.Count(); i++)
                        {
                            PartyMember Inventors = new PartyMember();
                            if (record.I72 != null && record.I72[i] != null) Inventors.Name = record.I72[i];
                            biblioData.Inventors.Add(Inventors);
                        }
                    }
                    /*---------------------*/
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
                    if (record.I74 != null || record.I41B != null || record.I83 != null)
                    {
                        legalEvent.LegalEvent = new LegalEvent();
                        string tmpNote = "";
                        if (record.I74 != null) tmpNote += "|| (74) Agente/s Nro | " + record.I74 + " ";
                        if (record.I41B != null) tmpNote += "|| Bol. Nro | " + record.I41B + " ";
                        if (record.I83 != null) tmpNote += "|| (83) Depósito Microorganismos | " + record.I83 + " ";
                        legalEvent.LegalEvent.Note = tmpNote;
                        legalEvent.LegalEvent.Language = "ES";
                        NoteTranslation noteTransl = new NoteTranslation();
                        tmpNote = "";
                        if (record.I74 != null) tmpNote += "|| (74) Agent/s Number | " + record.I74 + " ";
                        if (record.I41B != null) tmpNote += "|| Bulletin Number | " + record.I41B + " ";
                        if (record.I83 != null) tmpNote += "|| (83) Deposit Microorganisms | " + record.I83 + " ";
                        noteTransl.Tr = tmpNote;
                        noteTransl.Language = "EN";
                        noteTransl.Type = "INID";
                        legalEvent.LegalEvent.Translations.Add(noteTransl);

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
