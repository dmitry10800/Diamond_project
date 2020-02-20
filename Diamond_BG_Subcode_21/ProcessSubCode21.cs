using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Diamond_BG_Subcode_21
{
    public class ProcessSubCode21
    {
        public static List<Subcode21> ProcessingSubCode21(List<string> elements, string nameGazette)
        {
            List<Subcode21> outList = new List<Subcode21>();
            Subcode21 currentElement = null;
            LegalStatusEvents legalStatus = null;

            string fullStrWithPatents = "";
            if (elements.Count > 0)
            {

                for (int i = 0; i < elements.Count; i++)
                {
                    if (Regex.IsMatch(elements[i], @"^BG/EP"))
                        fullStrWithPatents += elements[i];
                }
            }

            var splittedRecords = Methods.SplitRecords(fullStrWithPatents);

            if (splittedRecords != null)
            {
                foreach (var splittedRecord in splittedRecords)
                {
                    currentElement = new Subcode21();
                    legalStatus = new LegalStatusEvents();

                    if (Regex.IsMatch(splittedRecord, @"BG/EP \d{7} [A-Z]\d{1,2}"))
                    {
                        //есть kind code

                        var patent = Regex.Match(splittedRecord, @"(?<PublNumber>BG/EP \d{7})\s*(?<Kind>[A-Z]\d{1,2})");

                        if (patent.Success)
                        {
                            currentElement.PublicationNumber = patent.Groups["PublNumber"].Value;
                            currentElement.PublicationKind = patent.Groups["Kind"].Value;
                            legalStatus.PatentNumber = patent.Groups["PublNumber"].Value;
                            legalStatus.EventDate = Methods.GetLegalEventDate(nameGazette);
                            currentElement.LegalStatusEvents = legalStatus;
                        }
                    }
                    else if (Regex.IsMatch(splittedRecord, @"BG/EP \d{7}"))
                    {
                        //нету kind code

                        var patent = Regex.Match(splittedRecord, @"(?<PublNumber>BG/EP \d{7})");

                        if (patent.Success)
                        {
                            currentElement.PublicationNumber = patent.Groups["PublNumber"].Value;
                            legalStatus.PatentNumber = patent.Groups["PublNumber"].Value;
                            legalStatus.EventDate = Methods.GetLegalEventDate(nameGazette);
                            currentElement.LegalStatusEvents = legalStatus;
                        }
                    }
                    outList.Add(currentElement);
                }
                return outList;
            }
            return null;
        }
    }
}
