using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_OA
{
    public class ProcessFirstList
    {
        private static readonly string I11 = "(11)";
        private static readonly string I21 = "(21)";
        private static readonly string I22 = "(22)";
        private static readonly string I30 = "(30)";
        private static readonly string I51 = "(51)";
        private static readonly string I54 = "(54)";
        private static readonly string I57 = "(57)";
        private static readonly string I73 = "(73)";
        private static readonly string I72 = "(72)";
        private static readonly string I74 = "(74)";

        public class ElementOut
        {
            public string I11 { get; set; }
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string[] I30C { get; set; }
            public string[] I30N { get; set; }
            public string[] I30D { get; set; }
            public string I54 { get; set; }
            public string I57 { get; set; }
            public string[] I72N { get; set; }
            public string[] I72C { get; set; }
            public string[] I73C { get; set; }
            public string[] I73A { get; set; }
            public string[] I73N { get; set; }
            public string I74N { get; set; }
            public string I74A { get; set; }
            public string I74C { get; set; }
            public string I86 { get; set; }
            public string[] I51D { get; set; }
            public string[] I51C { get; set; }
        }

        public List<ElementOut> OutputValue(List<XElement> elemList)
        {
            List<ElementOut> ElementsOut = new List<ElementOut>();
            ElementOut currentElement;
            if (elemList != null)
            {
                for (int i = 0; i < elemList.Count; ++i)
                {
                    var element = elemList[i];
                    string value = element.Value;
                    string tmpRecordValue = null;
                    string[] splittedRecord = null;
                    int tmpInc;
                    string[] imageNames;
                    string tmpRecValue;
                    if (value.StartsWith(I11) && Regex.IsMatch(value, @"\(11\)\s*\d+"))
                    {
                        currentElement = new ElementOut();
                        ElementsOut.Add(currentElement);
                        imageNames = null;
                        tmpRecordValue = "";
                        tmpInc = i;
                        do
                        {
                            /*image search*/
                            if (elemList[tmpInc].Name.LocalName == "PlacedImage")
                            {
                                imageNames = (imageNames ?? Enumerable.Empty<string>()).Concat(new string[] { elemList[tmpInc].Attribute("image").Value }).ToArray();
                            }
                            tmpRecordValue += elemList[tmpInc].Value + "\n";
                            ++tmpInc;
                        } while (tmpInc < elemList.Count()
                        && !Regex.IsMatch(elemList[tmpInc].Value, @"^\(11\)\s*\d+")
                        //&& !elemList[tmpInc].Value.StartsWith(I11)
                        );
                        if (tmpRecordValue != null)
                        {
                            splittedRecord = Methods.RecSplit(tmpRecordValue);
                        }
                        foreach (var inidCode in splittedRecord)
                        {
                            tmpRecValue = null;
                            /*Act number*/
                            if (inidCode.StartsWith(I11))
                            {
                                tmpRecValue = inidCode.Replace(I11, "").Replace("\n", "").Trim();
                                currentElement.I11 = tmpRecValue;
                                /*Saving images*/
                                //if (imageNames != null)
                                //{
                                //    int imgCount = 1;
                                //    foreach (var image in imageNames)
                                //    {
                                //        string ext = Path.GetExtension(Images[image]);
                                //        string imageFileName = Path.Combine(root.FullName, Images[image]);
                                //        if (File.Exists(imageFileName))
                                //            try
                                //            {
                                //                File.Copy(imageFileName, Path.Combine(processed.FullName, currentElement.I11 + "_" + imgCount) + ext);
                                //            }
                                //            catch (Exception)
                                //            {
                                //                Console.WriteLine("Image already exist:\t" + fileName);
                                //            }
                                //        else
                                //        {
                                //            Console.WriteLine("Cannot locate file " + fileName);
                                //        }
                                //        imgCount++;
                                //    }
                                //}

                            }
                            /*Priority value*/
                            if (inidCode.StartsWith(I51))
                            {
                                var intClass = Methods.IntClassSplit(inidCode);
                                if (intClass != null)
                                {
                                    currentElement.I51D = intClass.Date;
                                    currentElement.I51C = intClass.Class;
                                }
                            }
                            /*21 and 86*/
                            if (inidCode.StartsWith(I21))
                            {
                                tmpRecValue = inidCode.Replace(I21, "").Replace("\n", "").Trim();
                                if (tmpRecValue.Contains("-"))
                                {
                                    currentElement.I21 = tmpRecValue.Remove(tmpRecValue.IndexOf("-")).Trim();
                                    try
                                    {
                                        currentElement.I86 = tmpRecValue.Substring(tmpRecValue.IndexOf("-") + 1).Trim();
                                    }
                                    catch (Exception)
                                    {
                                        currentElement.I86 = tmpRecValue.Substring(tmpRecValue.IndexOf("-")).Trim();
                                    }
                                }
                                else if (tmpRecValue.Contains("–"))
                                {
                                    currentElement.I21 = tmpRecValue.Remove(tmpRecValue.IndexOf("–")).Trim();
                                    currentElement.I86 = tmpRecValue.Substring(tmpRecValue.IndexOf("–") + 1).Trim();
                                }
                                else
                                {
                                    currentElement.I21 = tmpRecValue;
                                }
                            }
                            /*22 Date*/
                            if (inidCode.StartsWith(I22))
                            {
                                tmpRecValue = Methods.DateNormalize(inidCode.Replace(I22, "").Replace("\n", "").Trim());
                                currentElement.I22 = tmpRecValue;
                            }
                            /*30 Priority*/
                            if (inidCode.StartsWith(I30))
                            {
                                var priority = Methods.PrioritySplitting(inidCode);
                                if (priority != null)
                                {
                                    currentElement.I30D = priority.Date;
                                    currentElement.I30C = priority.Country;
                                    currentElement.I30N = priority.Number;
                                }
                            }
                            /*54 title*/
                            if (inidCode.StartsWith(I54))
                            {
                                tmpRecValue = inidCode.Replace(I54, "").Replace("-\n", " ").Replace("\n", " ").Trim();
                                currentElement.I54 = tmpRecValue;
                            }
                            /*57 description*/
                            if (inidCode.StartsWith(I57))
                            {
                                tmpRecValue = inidCode.Replace(I57, "").Replace("\n", " ").Trim();
                                currentElement.I57 = tmpRecValue;
                            }
                            /*72 Inventor*/
                            if (inidCode.StartsWith(I72))
                            {
                                var inventor = Methods.InventorSplit(inidCode);
                                if (inventor != null)
                                {
                                    currentElement.I72C = inventor.Country;
                                    currentElement.I72N = inventor.Name;
                                }
                            }
                            /*73 Assignee*/
                            if (inidCode.StartsWith(I73))
                            {
                                var assignee = Methods.AssigneeSplitting(inidCode);
                                if (assignee != null)
                                {
                                    currentElement.I73A = assignee.Address;
                                    currentElement.I73N = assignee.Name;
                                    currentElement.I73C = assignee.Country;
                                }
                            }
                            /*74 Agent*/
                            if (inidCode.StartsWith(I74))
                            {
                                var agent = Methods.AgentSplitting(inidCode);
                                if (agent != null)
                                {
                                    currentElement.I74A = agent.Address;
                                    currentElement.I74N = agent.Name;
                                    currentElement.I74C = agent.Country;
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
