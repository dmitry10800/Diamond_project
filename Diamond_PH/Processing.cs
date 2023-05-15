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
            var elementsOut = new List<Elements>();

            foreach (var record in records)
            {
                var element = new Elements();

               var formateRecord =  record.Replace("\r", " ").Replace("\n", "");

                var regex = new Regex(@"(?<id>.{0,10}\d)\s\s?(?<number>\d{0,2}\/\d{4}\/\d{4,7})\.?\s\s?(?<title>.+)\s\s?(?<date1>\d{2}\/\d{2}\/\d{4})\s\s?(?<date2>\d{2}\/\d{2}\/\d{4})\s\s?(?<comp>.*)");

                var match = regex.Match(formateRecord);

                if (match.Success)
                {
                    element.AppNumber = match.Groups["number"].Value.Trim();

                    element.Title = match.Groups["title"].Value.Trim();

                    var ruCulture = new CultureInfo("ru-RU");

                    var date1 = match.Groups["date1"].Value.Trim();

                    element.AppDate = DateTime.Parse(date1, ruCulture.DateTimeFormat).ToString("yyyy/MM/dd").Replace(".", "/");

                    var date2 = match.Groups["date2"].Value.Trim();

                    element.EventDate = DateTime.Parse(date2, ruCulture.DateTimeFormat).ToString("yyyy/MM/dd").Replace(".","/");

                    var ownerInfo = match.Groups["comp"].Value.Trim();

                    var owners = new List<Owner>();

                    var pattern = new Regex(@"(?<=\].?)");

                    var match1 = pattern.Split(ownerInfo).Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (var item in match1)
                    {
                        var owner = new Owner();

                        var regex1 = new Regex(@"(?<company>.+)\s\[(?<kind>[A-Z]{2})");

                        var match2 = regex1.Match(item);

                        if (match2.Success)
                        {

                            var name = match2.Groups["company"].Value.Trim();

                            if (name.Contains("and"))
                            {
                               var formateName =  name.Replace("and", "").Trim();

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
