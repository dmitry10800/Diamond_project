using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_BA
{
    public class ProcessThirdList
    {
        private static readonly string I11 = "(11)";
        private static readonly string I21 = "(21)";
        private static readonly string I22 = "(22)";
        private static readonly string I26 = "(26)";
        private static readonly string I51 = "(51)";
        private static readonly string I54 = "(54)";
        private static readonly string I57 = "(57)";
        private static readonly string I75 = "(75)";

        public class ElementOut
        {
            public string I11 { get; set; }
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string I26 { get; set; }
            public string[] I51Version { get; set; }
            public string[] I51Class { get; set; }
            public string I54 { get; set; }
            public string I57 { get; set; }
            public string[] I75N { get; set; }
            public string[] I75A { get; set; }
            public string[] I75C { get; set; }

        }

        public List<ElementOut> OutputValue(List<XElement> elemList)
        {
            var ElementsOut = new List<ElementOut>();
            ElementOut currentElement;
            if (elemList != null)
            {

                for (var i = 0; i < elemList.Count; ++i)
                {
                    var element = elemList[i];
                    var value = element.Value;
                    string tmpRecordValue = null;
                    string[] splittedRecord = null;
                    int tmpInc;
                    string tmpRecValue;
                    if (value.StartsWith(I11) && Regex.IsMatch(value, @"\(11\)\s*BAP\d+"))
                    {
                        currentElement = new ElementOut();
                        ElementsOut.Add(currentElement);
                        tmpRecordValue = "";
                        tmpInc = i;
                        do
                        {
                            tmpRecordValue += elemList[tmpInc].Value + "\n";
                            ++tmpInc;
                        } while (tmpInc < elemList.Count() && !Regex.IsMatch(elemList[tmpInc].Value, @"\(11\)\s*BAP\d+"));
                        if (tmpRecordValue != null)
                        {
                            splittedRecord = Methods.RecSplit(tmpRecordValue);
                        }
                        foreach (var inidCode in splittedRecord)
                        {
                            tmpRecValue = null;
                            /*11*/
                            if (inidCode.StartsWith(I11))
                            {
                                tmpRecValue = inidCode.Replace(I11, "").Replace("\n", "").Trim();
                                currentElement.I11 = tmpRecValue;
                            }
                            /*21*/
                            if (inidCode.StartsWith(I21))
                            {
                                tmpRecValue = inidCode.Replace(I21, "").Replace("\n", "").Trim();
                                currentElement.I21 = tmpRecValue;
                            }
                            /*22*/
                            if (inidCode.StartsWith(I22))
                            {
                                tmpRecValue = inidCode.Replace(I22, "").Replace("\n", "").Trim();
                                currentElement.I22 = tmpRecValue;
                            }
                            /*26*/
                            if (inidCode.StartsWith(I26))
                            {
                                tmpRecValue = inidCode.Replace(I26, "").Replace("\n", "").Trim();
                                if (tmpRecValue != "")
                                {
                                    currentElement.I26 = tmpRecValue;
                                }
                            }
                            /*51*/
                            if (inidCode.StartsWith(I51))
                            {
                                var tmpResults = Methods.ClassificationInfoSplit(inidCode.Replace("\n", "").Trim());
                                currentElement.I51Class = tmpResults.Classification;
                                currentElement.I51Version = tmpResults.Version;
                            }
                            /*54 title*/
                            if (inidCode.StartsWith(I54))
                            {
                                tmpRecValue = inidCode.Replace(I54, "").Replace("-\n", " ").Replace("\n", " ").Trim();
                                currentElement.I54 = tmpRecValue;
                            }
                            /*57*/
                            if (inidCode.StartsWith(I57))
                            {
                                tmpRecValue = inidCode.Replace(I57, "").Replace("\n", " ").Trim();
                                if (tmpRecValue != "")
                                {
                                    currentElement.I57 = tmpRecValue;
                                }
                            }
                            /*72*/
                            if (inidCode.StartsWith(I75))
                            {
                                var tmpValue = Methods.NameSplWithAddress(inidCode);
                                if (tmpValue.Name != null) currentElement.I75N = tmpValue.Name;
                                if (tmpValue.Country != null) currentElement.I75C = tmpValue.Country;
                                if (tmpValue.Address != null) currentElement.I75A = tmpValue.Address;
                            }
                        }
                    }
                }
            }
            return ElementsOut;
        }
    }
}
