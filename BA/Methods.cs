using Integration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BA
{
    public class Methods
    {
        public static List<FileInfo> GetTetmlFiles(string path)
        {
            var pathInfo = new DirectoryInfo(path);
            if (pathInfo.Exists)
            {
                return pathInfo.GetFiles("*.tetml", SearchOption.TopDirectoryOnly).ToList();
            }
            else
            {
                Console.WriteLine($"Folder \"{path}\" doesn't exists");
                return null;
            }
        }
        public static List<Priority> PrioritySplit(string prioString)
        {
            List<Priority> priorityStruct = new List<Priority>();
            string tmpPrio = prioString.Replace("(31)", "").Trim();
            string[] lineSplittedPrio = null;
            string datePattern = @"\d{4}\-\d{2}\-\d{2}";
            string countryPattern = @"[A-Z]{2}$";
            /*If more than one priority separated with new line*/
            if (tmpPrio.Contains("\n"))
            {
                lineSplittedPrio = tmpPrio.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            }
            else { lineSplittedPrio = (lineSplittedPrio ?? Enumerable.Empty<string>()).Concat(new string[] { tmpPrio }).ToArray(); }
            /*Trying to split each record to date/number/country*/
            if (lineSplittedPrio != null)
            {
                foreach (var rec in lineSplittedPrio)
                {
                    /*add found value to output*/
                    Priority record = new Priority
                    {
                        Date = Regex.Match(rec, datePattern).Value.Trim(),
                        Number = rec.Remove(rec.IndexOf(Regex.Match(rec, datePattern).Value.Trim())).Trim(),
                        Country = Regex.Match(rec, countryPattern).Value.Trim()
                    };
                    priorityStruct.Add(record);
                }
            }
            if (priorityStruct != null) return priorityStruct;
            else return null;
        }

        public static List<string> RecSplit(string recString)
        {
            List<string> splittedRecord = null;
            string tempStrC = recString
                .Replace("(32)", "")
                .Replace("(33)", "")
                .Replace("Broj ostalih patentnih zahtjeva:", "(I99)")
                .Replace("�", "ti")
                .Trim();
            string tmpDescValue = null;
            if (recString != "")
            {
                if (recString.Contains("(57)"))
                {
                    tmpDescValue = tempStrC.Substring(tempStrC.IndexOf("(57)")).Trim();
                    tempStrC = tempStrC.Remove(tempStrC.IndexOf("(57)")).Trim();
                }
                Regex regexPatOne = new Regex(@"\(\d{2}\)", RegexOptions.IgnoreCase);
                MatchCollection matchesClass = regexPatOne.Matches(recString);
                if (matchesClass.Count > 0)
                {
                    foreach (Match matchC in matchesClass)
                    {
                        tempStrC = tempStrC.Replace(matchC.Value, "***" + matchC.Value);
                    }
                }
                /*Splitting record*/
                splittedRecord = tempStrC.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            if (tmpDescValue != null)
            {
                splittedRecord.Add(tmpDescValue);
            }
            return splittedRecord;
        }

        public static List<Ipc> ClassificationInfoSplit(string tmpString)
        {
            List<Ipc> priority = new List<Ipc>();
            string[] splClass = null;
            string tmpClass = tmpString.Replace("(51)", "").Replace(" ", "").Trim();
            if (tmpClass.Contains(","))
            {
                splClass = tmpClass.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            }
            else
            {
                splClass = (splClass ?? Enumerable.Empty<string>()).Concat(new string[] { tmpClass }).ToArray();
            }
            if (splClass != null)
            {
                foreach (var record in splClass)
                {
                    if (Regex.IsMatch(record, @"\(\d{4}\.\d{2}\)"))
                    {
                        priority.Add(new Ipc 
                        {
                            Class = record.Remove(record.IndexOf(Regex.Match(record, @"\(\d{4}\.\d{2}\)").Value)).Insert(4, " "),
                            Date = Regex.Match(record, @"\(\d{4}\.\d{2}\)").Value.Replace("(", "").Replace(")", "").Trim()
                        });
                    }
                    else
                    {
                        priority.Add(new Ipc
                        {
                            Class = record.Length > 3 ? record.Insert(4, " ") : record,
                            Date = null
                        });
                    }
                }
            }
            if (splClass != null) return priority;
            else return null;
        }

        /*Without address - 71, 73, 74*/
        public static List<PartyMember> GetPersonsShortInfo(string tmpString)
        {
            List<PartyMember> persons = new List<PartyMember>();
            var person = new PartyMember();
            string tmpValues = tmpString.Replace("(71)", "").Replace("(73)", "").Replace("(74)", "").Replace("\n", "****").Trim();
            if (Regex.IsMatch(tmpValues, @"\*{4}[A-Z]{2}\*{4}"))
            {
                string[] tmpSplValues = Regex.Split(tmpValues, @"(?<=\*{4}[A-Z]{2}\*{4})").Where(d => d != "").ToArray();
                foreach (var rec in tmpSplValues)
                {
                    if (Regex.IsMatch(rec, @"\*{4}[A-Z]{2}\*{4}"))
                    {
                        person.Country = Regex.Match(rec, @"\*{4}[A-Z]{2}\*{4}$").Value.Replace("****", " ").Trim().Trim(',').Trim();
                        person.Name = rec.Remove(rec.IndexOf(Regex.Match(rec, @"\*{4}[A-Z]{2}\*{4}$").Value)).Replace("****", " ").Trim().Trim(',').Trim();
                    }
                    else 
                    {
                        person.Name = rec.Replace("****", " ").Trim().Trim(',').Trim();
                    }
                    persons.Add(person);
                }
            }
            else 
                person.Name = tmpValues.Replace("****", " ").Trim().Trim(',').Trim();
            persons.Add(person);

            return persons;
        }
        /*With address - 72, 75*/
        public static List<PartyMember> GetPersonsInfo(string tmpString)
        {
            List<PartyMember> persons = new List<PartyMember>();
            
            List<string> tmpSplValues = new List<string>();
            string tmpValues = tmpString.Replace("(72)", "").Replace("(73)", "").Replace("(75)", "").Replace("\n", "****").Trim();
            if (Regex.IsMatch(tmpValues, @"\*{4}[A-Z]{2}\*{4}"))
            {
                tmpSplValues = Regex.Split(tmpValues, @"(?<=\*{4}[A-Z]{2}\*{4})").Where(d => d != "").Select(x => x.TrimEnd('*')).ToList();
            }
            else
            {
                tmpSplValues.Add(tmpValues);
            }
            foreach (var rec in tmpSplValues)
            {
                string tmpName = rec.Remove(rec.IndexOf(Regex.Match(rec, @"\*{4}[A-Z]{2}$").Value)).Trim().Trim(',').Trim();

                if (tmpName.Contains("****"))
                {
                    persons.Add(new PartyMember
                    {
                        Name = tmpName.Remove(tmpName.IndexOf("****")).Trim(),
                        Address1 = tmpName.Substring(tmpName.IndexOf("****")).Replace("****", " ").Trim(),
                        Country = Regex.Match(rec, @"\*{4}[A-Z]{2}$").Value.Replace("****", " ").Trim().Trim(',').Trim()
                    });
                }
                else
                {
                    persons.Add(new PartyMember
                    {
                        Name = tmpName.Replace("****", " ").Trim().Trim(',').Trim(),
                        Country = Regex.Match(rec, @"\*{4}[A-Z]{2}$").Value.Replace("****", " ").Trim().Trim(',').Trim()
                    });
                }
            }
            return persons;
        }
    }
}
