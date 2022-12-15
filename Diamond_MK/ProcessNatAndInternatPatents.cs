using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_MK
{
    public class ProcessNatAndInternatPatents
    {
        private static readonly string I11 = "(11)";
        private static readonly string I13 = "(13)";
        private static readonly string I21 = "(21)";
        private static readonly string I22 = "(22)";
        private static readonly string I30 = "(30)";
        private static readonly string I45 = "(45)";
        private static readonly string I51 = "(51)";
        private static readonly string I54 = "(54)";
        private static readonly string I57 = "(57)";
        private static readonly string I73 = "(73)";
        private static readonly string I72 = "(72)";
        private static readonly string I74 = "(74)";
        private static readonly string I96 = "(96)";
        private static readonly string I97 = "(97)";

        public class ElementOut
        {
            public string I11 { get; set; }
            public string I13 { get; set; }
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string[] I30C { get; set; }
            public string[] I30N { get; set; }
            public string[] I30D { get; set; }
            public string I45 { get; set; }
            public string[] I51 { get; set; }
            public string I54 { get; set; }
            public string I57 { get; set; }
            public string[] I72 { get; set; }
            public string I73 { get; set; }
            public string I74N { get; set; }
            public string I74A { get; set; }
            public string I96D { get; set; }
            public string I96N { get; set; }
            public string I97D { get; set; }
            public string I97N { get; set; }
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
                    if (value.StartsWith(I51))
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
                            tmpRecordValue += elemList[tmpInc].Value.Replace("\n", " ") + "\n";
                            ++tmpInc;
                        } while (tmpInc < elemList.Count()
                        && !elemList[tmpInc].Value.StartsWith(I51));
                        if (tmpRecordValue != null)
                        {
                            splittedRecord = Methods.RecSplit(tmpRecordValue);
                        }
                        foreach (var inidCode in splittedRecord)
                        {
                            tmpRecValue = null;
                            /*Priority value*/
                            if (inidCode.StartsWith(I51))
                            {
                                string[] splPrio = null;
                                tmpRecValue = inidCode.Replace(I51, "").Replace("\n", " ").Replace(" ", "").Trim();
                                if (tmpRecValue.Contains(","))
                                {
                                    splPrio = tmpRecValue.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                }
                                try
                                {
                                    currentElement.I51 = splPrio.Select(x => x.Insert(4, " ")).ToArray();
                                }
                                catch (Exception)
                                {
                                    currentElement.I51 = splPrio;
                                }
                            }
                            /*Act number*/
                            if (inidCode.StartsWith(I11))
                            {
                                tmpRecValue = inidCode.Replace(I11, "").Replace("\n", "").Trim();
                                currentElement.I11 = tmpRecValue;
                                ///*Saving images*/
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
                            /**/
                            if (inidCode.StartsWith(I13))
                            {
                                tmpRecValue = inidCode.Replace(I13, "").Replace("\n", "").Trim();
                                currentElement.I13 = tmpRecValue;
                            }
                            /**/
                            if (inidCode.StartsWith(I21))
                            {
                                tmpRecValue = inidCode.Replace(I21, "").Replace("\n", "").Trim();
                                currentElement.I21 = tmpRecValue;
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
                                //string[] splPriorityRecord = null;
                                List<string> prioInstance = new List<string>();
                                tmpRecValue = inidCode.Replace(I30, "").Trim();
                                if (tmpRecValue.Contains("\n") && (tmpRecValue.Contains(" and ") || tmpRecValue.Contains(" and")))
                                {
                                    string[] tmpSplValue = tmpRecValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    if (tmpSplValue.Count() > 0)
                                    {
                                        foreach (var priority in tmpSplValue)
                                        {
                                            if (priority.Contains(" and"))
                                            {
                                                string[] tmpSlpValueA = priority.Split(new string[] { " and" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                                foreach (var item in tmpSlpValueA)
                                                {
                                                    prioInstance.Add(item.Trim());
                                                }
                                            }
                                            else
                                            {
                                                prioInstance.Add(priority.Trim());
                                            }
                                        }
                                    }
                                }
                                if (tmpRecValue.Contains(" and") && !tmpRecValue.Contains("\n"))
                                {
                                    string[] tmpSlpValueA = tmpRecValue.Split(new string[] { " and" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    foreach (var item in tmpSlpValueA)
                                    {
                                        prioInstance.Add(item.Trim());
                                    }
                                }
                                if (!tmpRecValue.Contains(" and") && !tmpRecValue.Contains("\n") && tmpRecValue.Contains(" "))
                                {
                                    try
                                    {
                                        currentElement.I30N = (currentElement.I30N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Remove(tmpRecValue.IndexOf(" ")).Trim() }).ToArray();
                                        currentElement.I30D = (currentElement.I30D ?? Enumerable.Empty<string>()).Concat(new string[] { Methods.DateNormalize(tmpRecValue.Remove(tmpRecValue.LastIndexOf(" ")).Substring(tmpRecValue.IndexOf(" ")).Trim()) }).ToArray();
                                        currentElement.I30C = (currentElement.I30C ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Substring(tmpRecValue.LastIndexOf(" ")).Trim() }).ToArray();

                                    }
                                    catch (Exception A)
                                    {
                                        Console.WriteLine("Priority identification error in:\t" + currentElement.I11);
                                    }
                                }
                                if (prioInstance != null)
                                {
                                    foreach (var priorityRecords in prioInstance)
                                    {
                                        if (priorityRecords.Contains(" "))
                                        {
                                            try
                                            {
                                                currentElement.I30N = (currentElement.I30N ?? Enumerable.Empty<string>()).Concat(new string[] { priorityRecords.Remove(priorityRecords.IndexOf(" ")).Trim() }).ToArray();
                                                currentElement.I30D = (currentElement.I30D ?? Enumerable.Empty<string>()).Concat(new string[] { Methods.DateNormalize(priorityRecords.Remove(priorityRecords.LastIndexOf(" ")).Substring(priorityRecords.IndexOf(" ")).Trim()) }).ToArray();
                                                currentElement.I30C = (currentElement.I30C ?? Enumerable.Empty<string>()).Concat(new string[] { priorityRecords.Substring(priorityRecords.LastIndexOf(" ")).Trim() }).ToArray();
                                            }
                                            catch (Exception E)
                                            {
                                                Console.WriteLine("Priority identification error in:\t" + currentElement.I11);
                                            }
                                        }
                                    }
                                }

                                //if (tmpRecValue.Contains("\n") &&  tmpRecValue != "" && !tmpRecValue.Contains(" and "))
                                //{
                                //    splPriorityRecord = tmpRecValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                //}
                                //if (splPriorityRecord != null && splPriorityRecord.Count() > 1)
                                //{
                                //    foreach (var prioRecord in splPriorityRecord)
                                //    {
                                //        if (prioRecord.Contains(" "))
                                //        {
                                //            try
                                //            {
                                //                currentElement.I30N = (currentElement.I30N ?? Enumerable.Empty<string>()).Concat(new string[] { prioRecord.Remove(prioRecord.IndexOf(" ")).Trim() }).ToArray();
                                //                currentElement.I30D = (currentElement.I30D ?? Enumerable.Empty<string>()).Concat(new string[] { DateNormalize(tmpRecValue.Remove(prioRecord.LastIndexOf(" ")).Substring(prioRecord.IndexOf(" ")).Trim()) }).ToArray();
                                //                currentElement.I30C = (currentElement.I30C ?? Enumerable.Empty<string>()).Concat(new string[] { prioRecord.Substring(prioRecord.LastIndexOf(" ")).Trim() }).ToArray();
                                //            }
                                //            catch (Exception)
                                //            {
                                //                Console.WriteLine("Priority identification error in:\t" + currentElement.I11);
                                //            }
                                //        }
                                //    }
                                //}
                                //else if (tmpRecValue.Contains(" ") && !tmpRecValue.Contains(" and "))
                                //{
                                //    try
                                //    {
                                //        currentElement.I30N = (currentElement.I30N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Remove(tmpRecValue.IndexOf(" ")).Trim() }).ToArray();
                                //        currentElement.I30D = (currentElement.I30D ?? Enumerable.Empty<string>()).Concat(new string[] { DateNormalize(tmpRecValue.Remove(tmpRecValue.LastIndexOf(" ")).Substring(tmpRecValue.IndexOf(" ")).Trim()) }).ToArray();
                                //        currentElement.I30C = (currentElement.I30C ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Substring(tmpRecValue.LastIndexOf(" ")).Trim() }).ToArray();
                                //    }
                                //    catch (Exception)
                                //    {
                                //        Console.WriteLine("Priority identification error in:\t" + currentElement.I11);
                                //    }
                                //}
                            }
                            /*96*/
                            if (inidCode.StartsWith(I96))
                            {
                                string tmpDateValue = null;
                                string tmpAgentValue = inidCode.Replace(I96, "").Replace("\n", "").Trim();
                                if (tmpAgentValue.Contains("/"))
                                {
                                    tmpDateValue = Regex.Match(tmpAgentValue, @"\d{2}\/\d{2}\/\d{4}").Value;
                                    if (tmpDateValue != null)
                                    {
                                        currentElement.I96D = Methods.DateNormalize(tmpDateValue.Trim());
                                        currentElement.I96N = tmpAgentValue.Replace(tmpDateValue, "").Replace("/", "").Trim();
                                    }
                                }
                                else
                                {
                                    currentElement.I96N = tmpAgentValue.Trim();
                                    currentElement.I96D = "";
                                }
                            }
                            /*97*/
                            if (inidCode.StartsWith(I97))
                            {
                                string tmpDateValue = null;
                                string tmpAgentValue = inidCode.Replace(I97, "").Replace("\n", "").Trim();
                                if (tmpAgentValue.Contains("/"))
                                {
                                    tmpDateValue = Regex.Match(tmpAgentValue, @"\d{2}\/\d{2}\/\d{4}").Value;
                                    if (tmpDateValue != null)
                                    {
                                        currentElement.I97D = Methods.DateNormalize(tmpDateValue.Trim());
                                        currentElement.I97N = tmpAgentValue.Replace(tmpDateValue, "").Replace("/", "").Trim();
                                    }
                                }
                                else
                                {
                                    currentElement.I97N = tmpAgentValue.Trim();
                                    currentElement.I97D = "";
                                }
                            }
                            /*45 date*/
                            if (inidCode.StartsWith(I45))
                            {
                                tmpRecValue = inidCode.Replace(I45, "").Replace("\n", " ").Trim();
                                if (tmpRecValue.Length > 12)
                                {
                                    string tmpValue = Regex.Match(tmpRecValue, @"\d{2}\/\d{2}\/\d{4}").Value;
                                    currentElement.I45 = Methods.DateNormalize(tmpValue);
                                    currentElement.I74A = tmpRecValue.Replace(tmpValue, "").Trim();
                                }
                                else
                                {
                                    currentElement.I45 = Methods.DateNormalize(tmpRecValue);
                                }
                            }
                            /*54 title*/
                            if (inidCode.StartsWith(I54))
                            {
                                tmpRecValue = inidCode.Replace(I54, "").Replace("-", " ").Replace("\n", " ").Trim();
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
                                List<string> nameInstance = new List<string>();
                                tmpRecValue = inidCode.Replace(I72, "").Replace("\n", " ");
                                if (tmpRecValue.Contains(";") && tmpRecValue.Contains(" and "))
                                {
                                    string[] tmpSplValue = tmpRecValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    if (tmpSplValue.Count() > 0)
                                    {
                                        foreach (var inventor in tmpSplValue)
                                        {
                                            if (inventor.Contains(" and "))
                                            {
                                                string[] tmpSlpValueA = inventor.Split(new string[] { " and " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                                foreach (var item in tmpSlpValueA)
                                                {
                                                    nameInstance.Add(item.Trim());
                                                }
                                            }
                                            else
                                            {
                                                nameInstance.Add(inventor.Trim());
                                            }
                                        }

                                    }
                                }
                                if (tmpRecValue.Contains(" and ") && !tmpRecValue.Contains(";"))
                                {
                                    string[] tmpSlpValueA = tmpRecValue.Split(new string[] { " and " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    foreach (var item in tmpSlpValueA)
                                    {
                                        nameInstance.Add(item.Trim());
                                    }
                                }
                                if (nameInstance != null)
                                {
                                    currentElement.I72 = nameInstance.ToArray();
                                }
                            }
                            /*73 Agent*/
                            if (inidCode.StartsWith(I73))
                            {
                                tmpRecValue = inidCode.Replace(I73, "").Replace("\n", " ").Trim();
                                currentElement.I73 = tmpRecValue;
                            }
                            /*74 Agent*/
                            if (inidCode.StartsWith(I74))
                            {
                                tmpRecValue = inidCode.Replace(I74, "").Trim();
                                if (tmpRecValue.Contains("\n"))
                                {
                                    currentElement.I74N = tmpRecValue.Remove(tmpRecValue.IndexOf("\n"));
                                    currentElement.I74A = tmpRecValue.Substring(tmpRecValue.IndexOf("\n")).Replace("\n", " ").Trim();
                                }
                                else
                                {
                                    currentElement.I74N = tmpRecValue.Trim();
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
