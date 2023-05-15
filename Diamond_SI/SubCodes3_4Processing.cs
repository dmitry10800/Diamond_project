using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_SI
{
    public class SubCodes3_4Processing
    {
        public static (List<Subcode3>, List<Subcode4>) ProcessSubcodes(List<XElement> elements)
        {
            var listSubCode3Elements = new List<Subcode3>();
            var listSubCode4Elements = new List<Subcode4>();

            var allElements = new List<string>();
            var tempInc = 0;
            for (var i = 0; i < elements.Count; i++)
            {
                tempInc = i;

                var flag = Regex.IsMatch(elements[i].Value, @"^[^\p{L}]");

                if (flag)
                {
                    var regex = new Regex(@"^\d{3}");
                    var matches = regex.Matches(elements[tempInc].Value);
                    if (matches.Count > 0)
                    {
                        allElements.Add(elements[i].Value.Replace(matches[0].Value, "****" + matches[0].Value));
                    }
                    else
                        allElements.Add(elements[i].Value);
                }
            }

            string[] splittedRecord;
            for (var i = 0; i < allElements.Count; i++)
            {
                splittedRecord = null;
                int ink;
                var recordsStr = "";
                if (allElements[i].StartsWith("****"))
                {
                    ink = i;
                    do
                    {
                        recordsStr += allElements[ink] + "\n";
                        ink++;
                    } while (ink < allElements.Count && !allElements[ink].StartsWith("****"));


                    if (!string.IsNullOrEmpty(recordsStr))
                    {
                        splittedRecord = Methods.RecordsSplit(recordsStr);
                    }

                    if (splittedRecord != null)
                    {
                        if (Regex.IsMatch(splittedRecord[0], @"^\d{5}\b"))
                        {
                            //Subcode 3
                            listSubCode3Elements.Add(SubCode3Processing.ProcessSubCode3Element(splittedRecord));
                        }

                        if (Regex.IsMatch(splittedRecord[0], @"^\d{7}\b"))
                        {
                            //Subcode 4
                            listSubCode4Elements.Add(SubCode4Processing.ProcessSubCode4Element(splittedRecord));
                        }
                    }
                }
            }
            return (listSubCode3Elements, listSubCode4Elements);
        }
    }
}
