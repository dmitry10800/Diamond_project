using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace EE
{
    class Process
    {
        public static void Sub5(string v)
        {
            /*Process data from TXT file*/
            List<OutElements.Subcode5> elems = new List<OutElements.Subcode5>();
            //var pat = new Regex(@"(?=\d{6})");
            var getText = File.ReadAllText(v).Replace("\r\n", " ").Replace("\n", " ");
            //var splitText = pat.Split(getText).Where(x => x != "").Select(x => x.Trim()).ToList();
            var recordPat = new Regex(@"(?<AppNumber>\b\d+\b)\s*(?<PubKind>[A-Z]\d+)\s*(?<Date43>\d{4}\.\d{2}\.\d{2})\s*(?<LeNote>№\d+)\s*(?<LeDate>\d{4}\.\d{2}\.\d{2}\b)");
            var matches = recordPat.Matches(getText);
            foreach (Match item in matches)
            {
                var a = recordPat.Match(item.Value);
                if (a.Success)
                {
                    elems.Add(new OutElements.Subcode5
                    {
                        AppNumber = a.Groups["AppNumber"].Value,
                        PubKind = a.Groups["PubKind"].Value,
                        Date43 = a.Groups["Date43"].Value,
                        LeBulletinNumber = a.Groups["LeNote"].Value,
                        LeDate = a.Groups["LeDate"].Value,
                    });
                }
                else
                    Console.WriteLine($"Record pattern doesn't match!\t{item}");
            }
            Console.WriteLine();
            /*Conver to Diamond*/
            var diamFormat = ConvertToDiamond.Sub5(elems);
            /*Send on Diamond staging or Production*/
            Methods.SendToDiamond(diamFormat);
        }

        public static void Sub12(string v)
        {
            /*Process data from TXT file*/
            List<OutElements.Subcode12> elems = new List<OutElements.Subcode12>();
            var pat = new Regex(@"(?=\d{6})");
            var getText = File.ReadAllText(v).Replace("\r\n", " ").Replace("\n", " ");
            var splitText = pat.Split(getText).Where(x => x != "").Select(x => x.Trim()).ToList();
            var recordPat = new Regex(@"(?<number>\d{6})\s*(?<kind>[A-Z]{1}\d)\s*(?<dateFirst>\d{4}\.\d{2}\.\d{2})\s*(?<numberSecond>№\d+\b)\s*(?<countriesFirst>.*)\s*(?<dateSecond>\d{4}\.\d{2}\.\d{2})\s*(?<countriesSecornd>.*)");
            foreach (var rec in splitText)
            {
                var a = recordPat.Match(rec);
                if (a.Success)
                {
                    elems.Add(new OutElements.Subcode12
                    {
                        PubNumber = a.Groups["number"].Value,
                        LeNumber = a.Groups["number"].Value,
                        PubKind = a.Groups["kind"].Value,
                        I45Date = a.Groups["dateFirst"].Value.Replace(".", "-"),
                        LeBulletinNumber = a.Groups["numberSecond"].Value,
                        LeCcTerminated = a.Groups["countriesFirst"].Value,
                        LeDate = a.Groups["dateSecond"].Value.Replace(".", "-"),
                        LeCcValid = a.Groups["countriesSecornd"].Value
                    });
                }
                else
                    Console.WriteLine($"Record pattern doesn't match!\t{rec}");
            }
            /*Conver to Diamond*/
            var diamFormat = ConvertToDiamond.Sub12(elems);
            /*Send on Diamond staging or Production*/
            Methods.SendToDiamond(diamFormat);
        }
    }
}
