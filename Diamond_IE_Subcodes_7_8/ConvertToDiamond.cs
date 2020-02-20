using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integration;

namespace Diamond_IE_Subcodes_7_8
{
    public class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub7_8Convert(List<SubCode7_8> elements, string gazetteName, string numberSubCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 0;
            foreach (var elem in elements)
            {
                Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                Biblio biblio = new Biblio();
                biblio.Assignees = new List<PartyMember>();
                biblio.Ipcs = new List<Ipc>();
                legalEvent.LegalEvent = new LegalEvent();
                legalEvent.SubCode = numberSubCode;
                legalEvent.SectionCode = "MK";
                legalEvent.CountryCode = "IE";
                legalEvent.Id = id++;
                if (elem.Grantee_Assignee_OwnerInformation != null)
                {
                    foreach (var item in elem.Grantee_Assignee_OwnerInformation)
                    {
                        biblio.Assignees.Add(new PartyMember { Name = item });
                    }
                }

                if (elem.IpcClassifications != null)
                {
                    foreach (var item in elem.IpcClassifications)
                    {
                        biblio.Ipcs.Add(new Ipc { Class = item.Classification, Date = item.IPC_Version });
                    }
                }
                biblio.Publication.Number = elem.PublicationNumber;
                biblio.Titles.Add(new Title { Text = elem.Title, Language = "EN" });
                legalEvent.GazetteName = gazetteName;
                legalEvent.LegalEvent.Date = elem.LegalEventDate;
                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }

            return legalEvents;
        }
    }
}
