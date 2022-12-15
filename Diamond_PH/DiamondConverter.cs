using System.Collections.Generic;
using Integration;

namespace Diamond_PH
{
    class DiamondConverter
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub7Convert(List<Elements> records)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            int id = 1;

            foreach (var record in records)
            {
                Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();

                legalEvent.GazetteName = PH_main.currentFileName.Replace(".txt", ".pdf");

                legalEvent.SubCode = "7";
                legalEvent.SectionCode = "MK";
                legalEvent.CountryCode = "PH";
                legalEvent.Id = id++;
                Biblio biblio = new Biblio();

                if (record.AppNumber != null)
                {
                    biblio.Application.Number = record.AppNumber;
                }

                if(record.AppDate != null)
                {
                    biblio.Application.Date = record.AppDate;
                }

                if(record.EventDate != null)
                {
                    legalEvent.LegalEvent = new LegalEvent();
                    legalEvent.LegalEvent.Date = record.EventDate;
                }

                if(record.Title != null)
                {
                    biblio.Titles = new List<Title>();
                    biblio.Titles.Add(new Title { Text = record.Title, Language = "EN" });
                }

                if(record.Owner != null)
                {
                    biblio.Assignees = new List<PartyMember>();
                    foreach (var item in record.Owner)
                    {
                        biblio.Assignees.Add(new PartyMember { Name = item.Name, Country = item.Country });
                    }
                    
                }    
                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }

            return legalEvents;
        }
    }
}
