using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Diamond_RU
{
    public class Methods
    {
        internal static (string, string) GetAbbreviation(string s)
        {
            switch (s)
            {
                case var form when new Regex(@"Общество с ограниченной ответственностью", RegexOptions.IgnoreCase).IsMatch(form): return ("ООО", "Общество с ограниченной ответственностью");
                case var form when new Regex(@"Общество с дополнительной ответственностью", RegexOptions.IgnoreCase).IsMatch(form): return ("ОДО", "Общество с дополнительной ответственностью");
                case var form when new Regex(@"Частное унитарное предприятие", RegexOptions.IgnoreCase).IsMatch(form): return ("ЧУП", "Частное унитарное предприятие");
                case var form when new Regex(@"Закрытое акционерное общество", RegexOptions.IgnoreCase).IsMatch(form): return ("ЗАО", "Закрытое акционерное общество");
                case var form when new Regex(@"Открытое акционерное общество", RegexOptions.IgnoreCase).IsMatch(form): return ("ОАО", "Открытое акционерное общество");
                case var form when new Regex(@"Акционерное общество", RegexOptions.IgnoreCase).IsMatch(form): return ("АО", "Акционерное общество");
                case var form when new Regex(@"Производственный кооператив", RegexOptions.IgnoreCase).IsMatch(form): return ("ПК", "Производственный кооператив");
                //case var form when new Regex(@"Унитарное предприятие", RegexOptions.IgnoreCase).IsMatch(form): return "УП";
                default: return (null, null);
            }
        }

        internal static List<string> GetListNames()
        {
            return new List<string>()
            {
                "Общество с ограниченной ответственностью",
                "Общество с дополнительной ответственностью",
                "Частное унитарное предприятие",
                "Закрытое акционерное общество",
                "Открытое акционерное общество",
                "Производственный кооператив",
                "Унитарное предприятие"
            };
        }

        internal static string ReplaceNameOrganization(string s)
        {
            string tempStr = s;
            string abbriv = "", fullName = "";
            foreach (var name in GetListNames())
            {
                if (tempStr.Contains(name))
                {
                    tempStr = tempStr.Replace(name, GetAbbreviation(tempStr).Item1);
                    return Regex.Replace(tempStr, @"\([A-Z]{2}\)", "").Trim();
                }
            }
            return null;
        }

        internal static string GetDateFromNameArchive()
        {
            string date = Regex.Match(RU_main.NameArchive, @"RU_(?<value>\d{8})_.*", RegexOptions.IgnoreCase)
                ?.Groups["value"].Value;
            return date;
        }

        internal static (string, string) GetNameChapterForName(string nameChapter)
        {
            switch (nameChapter)
            {
                case "HZ9A":
                case "HC9A":
                case "HE9A":
                case "FA9A01":
                case "FA9A02":
                case "FA9A03":
                case "FA9A04":
                case "FZ9A":
                    return ("ИЗВЕЩЕНИЕ К ЗАЯВКЕ НА ИЗОБРЕТЕНИЕ", "NOTIFICATIONS TO THE APPLICATION FOR THE INVENTION");
                case "PD4A":
                case "PC4A01":
                case "PC4A03":
                case "TC4A":
                case "TE4A":
                case "MZ4A":
                case "MM4A":
                case "MF4A01":
                case "MF4A02":
                case "NG4A":
                case "NF4A":
                case "QC4A01":
                    return ("ИЗВЕЩЕНИЕ К ПАТЕНТУ НА ИЗОБРЕТЕНИЕ", "NOTIFICATIONS TO THE PATENT FOR THE INVENTION");
                case "PC9K01":
                case "PC9K02":
                case "PD9K":
                case "TC9K":
                case "TE9K":
                case "MZ9K":
                case "MM9K":
                case "MG9K":
                case "MF9K01":
                case "NF9K":
                case "QC9K01":
                    return ("ИЗВЕЩЕНИЕ К ПАТЕНТУ НА ПОЛЕЗНУЮ МОДЕЛЬ", "NOTIFICATIONS TO THE PATENT FOR THE UTILITY MODEL");

            }
            return (null, null);
        }

        internal static (string, string) GetDescriptionForNotes(string nameSubcode)
        {
            switch (nameSubcode)
            {
                case "PC4A01": //3
                case "PC9K01": //4
                case "PC4A03": //5
                    return ("Номер государственной регистрации отчуждения исключительного права", "Registration number of transfer of an exclusive right");
                case "PC9K02": //6
                    return ("Номер государственной регистрации перехода исключительного права", "Registration number of transfer of an exclusive right");
            }
            return (null, null);
        }
    }
}
