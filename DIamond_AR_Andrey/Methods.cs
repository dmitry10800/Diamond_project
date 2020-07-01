using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DIamond_AR_Andrey
{
    class Methods
    {
        public static List<string> RecSplit(string s, string[] parameters)
        {
            foreach (var param in parameters)
            {
                if (!string.IsNullOrEmpty(param))
                    s = s.Replace(param, $"***{param}");
            }

            return s.Split("***", StringSplitOptions.RemoveEmptyEntries).ToList();
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

        public static string FindCountry(string s)
        {
            var countries = new List<string>
            {
                ", AM",", AL",", AO",", AQ",", AR",", AS",", AT",", AU",", AW",", AX",", AZ",", BA",", BB",", BD",", BE",", BF",", BG",", BH",", BI",", BJ",", BL",", BM",", BO",", BQ",", BR",", BS",", BT",", BV",", BW",", BY",", BZ",", CA",", CC",", CD",", CF",", CG",", CH",", CI",", CK",", CL",", CM",", CN",", CO",", CR",", CU",", CV",", CW",", CX",", CY",", CZ",", DE",", DJ",", DK",", DM",", DO",", DZ",", EC",", EE",", EG",", EH",", ER",", ES",", ET",", FI",", FJ",", FK",", FM",", FO",", FR",", GA",", GB",", GD",", GE",", GF",", GG",", GH",", GI",", GL",", GM",", GN",", GP",", GQ",", GR",", GS",", GT",", GU",", GW",", GY",", HK",", HM",", HN",", HR",", HT",", HU",", ID",", IE",", IL",", IM",", IN",", IO",", IQ",", IR",", IS",", IT",", JE",", JM",", JO",", JP",", KE",", KG",", KH",", KI",", KM",", KN",", KP",", KR",", KW",", KY",", KZ",", LA",", LB",", LC",", LI",", LK",", LR",", LS",", LT",", LY",", MA",", MC",", MD",", ME",", MF",", MG",", MH",", MK",", ML",", MN",", MM",", MO",", MP",", MQ",", MR",", MS",", MT",", MU",", MV",", MW",", MX",", MY",", MZ",", NA",", NC",", NE",", NF",", NG",", NI",", NL",", NO",", NP",", NR",", NU",", NZ",", OM",", PA",", PE",", PF",", PG",", PH",", PK",", PL",", PM",", PN",", PR",", PS",", PT",", PW",", PY",", QA",", RE",", RO",", RS",", RU",", RW",", SA",", SB",", SC",", SD",", SE",", SG",", SH",", SI",", SJ",", SK",", SL",", SM",", SN",", SO",", SR",", SS",", ST",", SV",", SX",", SY",", SZ",", TC",", TD",", TF",", TG",", TH",", TJ",", TK",", TL",", TM",", TN",", TO",", TR",", TT",", TV",", TW",", TZ",", UA",", UG",", UK",", UM",", US",", UY",", UZ",", VA",", VC",", VE",", VG",", VI",", VN",", VU",", WF",", WS",", YE",", YT",", ZA",", ZM",", ZW",", EM"
            };
            switch (s)
            {
                case var country when new Regex(@"Timor-Leste", RegexOptions.IgnoreCase).Match(country).Success: return "TL";
                case var country when new Regex(@"Turquemenistão", RegexOptions.IgnoreCase).Match(country).Success: return "TM";
                case var country when new Regex(@"Tunísia", RegexOptions.IgnoreCase).Match(country).Success: return "TN";
                case var country when new Regex(@"Tonga", RegexOptions.IgnoreCase).Match(country).Success: return "TO";
                case var country when new Regex(@"Peru", RegexOptions.IgnoreCase).Match(country).Success: return "TR";
                case var country when new Regex(@"Trindade e Tobago", RegexOptions.IgnoreCase).Match(country).Success: return "TT";
                case var country when new Regex(@"Tuvalu", RegexOptions.IgnoreCase).Match(country).Success: return "TV";
                case var country when new Regex(@"Taiwan, província da China", RegexOptions.IgnoreCase).Match(country).Success: return "TW";
                case var country when new Regex(@"Tanzânia, República Unida da", RegexOptions.IgnoreCase).Match(country).Success: return "TZ";
                case var country when new Regex(@"Ucrânia", RegexOptions.IgnoreCase).Match(country).Success: return "UA";
                case var country when new Regex(@"Uganda", RegexOptions.IgnoreCase).Match(country).Success: return "UG";
                case var country when new Regex(@"Reino Unido", RegexOptions.IgnoreCase).Match(country).Success: return "UK";
                case var country when new Regex(@"Ilhas Menores Distantes dos Estados Unidos", RegexOptions.IgnoreCase).Match(country).Success: return "UM";
                case var country when new Regex(@"(Estados Unidos da América)|(Estados Unidos)", RegexOptions.IgnoreCase).Match(country).Success: return "US";
                case var country when new Regex(@"Uruguai", RegexOptions.IgnoreCase).Match(country).Success: return "UY";
                case var country when new Regex(@"Usbequistão", RegexOptions.IgnoreCase).Match(country).Success: return "UZ";
                case var country when new Regex(@"Santa Sé", RegexOptions.IgnoreCase).Match(country).Success: return "VA";
                case var country when new Regex(@"São Vicente e Granadinas", RegexOptions.IgnoreCase).Match(country).Success: return "VC";
                case var country when new Regex(@"Venezuela (República Bolivariana da)", RegexOptions.IgnoreCase).Match(country).Success: return "VE";
                case var country when new Regex(@"Ilhas Virgens Britânicas", RegexOptions.IgnoreCase).Match(country).Success: return "VG";
                case var country when new Regex(@"Ilhas Virgens (EUA)", RegexOptions.IgnoreCase).Match(country).Success: return "VI";
                case var country when new Regex(@"Vietnã", RegexOptions.IgnoreCase).Match(country).Success: return "VN";
                case var country when new Regex(@"Vanuatu", RegexOptions.IgnoreCase).Match(country).Success: return "VU";
                case var country when new Regex(@"Wallis e Futunac", RegexOptions.IgnoreCase).Match(country).Success: return "WF";
                case var country when new Regex(@"Samoa", RegexOptions.IgnoreCase).Match(country).Success: return "WS";
                case var country when new Regex(@"Iémen", RegexOptions.IgnoreCase).Match(country).Success: return "YE";
                case var country when new Regex(@"Mayotte", RegexOptions.IgnoreCase).Match(country).Success: return "YT";
                case var country when new Regex(@"República da África do Sul", RegexOptions.IgnoreCase).Match(country).Success: return "ZA";
                case var country when new Regex(@"Zâmbia", RegexOptions.IgnoreCase).Match(country).Success: return "ZM";
                case var country when new Regex(@"Zimbábue", RegexOptions.IgnoreCase).Match(country).Success: return "ZW";
                case var country when new Regex(@"União Europ(é|e)ia", RegexOptions.IgnoreCase).Match(country).Success: return "EM";
            }

            return s;
        }
    }
}
