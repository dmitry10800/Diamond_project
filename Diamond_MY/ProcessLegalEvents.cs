using Diamond.Core.Models;
using Integration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_MY
{
    class ProcessLegalEvents
    {
        public static Regex Pattern = new Regex(@"(?<Id>\d+)\s[A-Z]{2}-*(?<PatNumber>\d+)-*(?<Kind>[A-Z]{1})\s(?<AppNumber>.*)");
        public static List<LegalStatusEvent> Process2SubCode(List<XElement> elements)
        {
            List<LegalStatusEvent> legalEvents = new List<LegalStatusEvent>();
            var records = GetTextFromXelements(elements);
            var eventDate = GetDateFromGazetteName(MY_main.CurrentFileName);
            var gazetteName = Path.GetFileName(MY_main.CurrentFileName.Replace(".tetml", ".pdf"));
            int leCounter = 1;
            foreach (var record in records)
            {
                var match = Pattern.Match(record);
                if (match.Success)
                {
                    LegalStatusEvent lEvent = new LegalStatusEvent();
                    lEvent.GazetteName = gazetteName;
                    /*Setting subcode*//*Setting Section Code*/
                    lEvent.SubCode = "2";
                    lEvent.SectionCode = "MK";
                    /*Setting Country Code*/
                    lEvent.CountryCode = "MY";
                    /*Setting File Name*/
                    lEvent.Id = leCounter++; // creating uniq identifier
                    lEvent.Biblio = new Biblio();
                    lEvent.LegalEvent = new LegalEvent();
                    lEvent.Biblio.Application.Number = match.Groups["AppNumber"].Value.Trim();
                    lEvent.Biblio.Publication.Number = match.Groups["PatNumber"].Value.Trim();
                    lEvent.Biblio.Publication.Kind = match.Groups["Kind"].Value.Trim();
                    lEvent.LegalEvent.Date = eventDate;
                    legalEvents.Add(lEvent);
                }
                else
                {
                    Console.WriteLine($"Record \"{record}\" doesn't match pattern");
                }
            }

            return legalEvents;
        }
        private static List<string> GetTextFromXelements(List<XElement> xElements)
        {
            return String.Join("\n", xElements.Select(x => x.Value).ToList())
                .Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => Pattern.Match(x).Success)
                .ToList();
        }
        private static string GetDateFromGazetteName(string name)
        {
            var pattern = new Regex(@"(?<Year>\d{4})(?<Month>\d{2})(?<Day>\d{2})");
            var date = System.IO.Path.GetFileNameWithoutExtension(name);
            if (pattern.IsMatch(name))
            {
                var a = pattern.Match(name);
                return $"{a.Groups["Year"].Value}-{a.Groups["Month"].Value}-{a.Groups["Day"].Value}";
            }
            return date;
        }
    }
}
