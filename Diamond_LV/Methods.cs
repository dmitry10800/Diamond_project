using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_LV
{
    class Methods
    {

        public List<string> LoadAllFiles(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);

            List<string> files = new List<string>();

            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            return files;
        }

        public string NameNewspaper(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);

            FileInfo[] files = dir.GetFiles("*.tetml", SearchOption.AllDirectories);

            string tmp = files[0].ToString().Replace(".tetml", ".pdf");

            return tmp;
        }



        public string BuildAllWorkText(string path)
        {
            List<string> files = LoadAllFiles(path);

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

            string allText = fullText.Trim();

            Regex regex = new Regex(@"(.+?)(\(51\).*)", RegexOptions.Singleline);

            string[] workText = regex.Split(allText);

            workText = workText.Where(val => !string.IsNullOrEmpty(val)).ToArray();

            Regex pattern = new Regex(@"\(51\)");

            string workLine = "";

            foreach (var item in workText)
            {
                Match match = pattern.Match(item);

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
            List<string> records = new List<string>();

            if (text.Contains(inid))
            {
                  Regex pattern = new Regex(@"(?=^" + inid.Replace("(", @"\(").Replace(")", @"\)") + ")", RegexOptions.Multiline);
                

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
            string tmpRecordString = record;

            string I51Value = null;
            string I57Value = null;
            string I51 = "(51)";
            string I21 = "(21)";
            string I57 = "(57)";
            List<string> splittedRecords = new List<string>();

            if (record.Contains(I51))
            {
                I51Value = tmpRecordString.Substring(tmpRecordString.IndexOf(I51), tmpRecordString.IndexOf(I21)).Trim();
                tmpRecordString = tmpRecordString.Remove(tmpRecordString.IndexOf(I51), tmpRecordString.IndexOf(I21)).Trim();
            }

            if (record.Contains(I57))
            {
                I57Value = tmpRecordString.Substring(tmpRecordString.IndexOf(I57)).Trim();
                tmpRecordString = tmpRecordString.Remove(tmpRecordString.IndexOf(I57)).Trim();
            }

            if (tmpRecordString != "")
            {
                Regex pattern = new Regex(@"\(\d{2}\)\s*", RegexOptions.IgnoreCase);
                MatchCollection matches = pattern.Matches(tmpRecordString);
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

                if (!string.IsNullOrEmpty(I57Value))
                {
                    splittedRecords.Add(I57Value);
                }
            }
            return splittedRecords;
        }

        public List<(string, string)> GetIpcs(string text)
        {
            var ipcs = new List<(string, string)>();
         
            if (text.Contains('('))
            {
                Regex pattern = new Regex(@"(?<Class>.*)\s*\((?<Edition>.*\d)");
                Regex regex = new Regex(@"\)\s");
                string[] values = regex.Split(text);
                foreach (var item in values)
                {
                    Match match = pattern.Match(item);
                    if (match.Success)
                    {
                        string iClass = match.Groups["Class"].Value.Trim();
                        string iEdition = match.Groups["Edition"].Value.Replace(".", "/").Trim()+"/01";
                        ipcs.Add((iClass, iEdition));
                    }
                }
                return ipcs;
            }
            else
            {
                Regex regex = new Regex(@"(?=\s[A-Z])");
                string[] values = regex.Split(text);
                foreach (var item in values)
                {
                    string iClass = item.Trim();
                    string iEdition = null;
                    ipcs.Add((iClass, iEdition));
                }
                return ipcs;
            }
        }

        public void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                string tmpValue = JsonConvert.SerializeObject(rec);
                string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                //string url = @"https://diamond.lighthouseip.online/external-api/import/legal-event";
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
