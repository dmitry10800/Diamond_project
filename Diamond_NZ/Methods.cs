﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_NZ
{
    class Methods
    {
        public static string XElemToString(List<XElement> v)
        {
            return string.Join(" ", v.Select(x => x.Value));
        }
        public static string GetDateFromGazette(string v)
        {
            var date = new Regex(@"[A-Z]_(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})");
            var _ = date.Match(v);
            if (_.Success)
            {
                return _.Groups["year"].Value + "-" + _.Groups["month"].Value + "-" + _.Groups["day"].Value;
            }
            else return null;
        }
        public static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
                //string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event"; // STAGING
                var url = @"https://diamond.lighthouseip.online/external-api/import/legal-event"; // PRODUCTION
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }

        public static List<string> SplitByNumberAndName(string v)
        {
            var splittedRecords = new List<string>();
            v = v.Replace("\n", " ");
            var yearPattern = new Regex(@"(\d+th\sYear)");
            var recordPattern = new Regex(@"(?<AppNumber>\(21\).*)(?<Owner>\(71\).*)");
            var tmpList = yearPattern.Split(v);
            foreach (var rec in tmpList)
            {
                if (yearPattern.Match(rec).Success)
                {
                    splittedRecords.Add(rec);
                }
                else if (rec.Trim().StartsWith("(21)"))
                {
                    var a = rec.Trim().Replace("(21)", "*****(21)").Replace("\n", " ").Trim();
                    var b = a.Split(new string[] { "*****" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    splittedRecords.AddRange(b);
                }
            }
            return splittedRecords;
        }
    }
}
