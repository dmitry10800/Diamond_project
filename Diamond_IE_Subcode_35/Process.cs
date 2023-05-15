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

namespace Diamond_IE_Subcode_35
{
    class Process
    {
        public static string CurrentFileName;
        public List<Record> Start(string path)
        {
            var dir = new DirectoryInfo(path);

            var records = new List<Record>();

            var files = new List<string>();

            foreach (var file in dir.GetFiles("*.txt", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }


            foreach (var txtFile in files)
            {
                CurrentFileName = txtFile;

                var text = File.ReadAllText(txtFile);

                var pattern = new Regex(@"(?>(\d{2,}\r\n\D))");

                var notes = pattern.Split(text).Where(val => !string.IsNullOrEmpty(val)).ToList();

                var formateNotes = new List<string>();

                for (var i = 0; i < notes.Count; i++)
                {
                    formateNotes.Add(notes[i] + notes[++i]);
                }

                foreach (var element in formateNotes)
                {
                    var record = new Record();

                    var elem = element.Replace('\r', '~').Replace("~","").Trim();

                    var regex = new Regex(@"(?<number>.+\d)\n(?<name>.+\n*)(?<date>\d{2}.\d{2}.\d{4})\n(?<number2>\d+)\n(?<number3>\d+.\d)\n?(?<note>\D+)?\s?",RegexOptions.Singleline);

                    var match = regex.Match(elem);

                    if (match.Success)
                    {
                        record.pubNumber = match.Groups["number2"].Value.Trim();

                        record.appNumber = match.Groups["number3"].Value.Trim();

                        var cultureInfo = new CultureInfo("ru-Ru");

                        record.appDate = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo.DateTimeFormat).ToString("yyyy/MM/dd").Replace(".", "/");

                        var note = match.Groups["note"].Value.Trim();


                        var name = match.Groups["name"].Value.Trim();

                        var names = name.Split('\n').ToList();

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