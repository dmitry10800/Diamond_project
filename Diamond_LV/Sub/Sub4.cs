using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Diamond_LV.Sub
{
    class Sub4
    {
        private string I51 = "(51)";
        private string I11 = "(11)";
        private string I21 = "(21)";
        private string I22 = "(22)";
        private string I43 = "(43)";
        private string I45 = "(45)";
        private string I31 = "(31)";
        private string I32 = "(32)";
        private string I33 = "(33)";
        private string I86 = "(86)";
        private string I87 = "(87)";      
        private string I73 = "(73)";
        private string I72 = "(72)";
        private string I74 = "(74)";
        private string I54 = "(54)";
        private string I57 = "(57)";
        private string I62 = "(62)";

        public List<Patent> Start (string path)
        {
            List<Patent> patents = new List<Patent>();

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

                    patent.newspaperName = nameNews;

                    List<string> i31List = new List<string>();

                    List<string> i32List = new List<string>();

                    List<string> i33List = new List<string>();

                    foreach (var element in splittedRecords)
                    {

                        if (element.StartsWith(I21))
                        {
                            patent.i21 = element.Replace(I21, "").Trim();
                        }
                        else
                        if (element.StartsWith(I22))
                        {
                            string date = element.Replace(I22, "").Trim();

                            CultureInfo ruCulture = new CultureInfo("ru-RU");

                            patent.i22 = DateTime.Parse(date, ruCulture.DateTimeFormat).ToString("yyyy/MM/dd");

                        }
                        else
                        if (element.StartsWith(I43))
                        {
                            string date = element.Replace(I43, "").Trim();

                            CultureInfo ruCulture = new CultureInfo("ru-RU");

                            patent.i43 = DateTime.Parse(date, ruCulture.DateTimeFormat).ToString("yyyy/MM/dd");
                        }
                        else
                        if (element.StartsWith(I45))
                        {
                            string date = element.Replace(I45, "").Trim();

                            Regex regex = new Regex(@"(?<date>\d{2}.\d{2}.\d{4})\s*(?<info>.+)");

                            Match match = regex.Match(date);

                            if (match.Success)
                            {
                                CultureInfo ruCulture = new CultureInfo("ru-RU");

                                string dateForNote = DateTime.Parse(match.Groups["date"].Value.Trim(), ruCulture.DateTimeFormat).ToString("yyyy/MM/dd");

                                patent.note = "|| (45) | " + dateForNote +" "+ match.Groups["info"].Value.Trim();
                            }
                            else
                            {
                                CultureInfo ruCulture = new CultureInfo("ru-RU");

                                patent.i45 = DateTime.Parse(date, ruCulture.DateTimeFormat).ToString("yyyy/MM/dd");
                            }
                        }
                        else
                        if (element.StartsWith(I31))
                        {
                            string I_31 = element.Replace(I31, "").Trim();

                            i31List.Add(I_31);
                        }
                        else
                        if (element.StartsWith(I32))
                        {
                            string date = element.Replace(I32, "").Trim();

                            CultureInfo ruCulture = new CultureInfo("ru-RU");

                            string I_32 = DateTime.Parse(date, ruCulture.DateTimeFormat).ToString("yyyy/MM/dd");

                            i32List.Add(I_32);
                        }
                        else
                        if (element.StartsWith(I33))
                        {
                            string I_33 = element.Replace(I33, "").Trim();


                            string [] notes = I_33.Split('\n');

                            for (int i = 0; i < notes.Length; i++)
                            {
                                if (i==0)
                                {
                                    i33List.Add(notes[i].Trim().ToString());
                                }

                                Regex pattern = new Regex(@"(?<number>.+)\s(?<date>\d{2}.\d{2}.\d{4})\s(?<kind>[A-Z]{2})");

                                Match match1 = pattern.Match(notes[i]);

                                if (match1.Success)
                                {
                                    i31List.Add(match1.Groups["number"].Value.Trim());

                                    string date = match1.Groups["date"].Value.Trim();

                                    CultureInfo cultureInfo = new CultureInfo("ru-Ru");

                                    i32List.Add(DateTime.Parse(date, cultureInfo.DateTimeFormat).ToString("yyyy/MM/dd"));

                                    i33List.Add(match1.Groups["kind"].Value.Trim());
                                }

                                else
                                {
                                    Regex regex = new Regex(@"(?<date>\d{2}.\d{2}.\d{4})\s(?<number>.+)\s(?<kind>[A-Z]{2})");

                                    Match match = regex.Match(notes[i]);

                                    if (match.Success)
                                    {
                                        i31List.Add(match1.Groups["number"].Value.Trim());

                                        string date = match1.Groups["date"].Value.Trim();

                                        CultureInfo cultureInfo = new CultureInfo("ru-Ru");

                                        i32List.Add(DateTime.Parse(date, cultureInfo.DateTimeFormat).ToString("yyyy/MM/dd"));

                                        i33List.Add(match1.Groups["kind"].Value.Trim());
                                    }
                                }

                            }

                            patent.i31 = i31List;

                            patent.i32 = i32List;

                            patent.i33 = i33List;
                            
                        }
                        else
                        if (element.StartsWith(I73))
                        {
                            string I_73 = element.Replace(I73, "").Trim();

                            Regex regex = new Regex(@"(?<=\b[A-Z]{2}\b$)", RegexOptions.Multiline);

                            var notes = regex.Split(I_73).Where(val => !string.IsNullOrEmpty(val)).Select(x => x.Trim()).ToList();



                            Regex pattern = new Regex(@"(?<name>.+?),(?<adress>.+),\s(?<kind>[A-Z]{2})");

                            Regex pattern1 = new Regex(@"(?<name>.+?);(?<adress>.+),\s(?<kind>[A-Z]{2})");

                            foreach (var item in notes)
                            {
                                string note = item.Replace("\n", " ").Trim();

                                if (note.Contains(';'))
                                {
                                    Match match = pattern1.Match(note);

                                    if (match.Success)
                                    {
                                        patent.i73.Add(new Person
                                        {
                                            name = match.Groups["name"].Value.Trim(),
                                            adress = match.Groups["adress"].Value.Trim(),
                                            country = match.Groups["kind"].Value.Trim(),
                                        });
                                    }
                                }
                                else
                                {
                                    Match match = pattern.Match(note);

                                    if (match.Success)
                                    {
                                        patent.i73.Add(new Person
                                        {
                                            name = match.Groups["name"].Value.Trim(),
                                            adress = match.Groups["adress"].Value.Trim(),
                                            country = match.Groups["kind"].Value.Trim(),
                                        });
                                    }
                                }
                               
                            }
                        }
                        else
                        if (element.StartsWith(I72))
                        {
                            string I_72 = element.Replace(I72, "").Trim();

                            Regex regex = new Regex(@"(?<=\),?$)", RegexOptions.Multiline);

                            var notes = regex.Split(I_72).Where(val => !string.IsNullOrEmpty(val)).Select(x => x.Trim()).ToList();

                            Regex pattern = new Regex(@"(?<name>.+?)\s\(?(?<kind>[A-Z]{2})");

                            foreach (var item in notes)
                            {
                                string note = item.Replace("\n", " ").Trim();

                                Match match = pattern.Match(note);

                                if (match.Success)
                                {
                                    patent.i72.Add(new Person
                                    {
                                        name = match.Groups["name"].Value.TrimEnd(',').Trim(),
                                        country = match.Groups["kind"].Value.Trim()
                                    });
                                }

                            }
                        }
                        else
                        if (element.StartsWith(I74))
                        {
                            string I_74 = element.Replace(I74, "").Trim();

                            Regex regex = new Regex(@"(?<=\b[A-Z]{2}\b$)", RegexOptions.Multiline);

                            var notes = regex.Split(I_74).Where(val => !string.IsNullOrEmpty(val)).Select(x => x.Trim()).ToList();

                            Regex pattern = new Regex(@"(?<name>.+?),(?<adress>.+),\s(?<kind>[A-Z]{2})");

                            foreach (var item in notes)
                            {
                                string note = item.Replace("\n", " ").Trim();

                                Match match = pattern.Match(note);

                                if (match.Success)
                                {
                                    patent.i74.Add(new Person
                                    {
                                        name = match.Groups["name"].Value.TrimEnd(',').Trim(),
                                        country = match.Groups["kind"].Value.Trim(),
                                        adress = match.Groups["adress"].Value.Trim()
                                    });
                                }

                            }
                        }
                        else
                        if (element.StartsWith(I54))
                        {
                            string I_54 = element.Replace(I54, "").Trim();

                            List<string> i54List = new List<string>();

                            string[] lines = I_54.Split('\n');



                            if(lines.Length%2 == 0)
                            {
                                string[] latvianText = lines.Take(lines.Length / 2).ToArray();

                                string[] englishText = lines.Skip(lines.Length / 2).ToArray();
                                
                                string tmp = null;

                                for (int i = 0; i < latvianText.Length; i++)
                                {
                                    tmp += latvianText[i] + " ";
                                }

                                string tmp1 = null;

                                for (int i = 0; i < englishText.Length; i++)
                                {
                                    tmp1 += englishText[i] + " ";
                                }

                                i54List.Add(tmp);
                                i54List.Add(tmp1);

                                patent.i54 = i54List;

                            }
                            else
                            {
                                List<string> latvianText = new List<string>();

                                List<string> englishText = new List<string>();
                                for (int i = 0; i < lines.Length; i++)
                                {
                                    string stringToCheck = lines[i];
                                    if (!Regex.IsMatch(stringToCheck, @"\P{IsBasicLatin}"))
                                    {
                                        englishText.Add(stringToCheck);
                                    }
                                    else
                                    {
                                        latvianText.Add(stringToCheck);
                                    }
                                }
                                string tmp = null;

                                for (int i = 0; i < latvianText.Count; i++)
                                {
                                    tmp += latvianText[i] + " ";
                                }

                                string tmp1 = null;

                                for (int i = 0; i < englishText.Count; i++)
                                {
                                    tmp1 += englishText[i] + " ";
                                }

                                i54List.Add(tmp);
                                i54List.Add(tmp1);

                                patent.i54 = i54List;

                            }
                        }
                        else
                        if (element.StartsWith(I57))
                        {
                            patent.i57 = element.Replace(I57, "").Replace("\n", " ").Trim();
                        }
                        else
                        if (element.StartsWith(I86))
                        {
                            string I_86 = element.Replace(I86, "").Trim();

                            Regex regex = new Regex(@"(?<number>.+)\s(?<date>\d{2}.\d{2}.\d{4})");

                            Match match = regex.Match(I_86);

                            if (match.Success)
                            {

                                patent.i86PCTappNumber = match.Groups["number"].Value.Trim();

                                CultureInfo cultureInfo = new CultureInfo("ru-RU");

                                string date = match.Groups["date"].Value.Trim();

                                patent.i86PCTappDate = DateTime.Parse(date, cultureInfo.DateTimeFormat).ToString("yyyy/MM/dd");
                            }
                        }
                        else
                        if (element.StartsWith(I87))
                        {
                            string I_87 = element.Replace(I87, "").Trim();

                            Regex regex = new Regex(@"(?<number>.+)\s(?<date>\d{2}.\d{2}.\d{4})");

                            Match match = regex.Match(I_87);

                            if (match.Success)
                            {

                                patent.i87PCTpubNumber = match.Groups["number"].Value.Trim();

                                CultureInfo cultureInfo = new CultureInfo("ru-RU");

                                string date = match.Groups["date"].Value.Trim();

                                patent.i87PCTpubDate = DateTime.Parse(date, cultureInfo.DateTimeFormat).ToString("yyyy/MM/dd");
                            }

                        }
                        else
                        if (element.StartsWith(I62))
                        {
                            string I_62 = element.Replace(I62, "").Trim();

                            List<string> i62 = I_62.Split('/').ToList<string>();

                            patent.i62 = i62;
                        }
                        else
                        if (element.StartsWith(I51))
                        {
                            string I_51 = element.Replace(I51, "").Trim();

                            Regex regex = new Regex(@"\(11\)\s.+");

                            string[] splitString = regex.Split(I_51);

                            string tmp = null;
                            foreach (var item in splitString)
                            {
                                tmp += item;
                            }

                            patent.i51 = methods.GetIpcs(tmp.Replace("\n", " ").Trim());

                            Match match = regex.Match(I_51);
                            if (match.Success)
                            {
                                string I_11 = match.Value.Replace(I11,"").Trim();

                                Regex regex1 = new Regex(@"(?<number>.+)\s(?<kind>.+)");

                                Match match1 = regex1.Match(I_11);

                                if (match1.Success)
                                {
                                    patent.i11 = match1.Groups["number"].Value.Trim();
                                    patent.i13 = match1.Groups["kind"].Value.Trim();
                                }

                                else {
                                    patent.i11 = I_11;
                                }

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
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return patents;
        }
    }
}
