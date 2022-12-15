using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Diamond_PH
{
    public class Processing
    {
        public static List<Elements> SubCode7(List<string> records)
        {
            List<Elements> elementsOut = new List<Elements>();

            foreach (var record in records)
            {
                Elements element = new Elements();

               string formateRecord =  record.Replace("\r", " ").Replace("\n", "");

                Regex regex = new Regex(@"(?<id>.{0,10}\d)\s\s?(?<number>\d{0,2}\/\d{4}\/\d{4,7})\.?\s\s?(?<title>.+)\s\s?(?<date1>\d{2}\/\d{2}\/\d{4})\s\s?(?<date2>\d{2}\/\d{2}\/\d{4})\s\s?(?<comp>.*)");

                Match match = regex.Match(formateRecord);

                if (match.Success)
                {
                    element.AppNumber = match.Groups["number"].Value.Trim();

                    element.Title = match.Groups["title"].Value.Trim();

                    CultureInfo ruCulture = new CultureInfo("ru-RU");

                    string date1 = match.Groups["date1"].Value.Trim();

                    element.AppDate = DateTime.Parse(date1, ruCulture.DateTimeFormat).ToString("yyyy/MM/dd").Replace(".", "/");

                    string date2 = match.Groups["date2"].Value.Trim();

                    element.EventDate = DateTime.Parse(date2, ruCulture.DateTimeFormat).ToString("yyyy/MM/dd").Replace(".","/");

                    string ownerInfo = match.Groups["comp"].Value.Trim();

                    List<Owner> owners = new List<Owner>();

                    Regex pattern = new Regex(@"(?<=\].?)");

                    var match1 = pattern.Split(ownerInfo).Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (var item in match1)
                    {
                        Owner owner = new Owner();

                        Regex regex1 = new Regex(@"(?<company>.+)\s\[(?<kind>[A-Z]{2})");

                        Match match2 = regex1.Match(item);

                        if (match2.Success)
                        {

                            string name = match2.Groups["company"].Value.Trim();

                            if (name.Contains("and"))
                            {
                               string formateName =  name.Replace("and", "").Trim();

                                owner.Name = formateName;   
                            }
                            else
                            {
                                owner.Name = name;
                            }

                            owner.Country = match2.Groups["kind"].Value.Trim();

                            owners.Add(owner);
                        }

                    }

                    element.Owner = owners;

                    elementsOut.Add(element);
                }

                else
                {
                    Console.WriteLine($"{ record}");
                }
            }

           

            return elementsOut;
        }
    }
}
