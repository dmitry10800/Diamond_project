using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_BE
{
    public class Processing
    {
        private static readonly string _11 = "(11)";
        private static readonly string _21 = "(21)";
        private static readonly string _12 = "(12)";
        private static readonly string _47 = "(47)";
        private static readonly string _73 = "(73)";
        private static readonly string _74 = "(74)";
        private static readonly string _54 = "(54)";
        private static readonly string EventDate = "(99)";
        public static List<Elements> SubCode4(List<XElement> elements)
        {
            var elementsOut = new List<Elements>();

            if (elements != null && elements.Count > 0)
            {
                string[] splittedRecord = null;
                string tmpRecordValue;
                int tmpInc;
                for (var i = 0; i < elements.Count; i++)
                {
                    var value = elements[i].Value;
                    if (value.StartsWith(_21))
                    {
                        var currentElement = new Elements();
                        elementsOut.Add(currentElement);
                        tmpRecordValue = "";
                        tmpInc = i;
                        do
                        {
                            tmpRecordValue += elements[tmpInc].Value + "\n";
                            ++tmpInc;
                        } while (tmpInc < elements.Count && !elements[tmpInc].Value.StartsWith(_21));

                        if (tmpRecordValue != null)
                            splittedRecord = Methods.RecSplit(tmpRecordValue);

                        foreach (var record in splittedRecord)
                        {
                            if (record.StartsWith(_11))
                            {
                                currentElement.PubNumber = record.Replace(_11, "").Trim();
                            }
                            if (record.StartsWith(_21))
                            {
                                currentElement.AppNumber = record.Replace(_21, "").Trim();
                            }
                            if (record.StartsWith(_12))
                            {
                                currentElement.PubLang = record.Replace(_12, "").Trim();
                            }
                            if (record.StartsWith(_47))
                            {
                                currentElement.Date47 = Methods.DateNormalize(record.Replace(_47, "").Trim());
                                currentElement.Date45 = currentElement.Date47;
                            }
                            if (record.StartsWith(_73))
                            {
                                var split = record.Split('\n');
                                var owner = new Owner();
                                owner.Name = split[0].Replace(_73, "").Trim();
                                owner.Address = split[1].Trim() + " " + split[2].Trim();
                                owner.Country = Methods.ToOWNC(split.Last());
                                currentElement.Owner = owner;
                            }
                            if (record.StartsWith(_74))
                            {
                                var agent = new Agent();
                                agent.Name = record.Replace(_74, "").Replace("\n", " ").Trim();
                                currentElement.Agent = agent;
                            }
                            if (record.StartsWith(_54))
                            {
                                currentElement.Title = record.Replace(_54, "").Replace("\n", " ").Trim();
                            }
                            if (record.StartsWith(EventDate))
                            {
                                currentElement.EventDate = Methods.DateNormalize(record.Replace(EventDate, "").Trim());
                            }
                        }
                    }
                }
            }


            return elementsOut;
        }
    }
}
