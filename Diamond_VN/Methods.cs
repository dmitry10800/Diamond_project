using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Diamond_VN
{
    class Methods
    {
        public static List<FileInfo> GetTetmlFiles(string path)
        {
            return Directory.GetFiles(path, @"*.tetml", SearchOption.TopDirectoryOnly).Select(x => new FileInfo(x)).ToList();
        }
        public static List<string> RecSplit(string s, string[] parameters)
        {
            string tmpAbstract = null;
            var splittedRecords = new List<string>();
            if (s.Contains(Processing.Abstract))
            {
                tmpAbstract = s.Substring(s.IndexOf(Processing.Abstract)).Trim();
                s = s.Remove(s.IndexOf(Processing.Abstract));

            }


            foreach (var param in parameters)
            {
                if (!string.IsNullOrEmpty(param))
                    s = s.Replace(param, $"***{param}");
            }

            s = s.Replace("&amp;", "&");

            splittedRecords = s.Split("***", StringSplitOptions.RemoveEmptyEntries).ToList();

            if (tmpAbstract != null)
                splittedRecords.Add(tmpAbstract);

            return splittedRecords;
        }

        public static string ToLang(string s)
        {
            if (s.Contains("VN"))
                s = "VI";
            else
                s = "EN";

            return s;
        }

        public static string ToParties(string s)
        {
            //char[] before = { '\u1eb6', '\u1ec1', '\u1ec6', '\u00f5', '\u1ead', '\u00e0', '\u00c0', '\u1ed9', '\u1ee5', '\u1ea7', '\u1ee1', '\u1edd' };
            //char[] after = { '\u0041', '\u0065', '\u0045', '\u006f', '\u0061', '\u0061', '\u0041', '\u006f', '\u0075', '\u0061', '\u006f', '\u006f' };

            char[] before = { 'Ă', 'Á', 'À', 'Â', 'Ã', 'È', 'É', 'Ê', 'Ì', 'Í', 'Ò', 'Ó', 'Ô', 'Õ', 'Ù', 'Ú', 'Ý', 'ă', 'â', 'à', 'á', 'ã', 'è', 'é', 'ê', 'ì', 'í', 'ò', 'ó', 'ô', 'õ', 'ù', 'ý', 'Ỳ', 'Ỹ', 'ỳ', 'ỹ', 'Ỷ', 'ỷ', 'Ỵ', 'ỵ', 'ự', 'Ự', 'ử', 'Ử', 'ữ', 'Ữ', 'ừ', 'Ừ', 'ứ', 'Ứ', 'ư', 'Ư', 'ụ', 'Ụ', 'ủ', 'Ủ', 'ũ', 'Ũ', 'ợ', 'Ợ', 'ở', 'Ở', 'ỡ', 'Ỡ', 'ờ', 'Ờ', 'ớ', 'Ớ', 'ơ', 'Ơ', 'ộ', 'Ộ', 'ổ', 'Ổ', 'ỗ', 'Ỗ', 'ồ', 'Ồ', 'ố', 'Ố', 'ọ', 'Ọ', 'ỏ', 'Ỏ', 'ị', 'Ị', 'ỉ', 'Ỉ', 'ĩ', 'Ĩ', 'ệ', 'Ệ', 'ể', 'Ể', 'ễ', 'Ễ', 'ề', 'Ề', 'ế', 'Ế', 'ẹ', 'Ẹ', 'ẻ', 'Ẻ', 'ẽ', 'Ẽ', 'ặ', 'Ặ', 'ẳ', 'Ẳ', 'ẵ', 'Ẵ', 'ằ', 'Ằ', 'ắ', 'Ắ', 'ă', 'Ă', 'ậ', 'Ậ', 'ẩ', 'Ẩ', 'ẫ', 'Ẫ', 'ầ', 'Ầ', 'ấ', 'Ấ', 'ạ', 'Ạ', 'ả', 'Ả', 'đ', '₫', 'Đ' };

            char[] after = { 'A', 'A', 'A', 'A', 'A', 'E', 'E', 'E', 'I', 'I', 'O', 'O', 'O', 'O', 'U', 'U', 'Y', 'a', 'a', 'a', 'a', 'a', 'e', 'e', 'e', 'i', 'i', 'o', 'o', 'o', 'o', 'u', 'y', 'Y', 'Y', 'y', 'y', 'Y', 'y', 'Y', 'y', 'u', 'U', 'u', 'U', 'u', 'U', 'u', 'U', 'u', 'U', 'u', 'U', 'u', 'U', 'u', 'U', 'u', 'U', 'o', 'O', 'o', 'O', 'o', 'O', 'o', 'O', 'o', 'O', 'o', 'O', 'o', 'O', 'o', 'O', 'o', 'O', 'o', 'O', 'o', 'O', 'o', 'O', 'o', 'O', 'i', 'I', 'i', 'I', 'i', 'I', 'e', 'E', 'e', 'E', 'e', 'E', 'e', 'E', 'e', 'E', 'e', 'E', 'e', 'E', 'e', 'E', 'a', 'A', 'a', 'A', 'a', 'A', 'a', 'A', 'a', 'A', 'a', 'A', 'a', 'A', 'a', 'A', 'a', 'A', 'a', 'A', 'a', 'A', 'a', 'A', 'a', 'A', 'd', 'd', 'D' };

            StringBuilder strB = new StringBuilder(s);
            for (int i = 0; i < before.Length; i++)
            {
                strB.Replace(before[i], after[i]);
            }

            return strB.ToString();
        }

        public static string DateNormalize(string s)
        {
            string dateNormalized = s;
            var date = Regex.Match(s, @"(?<day>\d{2})[^0-9]*(?<month>\d{1,2})[^0-9]*(?<year>\d{4})");
            var day = date.Groups["day"].Value;
            if (date.Groups["day"].Length == 1)
                day = $"0{day}";
            dateNormalized = date.Groups["year"].Value + date.Groups["month"].Value + day;

            if (!string.IsNullOrEmpty(dateNormalized))
            {
                dateNormalized = $"{date.Groups["year"].Value}-{date.Groups["month"].Value}-{day}";
            }

            return dateNormalized;
        }

        public static string PCT(string s)
        {
            char[] before = { '\u1ec1' };
            char[] after = { '\u0045' };

            StringBuilder strB = new StringBuilder(s);
            for (int i = 0; i < before.Length; i++)
            {
                strB.Replace(before[i], after[i]);
            }

            return strB.ToString();
        }

        public static int? ToInt(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            str = str.Trim(' ', '\n', '\r');

            if (int.TryParse(str, NumberStyles.None, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            return null;
        }

        public static string ConvertText(string str)
        {
            char[] TCVN3_char = { '\u03BC', '\u2212', '\u00FC', '\u00FB', '\u00FE', '\u00FA', '\u00F9', '\u00F7', '\u00F6', '\u00F5', '\u00F8', '\u00F1', '\u00F4', '\u00EE', '\u00EC', '\u00EB', '\u00EA', '\u00ED', '\u00E9', '\u00E7', '\u00E6', '\u00E5', '\u00E8', '\u00E1', '\u00E4', '\u00DE', '\u00D8', '\u00D6', '\u00D4', '\u00D3', '\u00D2', '\u00D5', '\u00CF', '\u00CE', '\u00D1', '\u00C6', '\u00BD', '\u00BC', '\u00AB', '\u00BE', '\u00CB', '\u00C9', '\u00C8', '\u00C7', '\u00CA', '\u00B6', '\u00B9', '\u00AD', '\u00A6', '\u00AC', '\u00A5', '\u00F2', '\u00DC', '\u00AE', '\u00A8', '\u00A1', '\u00F3', '\u00EF', '\u00E2', '\u00BB', '\u00E3', '\u00DF', '\u00DD', '\u00D7', '\u00AA', '\u00D0', '\u00CC', '\u00B7', '\u00A9', '\u00B8', '\u00B5', '\u00A4', '\u00A7', '\u00A3', '\u00A2', '\u0045' };
            char[] Unicode_char = { '\u00E0', '\u01B0', '\u1EF9', '\u1EF7', '\u1EF5', '\u1EF3', '\u1EF1', '\u1EEF', '\u1EED', '\u1EEB', '\u1EE9', '\u1EE7', '\u1EE5', '\u1EE3', '\u1EE1', '\u1EDF', '\u1EDD', '\u1EDB', '\u1ED9', '\u1ED7', '\u1ED5', '\u1ED3', '\u1ED1', '\u1ECF', '\u1ECD', '\u1ECB', '\u1EC9', '\u1EC7', '\u1EC5', '\u1EC3', '\u1EC1', '\u1EBF', '\u1EBD', '\u1EBB', '\u1EB9', '\u1EB7', '\u1EB5', '\u1EB3', '\u00F4', '\u1EAF', '\u1EAD', '\u1EAB', '\u1EA9', '\u1EA7', '\u1EA5', '\u1EA3', '\u1EA1', '\u01B0', '\u01AF', '\u01A1', '\u01A0', '\u0169', '\u0129', '\u0111', '\u0103', '\u0102', '\u00FA', '\u00F9', '\u00F5', '\u1EB1', '\u00F3', '\u00F2', '\u00ED', '\u00EC', '\u00EA', '\u00E9', '\u00E8', '\u00E3', '\u00E2', '\u00E1', '\u00E1', '\u00D4', '\u0110', '\u00CA', '\u00C2', '\u1ec1' };

            //string[] TCVN3_cap = { "\u0041\u00E0", "\u0041\u1EA3", "\u0041\u00E3", "\u0041\u00E1", "\u0041\u1EA1", "\u0045\u00E8", "\u0045\u1EBB", "\u0045\u1EBD", "\u0045\u00E9", "\u0045\u1EB9", "\u0049\u00EC", "\u0049\u1EC9", "\u0049\u0129", "\u0049\u00ED", "\u0049\u1ECB", "\u004F\u00F2", "\u004F\u1ECF", "\u004F\u00F5", "\u004F\u00F3", "\u004F\u1ECD", "\u0055\u00F9", "\u0055\u1EE7", "\u0055\u0169", "\u0055\u00FA", "\u0055\u1EE5", "\u0059\u1EF3", "\u0059\u1EF7", "\u0059\u1EF9", "\u0059\u00FD", "\u0059\u1EF5", "\u0102\u1EB1", "\u0102\u1EB3", "\u0102\u1EB5", "\u0102\u1EAF", "\u0102\u1EB7", "\u00C2\u1EA7", "\u00C2\u1EA9", "\u00C2\u1EAB", "\u00C2\u1EA5", "\u00C2\u1EAD", "\u00CA\u1EC1", "\u00CA\u1EC3", "\u00CA\u1EC5", "\u00CA\u1EBF", "\u00CA\u1EC7", "\u00D4\u1ED3", "\u00D4\u1ED5", "\u00D4\u1ED7", "\u00D4\u1ED1", "\u00D4\u1ED9", "\u01A0\u1EDD", "\u01A0\u1EDF", "\u01A0\u1EE1", "\u01A0\u1EDB", "\u01A0\u1EE3", "\u01AF\u1EEB", "\u01AF\u1EED", "\u01AF\u1EEF", "\u01AF\u1EE9", "\u01AF\u1EF1" };
            //string[] Unicode_cap = { "\u00C0", "\u1EA2", "\u00C3", "\u00C1", "\u1EA0", "\u00C8", "\u1EBA", "\u1EBC", "\u00C9", "\u1EB8", "\u00CC", "\u1EC8", "\u0128", "\u00CD", "\u1ECA", "\u00D2", "\u1ECE", "\u00D5", "\u00D3", "\u1ECC", "\u00D9", "\u1EE6", "\u0168", "\u00DA", "\u1EE4", "\u1EF2", "\u1EF6", "\u1EF8", "\u00DD", "\u1EF4", "\u1EB0", "\u1EB2", "\u1EB4", "\u1EAE", "\u1EB6", "\u1EA6", "\u1EA8", "\u1EAA", "\u1EA4", "\u1EAC", "\u1EC0", "\u1EC2", "\u1EC4", "\u1EBE", "\u1EC6", "\u1ED2", "\u1ED4", "\u1ED6", "\u1ED0", "\u1ED8", "\u1EDC", "\u1EDE", "\u1EE0", "\u1EDA", "\u1EE2", "\u1EEA", "\u1EEC", "\u1EEE", "\u1EE8", "\u1EF0" };

            //char[] before = { '\u0061', '' };

            //char[] TCVN3_char = { '\u00a7', '\u00ac', '\u006e', '\u0079', '\u00aa', '\u0075', '\u0063', '\u00c7', '\u0075', '\u00ca', '\u0070', '\u0062', '\u00bb', '\u006e', '\u0067', '\u00ae', '\u00e9', '\u0063', '\u0071', '\u0075', '\u0079', '\u00d2', '\u006e', '\u0053', '\u00b8', '\u0068', '\u00d5', '\u00aa', '\u00c7', '\u00ae', '\u00e9', '\u03bc', '\u00c8', '\u00de', '\u00ab' };

            //char[] Unicode_char ={'\u0110','\u01a1','\u006e','\u0079','\u00ea','\u0075','\u0063','\u1ea7','\u0075','\u1ea5','\u0070','\u0042','\u1eb1','\u006e','\u0067','\u0111','\u1ed9','\u0063','\u0071','\u0075','\u0079','\u1ec1','\u006e','\u0073','\u00e1','\u0068','\u1ebf', '\u00ea', '\u1ea7', '\u0111', '\u1ed9', '\u00e0', '\u1ea9', '\u1ecb', '\u00f4' };


            StringBuilder strB = new StringBuilder(str);
            for (int i = 0; i < TCVN3_char.Length; i++)
            {
                strB.Replace(TCVN3_char[i], Unicode_char[i]);
            }
            //for (int i = 0; i < TCVN3_cap.Length; i++)
            //{
            //    strB.Replace(TCVN3_cap[i], Unicode_cap[i]);
            //}
            return strB.ToString();
        }
    }
}
