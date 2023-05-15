using System.Collections.Generic;
using Integration;

namespace Diamond_ES
{
    class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub3Convert(List<SubCode3> elements, string gazetteName)
        {
            var legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 0;
            foreach (var elem in elements)
            {
                var legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                var biblio = new Biblio();
                biblio.Assignees = new List<PartyMember>();
                biblio.Agents = new List<PartyMember>();
                biblio.Related = new List<RelatedDocument>();
                legalEvent.LegalEvent = new LegalEvent();
                legalEvent.LegalEvent.Translations = new List<NoteTranslation>();
                legalEvent.SubCode = "3";
                legalEvent.SectionCode = "MZ";
                legalEvent.CountryCode = "ES";
                legalEvent.Id = id++;
                if (elem.GranteeAssigneeOwnerInformation != null)
                {
                    foreach (var item in elem.GranteeAssigneeOwnerInformation)
                    {
                        biblio.Assignees.Add(new PartyMember { Name = item });
                    }
                }
                biblio.Publication.Number = elem.PublicationNumber;
                biblio.Publication.Kind = elem.PublicationKind;
                biblio.Application.Number = elem.ApplicationNumber;
                biblio.Application.Date = elem.ApplicationDate;
                biblio.Titles.Add(new Title { Text = elem.TitleText, Language = "ES" });
                if (elem.AgentName != null)
                {
                    foreach (var item in elem.AgentName)
                    {
                        biblio.Agents.Add(new PartyMember { Name = item });
                    }
                }
                var related = new RelatedDocument();
                if (elem.RelatedPublicationInformation != null)
                {
                    foreach (var relatedPublicationInformation in elem.RelatedPublicationInformation)
                    {
                        related.Number = relatedPublicationInformation.Number;
                        related.Date = relatedPublicationInformation.Date;
                        related.Type = relatedPublicationInformation.InidNumber;
                    }
                }
                biblio.Related.Add(related);
                legalEvent.GazetteName = gazetteName;
                legalEvent.LegalEvent.Date = elem.LegalStatusEvents_EventDate;
                if (elem.LegalStatusEvents_Note != null)
                {
                    for (var i = 0; i < 2; i++)
                    {
                        if (i == 0)
                        {
                            legalEvent.LegalEvent.Language = elem.LegalStatusEvents_Note[0].Language;
                            legalEvent.LegalEvent.Note = elem.LegalStatusEvents_Note[0].Note;
                        }

                        if (i == 1)
                        {
                            legalEvent.LegalEvent.Translations.Add(new NoteTranslation { Language = elem.LegalStatusEvents_Note[1].Language, Tr = elem.LegalStatusEvents_Note[1].Note, Type = "note" });
                        }
                    }
                }
                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }

            return legalEvents;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub11Convert(List<SubCode11> elements, string gazetteName)
        {
            var legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 0;
            foreach (var elem in elements)
            {
                var legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                var biblio = new Biblio();
                biblio.Assignees = new List<PartyMember>();
                biblio.Agents = new List<PartyMember>();
                legalEvent.LegalEvent = new LegalEvent();
                legalEvent.LegalEvent.Translations = new List<NoteTranslation>();
                legalEvent.SubCode = "11";
                legalEvent.SectionCode = "MZ";
                legalEvent.CountryCode = "ES";
                legalEvent.Id = id++;
                if (elem.GranteeAssigneeOwnerInformation != null)
                {
                    foreach (var item in elem.GranteeAssigneeOwnerInformation)
                    {
                        biblio.Assignees.Add(new PartyMember { Name = item });
                    }
                }
                biblio.Publication.Number = elem.PublicationNumber;
                biblio.Publication.Kind = elem.PublicationKind;
                biblio.Application.Number = elem.ApplicationNumber;
                biblio.Application.Date = elem.ApplicationDate;
                if (elem.AgentName != null)
                {
                    foreach (var item in elem.AgentName)
                    {
                        biblio.Agents.Add(new PartyMember { Name = item });
                    }
                }
                biblio.Titles.Add(new Title { Text = elem.TitleText, Language = "ES" });
                legalEvent.LegalEvent.Date = elem.LegalStatusEvents_EventDate;
                if (elem.LegalStatusEvents_Note != null)
                {
                    for (var i = 0; i < 2; i++)
                    {
                        if (i == 0)
                        {
                            legalEvent.LegalEvent.Language = elem.LegalStatusEvents_Note[0].Language;
                            legalEvent.LegalEvent.Note = elem.LegalStatusEvents_Note[0].Note;
                        }

                        if (i == 1)
                        {
                            legalEvent.LegalEvent.Translations.Add(new NoteTranslation { Language = elem.LegalStatusEvents_Note[1].Language, Tr = elem.LegalStatusEvents_Note[1].Note, Type = "note" });
                        }
                    }
                }
                legalEvent.Biblio = biblio;
                legalEvent.GazetteName = gazetteName;
                legalEvents.Add(legalEvent);
            }

            return legalEvents;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub12Convert(List<SubCode12> elements, string gazetteName)
        {
            var legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 0;
            foreach (var elem in elements)
            {
                var legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                var biblio = new Biblio();
                biblio.Assignees = new List<PartyMember>();
                biblio.Agents = new List<PartyMember>();
                legalEvent.LegalEvent = new LegalEvent();
                legalEvent.LegalEvent.Translations = new List<NoteTranslation>();
                legalEvent.SubCode = "12";
                legalEvent.SectionCode = "MZ";
                legalEvent.CountryCode = "ES";
                legalEvent.Id = id++;
                if (elem.GranteeAssigneeOwnerInformation != null)
                {
                    foreach (var item in elem.GranteeAssigneeOwnerInformation)
                    {
                        biblio.Assignees.Add(new PartyMember { Name = item });
                    }
                }
                biblio.Publication.Number = elem.PublicationNumber;
                biblio.Publication.Kind = elem.PublicationKind;
                biblio.Application.Number = elem.ApplicationNumber;
                biblio.Application.Date = elem.ApplicationDate;
                biblio.Titles.Add(new Title { Text = elem.TitleText, Language = "ES" });
                if (elem.AgentName != null)
                {
                    foreach (var item in elem.AgentName)
                    {
                        biblio.Agents.Add(new PartyMember { Name = item });
                    }
                }
                legalEvent.LegalEvent.Date = elem.LegalStatusEvents_EventDate;
                if (elem.LegalStatusEvents_Note != null)
                {
                    for (var i = 0; i < 2; i++)
                    {
                        if (i == 0)
                        {
                            legalEvent.LegalEvent.Language = elem.LegalStatusEvents_Note[0].Language;
                            legalEvent.LegalEvent.Note = elem.LegalStatusEvents_Note[0].Note;
                        }

                        if (i == 1)
                        {
                            legalEvent.LegalEvent.Translations.Add(new NoteTranslation { Language = elem.LegalStatusEvents_Note[1].Language, Tr = elem.LegalStatusEvents_Note[1].Note, Type = "note" });
                        }
                    }
                }
                legalEvent.GazetteName = gazetteName;
                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }

            return legalEvents;
        }
    }
}
