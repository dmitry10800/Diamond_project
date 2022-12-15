using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_ZA
{
    public class ProcessFirstList
    {
        private static readonly string I54 = "54:";
        private static readonly string I71 = "71:";
        private static readonly string I72 = "72:";
        private static readonly string I31 = "31:";
        private static readonly string I32 = "32:";
        private static readonly string I33 = "33:";

        public class ElementOut
        {
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string I54 { get; set; }
            public string[] I71Name { get; set; }
            public string[] I71Adress { get; set; }
            public string[] I71CountryCode { get; set; }
            public string[] I72 { get; set; }
            public string[] I31 { get; set; }
            public string[] I32 { get; set; }
            public string[] I33 { get; set; }
        }

        public List<ElementOut> OutputValue(List<XElement> elemList)
        {
            List<ElementOut> ElementsOut = new List<ElementOut>();
            ElementOut currentElement = null;
            List<string> sortedElements = null;
            sortedElements = new List<string>();
            string I22Date = null;
            if (elemList != null)
            {
                for (int i = 0; i < elemList.Count; ++i)
                {
                    var element = elemList[i];
                    string value = element.Value;

                    if (Regex.IsMatch(value, @"\d{4}\/\d{5}\s*\~") /*&& !value.StartsWith("")*/)
                    {
                        int tmpInc = i;
                        string tmpRecordValue = "";
                        do
                        {
                            tmpRecordValue += elemList[tmpInc].Value + " ";
                            ++tmpInc;
                        } while (tmpInc < elemList.Count()
                        && !Regex.IsMatch(elemList[tmpInc].Value, @"\d{4}\/\d{5}\s*\~")
                        && !elemList[tmpInc].Value.StartsWith("- APPLIED ON"));
                        sortedElements.Add(tmpRecordValue.Trim());
                    }
                    if (Regex.IsMatch(value, @"\-\s*APPLIED\s*ON"))
                    {
                        sortedElements.Add(value.Trim());
                    }
                }
                if (sortedElements != null)
                {
                    foreach (var record in sortedElements)
                    {
                        if (Regex.IsMatch(record, @"\-\s*APPLIED\s*ON"))
                        {
                            string tmpRec = record.Substring(record.IndexOf("APPLIED"));
                            I22Date = tmpRec.Replace("APPLIED ON", "").Replace("-", "").Replace(".", "").Replace("/", "-").Trim();
                        }
                        string[] recordSplitted = Methods.RecSplit(record);
                        if (recordSplitted != null)
                        {
                            foreach (var item in recordSplitted)
                            {
                                /*21*/
                                if (Regex.IsMatch(item, @"^\d{4}\/\d{5}"))
                                {
                                    currentElement = new ElementOut();
                                    ElementsOut.Add(currentElement);
                                    currentElement.I21 = item;
                                }
                                /*22*/
                                if (I22Date != null)
                                {
                                    if (Regex.IsMatch(I22Date, @"\d{4}\-\d{2}\-\d{2}"))
                                    {
                                        currentElement.I22 = Regex.Match(I22Date, @"\d{4}\-\d{2}\-\d{2}").Value;
                                    }
                                    else if (Regex.IsMatch(I22Date, @"\d+\-\d{2}\-\d{4}"))
                                    {
                                        var date = Regex.Match(I22Date, @"(?<month>\d+)\-(?<day>\d{2})\-(?<year>\d{4})");
                                        if (date.Groups["month"].Length == 1)
                                        {
                                            currentElement.I22 = date.Groups["year"].Value + "-0" + date.Groups["month"].Value + "-" + date.Groups["day"].Value;
                                        }
                                        else
                                        {
                                            currentElement.I22 = date.Groups["year"].Value + "-" + date.Groups["month"].Value + "-" + date.Groups["day"].Value;
                                        }
                                    }
                                    else currentElement.I22 = I22Date;
                                }
                                /*54*/
                                if (item.StartsWith(I54))
                                {
                                    currentElement.I54 = item.Replace(I54, "").Trim();
                                }
                                /*71*/
                                if (item.StartsWith(I71))
                                {
                                    var (ownerName, ownerAddress, ownerCountry) = Methods.OwnerSplit(item);
                                    currentElement.I71Name = ownerName;
                                    currentElement.I71Adress = ownerAddress;
                                    currentElement.I71CountryCode = ownerCountry;
                                }
                                /*72*/
                                if (item.StartsWith(I72))
                                {
                                    currentElement.I72 = Methods.StSplit(item.Trim());
                                }
                                /*31*/
                                if (item.StartsWith(I31))
                                {
                                    currentElement.I31 = (currentElement.I31 ?? Enumerable.Empty<string>()).Concat(new string[] { item.Replace(I31, "").Trim() }).ToArray();
                                }
                                /*32*/
                                if (item.StartsWith(I32))
                                {
                                    string tmpItem = item.Replace(I32, "").Replace(";", "").Trim();
                                    if (Regex.IsMatch(tmpItem, @"\d{2}\/\d{2}\/\d{4}")) tmpItem = Regex.Match(tmpItem, @"\d{2}\/\d{2}\/\d{4}").Value;
                                    currentElement.I32 = (currentElement.I32 ?? Enumerable.Empty<string>()).Concat(new string[] { Methods.DateSwap(tmpItem) }).ToArray();
                                }
                                /*33*/
                                if (item.StartsWith(I33))
                                {
                                    currentElement.I33 = (currentElement.I33 ?? Enumerable.Empty<string>()).Concat(new string[] { item.Replace(I33, "").Trim() }).ToArray();
                                }
                            }
                        }
                    }
                }
            }
            return ElementsOut;
        }
    }
}
