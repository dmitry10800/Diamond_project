using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_TR
{
    class Processing
    {
        public static Regex pattern = new Regex(@"\d{4}\/\d{2,}");

        public static List<Elements> AllSubsProcess(List<XElement> elements)
        {
            List<Elements> elementsOut = new List<Elements>();

            int id = 1;
            var ints = new List<int>();
            for (int i = 0; i < elements.Count; i++)
            {
                int inc = i;
                string tmpVal = null;
                var value = elements[i].Value;
                if (pattern.Match(value).Success)
                {
                    if (i == 996)
                    {

                    }
                    var curElem = new Elements();
                    value = value.Replace("\n", " ").Trim();
                    var pubNumber = pattern.Match(value).Value;
                    if (value == pubNumber)
                    {
                        value = value + elements[inc + 1].Value;
                    }

                    if (value.Contains("2013/03004"))
                    {

                    }
                    var matchLowerCase = Regex.Match(value, @"\p{Ll}+");
                    var title = "";
                    var applicantName = "";
                    if (matchLowerCase.Success)
                    {
                        do
                        {
                            tmpVal += elements[inc].Value;
                            ++inc;
                        } while (inc < elements.Count && !pattern.Match(elements[inc].Value).Success);

                        applicantName = Regex.Match(tmpVal, @"(\p{Lu}{2,}){2,}.*", RegexOptions.Singleline).Value;
                        pubNumber = pattern.Match(tmpVal).Value;
                        if (string.IsNullOrEmpty(applicantName))
                        {
                            var strings = tmpVal.Split('.');
                            title = strings[0].Replace(pubNumber, "").Trim().Replace("\n", " ");
                            applicantName = tmpVal.Replace(strings[0], "").Trim().TrimStart('.').Trim();
                        }
                        else
                            title = tmpVal.Replace(applicantName, "").Replace(pubNumber, "").Trim();
                    }
                    else
                    {
                        if (inc + 1 < elements.Count && pattern.Match(elements[inc + 1].Value).Success)
                        {
                            continue;
                        }
                        else
                        {
                            tmpVal = value + "\n\n";
                            do
                            {
                                if (inc + 1 < elements.Count)
                                {
                                    inc++;
                                    tmpVal += elements[inc].Value;
                                }
                            } while (inc < elements.Count - 1 && !pattern.Match(elements[inc + 1].Value).Success);

                            var strings = tmpVal.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
                            pubNumber = pattern.Match(tmpVal).Value;
                            title = strings[0].Replace(pubNumber, "").Trim();
                            applicantName = tmpVal.Replace(strings[0], "").Replace("\n", "").Trim();
                        }
                    }

                    if (string.IsNullOrEmpty(pubNumber))
                    {

                    }

                    var matchLowerAndUpperApp = Regex.Match(applicantName, @"\p{Ll}+.+\p{Lu}+");
                    var matchLowerAndUpperTitle = Regex.Match(applicantName, @"\p{Ll}+.+\p{Lu}+");
                    if (matchLowerAndUpperApp.Success || matchLowerAndUpperTitle.Success)
                    {
                        ints.Add(i);
                    }

                    curElem.PubNumber = pubNumber;
                    curElem.id = id;
                    curElem.Title = new Title
                    {
                        Text = title.Replace("\n", " "),
                        Language = "tr"
                    };
                    curElem.Applicants = new List<Applicant>
                    {
                        new Applicant
                        {
                            Name = applicantName.Replace("\n", " ")
                        }
                    };

                    elementsOut.Add(curElem);
                }
            }

            return elementsOut;
        }
    }
}
