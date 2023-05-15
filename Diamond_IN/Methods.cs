using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace Diamond_IN
{
    class Methods
    {
        public static string[] ClearString(string[] s)
        {
            if (s.Count() > 0)
            {
                return s.Select(x => x.Trim())
                    .Where(x => x != "SNO"
                    && x != "LOCATION"
                    && x != "APPLICATION NUMBER"
                    && x != "FER DATE"
                    && x != "ADDRESS FOR SERVICE"
                    && x != "EMAIL"
                    && !x.StartsWith("The Patent Office Journal")
                    && !x.StartsWith("WEEKLY ISSUED FER"))
                    .ToArray();
            }
            return null;
        }
        public static string DateProcess(string s)
        {
            if (Regex.IsMatch(s, @"\d{2}\/\d{2}\/\d{4}"))
            {
                var regex = Regex.Match(s, @"(?<day>\d{2})\/(?<month>\d{2})\/(?<year>\d{4})");
                return regex.Groups["year"].Value + "-" + regex.Groups["month"].Value + "-" + regex.Groups["day"].Value;
            }
            else return s;
        }
        public static ProcessFebTableData.ElementsForOutput.AgentStruct AgentInfoSplit(string s)
        {
            if (s != null)
            {
                var agent = new ProcessFebTableData.ElementsForOutput.AgentStruct();
                var tmpValue = s.Trim();
                if (tmpValue.Contains(","))
                {
                    agent.Name = tmpValue.Remove(tmpValue.IndexOf(",")).Trim();
                    agent.Address = tmpValue.Substring(tmpValue.IndexOf(",")).Trim().Trim(',').Trim();
                    agent.Country = "IN";
                }
                else
                {
                    agent.Name = tmpValue.Trim();
                    agent.Address = "";
                    agent.Country = "IN";
                }
                if (agent != null) return agent;
            }
            return null;
        }

        public static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
                var url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                //string url = @"https://diamond.lighthouseip.online/external-api/import/legal-event";
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
