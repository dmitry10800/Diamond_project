using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Diamond_NZ_Subcodes_1_2_4
{
    public class Methods
    {
        public static List<string> GetListNamesCompanies()
        {
            return new List<string>
            {
                "PLC",
                "Public Limited Company",
                "Ltd",
                "Limited",
                "Inc",
                "Incorporated",
                "Corp",
                "Corporation",
                "LLC",
                "Limited Liability Company",
                "LDC",
                "Limited Duration Company",
                "IBC",
                "International Business Company",
                "IC",
                "International Company",
                "& Со",
                "and Company",
                "LP",
                "Limited Partnership",
                "SA",
                "Sosiedad Anonima",
                "Societe Anonyme",
                "SARL",
                "Societe a Responsidilite Limitee",
                "BV",
                "Vennootschap Met Beperkte Aansparkelij kheid",
                "NV",
                "Naamlose Vennootschap",
                "AVV",
                "GmbH",
                "Gesellschaft mit beschrakter Haftung",
                "AG",
                "Aktiengesellschaft"
            };
        }

        public static string MonthNameToDigit(string s)
        {
            switch (s)
            {
                case var month when new Regex(@"Jan", RegexOptions.IgnoreCase).IsMatch(month): return "01";
                case var month when new Regex(@"Feb", RegexOptions.IgnoreCase).IsMatch(month): return "02";
                case var month when new Regex(@"Mar", RegexOptions.IgnoreCase).IsMatch(month): return "03";
                case var month when new Regex(@"Apr", RegexOptions.IgnoreCase).IsMatch(month): return "04";
                case var month when new Regex(@"May", RegexOptions.IgnoreCase).IsMatch(month): return "05";
                case var month when new Regex(@"Jun", RegexOptions.IgnoreCase).IsMatch(month): return "06";
                case var month when new Regex(@"Jul", RegexOptions.IgnoreCase).IsMatch(month): return "07";
                case var month when new Regex(@"Aug", RegexOptions.IgnoreCase).IsMatch(month): return "08";
                case var month when new Regex(@"Sept", RegexOptions.IgnoreCase).IsMatch(month): return "09";
                case var month when new Regex(@"Oct", RegexOptions.IgnoreCase).IsMatch(month): return "10";
                case var month when new Regex(@"Nov", RegexOptions.IgnoreCase).IsMatch(month): return "11";
                case var month when new Regex(@"Dec", RegexOptions.IgnoreCase).IsMatch(month): return "12";
                default: return "00";
            }
        }

        internal static string DateNormalize(string s)
        {
            var dateNormalized = s;
            if (Regex.IsMatch(s, @"\d{2} \w+ \d{4}"))
            {
                var date = Regex.Match(s, @"(?<day>\d{2}) (?<month>\w+) (?<year>\d{4})");
                dateNormalized = date.Groups["year"].Value + "-" + MonthNameToDigit(date.Groups["month"].Value) + "-" + date.Groups["day"].Value;
            }
            return dateNormalized;
        }

        internal static (AgentInformation, AgentInformation) AgentInformationNormalize(string s) //2-ой Item для (74) new 
        {
            var agentInf = new AgentInformation();
            var agentInfNEW = new AgentInformation();
            string oldAgent = "", newAgent = "";

            var agentInfo = Regex.Match(s, @"(?<OldAgent>.+[^To:])\s*To:\s*(?<NewAgent>.*)");
            if (agentInfo.Success)
            {
                oldAgent = agentInfo.Groups["OldAgent"].Value;
                newAgent = agentInfo.Groups["NewAgent"].Value;

                agentInf = GetAgentInformation(oldAgent);
                agentInfNEW = GetAgentInformation(newAgent);
            }
            else
            {
                return (null, null);
            }
            return (agentInf, agentInfNEW);
        }

        internal static AgentInformation GetAgentInformation(string str)    //string[] array)
        {
            var tempStr = str;

            var arrayNames = GetListNamesCompanies();
            foreach (var item in arrayNames)
            {
                var regex = new Regex($", {item}", RegexOptions.IgnoreCase);
                var matches = regex.Matches(tempStr);

                if (matches.Count > 0)
                {
                    tempStr = tempStr.Replace(matches[0].Value, matches[0].Value);
                    break;
                }
            }

            var splittedAgent = tempStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();

            var agent = new AgentInformation();
            if (splittedAgent.Length > 0 && !string.IsNullOrEmpty(splittedAgent[0]))
            {
                agent.Name = splittedAgent[1].Trim();
                agent.Country = Regex.Replace(splittedAgent.Last(), @"\s+\d{4}$", "").Trim();
                if (agent.Country.Length > 2)
                    return null;

                var tempAddress = "";

                for (var i = 2; i < splittedAgent.Length - 1; i++)
                {
                    tempAddress += splittedAgent[i] + ", ";
                }

                agent.Address = tempAddress.Trim().TrimEnd(',').Trim();
                return agent;
            }
            return null;
        }

        internal static (List<ApplicantInformation>, List<ApplicantInformation>) ApplicantInformationNormalize(string s) //2-ой Item для (74) new 
        {
            var aplicantInf = new List<ApplicantInformation>();
            var aplicantInfNEW = new List<ApplicantInformation>();
            string oldApplicant = "", newApplicant = "";

            var applicantInfo = Regex.Match(s, @"(?<OldApp>.+[^To:])\s*To:\s*(?<NewApp>.*)");
            if (applicantInfo.Success)
            {
                oldApplicant = applicantInfo.Groups["OldApp"].Value;
                newApplicant = applicantInfo.Groups["NewApp"].Value;
                aplicantInf = GetApplicantInformation(oldApplicant);
                aplicantInfNEW = GetApplicantInformation(newApplicant);
            }
            else
            {
                return (null, null);
            }
            return (aplicantInf, aplicantInfNEW);
        }

        internal static List<ApplicantInformation> GetApplicantInformation(string str)    //string[] array)
        {
            var outApplicants = new List<ApplicantInformation>();
            var tempStr = str;

            var arrayNames = GetListNamesCompanies();
            foreach (var item in arrayNames)
            {
                var regex = new Regex($", {item}", RegexOptions.IgnoreCase);
                var matches = regex.Matches(tempStr);

                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        tempStr = tempStr.Replace(match.Value, match.Value.Replace(",", " "));
                    }
                }
            }

            var splittedAssignes = tempStr.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();

            ApplicantInformation applic;

            foreach (var splittedAssigne in splittedAssignes)
            {
                applic = new ApplicantInformation();
                var splittedAssign = splittedAssigne.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();

                applic.Name = splittedAssign[1].Trim();
                applic.Country = Regex.Replace(splittedAssign.Last(), @"\s+\d{4}$", "").Trim();
                if (applic.Country.Length > 2)
                    return null;

                var tempAddress = "";

                for (var i = 2; i < splittedAssign.Length - 1; i++)
                {
                    tempAddress += splittedAssign[i] + ", ";
                }

                applic.Address = tempAddress.Trim().TrimEnd(',').Trim();
                outApplicants.Add(applic);
            }


            return outApplicants;
        }
    }
}
