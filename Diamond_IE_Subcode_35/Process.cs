using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_IE_Subcode_35
{
    class Process
    {
        public static string CurrentFileName;
        public List<Record> Start(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);

            List<Record> records = new List<Record>();

            List<string> files = new List<string>();

            foreach (FileInfo file in dir.GetFiles("*.txt", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }


            foreach (string txtFile in files)
            {
                CurrentFileName = txtFile;

                string text = File.ReadAllText(txtFile);

                Regex pattern = new Regex(@"(?>(\d{2,}\r\n\D))");

                List<string> notes = pattern.Split(text).Where(val => !string.IsNullOrEmpty(val)).ToList();

                List<string> formateNotes = new List<string>();

                for (int i = 0; i < notes.Count; i++)
                {
                    formateNotes.Add(notes[i] + notes[++i]);
                }

                foreach (var element in formateNotes)
                {
                    Record record = new Record();

                    string elem = element.Replace('\r', '~').Replace("~","").Trim();

                    Regex regex = new Regex(@"(?<number>.+\d)\n(?<name>.+\n*)(?<date>\d{2}.\d{2}.\d{4})\n(?<number2>\d+)\n(?<number3>\d+.\d)\n?(?<note>\D+)?\s?",RegexOptions.Singleline);

                    Match match = regex.Match(elem);

                    if (match.Success)
                    {
                        record.pubNumber = match.Groups["number2"].Value.Trim();

                        record.appNumber = match.Groups["number3"].Value.Trim();

                        CultureInfo cultureInfo = new CultureInfo("ru-Ru");

                        record.appDate = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo.DateTimeFormat).ToString("yyyy/MM/dd").Replace(".", "/");

                        string note = match.Groups["note"].Value.Trim();


                        string name = match.Groups["name"].Value.Trim();

                        List<string> names = name.Split('\n').ToList();

                        foreach (var item in names)
                        {
                            record.owner.Add(new Person
                            {
                                name = item
                            });
                        }

                        records.Add(record);
                    }

                    else
                    {
                        Console.WriteLine($"{element}");
                    }
                }
            }
                return records;
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