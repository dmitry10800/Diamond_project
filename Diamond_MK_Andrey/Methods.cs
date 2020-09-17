using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace Diamond_MK_Andrey
{
    public class Methods
    {
        public static List<FileInfo> GetTetmlFiles(string path)
        {
            return Directory.GetFiles(path, @"*.tetml", SearchOption.TopDirectoryOnly).Select(x => new FileInfo(x)).ToList();
        }

        public static List<string> RecSplit(string s, string[] parameters)
        {
            foreach (var param in parameters)
            {
                if (!string.IsNullOrEmpty(param))
                    s = s.Replace(param, $"***{param}");
            }

            return s.Split("***", StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public static string DateNormalize(string s)
        {
            string dateNormalized = s;
            var date = Regex.Match(s, @"(?<day>\d{2})[^0-9]*(?<month>\d{1,2})[^0-9]*(?<year>\d{4})");
            var day = date.Groups["day"].Value;
            if (date.Groups["day"].Length == 1)
                day = $"0{day}";
            dateNormalized = date.Groups["year"].Value + date.Groups["month"].Value + day;

            if (!string.IsNullOrEmpty(dateNormalized))
            {
                dateNormalized = $"{date.Groups["year"].Value}-{date.Groups["month"].Value}-{day}";
            }

            return dateNormalized;
        }

        public static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var @event in events)
            {
                string tmpValue = JsonConvert.SerializeObject(@event);
                string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event"; //Staging
                //string url = @"https://diamond.lighthouseip.online/external-api/import/legal-event"; //Production
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync(url, content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
