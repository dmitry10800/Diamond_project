using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integration;

namespace Diamond_MX
{
    class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub1(List<OutElements.SubSecondThird> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            string dateFromName = Methods.GetDateFromGazette(MX_main.CurrentFileName);
            if (elementOuts != null)
            {
                int leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = Path.GetFileName(MX_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    if (record.AppNumber != null)
                    {
                        var a = Methods.ChooseSubCode(record.AppNumber);
                        if (a != null) legalEvent.SubCode = a;
                        else
                            continue;
                    }
                    else
                        Console.WriteLine("Error");

                    legalEvent.SectionCode = "PC";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "MX";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Application.Number = record.AppNumber;
                    /*73 name and address*/
                    biblioData.Assignees = new List<PartyMember>();
                    legalEvent.NewBiblio = new Biblio();
                    legalEvent.NewBiblio.Assignees = new List<PartyMember>();
                    /*Notes*/
                    legalEvent.LegalEvent = new LegalEvent { Date = dateFromName.Replace("-", "/") };
                    string tmpLeNote = null;
                    string tmpLeNoteTranslation = null;
                    if (string.IsNullOrEmpty(record.PubNumber))
                    {
                        tmpLeNote = string.Concat(tmpLeNote, $"|| Número de concesión | missed in the original document ");
                        tmpLeNoteTranslation = string.Concat(tmpLeNoteTranslation, $"|| Registration number of Legal Event | missed in the original document ");
                    }
                    else
                    {
                        biblioData.Publication.Number = record.PubNumber;
                    }
                    if (record.New73Holder.Count == 0)
                    {
                        tmpLeNote = string.Concat(tmpLeNote, $"|| (73)new is missing in the original document ");
                        tmpLeNoteTranslation = string.Concat(tmpLeNoteTranslation, $"|| (73)new is missing in the original document ");
                    }
                    if (!string.IsNullOrEmpty(tmpLeNote))
                    {
                        legalEvent.LegalEvent.Language = "ES";
                        legalEvent.LegalEvent.Note = tmpLeNote.Trim();
                        legalEvent.LegalEvent.Translations.Add(new NoteTranslation
                        {
                            Language = "EN",
                            Tr = tmpLeNoteTranslation.Trim()
                        });
                    }

                    /**********************/
                    foreach (var item in record.New73Holder)
                        legalEvent.NewBiblio.Assignees.Add(new PartyMember { Name = item });
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
    }
}
