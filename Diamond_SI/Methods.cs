using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Diamond_SI
{
    public class Methods
    {
        internal static string[] RecordsSplit(string s)
        {
            string tempS = s;
            tempS = tempS.Replace("****", "").Trim();

            var splittedRecord = tempS.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();

            return splittedRecord;
        }

        internal static string[] Subcode20RecordSplit(string s)
        {
            var tempStr = s;
            var fields = new string[] { "\\(51\\)", "\\(11\\)", "\\(13\\)", "\\(46\\)", "\\(21\\)", "\\(22\\)", "\\(86\\)", "\\(96\\)", "\\(87\\)", "\\(97\\)", "\\(30\\)", "\\(72\\)", "\\(73\\)", "\\(74\\)", "\\(54\\)"};

            string correct51Field = "(51) ";

            Regex reg1 = new Regex(@"[A-Z]{1}\d{2}[A-Z]{1} \d{1,3}/\d{1,2}");
            MatchCollection matches51 = reg1.Matches(tempStr);

            if(matches51.Count > 0)
            {
                foreach(Match oneMatch in matches51)
                {
                    correct51Field += oneMatch.Value + "\n";
                    tempStr = tempStr.Replace(oneMatch.Value, "");
                }
                correct51Field = correct51Field.TrimEnd("\n".ToCharArray());
            }

            foreach(var item in fields)
            {
                Regex reg = new Regex(item);
                MatchCollection matches = reg.Matches(tempStr);
                if(matches.Count > 0)
                {
                    foreach(Match match in matches)
                    {
                        tempStr = tempStr.Replace(match.Value, "***" + match.Value);
                    }
                }
            }

            var splittedRecord = tempStr.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();

            for(int i = 0; i<splittedRecord.Length; i++)
            {
                if (splittedRecord[i].Contains(@"(51)"))
                {
                    splittedRecord[i] = correct51Field;
                    break;
                }
            }

            return splittedRecord;
        }

        internal static string[] GetClassificationArray(string s)
        {
           return s.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
        }

        internal static string[] GetPrioritiesArray(string s)
        {
            return s.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
        }

        internal static List<Fields_86_87_96_97> GetListField86_87_96_97(string str, int numberField)
        {
            List<Fields_86_87_96_97> outList = new List<Fields_86_87_96_97>();
            var list = GetClassificationArray(str);
            if (list.Length == 1)
                list = list[0].Split(new string[] {";"}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())
                    .ToArray();
            Fields_86_87_96_97 field = null;

            foreach (var item in list)
            {
                field = new Fields_86_87_96_97();

                Match field86_87_96_97 = null;

                switch (numberField)
                {
                    case 86:
                        //field86_87_96_97 = Regex.Match(item, @"(?<Date>\d{2}\.\d{2}\.\d{4})\s*[A-Z]{2}\s*(?<Number>.*)");
                        field86_87_96_97 = Regex.Match(item, @"(?<Date>\d{2}\.\d{2}\.\d{4})\s*(?<Country>[A-Z]{2})\s*(?<Number>.*)");
                        break;
                    case 87:
                        //field86_87_96_97 = Regex.Match(item, @"[A-Z]{2}\s*(?<Number>.*)\s*,\s*(?<Date>\d{2}\.\d{2}\.\d{4})");
                        field86_87_96_97 = Regex.Match(item, @"(?<Country>[A-Z]{2})\s*(?<Number>.*)\s*,\s*(?<Date>\d{2}\.\d{2}\.\d{4})");
                        break;
                    case 96:
                        field86_87_96_97 = Regex.Match(item, @"(?<Date>\d{2}\.\d{2}\.\d{4})\s*(?<Country>[A-Z]{2})\s*(?<Number>.*)");
                        break;
                    case 97:
                        field86_87_96_97 = Regex.Match(item, @"(?<Country>[A-Z]{2})\s*(?<Number>.+)\s*(?<Kind>[A-Z]{1}\d{0,2})\s*,\s*(?<Date>\d{2}\.\d{2}\.\d{4})");
                        break;
                }

                field.Country = field86_87_96_97.Groups["Country"].Value;
                field.Date = DateNormalize(field86_87_96_97.Groups["Date"].Value);
                field.kind = field86_87_96_97.Groups["Kind"].Value;
                field.Number = field86_87_96_97.Groups["Number"].Value;
               
                outList.Add(field);
            }
            return outList;
        }

        internal static List<PriorityInformation> GetPriorityList(string prioritiesStr)
        {
            List<PriorityInformation> outList = new List<PriorityInformation>();
            PriorityInformation priorityInformation = null;
            var list = GetPrioritiesArray(prioritiesStr);

            foreach(var item in list)
            {
                priorityInformation = new PriorityInformation();
                var priority = Regex.Match(item, @"(?<Date>\d{2}\.\d{2}\.\d{4})\s*(?<Country>[A-Z]{2})\s*(?<Number>.*)");
                priorityInformation.PriorityCountryCode = priority.Groups["Country"].Value;
                priorityInformation.PriorityDate = DateNormalize(priority.Groups["Date"].Value);
                priorityInformation.PriorityNumber = priority.Groups["Number"].Value;
                outList.Add(priorityInformation);
            }
            return outList;
        }

        internal static List<PersonInformation> GetInventors_Grantee_Assignee_Owner_AgentInformation(string personsInfoStr)
        {
            PersonInformation personInfo = null;
            List<PersonInformation> outList = new List<PersonInformation>();
            personsInfoStr = personsInfoStr.Replace("\n", " ").Trim();
            var allPersons = GetPrioritiesArray(personsInfoStr);

            foreach (var person in allPersons)
            {
                personInfo = new PersonInformation();
                if (person.Contains(","))
                {
                    var splittedPersonInfo = person.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                    personInfo.Name = splittedPersonInfo[0];
                    personInfo.Country = splittedPersonInfo.Last();

                    string address = "";
                    for(int i = 0; i < splittedPersonInfo.Length; i++)
                    {
                        if(!splittedPersonInfo[i].Contains(personInfo.Name) && !splittedPersonInfo[i].Contains(personInfo.Country))
                        {
                            address += splittedPersonInfo[i] + ", ";
                        }
                    }
                    personInfo.Address = NormalizeStr(address);
                }
                else
                    personInfo.Name = person;


                outList.Add(personInfo);
            }
            return outList;
        }

        internal static List<Fields_86_87_96_97> NormalizeList(List<Fields_86_87_96_97> list, int size)
        {
            while (list.Count < size)
            {
                list.Add(new Fields_86_87_96_97 { Number = null, Country = null, Date = null, kind = null });
            }
            return list;
        }

        internal static string NormalizeStr(string s)
        {
            char[] MyChar = { ' ', ',' };
            return s.TrimEnd(MyChar).Trim();
        }
        internal static string DateNormalize(string s)
        {
            string dateNormalized = s;
            if (Regex.IsMatch(s, @"\d{2}\/*\-*\.*\d{2}\/*\-*\.*\d{4}"))
            {
                var date = Regex.Match(s, @"(?<day>\d{2})\/*\-*\.*(?<month>\d{2})\/*\-*\.*(?<year>\d{4})");
                dateNormalized = date.Groups["year"].Value + "-" + date.Groups["month"].Value + "-" + date.Groups["day"].Value;
            }
            return dateNormalized;
        }
    }
}
