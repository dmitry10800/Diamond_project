using System.Collections.Generic;
using System.Net.Mime;
using Integration;

namespace Diamond_TR
{
    class DiamondConverter
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub10Convert(List<Elements> elements)
        {
            var legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 1;
            var gazetteName = TR_main.currentFileName.Replace(".tetml", ".pdf");
            var gazetteDate = gazetteName.Split('_').Skip(1).First();
            gazetteDate = gazetteDate.Substring(0, 4) + "-" + gazetteDate.Substring(4, 2) + "-" + gazetteDate.Substring(6);
            foreach (var rec in elements)
            {
                var legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                legalEvent.GazetteName = TR_main.currentFileName.Replace(".tetml", ".pdf");
                legalEvent.SubCode = "10";
                legalEvent.SectionCode = "TD";
                legalEvent.CountryCode = "TR";
                legalEvent.Id = id++;
                var biblio = new Biblio();
                biblio.Applicants = new List<PartyMember>();

                biblio.Titles = new List<Integration.Title>
                {
                    new Integration.Title
                    {
                        Text = rec.Title.Text,
                        Language = "TR"
                    }
                };

                legalEvent.LegalEvent = new LegalEvent
                {
                    Date = gazetteDate
                };

                biblio.Application = new MediaTypeNames.Application
                {
                    Number = rec.PubNumber
                };

                if (rec.Applicants != null)
                {
                    foreach (var applicant in rec.Applicants)
                    {
                        biblio.Applicants.Add(new PartyMember
                        {
                            Name = applicant.Name
                        });
                    }
                }


                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }

            return legalEvents;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub13Convert(List<Elements> elements)
        {
            var legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 1;
            var gazetteName = TR_main.currentFileName.Replace(".tetml", ".pdf");
            var gazetteDate = gazetteName.Split('_').Skip(1).First();
            gazetteDate = gazetteDate.Substring(0, 4) + "-" + gazetteDate.Substring(4, 2) + "-" + gazetteDate.Substring(6);
            foreach (var rec in elements)
            {
                var legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                legalEvent.GazetteName = TR_main.currentFileName.Replace(".tetml", ".pdf");
                legalEvent.SubCode = "13";
                legalEvent.SectionCode = "MM";
                legalEvent.CountryCode = "TR";
                legalEvent.Id = id++;
                var biblio = new Biblio();
                biblio.Applicants = new List<PartyMember>();

                biblio.Titles = new List<Integration.Title>
                {
                    new Integration.Title
                    {
                        Text = rec.Title.Text,
                        Language = "TR"
                    }
                };

                legalEvent.LegalEvent = new LegalEvent
                {
                    Date = gazetteDate
                };

                biblio.Application = new MediaTypeNames.Application
                {
                    Number = rec.PubNumber
                };

                if (rec.Applicants != null)
                {
                    foreach (var applicant in rec.Applicants)
                    {
                        biblio.Applicants.Add(new PartyMember
                        {
                            Name = applicant.Name
                        });
                    }
                }


                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }

            return legalEvents;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub16Convert(List<Elements> elements)
        {
            var legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 1;
            var gazetteName = TR_main.currentFileName.Replace(".tetml", ".pdf");
            var gazetteDate = gazetteName.Split('_').Skip(1).First();
            gazetteDate = gazetteDate.Substring(0, 4) + "-" + gazetteDate.Substring(4, 2) + "-" + gazetteDate.Substring(6);
            foreach (var rec in elements)
            {
                var legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                legalEvent.GazetteName = TR_main.currentFileName.Replace(".tetml", ".pdf");
                legalEvent.SubCode = "16";
                legalEvent.SectionCode = "FD";
                legalEvent.CountryCode = "TR";
                legalEvent.Id = id++;
                var biblio = new Biblio();
                biblio.Applicants = new List<PartyMember>();

                biblio.Titles = new List<Integration.Title>
                {
                    new Integration.Title
                    {
                        Text = rec.Title.Text,
                        Language = "TR"
                    }
                };

                legalEvent.LegalEvent = new LegalEvent
                {
                    Date = gazetteDate
                };

                biblio.Application = new MediaTypeNames.Application
                {
                    Number = rec.PubNumber
                };

                if (rec.Applicants != null)
                {
                    foreach (var applicant in rec.Applicants)
                    {
                        biblio.Applicants.Add(new PartyMember
                        {
                            Name = applicant.Name
                        });
                    }
                }


                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }

            return legalEvents;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub17Convert(List<Elements> elements)
        {
            var legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 1;
            var gazetteName = TR_main.currentFileName.Replace(".tetml", ".pdf");
            var gazetteDate = gazetteName.Split('_').Skip(1).First();
            gazetteDate = gazetteDate.Substring(0, 4) + "-" + gazetteDate.Substring(4, 2) + "-" + gazetteDate.Substring(6);
            foreach (var rec in elements)
            {
                var legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                legalEvent.GazetteName = TR_main.currentFileName.Replace(".tetml", ".pdf");
                legalEvent.SubCode = "17";
                legalEvent.SectionCode = "FA";
                legalEvent.CountryCode = "TR";
                legalEvent.Id = id++;
                var biblio = new Biblio();
                biblio.Applicants = new List<PartyMember>();

                biblio.Titles = new List<Integration.Title>
                {
                    new Integration.Title
                    {
                        Text = rec.Title.Text,
                        Language = "TR"
                    }
                };

                biblio.Application = new MediaTypeNames.Application
                {
                    Number = rec.PubNumber
                };

                legalEvent.LegalEvent = new LegalEvent
                {
                    Date = gazetteDate
                };

                if (rec.Applicants != null)
                {
                    foreach (var applicant in rec.Applicants)
                    {
                        biblio.Applicants.Add(new PartyMember
                        {
                            Name = applicant.Name
                        });
                    }
                }


                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }

            return legalEvents;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub30Convert(List<Elements> elements)
        {
            var legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 1;
            var gazetteName = TR_main.currentFileName.Replace(".tetml", ".pdf");
            var gazetteDate = gazetteName.Split('_').Skip(1).First();
            gazetteDate = gazetteDate.Substring(0, 4) + "-" + gazetteDate.Substring(4, 2) + "-" + gazetteDate.Substring(6);
            foreach (var rec in elements)
            {
                var legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                legalEvent.GazetteName = TR_main.currentFileName.Replace(".tetml", ".pdf");
                legalEvent.SubCode = "30";
                legalEvent.SectionCode = "EZ";
                legalEvent.CountryCode = "TR";
                legalEvent.Id = id++;
                var biblio = new Biblio();
                biblio.Applicants = new List<PartyMember>();

                biblio.Titles = new List<Integration.Title>
                {
                    new Integration.Title
                    {
                        Text = rec.Title.Text,
                        Language = "TR"
                    }
                };

                legalEvent.LegalEvent = new LegalEvent
                {
                    Date = gazetteDate
                };

                biblio.Application = new MediaTypeNames.Application
                {
                    Number = rec.PubNumber
                };

                if (rec.Applicants != null)
                {
                    foreach (var applicant in rec.Applicants)
                    {
                        biblio.Applicants.Add(new PartyMember
                        {
                            Name = applicant.Name
                        });
                    }
                }


                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }

            return legalEvents;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub31Convert(List<Elements> elements)
        {
            var legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 1;
            var gazetteName = TR_main.currentFileName.Replace(".tetml", ".pdf");
            var gazetteDate = gazetteName.Split('_').Skip(1).First();
            gazetteDate = gazetteDate.Substring(0, 4) + "-" + gazetteDate.Substring(4, 2) + "-" + gazetteDate.Substring(6);
            foreach (var rec in elements)
            {
                var legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                legalEvent.GazetteName = TR_main.currentFileName.Replace(".tetml", ".pdf");
                legalEvent.SubCode = "31";
                legalEvent.SectionCode = "EZ";
                legalEvent.CountryCode = "TR";
                legalEvent.Id = id++;
                var biblio = new Biblio();
                biblio.Applicants = new List<PartyMember>();

                biblio.Titles = new List<Integration.Title>
                {
                    new Integration.Title
                    {
                        Text = rec.Title.Text,
                        Language = "TR"
                    }
                };

                legalEvent.LegalEvent = new LegalEvent
                {
                    Date = gazetteDate
                };

                biblio.Application = new MediaTypeNames.Application
                {
                    Number = rec.PubNumber
                };

                if (rec.Applicants != null)
                {
                    foreach (var applicant in rec.Applicants)
                    {
                        biblio.Applicants.Add(new PartyMember
                        {
                            Name = applicant.Name
                        });
                    }
                }


                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }

            return legalEvents;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub37Convert(List<Elements> elements)
        {
            var legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 1;
            foreach (var rec in elements)
            {
                var legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                legalEvent.GazetteName = TR_main.currentFileName.Replace(".tetml", ".pdf");
                legalEvent.SubCode = "37";
                legalEvent.SectionCode = "EZ";
                legalEvent.CountryCode = "TR";
                legalEvent.Id = id++;
                var biblio = new Biblio();
                biblio.Applicants = new List<PartyMember>();

                biblio.Titles = new List<Integration.Title>
                {
                    new Integration.Title
                    {
                        Text = rec.Title.Text,
                        Language = "TR"
                    }
                };

                biblio.Application = new MediaTypeNames.Application
                {
                    Number = rec.PubNumber
                };

                if (rec.Applicants != null)
                {
                    foreach (var applicant in rec.Applicants)
                    {
                        biblio.Applicants.Add(new PartyMember
                        {
                            Name = applicant.Name
                        });
                    }
                }


                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }

            return legalEvents;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub39Convert(List<Elements> elements)
        {
            var legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 1;
            var gazetteName = TR_main.currentFileName.Replace(".tetml", ".pdf");
            var gazetteDate = gazetteName.Split('_').Skip(1).First();
            gazetteDate = gazetteDate.Substring(0, 4) + "-" + gazetteDate.Substring(4, 2) + "-" + gazetteDate.Substring(6);
            foreach (var rec in elements)
            {
                var legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                legalEvent.GazetteName = TR_main.currentFileName.Replace(".tetml", ".pdf");
                legalEvent.SubCode = "39";
                legalEvent.SectionCode = "EZ";
                legalEvent.CountryCode = "TR";
                legalEvent.Id = id++;
                var biblio = new Biblio();
                biblio.Applicants = new List<PartyMember>();

                biblio.Titles = new List<Integration.Title>
                {
                    new Integration.Title
                    {
                        Text = rec.Title.Text,
                        Language = "TR"
                    }
                };

                legalEvent.LegalEvent = new LegalEvent
                {
                    Date = gazetteDate
                };

                biblio.Application = new MediaTypeNames.Application
                {
                    Number = rec.PubNumber
                };

                if (rec.Applicants != null)
                {
                    foreach (var applicant in rec.Applicants)
                    {
                        biblio.Applicants.Add(new PartyMember
                        {
                            Name = applicant.Name
                        });
                    }
                }


                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }

            return legalEvents;
        }
    }
}
