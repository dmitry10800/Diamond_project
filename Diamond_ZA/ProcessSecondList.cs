using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_ZA
{
    public class ProcessSecondList
    {
        private static readonly string I21 = "21:";
        private static readonly string I22 = "22:";
        private static readonly string I43 = "43:";
        private static readonly string I51 = "51:";
        private static readonly string I54 = "54:";
        private static readonly string I71 = "71:";
        private static readonly string I72 = "72:";
        private static readonly string I31 = "31:";
        private static readonly string I32 = "32:";
        private static readonly string I33 = "33:";
        private static readonly string I00 = "00:";

        public class ElementOut
        {
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string I54 { get; set; }
            public string I43 { get; set; }
            public string I71 { get; set; }
            public string I00 { get; set; }
            public string[] I51 { get; set; }
            public string I72 { get; set; }
            public string[] I31 { get; set; }
            public string[] I32 { get; set; }
            public string[] I33 { get; set; }
        }

        public List<ElementOut> OutputValue(List<XElement> elemList)
        {
            List<ElementOut> ElementsOut = new List<ElementOut>();

            if (elemList != null)
            {
                ElementOut currentElement = null;
                for (int i = 0; i < elemList.Count; ++i)
                {
                    var element = elemList[i];
                    string value = element.Value;
                    string[] recordSplitted = null;
                    if (value.StartsWith(I21))
                    {
                        currentElement = new ElementOut();
                        ElementsOut.Add(currentElement);
                        int tmpInc = i;
                        string tmpRecordValue = "";
                        do
                        {
                            tmpRecordValue += elemList[tmpInc].Value + " ";
                            ++tmpInc;
                        } while (tmpInc < elemList.Count()
                        && !elemList[tmpInc].Value.StartsWith(I21));
                        recordSplitted = Methods.RecSplitSecondList(tmpRecordValue);
                        if (recordSplitted != null)
                        {

                            foreach (var item in recordSplitted)
                            {
                                /*21*/
                                if (item.StartsWith(I21))
                                {
                                    currentElement.I21 = item.Replace(I21, "").Trim().Trim('.');
                                }
                                /*22*/
                                if (item.StartsWith(I22))
                                {
                                    if (Regex.IsMatch(item, @"\d{4}(\/|\-)\d{2}(\/|\-)\d{2}"))
                                    {
                                        currentElement.I22 = Regex.Match(item, @"\d{4}(\/|\-)\d{2}(\/|\-)\d{2}").Value;
                                    }
                                    else if (Regex.IsMatch(item, @"\d+(\/|\-)\d+(\/|\-)\d{4}"))
                                    {
                                        var s = Regex.Match(item, @"(?<month>\d+)(\/|\-)(?<day>\d+)(\/|\-)(?<year>\d{4})");
                                        string day = s.Groups["day"].Value.Trim();
                                        string month = s.Groups["month"].Value.Trim();
                                        string year = s.Groups["year"].Value.Trim();
                                        if (day.Length == 1) day = "0" + day;
                                        if (month.Length == 1) month = "0" + month;
                                        currentElement.I22 = year + "-" + month + "-" + day;
                                    }
                                    else currentElement.I22 = item.Replace(I22, "").Replace("/", "-").Trim();
                                }
                                /*43*/
                                if (item.StartsWith(I43))
                                {
                                    if (Regex.IsMatch(item, @"\d{4}(\/|\-)\d{2}(\/|\-)\d{2}"))
                                    {
                                        currentElement.I43 = Regex.Match(item, @"\d{4}(\/|\-)\d{2}(\/|\-)\d{2}").Value.Replace("/", "-");
                                    }
                                    else if (Regex.IsMatch(item, @"\d+(\/|\-)\d+(\/|\-)\d{4}"))
                                    {
                                        var s = Regex.Match(item, @"(?<month>\d+)(\/|\-)(?<day>\d+)(\/|\-)(?<year>\d{4})");
                                        string day = s.Groups["day"].Value.Trim();
                                        string month = s.Groups["month"].Value.Trim();
                                        string year = s.Groups["year"].Value.Trim();
                                        if (day.Length == 1) day = "0" + day;
                                        if (month.Length == 1) month = "0" + month;
                                        currentElement.I43 = year + "-" + month + "-" + day;
                                    }
                                    else currentElement.I43 = item.Replace(I43, "").Replace("/", "-").Trim();
                                }
                                /*51*/
                                if (item.StartsWith(I51))
                                {
                                    string tmpValue = item.Replace(I51, "").Trim().Replace(" ", ";").Replace(";;", ";");
                                    string[] splitedI51 = null;
                                    if (tmpValue.Contains(";"))
                                    {
                                        splitedI51 = tmpValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                                        foreach (var itemSplitted in splitedI51)
                                        {
                                            currentElement.I51 = (currentElement.I51 ?? Enumerable.Empty<string>()).Concat(new string[] { itemSplitted.Trim() }).ToArray();
                                        }
                                    }
                                    else
                                    {
                                        currentElement.I51 = (currentElement.I51 ?? Enumerable.Empty<string>()).Concat(new string[] { tmpValue.Trim() }).ToArray();
                                    }
                                }
                                /*54*/
                                if (item.StartsWith(I54))
                                {
                                    currentElement.I54 = item.Replace(I54, "").Trim();
                                }
                                /*71*/
                                if (item.StartsWith(I71))
                                {
                                    currentElement.I71 = item.Replace(I71, "").Trim();
                                }
                                /*72*/
                                if (item.StartsWith(I72))
                                {
                                    currentElement.I72 = item.Replace(I72, "").Trim();
                                }
                                /*00 Description*/
                                if (item.StartsWith(I00))
                                {
                                    if (item.Replace(I00, "").Trim().StartsWith("- "))
                                    {
                                        currentElement.I00 = item.Substring(item.IndexOf("- ") + 1).Trim();
                                    }
                                    else
                                    {
                                        currentElement.I00 = item.Replace(I00, "").Trim();
                                    }
                                }
                                /*31*/
                                if (item.StartsWith(I31))
                                {
                                    currentElement.I31 = (currentElement.I31 ?? Enumerable.Empty<string>()).Concat(new string[] { item.Replace(I31, "").Trim() }).ToArray();
                                }
                                /*32*/
                                if (item.StartsWith(I32))
                                {
                                    currentElement.I32 = (currentElement.I32 ?? Enumerable.Empty<string>()).Concat(new string[] { item.Replace(I32, "").Replace("/", "-").Trim() }).ToArray();
                                }
                                /*33*/
                                if (item.StartsWith(I33))
                                {
                                    string tmpValue = item.Replace(I33, "").Trim();
                                    tmpValue = Regex.IsMatch(tmpValue, @"^[A-Z]{2}.*\([A-Z]{2}\)") ? tmpValue = Regex.Match(tmpValue, @"[A-Z]{2}").Value : tmpValue = item.Replace(I33, "").Trim();
                                    currentElement.I33 = (currentElement.I33 ?? Enumerable.Empty<string>()).Concat(new string[] { tmpValue }).ToArray();
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
