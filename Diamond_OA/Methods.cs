using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Diamond_OA
{
    class Methods
    {
        /*Date*/
        public static string DateNormalize(string tmpDate)
        {
            string swapDate = tmpDate;
            string[] splitDate = null;
            try
            {
                if (tmpDate.Contains("/"))
                {
                    splitDate = tmpDate.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitDate.Count() == 3)
                    {
                        return swapDate = splitDate[2] + "-" + splitDate[1] + "-" + splitDate[0];
                    }
                }
                else if (tmpDate.Contains("."))
                {
                    splitDate = tmpDate.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitDate.Count() == 3)
                    {
                        return swapDate = splitDate[2] + "-" + splitDate[1] + "-" + splitDate[0];
                    }
                }
                return tmpDate;
            }
            catch (Exception)
            {
                return swapDate;
            }
        }

        public static string[] RecSplit(string recString)
        {
            string[] splittedRecord = null;
            string tempStrC = recString.Trim();
            string tmpDescValue = null;
            string I57 = "(57)";
            if (recString != "")
            {
                if (recString.Contains(I57))
                {
                    tmpDescValue = tempStrC.Substring(tempStrC.IndexOf(I57)).Trim();
                    tempStrC = tempStrC.Remove(recString.IndexOf(I57)).Trim();
                }
                Regex regexPatOne = new Regex(@"\(\d{2}\)", RegexOptions.IgnoreCase);
                MatchCollection matchesClass = regexPatOne.Matches(recString);
                if (matchesClass.Count > 0)
                {
                    foreach (Match matchC in matchesClass)
                    {
                        tempStrC = tempStrC.Replace(matchC.Value, "***" + matchC.Value);
                    }
                }
                /*Splitting record*/
                splittedRecord = tempStrC.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries);
            }
            if (tmpDescValue != null)
            {
                splittedRecord = (splittedRecord ?? Enumerable.Empty<string>()).Concat(new string[] { tmpDescValue }).ToArray();
            }
            return splittedRecord;
        }

        public class IntClassStruct
        {
            public string[] Date { get; set; }
            public string[] Class { get; set; }
        }
        /*51 internationa calssification splitting*/
        public static IntClassStruct IntClassSplit(string tmpIntClass)
        {
            string I51 = "(51)";
            IntClassStruct classInfo = new IntClassStruct();
            //string patternClass = @"[A-Z]{1}\d{2}[A-Z]{1}(\d{1}|\d{2}|\d{3})\/(\d{2}|\d{3}|\d{4})";
            string patternClass = @"[A-Z]{1}\d{2}[A-Z]{1}(\d+)\/(\d+)";
            string patternDate = @"\(\d{4}\.\d{2}\)";
            if (tmpIntClass != null && tmpIntClass != "")
            {
                string[] splIntClass = null;
                string tmpStr = tmpIntClass.Replace(I51, "").Replace(" ", "").Trim();
                if (tmpStr.Contains("\n"))
                {
                    splIntClass = tmpStr.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                }
                else
                {
                    splIntClass = (splIntClass ?? Enumerable.Empty<string>()).Concat(new string[] { tmpStr.Trim() }).ToArray();
                }
                if (splIntClass != null)
                {
                    foreach (var rec in splIntClass)
                    {
                        string dateValue = null;
                        string classValue = null;
                        if (Regex.IsMatch(rec, patternClass)) { classValue = Regex.Match(rec, patternClass).Value; }
                        if (Regex.IsMatch(rec, patternDate)) { dateValue = Regex.Match(rec, patternDate).Value.Replace("(", "").Replace(")", ""); }
                        if (dateValue != null && classValue != null)
                        {
                            classInfo.Class = (classInfo.Class ?? Enumerable.Empty<string>()).Concat(new string[] { classValue.Insert(4, " ") }).ToArray();
                            classInfo.Date = (classInfo.Date ?? Enumerable.Empty<string>()).Concat(new string[] { dateValue }).ToArray();
                        }
                    }
                }
            }
            if (classInfo != null) return classInfo;
            else return null;
        }

        /*30 priority splitting*/
        public class PriorityStruct
        {
            public string[] Country { get; set; }
            public string[] Number { get; set; }
            public string[] Date { get; set; }
        }
        public static PriorityStruct PrioritySplitting(string tmpString)
        {
            string I30 = "(30)";
            string patternDate = @"(\d{1}|\d{2})\/\d{2}\/\d{4}";
            string patternCountry = @"^[A-Z]{2}$";
            string[] splPrio = null;
            PriorityStruct priorityList = new PriorityStruct();
            string tmpPrio = tmpString.Replace(I30, "").Replace("n°", "***").Replace("du", "***").Trim();
            if (tmpPrio != null && tmpPrio != "")
            {
                if (tmpPrio.Contains("\n"))
                {
                    splPrio = tmpPrio.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                }
                else
                {
                    splPrio = (splPrio ?? Enumerable.Empty<string>()).Concat(new string[] { tmpPrio.Trim() }).ToArray();
                }
            }
            if (splPrio != null)
            {
                string countryValue = null;
                string dateValue = null;
                string numberValue = null;
                foreach (var rec in splPrio)
                {
                    string[] splittedValue = null;
                    if (rec.Contains("***")) { splittedValue = rec.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray(); }
                    if (splittedValue != null)
                    {
                        foreach (var item in splittedValue)
                        {
                            if (Regex.IsMatch(item, patternDate)) { dateValue = Regex.Match(item, patternDate).Value; }
                            if (Regex.IsMatch(item, patternCountry)) { countryValue = Regex.Match(item, patternCountry).Value; }
                            if (!Regex.IsMatch(item, patternDate) && !Regex.IsMatch(item, patternCountry)) { numberValue = item; }
                        }
                    }
                    if (countryValue != null && dateValue != null && numberValue != null)
                    {
                        priorityList.Date = (priorityList.Date ?? Enumerable.Empty<string>()).Concat(new string[] { DateNormalize(dateValue) }).ToArray();
                        priorityList.Country = (priorityList.Country ?? Enumerable.Empty<string>()).Concat(new string[] { countryValue }).ToArray();
                        priorityList.Number = (priorityList.Number ?? Enumerable.Empty<string>()).Concat(new string[] { numberValue }).ToArray();
                    }
                }

            }
            if (priorityList != null) return priorityList;
            else return null;
        }

        /*72 Inventor splitting*/
        public class InvStruct
        {
            public string[] Name { get; set; }
            public string[] Country { get; set; }
        }
        public static InvStruct InventorSplit(string tmpInv)
        {
            InvStruct inventorList = new InvStruct();
            string I72 = "(72)";
            string tmpRecValue = tmpInv.Replace(I72, "");
            if (Regex.IsMatch(tmpRecValue, @".*\([A-Z]{2}\)"))
            {
                Regex pat = new Regex(@".*?\([A-Z]{2}\)", RegexOptions.Singleline);
                MatchCollection matchCollection = pat.Matches(tmpRecValue);
                foreach (Match item in matchCollection)
                {
                    string tmpSplValue = item.Value.Replace("\n", " ");
                    tmpSplValue = Regex.Replace(tmpSplValue, @"^\s*et(\s|\n)", "");
                    string tmpSplName = null;
                    string tmpSplCountry = null;
                    if (tmpSplValue.Contains(")"))
                    {
                        tmpSplValue = tmpSplValue.Remove(tmpSplValue.LastIndexOf(")")).Trim();
                    }
                    if (tmpSplValue.Contains("("))
                    {
                        tmpSplName = tmpSplValue.Remove(tmpSplValue.LastIndexOf("(")).Trim();
                        tmpSplCountry = tmpSplValue.Substring(tmpSplValue.LastIndexOf("(")).Replace("(", "").Replace(")", "").Trim();
                    }
                    if (tmpSplName != null && tmpSplCountry != null)
                    {
                        inventorList.Name = (inventorList.Name ?? Enumerable.Empty<string>()).Concat(new string[] { tmpSplName.Replace("(", "").Trim(';').Trim() }).ToArray();
                        inventorList.Country = (inventorList.Country ?? Enumerable.Empty<string>()).Concat(new string[] { tmpSplCountry.Replace(".", "") }).ToArray();
                    }
                }
            }
            if (inventorList != null) return inventorList;
            else return null;
        }

        /*73 Assignee Information splitting*/
        public class AssigneeStruct
        {
            public string[] Name { get; set; }
            public string[] Address { get; set; }
            public string[] Country { get; set; }
        }
        public static AssigneeStruct AssigneeSplitting(string tmpString)
        {
            AssigneeStruct assigneeList = new AssigneeStruct();
            string I73 = "(73)";
            string tmpRecValue = tmpString.Replace(I73, "").Replace(";", "").Trim();
            string status = @"(\,.*\sv\.v\.i\.\,)|(\,.*\sllc\,)|(\,.*\sgmbh\,)|(\,.*\sinc(\.|\,)*\,)|(\,.*\sltd(\.|\,)*\,)|(\,.*\ss\.a\.\,)|(\,.*\ss\.l\.\,)|(\,.*\ss\.a\.u\.\,)";
            string[] splValue = null;
            string coutryCode = null;
            string name = null;
            string address = null;
            if (Regex.IsMatch(tmpRecValue, @"\([A-Z]{2}\)"))
            {
                splValue = Regex.Split(tmpRecValue, @"(?<=\([A-Z]{2}\))", RegexOptions.None).Where(d => d.Length > 2).ToArray();
                foreach (var record in splValue)
                {
                    if (record.Contains(","))
                    {
                        string tmpValue = Regex.Replace(record, @"^\s*et(\s|\n)", "");
                        string tmpSplName = null;
                        string tmpSplAddr = null;
                        if (Regex.IsMatch(tmpValue.ToLower(), status))
                        {
                            if (tmpValue.Count(f => f == ',') >= 2)
                            {
                                int firstIndex = tmpValue.IndexOf(",");
                                int secondIndex = tmpValue.IndexOf(",", firstIndex + 1);
                                tmpSplName = tmpValue.Remove(secondIndex).Trim();
                                tmpSplAddr = tmpValue.Substring(secondIndex).Trim(',').Trim();
                            }
                        }
                        else
                        {
                            tmpSplName = tmpValue.Remove(tmpValue.IndexOf(",")).Trim();
                            tmpSplAddr = tmpValue.Substring(tmpValue.IndexOf(",")).Trim(',').Trim();
                        }
                        name = tmpSplName;
                        if (Regex.IsMatch(tmpSplAddr, @"\([A-Z]{2}\)$"))
                        {
                            try
                            {
                                coutryCode = Regex.Match(tmpSplAddr, @"\([A-Z]{2}\)$").Value.Replace("(", "").Replace(")", "").Trim();
                                address = Regex.Replace(tmpSplAddr, @"\([A-Z]{2}\)$", "").Trim(',').Trim();
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("73 field address or CC error");
                            }
                        }
                        else if (!Regex.IsMatch(record, @"\([A-Z]{2}\)$") && coutryCode != null)
                        {
                            address = Regex.Replace(record.Substring(record.IndexOf(",")), @"\([A-Z]{2}\)$", "").Trim(',').Trim();
                        }
                        if (address != null && coutryCode != null && name != null)
                        {
                            /*Chineese names separated with comma, so part of name going to address*/
                            if (coutryCode.Replace("\n", " ") == "CN")
                            {
                                string partOfName = null;
                                if (address.Contains(","))
                                {
                                    partOfName = address.Remove(address.IndexOf(",")).Trim();
                                    address = address.Substring(address.IndexOf(",")).Trim().Trim(',').Trim();
                                    name = name + " " + partOfName;
                                }
                            }
                            assigneeList.Name = (assigneeList.Name ?? Enumerable.Empty<string>()).Concat(new string[] { name.Replace("-\n", "").Replace("\n", " ").Trim().Trim(',').Trim() }).ToArray();
                            assigneeList.Address = (assigneeList.Address ?? Enumerable.Empty<string>()).Concat(new string[] { address.Replace("\n", " ").Trim().Trim(',').Trim() }).ToArray();
                            assigneeList.Country = (assigneeList.Country ?? Enumerable.Empty<string>()).Concat(new string[] { coutryCode.Replace("\n", " ").Trim() }).ToArray();
                        }
                    }
                    else
                    {
                        coutryCode = null;
                        name = null;
                        address = null;
                    }
                }
            }
            else
            {
                assigneeList.Name = (assigneeList.Name ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue }).ToArray();
            }
            if (assigneeList != null) return assigneeList;
            else return null;
        }

        /*74 Agent*/
        public class AgentStruct
        {
            public string Name { get; set; }
            public string Address { get; set; }
            public string Country { get; set; }
        }
        public static AgentStruct AgentSplitting(string tmpStr)
        {
            AgentStruct agentList = new AgentStruct();
            string I74 = "(74)";
            string coutryCode = null;
            string name = null;
            string address = null;
            string tmpRecValue = tmpStr.Replace(I74, "").Replace("\n", " ").Trim();
            if (tmpRecValue.Contains(","))
            {
                string tmpValue = Regex.Replace(tmpRecValue, @"\.$", "").Trim();
                try
                {
                    coutryCode = Regex.Match(tmpValue, @"\([A-Z]{2}\)$").Value.Replace("(", "").Replace(")", "");
                    address = tmpValue.Remove(tmpValue.Length - 4).Substring(tmpValue.IndexOf(",")).Trim().TrimStart(',').Trim();
                    name = tmpValue.Remove(tmpValue.IndexOf(",")).Trim();
                }
                catch (Exception) { coutryCode = null; name = null; address = null; }
            }
            if (coutryCode != null && name != null && address != null)
            {
                agentList.Country = coutryCode;
                agentList.Address = address;
                agentList.Name = name;
            }
            if (agentList != null) return agentList;
            else return null;
        }

        /*72 Inventor for Utility Models*/
        public static string[] InventorSplitUM(string tmpStr)
        {
            string[] inventors = null;
            string I72 = "(72)";
            string[] invSplitted = null;
            string tmpInv = tmpStr.Replace("\n", "").Replace(I72, "").Trim();
            if (tmpInv != null && tmpInv != "")
            {
                if (tmpInv.Contains(";"))
                {
                    invSplitted = tmpInv.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim().Trim('.').Trim(',')).ToArray();
                }
                else
                {
                    invSplitted = (invSplitted ?? Enumerable.Empty<string>()).Concat(new string[] { tmpInv }).ToArray();
                }
            }
            if (invSplitted != null)
            {
                foreach (var rec in invSplitted)
                {
                    inventors = (inventors ?? Enumerable.Empty<string>()).Concat(new string[] { rec }).ToArray();
                }
            }
            if (inventors != null) return inventors;
            else return null;
        }

        /*Sending data into Diamond*/
        public static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                string tmpValue = JsonConvert.SerializeObject(rec);
                string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
