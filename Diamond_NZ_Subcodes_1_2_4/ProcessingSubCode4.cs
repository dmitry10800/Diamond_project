using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_NZ_Subcodes_1_2_4
{
    public class ProcessingSubCode4
    {
        public static List<SubCode4> ProcessSubCode4(List<XElement> elements)
        {
            var outList = new List<SubCode4>();
            if (elements != null && elements.Count > 0)
            {
                string[] splittedValues = null;
                SubCode4 currentElement;
                var tempInk = 0;
                for (var i = 0; i < elements.Count; i++)
                {
                    var recordValues = "";
                    var value = elements[i].Value;
                    tempInk = i;

                    if (Regex.IsMatch(value, @"^Patent Expired:"))
                    {
                        currentElement = new SubCode4();
                        outList.Add(currentElement);
                        var k = 0;
                        do
                        {
                            recordValues += elements[tempInk].Value + "\n";
                            tempInk++;

                        } while (tempInk < elements.Count && !elements[tempInk].Value.StartsWith("Patent Expired"));


                        if (!string.IsNullOrEmpty(recordValues))
                        {
                            splittedValues = SplitSubCode4(recordValues);
                        }

                        if (splittedValues != null)
                        {
                            foreach (var record in splittedValues)
                            {
                                if (record.StartsWith("Patent Expired:"))
                                {
                                    currentElement.PublicationNumber = record.Replace("Patent Expired:", "").Trim();
                                }

                                if (Regex.IsMatch(record, @"\d{2}\s*\w+\s*\d{4}"))
                                {
                                    currentElement.LegalEventDate = Methods.DateNormalize(record);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Regex.IsMatch(value, @"Patent Expired \d{6} Patent Expired: \d{6} \d{4} \d{2} \w+ \d{4}"))
                        {
                            currentElement = new SubCode4();
                            outList.Add(currentElement);
                            do
                            {
                                recordValues += elements[tempInk].Value;
                                tempInk++;
                            } while (tempInk < elements.Count && !elements[tempInk].Value.StartsWith("Patent Expired"));

                            SubCode4 recognizeCode4 = null;
                            if (!string.IsNullOrEmpty(recordValues))
                            {
                                recognizeCode4 = GetSubCode4Object(recordValues);
                            }

                            if (recognizeCode4 != null)
                            {
                                currentElement.PublicationNumber = recognizeCode4.PublicationNumber;
                                currentElement.LegalEventDate = recognizeCode4.LegalEventDate;
                            }
                        }
                    }
                }
            }
            return outList;
        }

        public static string[] SplitSubCode4(string s)
        {
            var tempStrC = s;
            var splittedRecords = tempStrC.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            return splittedRecords;
        }

        public static SubCode4 GetSubCode4Object(string s)
        {
            var sub = new SubCode4();

            var subCode4 = Regex.Match(s, @"Patent Expired \d{6} Patent Expired: (?<PublNumber>\d{6}) \d{4} (?<EventDate>\d{2} \w+ \d{4})");

            if (subCode4.Success)
            {
                sub.PublicationNumber = subCode4.Groups["PublNumber"].Value;
                sub.LegalEventDate = Methods.DateNormalize(subCode4.Groups["EventDate"].Value);
            }

            return sub;
        }
    }
}
