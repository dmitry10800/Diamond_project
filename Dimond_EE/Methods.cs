using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;


namespace Dimond_EE
{
    class Methods
    {
        public List<string> LoadAllFiles(string path)
        {
            var dir = new DirectoryInfo(path);

            var files = new List<string>();

            foreach (var file in dir.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            return files;
        }

        public string NameNewspaper(string path)
        {
            var dir = new DirectoryInfo(path);

            var files = dir.GetFiles("*.tetml", SearchOption.AllDirectories);

            var tmp = files[0].ToString().Replace(".tetml", ".pdf");

            return tmp;
        }

        public string BuildAllWorkText(string path)
        {

            var files = LoadAllFiles(path);

            XElement tet;

            List<XElement> xElements = null;

            string fullText = null;

            foreach (var tetFile in files)
            {
                tet = XElement.Load(tetFile);

                xElements = tet.Descendants().Where(d => d.Name.LocalName == "Text").ToList();

                foreach (var item in xElements)
                {
                    fullText += item.Value + "\n";
                }
            }

            var allText = fullText.Trim();

            var regex = new Regex(@"(.+?)(\(51\).*)", RegexOptions.Singleline);

            var workText = regex.Split(allText);

           workText = workText.Where(val => !string.IsNullOrEmpty(val)).ToArray();

            var pattern = new Regex(@"\(51\)");

            var workLine = "";
            
            foreach (var item in workText)
            {
                var match = pattern.Match(item);

                if (match.Success)
                {
                    workLine = item;
                    return workLine;
                }
            }

            return workLine;
        }

        public List<string> SplitByInid(string text, string inid)
        {
            var records = new List<string>();

            if (text.Contains(inid))
            {
                var pattern = new Regex(@"(?=" + inid.Replace("(", @"\(").Replace(")", @"\)") + ")");

                return records = pattern.Split(text).Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim()).ToList();
            }
            else
            {
                Console.WriteLine("В тексте нет данного айнида");
            }
            return records;
        }

        public List<string> RecSplit(string record)
        {
            var tmpRecordString = record;      
            string I51Value = null;
            var I51 = "(51)";
            var I11 = "(11)";
      
            var splittedRecords = new List<string>();

            if (record.Contains(I51))
            {
                I51Value = tmpRecordString.Substring(tmpRecordString.IndexOf(I51), tmpRecordString.IndexOf(I11)).Trim();
                tmpRecordString = tmpRecordString.Remove(tmpRecordString.IndexOf(I51), tmpRecordString.IndexOf(I11)).Trim();
            }

            if (tmpRecordString != "")
            {
                var pattern = new Regex(@"\(\d{2}\)\s*", RegexOptions.IgnoreCase);
                var matches = pattern.Matches(tmpRecordString);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        tmpRecordString = tmpRecordString.Replace(match.Value, "***" + match.Value);
                    }
                }

                splittedRecords = tmpRecordString.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (!string.IsNullOrEmpty(I51Value))
                {
                    splittedRecords.Add(I51Value);
                }            
            }
            return splittedRecords;
        }

        public List<(string, string)> GetIpcs(string text)
        {
            var ipcs = new List<(string, string)>();
            var pattern = new Regex(@"(?<Class>.*)\s*\((?<Edition>.*\d)");
            var regex = new Regex(@"\)\s");
            var values = regex.Split(text);
            foreach (var item in values)
            {
                var match = pattern.Match(item);
                if (match.Success)
                {
                    var iClass = match.Groups["Class"].Value.Trim();
                    var iEdition = match.Groups["Edition"].Value.Trim();
                    ipcs.Add((iClass, iEdition));
                }
            }
            return ipcs;
        }
        public void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
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
