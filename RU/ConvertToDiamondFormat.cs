using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Integration;

namespace RU
{
    class ConvertToDiamondFormat
    {
        private static string DateNormalize(string date)
        {
            var pattern = new Regex(@"(?<Year>\d{4})(?<Month>\d{2})(?<Day>\d{2})");
            var dateMatch = pattern.Match(date);
            if (dateMatch.Success)
                return $"{dateMatch.Groups["Year"].Value}-{dateMatch.Groups["Month"].Value}-{dateMatch.Groups["Day"].Value}";
            else
                Console.WriteLine("Date format doesn't match 6 digit");
            return date;
        }
        private static string GetGazetteName(string date, string number)
        {
            if (string.IsNullOrEmpty(date) || string.IsNullOrEmpty(number))
            {
                Console.WriteLine("Gazette name is not created");
                return null;
            }

            if (number.Length == 6)
                number = number.Substring(4);
            else
                Console.WriteLine("Gazette number format is incorrect");

            return $"RU_{date}_{number}.pdf";
        }
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub2ToDiamond(List<RecordElements.SudCode2> elementOuts)
        {
            List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            if (elementOuts != null)
            {
                int leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = GetGazetteName(record.B460i, record.B405i);
                    /*Setting subcode*/
                    legalEvent.SubCode = "2";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = record.B903i;
                    /*Setting Country Code*/
                    legalEvent.CountryCode = record.B190;
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.B110;
                    biblioData.Publication.Kind = record.B130;
                    biblioData.Publication.Date = DateNormalize(record.B140);

                    biblioData.Application.Number = record.B210;
                    biblioData.Application.Date = DateNormalize(record.B220);

                    foreach (var item in record.B731i)
                    {
                        biblioData.Assignees = new List<PartyMember>();
                        var assignee = new PartyMember
                        {
                            Name = item,
                            Language = "RU"
                        };

                        if (!string.IsNullOrEmpty(record.B980i))
                            assignee.Address1 = record.B980i;

                        biblioData.Assignees.Add(assignee);
                    }

                    /*Notes*/
                    legalEvent.LegalEvent = new LegalEvent
                    {
                        Note = $"|| Bulletin number | {record.B405i}",
                        Date = DateNormalize(record.B460i)
                    };
                    /**********************/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub7ToDiamond(List<RecordElements.SudCode7> elementOuts)
        {
            List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            if (elementOuts != null)
            {
                int leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = GetGazetteName(record.B460i, record.B405i);
                    /*Setting subcode*/
                    legalEvent.SubCode = "7";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = record.B903i;
                    /*Setting Country Code*/
                    legalEvent.CountryCode = record.B190;
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    Biblio newBiblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.B110;
                    biblioData.Publication.Kind = record.B130;
                    biblioData.Publication.Date = DateNormalize(record.B140);

                    biblioData.Application.Number = record.B210;
                    biblioData.Application.Date = DateNormalize(record.B220);

                    foreach (var item in record.B731i)
                    {
                        newBiblioData.Assignees = new List<PartyMember>();
                        var assignee = new PartyMember
                        {
                            Name = item,
                            Language = "RU"
                        };

                        if (!string.IsNullOrEmpty(record.B980i))
                            assignee.Address1 = record.B980i;

                        newBiblioData.Assignees.Add(assignee);
                    }

                    /*Notes*/
                    legalEvent.LegalEvent = new LegalEvent
                    {
                        Note = $"|| Bulletin number | {record.B405i}",
                        Date = DateNormalize(record.B460i)
                    };
                    /**********************/
                    legalEvent.Biblio = biblioData;
                    legalEvent.NewBiblio = newBiblioData;

                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
    }
}
