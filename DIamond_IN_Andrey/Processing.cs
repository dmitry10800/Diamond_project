using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DIamond_IN_Andrey
{
    class Processing
    {
        public static string Field12 = "(12) PATENT APPLICATION PUBLICATION";
        public static string Field19 = "(19) INDIA";
        public static string PubNumber = "(21) Application No.";
        public static string Field22 = "(22) Date of filing of Application";
        public static string Field43 = "(43) Publication Date";
        public static string Field51 = "(51) International classification";
        public static string Field54 = "(54) Title of the invention";
        public static string PRIN = "(31) Priority Document No";
        public static string PRID = "(32) Priority Date";
        public static string PRIC = "(33) Name of priority country";
        public static string PCT = "(86) International Application No";
        public static string WO = "(87) International Publication No";
        public static string Related61 = "(61) Patent of Addition to Application Number";
        public static string Related62 = "(62) Divisional to Application Number";
        public static string Field71 = "(71)Name of Applicant";
        public static string Field72 = "(72)Name of Inventor";
        public static string Field57 = "(57) Abstract";

        public static List<Elements> BiblioProcess(List<XElement> elements)
        {
            var elementsOut = new List<Elements>();

            if (elements == null)
                return null;

            for (int i = 0; i < elements.Count; i++)
            {
                int tmpInc = i;
                string tmpVal = null;

                var value = elements[i].Value;
                if (value.StartsWith(Field12))
                {
                    var curElem = new Elements();
                    do
                    {
                        tmpVal += elements[tmpInc].Value + "\n";
                        ++tmpInc;
                    } while (tmpInc < elements.Count && !elements[tmpInc].Value.StartsWith(Field12));

                    var splittedRecords = Methods.RecSplit(tmpVal, new string[] { Field12, Field19, PubNumber, Field22, Field43, Field51, Field54, PRIN, PRID, PRIC, PCT, WO, Related61, Field71, Field57 });

                    foreach (var record in splittedRecords)
                    {
                        if (record.StartsWith(PubNumber))
                        {
                            curElem.Kind = "A";
                            curElem.PubNumber = record
                                .Replace(PubNumber, "")
                                .Replace("A", "")
                                .Trim();
                        }

                        if (record.StartsWith(PRIN))
                            curElem.PRIN = record.Replace(PRIN, "").Replace("NA", "").Replace(":", "").Trim();

                        if (record.StartsWith(PRID))
                            curElem.PRID = record.Replace(PRID, "").Replace("NA", "").Replace(":", "").Trim();

                        if (record.StartsWith(PRIC))
                        {
                            var country = record.Replace(PRIC, "").Replace(":", "").Trim();
                            if (!country.Contains("NA") && !country.Contains("PCT"))
                            {
                                curElem.PRIC = Methods.ToCountry(country);
                            }
                        }

                        if (record.StartsWith(PCT))
                        {
                            if (record.Contains("PCT"))
                            {
                                var matchDate = Regex.Match(record, @"\d{2}\/\d{2}\/\d{4}");
                                var matchNumber = Regex.Match(record, @"PCT\/\p{L}{1,}\d{4}\/\d{4,}");

                                if (matchDate.Success && matchNumber.Success)
                                {
                                    curElem.PCT = new PCT
                                    {
                                        DateOfFiling = matchDate.Value,
                                        AppNumber = matchNumber.Value
                                    };
                                }
                            }
                        }

                        if (record.StartsWith(WO))
                        {
                            var text = record.Replace(WO, "").Replace(": NA", "").Replace("WO/", "").Trim();
                            if (!string.IsNullOrEmpty(text))
                                curElem.WO = text;
                        }

                        if (record.StartsWith(Related61))
                        {
                            var strings = record.Replace("\n", " ").Split("(62)".ToCharArray());
                            var appNumber = Regex.Match(strings[0], @"\d{4}\/\p{L}{3,}\/\d{4}").Value;
                            var appDate = Regex.Match(strings[0], @"\d{2}\/\d{2}\/\d{4}").Value;
                            var pubNumber = Regex.Match(strings[1], @"\d{4}\/\d{3,}\/\d{4}").Value;
                            var pubDate = Regex.Match(strings[1], @"\d{4}\/\d{2}\/\d{4}").Value;
                            if (!string.IsNullOrEmpty(appNumber) || !string.IsNullOrEmpty(appDate))
                            {
                                curElem.Related = new Related
                                {
                                    AppDate = appNumber,
                                    AppNumber = appDate,
                                    Inid = "61"
                                };
                            }
                            else if (!string.IsNullOrEmpty(pubNumber) || !string.IsNullOrEmpty(pubDate))
                            {
                                curElem.Related = new Related
                                {
                                    PubDate = pubDate,
                                    PubNumber = pubNumber,
                                    Inid = "61"
                                };
                            }
                        }
                    }

                    if (curElem.Related != null || !string.IsNullOrEmpty(curElem.PRIC) || !string.IsNullOrEmpty(curElem.PRID) || !string.IsNullOrEmpty(curElem.PRIN) || curElem.PCT != null || !string.IsNullOrEmpty(curElem.WO))
                        elementsOut.Add(curElem);
                }
            }

            return elementsOut;
        }

        public static List<Elements> BiblioProcess2(List<XElement> elements)
        {
            List<Elements> elementsOut = new List<Elements>();

            var text = "";

            foreach (var record in elements)
            {
                text += record.Value + "\n";
            }

            text = text.Replace("\n", " ");
            var matches = Regex.Matches(text, @"\d{1,3}\s*\d{4,}\s*\d{3,}\/\p{Lu}+\/\d{4}\s*\d{2}\/\d{2}\/\d{4}\s*(\d{2}:\d{2}:\d{2})?\s*\d{2}\/\d{2}\/\d{4}");
            var curElem = new Elements();
            foreach (Match match in matches)
            {
                var record = match.Value.Substring(1).Trim();
                var strings = record.Split(' ');
                curElem.PubNumber = strings[0];
                curElem.AppNumber = strings[1];
                curElem.PRID = strings.Last();

                elementsOut.Add(curElem);
            }

            return elementsOut;
        }
    }
}
