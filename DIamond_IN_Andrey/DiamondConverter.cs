using System.Collections.Generic;
using System.Net.Mime;
using Integration;

namespace DIamond_IN_Andrey
{
    class DiamondConverter
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> BiblioConvert1(List<Elements> elements)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 1;
            foreach (var rec in elements)
            {
                Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                legalEvent.GazetteName = IN_Diamond.currentFileName.Replace(".tetml", ".pdf");
                legalEvent.SubCode = "1";
                legalEvent.SectionCode = "BZ";
                legalEvent.CountryCode = "IN";
                legalEvent.Id = id++;
                Biblio biblio = new Biblio();

                biblio.Publication = new Publication
                {
                    Number = rec.PubNumber,
                    Kind = rec.Kind
                };

                if (!string.IsNullOrEmpty(rec.PRIC))
                {
                    biblio.Priorities = new List<Priority>
                    {
                        new Priority
                        {
                           Country = rec.PRIC,
                           Number = rec.PRIN,
                           Date = rec.PRID
                        }
                    };
                }

                if (rec.Related != null)
                {
                    biblio.Related = new List<RelatedDocument>();
                    if (!string.IsNullOrEmpty(rec.Related.PubDate))
                    {
                        biblio.Related.Add(new RelatedDocument
                        {
                            Date = rec.Related.PubDate,
                            Number = rec.Related.PubNumber,
                            Type = rec.Related.Inid
                        });
                    }
                    else if (!string.IsNullOrEmpty(rec.Related.AppDate))
                    {
                        biblio.Related.Add(new RelatedDocument
                        {
                            Date = rec.Related.AppDate,
                            Number = rec.Related.AppNumber,
                            Type = rec.Related.Inid
                        });
                    }
                }

                if (rec.PCT != null)
                {
                    biblio.IntConvention = new IntConvention
                    {
                        PctApplDate = rec.PCT.DateOfFiling,
                        PctApplNumber = rec.PCT.AppNumber,
                        PctPublNumber = rec.WO
                    };
                }


                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }

            return legalEvents;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> BiblioConvert3(List<Elements> elements)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 1;
            foreach (var rec in elements)
            {
                Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                legalEvent.GazetteName = IN_Diamond.currentFileName.Replace(".tetml", ".pdf");
                legalEvent.SubCode = "3";
                legalEvent.SectionCode = "BZ";
                legalEvent.CountryCode = "IN";
                legalEvent.Id = id++;
                Biblio biblio = new Biblio();

                biblio.Publication = new Publication
                {
                    Number = rec.PubNumber,
                    Kind = rec.Kind
                };

                if (!string.IsNullOrEmpty(rec.PRIC))
                {
                    biblio.Priorities = new List<Priority>
                    {
                        new Priority
                        {
                           Country = rec.PRIC,
                           Number = rec.PRIN,
                           Date = rec.PRID
                        }
                    };
                }

                if (rec.Related != null)
                {
                    biblio.Related = new List<RelatedDocument>();
                    if (!string.IsNullOrEmpty(rec.Related.PubDate))
                    {
                        biblio.Related.Add(new RelatedDocument
                        {
                            Date = rec.Related.PubDate,
                            Number = rec.Related.PubNumber,
                            Type = rec.Related.Inid
                        });
                    }
                    else if (!string.IsNullOrEmpty(rec.Related.AppDate))
                    {
                        biblio.Related.Add(new RelatedDocument
                        {
                            Date = rec.Related.AppDate,
                            Number = rec.Related.AppNumber,
                            Type = rec.Related.Inid
                        });
                    }
                }

                if (rec.PCT != null)
                {
                    biblio.IntConvention = new IntConvention
                    {
                        PctApplDate = rec.PCT.DateOfFiling,
                        PctApplNumber = rec.PCT.AppNumber,
                        PctPublNumber = rec.WO
                    };
                }


                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }

            return legalEvents;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> BiblioConvert2(List<Elements> elements)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var id = 1;
            foreach (var rec in elements)
            {
                Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                legalEvent.GazetteName = IN_Diamond.currentFileName.Replace(".tetml", ".pdf");
                legalEvent.SubCode = "2";
                legalEvent.SectionCode = "FG";
                legalEvent.CountryCode = "IN";
                legalEvent.Id = id++;
                Biblio biblio = new Biblio();

                biblio.Application = new MediaTypeNames.Application
                {
                    Number = rec.AppNumber
                };

                biblio.Publication = new Publication
                {
                    Number = rec.PubNumber
                };

                if (!string.IsNullOrEmpty(rec.PRID))
                {
                    biblio.Priorities = new List<Priority>
                    {
                        new Priority
                        {
                           Date = rec.PRID
                        }
                    };
                }

                legalEvent.Biblio = biblio;
                legalEvents.Add(legalEvent);
            }

            return legalEvents;
        }
    }
}
