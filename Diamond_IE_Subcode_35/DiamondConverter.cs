using Integration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diamond_IE_Subcode_35
{
    class DiamondConverter
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub35Convertor(List<Record> records)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();

            var id = 1;

            foreach (var record in records)
            {
                Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();

                legalEvent.GazetteName = Path.GetFileName(Process.CurrentFileName.Replace(".txt", ".pdf"));

                legalEvent.SubCode = "35";

                legalEvent.SectionCode = "FG";

                legalEvent.CountryCode = "IE";

                legalEvent.Id = id++;

                Biblio biblioData = new Biblio();

                if(record.pubNumber != null){
                    biblioData.Publication.Number = record.pubNumber;
                }

                if(record.appNumber != null)
                {
                    biblioData.Application.Number = record.appNumber;
                }

                if(record.appDate != null)
                {
                    biblioData.Application.Date = record.appDate;
                }

                if (record.owner != null)
                {
                    biblioData.Assignees = new List<PartyMember>();

                    foreach (var item in record.owner)
                    {
                        biblioData.Assignees.Add(new PartyMember
                        {
                            Name = item.name
                        }) ;
                    }
                }

                legalEvent.Biblio = biblioData;
                legalEvents.Add(legalEvent);

            }
               return legalEvents;
        }
    }
}
