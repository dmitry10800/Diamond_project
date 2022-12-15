using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dimond_EE.Subs
{
    class Sub5
    {
        private string I51 = "(51)";
        private string I11 = "(11)";
        private string I30 = "(30)";
        private string I96 = "(96)";
        private string I97 = "(97)";
        private string I54 = "(54)";
        private string I73 = "(73)";
        private string I72 = "(72)";
        private string I74 = "(74)";

        public List<Patent> Start(String path)
        {
            var patents = new List<Patent>();

            try
            {
                Methods methods = new Methods();

                string fullText = methods.BuildAllWorkText(path);

                List<string> records = methods.SplitByInid(fullText, I51);
             
                foreach (var record in records)
                {
                    Patent patent = new Patent();

                    List<string> splittedRecords = methods.RecSplit(record);

                    string nameNews = methods.NameNewspaper(path);

                    foreach (var element in splittedRecords)
                    {

                        patent.newspaperName = nameNews;

                        if (element.StartsWith(I51))
                        {
                            string I_51 = element.Replace(I51, "");
                            Regex regex = new Regex(@"\(10\)\s*.+?\d+");

                            string[] splitString = regex.Split(I_51);

                            string tmp = null;
                            foreach (var item in splitString)
                            {
                                tmp += item;
                            }

                            patent.i51 = methods.GetIpcs(tmp.Replace("\n"," ").Trim());

                            Match match = regex.Match(I_51);
                            if (match.Success)
                            {
                                patent.note.Add(match.Value.Trim());
                            }
                        }
                        else
                        if (element.StartsWith(I11))
                        {
                            string I_11 = element.Replace(I11, "").Replace("\n","").Trim();
                            Regex pattern = new Regex(@"(?<group1>.+?\s.+?\s)\s*(?<group2>[A-Z]{1}[0-9]{1,2})");
                            Match result = pattern.Match(I_11);
                            if (result.Success)
                            {
                                patent.i11 = result.Groups["group1"].Value.Trim();
                                patent.i13 = result.Groups["group2"].Value.Trim();
                            }
                        }
                        else
                        if (element.StartsWith(I30))
                        {
                            string I_30 = element.Replace(I30, "").Replace("\n", "").Trim();

                            Regex pattern = new Regex(@"(?=\d{2}\.\d{2}\.\d{4})");

                            var notes = pattern.Split(I_30).Where(x => !string.IsNullOrEmpty(x)).ToList();

                            Regex regex = new Regex(@"(?<Date>\d{2}\.\d{2}\.\d{4})\s*,\s*(?<Kind>[A-Z]{2})\s*,\s*(?<Number>.*)");

                            foreach (var item in notes)
                            {
                                Match match = regex.Match(item);

                                if (match.Success)
                                {

                                    var ruCulture = new System.Globalization.CultureInfo("ru-RU");

                                    string tmp = match.Groups["Date"].Value.Trim();

                                    patent.i30.Add(new Priorities
                                    {
                                        date = DateTime.Parse(tmp, ruCulture.DateTimeFormat).ToString("yyyy/MM/dd"),
                                        kind = match.Groups["Kind"].Value.Trim(),
                                        number = match.Groups["Number"].Value.Trim()
                                    });
                                }
                            }
                        }
                        else
                        if (element.StartsWith(I96))
                        {
                            string I_96 = element.Replace(I96, "").Replace("\n", "").Trim();
                            Regex regex = new Regex(@"(?<group1>\d{2}.\d{2}.\d{4}),*\s*(?<group2>.+)");
                            Match result = regex.Match(I_96);

                            if (result.Success)
                            {
                                var ruCulture = new System.Globalization.CultureInfo("ru-RU");

                                string date = result.Groups["group1"].Value.Trim();
                                patent.i96appDate = DateTime.Parse(date, ruCulture.DateTimeFormat).ToString("yyyy/MM/dd");
                                patent.i96appNumber = result.Groups["group2"].Value.Trim();
                            }
                        }
                        else
                        if (element.StartsWith(I97))
                        {
                            string I_97 = element.Replace(I97, "").Replace("\n", "").Trim();
                            Regex regex = new Regex(@"(?<group1>\d{2}.\d{2}.\d{4}),*\s*(?<group2>[A-Z]{2}),*\s*(?<group3>.+)");
                            Match match = regex.Match(I_97);
                            if (match.Success)
                            {
                                var ruCulture = new System.Globalization.CultureInfo("ru-RU");
                                string date = match.Groups["group1"].Value.Replace(".", "/").Trim();
                                patent.i97.Add(new Priorities
                                {
                                    date = DateTime.Parse(date, ruCulture.DateTimeFormat).ToString("yyyy/MM/dd"),
                                    kind = match.Groups["group2"].Value.Trim(),
                                    number = match.Groups["group3"].Value.Trim()
                                });
                            }
                        }
                        else
                        if (element.StartsWith(I54))
                        {
                            patent.i54 = element.Replace(I54, "").Replace("\n", " ").Trim();
                        }
                        else
                        if (element.StartsWith(I73))
                        {
                            string I_73 = element.Replace(I73, "").Trim();

                            Regex regex = new Regex(@"(?<=\b[A-Z]{2}\b$)");

                            var notes = regex.Split(I_73).Where(val => !string.IsNullOrEmpty(val)).Select(x => x.Trim()).ToList();

                            Regex pattern = new Regex(@"(?<name>.+?)\n(?<adress>.+\n?.+\n?.+),\s(?<kind>[A-Z]{2})");

                            foreach (var item in notes)
                            {
                                Match match = pattern.Match(item);

                                if (match.Success)
                                {
                                    patent.i73.Add(new Person
                                    {
                                        name = match.Groups["name"].Value.Trim(),
                                        adress = match.Groups["adress"].Value.Replace("\n", " ").Trim(),
                                        country = match.Groups["kind"].Value.Trim(),
                                    });
                                }                          
                            }
                        }
                        else
                        if (element.StartsWith(I72))
                        {
                            string I_72 = element.Replace(I72, "").Trim();

                            Regex regex = new Regex(@"(?<=\b[A-Z]{2}\b$)", RegexOptions.Multiline);

                            var notes = regex.Split(I_72).Where(val => !string.IsNullOrEmpty(val)).Select(x => x.Trim()).ToList();

                            Regex pattern = new Regex(@"(?<name>.+?)\n(?<adress>.+\n?.+\n?.+),\s(?<kind>[A-Z]{2})");

                            foreach (var item in notes)
                            {
                                Match match = pattern.Match(item);

                                if (match.Success)
                                {
                                    patent.i72.Add(new Person
                                    {
                                        name = match.Groups["name"].Value.Trim(),
                                        adress = match.Groups["adress"].Value.Replace("\n"," ").Trim(),
                                        country = match.Groups["kind"].Value.Trim(),
                                    });
                                }
                            }
                        }
                        else
                        if (element.StartsWith(I74))
                        {
                            
                            string I_74 = element.Replace(I74, "").Trim();
                            Regex regex = new Regex(@"(?<name>.+)\n(?<adress>.+\n.+),\s(?<kind>[A-Z]{2})\n(?<note>.+)");
                            

                            Match match = regex.Match(I_74);
                            if (match.Success)
                            {
                                patent.i74.Add(new Person
                                {
                                    name = match.Groups["name"].Value.Trim(),
                                    adress = match.Groups["adress"].Value.Replace("\n", " ").Trim(),
                                    country = match.Groups["kind"].Value.Trim()
                                });

                                patent.note.Add(match.Groups["note"].Value.Trim());
                            }
                           
                        }

                        else
                        {
                            Console.WriteLine($"Найден новый айнид, который не имеет обработки. {element}");
                        }
                    }

                    patents.Add(patent);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return patents;
        }
    }
}
