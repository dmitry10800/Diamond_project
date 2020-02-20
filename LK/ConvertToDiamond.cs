using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integration;

namespace LK
{
    class ConvertToDiamond
    {
        /*Applications without INIDs - Subcode 2*/
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub2Convertation(List<ProcessXls.Elements> elementOuts)
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

                    legalEvent.GazetteName = Path.GetFileName(LK_main.CurrentFileName.Replace(".xlsx", ".pdf").Replace(".XLSX", ".pdf"));
                    /*Setting subcode*//*Setting Section Code*/
                    legalEvent.SubCode = "2";
                    legalEvent.SectionCode = "FG";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "LK";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    biblioData.Assignees = new List<PartyMember>();
                    /*Elements output*/
                    biblioData.Publication.Number = record.Number;
                    biblioData.Application.Date = record.Date;
                    /*54 Title*/
                    biblioData.Titles.Add(new Title { Language = "EN", Text = record.Title.Replace("\n", " ").Trim() });
                    biblioData.Assignees.Add(new PartyMember
                    {
                        Name = record.Owner.Replace("\n", " ").Trim()
                    });

                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
    }
}
