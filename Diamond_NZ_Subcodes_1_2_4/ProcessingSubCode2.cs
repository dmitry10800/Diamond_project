using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_NZ_Subcodes_1_2_4
{
    public class ProcessingSubCode2
    {
        public static List<SubCode2> ProcessSubCode2(List<XElement> elements)
        {
            List<SubCode2> outList = new List<SubCode2>();
            if (elements != null && elements.Count > 0)
            {
                string[] splittedValues = null;
                SubCode2 currentElement;
                int tempInk = 0;
                for (int i = 0; i < elements.Count; i++)
                {
                    string recordValues = "";
                    var value = elements[i].Value;
                    tempInk = i;

                    if (Regex.IsMatch(value, @"^Patent Lapsed:"))
                    {
                        currentElement = new SubCode2();
                        outList.Add(currentElement);
                        int k = 0;
                        do
                        {
                            recordValues += elements[tempInk].Value + "\n";
                            tempInk++;

                        } while (tempInk < elements.Count && !elements[tempInk].Value.StartsWith("Patent Lapsed"));


                        if (!string.IsNullOrEmpty(recordValues))
                        {
                            splittedValues = SplitSubCode2(recordValues);
                        }

                        if (splittedValues != null)
                        {
                            foreach (var record in splittedValues)
                            {
                                if (record.StartsWith("Patent Lapsed:"))
                                {
                                    currentElement.PublicationNumber = record.Replace("Patent Lapsed:", "").Trim();
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
                        if (Regex.IsMatch(value, @"Patent Lapsed \d{6} Patent Lapsed: \d{6} \d{4} \d{2} \w+ \d{4}"))
                        {
                            currentElement = new SubCode2();
                            outList.Add(currentElement);
                            do
                            {
                                recordValues += elements[tempInk].Value;
                                tempInk++;
                            } while (tempInk < elements.Count && !elements[tempInk].Value.StartsWith("Patent Lapsed"));

                            SubCode2 recognizeCode2 = null;
                            if (!string.IsNullOrEmpty(recordValues))
                            {
                                recognizeCode2 = GetSubCode2Object(recordValues);
                            }

                            if (recognizeCode2 != null)
                            {
                                currentElement.PublicationNumber = recognizeCode2.PublicationNumber;
                                currentElement.LegalEventDate = recognizeCode2.LegalEventDate;
                            }
                        }
                    }
                }
            }
            return outList;
        }


        public static string[] SplitSubCode2(string s)
        {
            string tempStrC = s;
            string[] splittedRecords = tempStrC.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            return splittedRecords;
        }

        public static SubCode2 GetSubCode2Object(string s)
        {
            SubCode2 sub = new SubCode2();

            var subCode2 = Regex.Match(s, @"Patent Lapsed \d{6} Patent Lapsed: (?<PublNumber>\d{6}) \d{4} (?<EventDate>\d{2} \w+ \d{4})");

            if (subCode2.Success)
            {
                sub.PublicationNumber = subCode2.Groups["PublNumber"].Value;
                sub.LegalEventDate = Methods.DateNormalize(subCode2.Groups["EventDate"].Value);
            }

            return sub;
        }
    }
}
