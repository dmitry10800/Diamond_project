using Diamond_AU_Subcodes_4_5_6;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace AU
{
    class Processing
    {
        public static List<Elements.SubCode4> SubCode4(List<XElement> elements)
        {
            var sub4List = new List<Elements.SubCode4>();

            foreach (var elem in elements)
            {
                var isMatch = Regex.IsMatch(elem.Value, @"((\d{10}\s*\(\s*\d{1,2}[A-Z]{1,2}\s*\)\s*){2,}|\d{4}\s*\d{10}\s*\(\s*\d{1,2}[A-Z]{1,2}\s*\))");
                if (isMatch == true)
                {
                    var numbers = Regex.Matches(elem.Value, @"\d{10}\s*\(\s*1(2|3)(A|B|C|D|N){1,2}\s*\)");
                    foreach (Match number in numbers)
                    {
                        var currentElement = new Elements.SubCode4();
                        var eventDate = Regex.Match(AU_main.currentFileName, @"20\d{6}");
                        var year = eventDate.Value.Substring(0, 4);
                        var month = eventDate.Value.Substring(4, 2);
                        var day = eventDate.Value.Substring(eventDate.Value.Length - 2);

                        currentElement.AppNumber = Regex.Match(number.Value, @"\d{10}").Value;
                        currentElement.EventDate = $"{year}/{month}/{day}";
                        sub4List.Add(currentElement);
                    }
                }
            }

            return sub4List;
        }

        public static List<Elements.SubCode5> SubCode5(List<XElement> elements)
        {
            var sub5List = new List<Elements.SubCode5>();

            foreach (var elem in elements)
            {
                var match = Regex.Match(elem.Value, @"(\d{4})?\s*(\d{10}\s*\(\s*\d{1,2}[A-Z]{0,2}\s*\)\s*)+");
                if (match.Value == elem.Value)
                {
                    var numbers = Regex.Matches(elem.Value, @"\d{10}\s*\(\s*(1(4|5)|2[0-3])(E|F|G|N){0,2}\s*\)");
                    foreach (Match number in numbers)
                    {
                        var currentElement = new Elements.SubCode5();
                        var eventDate = Regex.Match(AU_main.currentFileName, @"20\d{6}");
                        var year = eventDate.Value.Substring(0, 4);
                        var month = eventDate.Value.Substring(4, 2);
                        var day = eventDate.Value.Substring(eventDate.Value.Length - 2);

                        currentElement.AppNumber = Regex.Match(number.Value, @"\d{10}").Value;
                        currentElement.EventDate = $"{year}/{month}/{day}";
                        sub5List.Add(currentElement);
                    }
                }
            }

            return sub5List;
        }

        public static List<Elements.SubCode6> SubCode6(List<XElement> elements)
        {
            var sub6List = new List<Elements.SubCode6>();


            foreach (var elem in elements)
            {
                var match = Regex.Match(elem.Value, @"(\d{4}\s*(\d{10}\s*\(\s*\d{1,2}[A-Z]{0,2}\s*\)\s*)+|\s*(\d{10}\s*\(\s*\d{1,2}[A-Z]{0,2}\s*\)\s*){3,})");
                if (match.Value == elem.Value)
                {
                    var numbers = Regex.Matches(elem.Value, @"\d{10}\s*\(\s*([1-9]|1(1|[7-8])|2[4-8])(A|B|C|D|N|CN|AN)?\s*\)");
                    foreach (Match number in numbers)
                    {
                        var currentElement = new Elements.SubCode6();
                        var eventDate = Regex.Match(AU_main.currentFileName, @"20\d{6}");
                        var year = eventDate.Value.Substring(0, 4);
                        var month = eventDate.Value.Substring(4, 2);
                        var day = eventDate.Value.Substring(eventDate.Value.Length - 2);

                        currentElement.AppNumber = Regex.Match(number.Value, @"\d{10}").Value;
                        currentElement.EventDate = $"{year}/{month}/{day}";
                        sub6List.Add(currentElement);
                    }
                }
            }

            return sub6List;
        }

        public static string DateNormalize(string s)
        {
            var dateNormalized = s;
            var date = Regex.Match(s, @"(?<month>\d{2})[^0-9]*(?<day>\d{1,2})[^0-9]*(?<year>\d{4})");
            var day = date.Groups["day"].Value;
            if (date.Groups["day"].Length == 1)
                day = $"0{day}";
            dateNormalized = date.Groups["year"].Value + date.Groups["month"].Value + day;
            return dateNormalized;
        }
    }
}
