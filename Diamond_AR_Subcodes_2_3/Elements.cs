using System;
using System.Collections.Generic;
using System.Text;

namespace Diamond_AR_Subcodes_2_3
{
    class Elements
    {
        public string PlainLang { get; set; }
        public string PubNumber { get; set; }
        public string Kind { get; set; }
        public string AppNumber { get; set; }
        public string AppDate { get; set; }
        public string EffectiveDate { get; set; }
        public string ExpirationDate { get; set; }
        public List<Priority> Priorities { get; set; }
        public string Date47 { get; set; }
        public List<Ipc> Ipcs { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public List<Applicant> Applicant { get; set; }
        public List<Agent> Agents { get; set; }
        public List<Inventor> Inventors { get; set; }
        public string Date45 { get; set; }
        public string Date41 { get; set; }
        public bool Is2 { get; set; }
        public string Note { get; set; }
        public string Tranlation { get; set; }
        public string IpcVersion { get; set; }
        public string PubDate { get; set; }

        public string[] Countries()
        {
            return new[]
            {
                ", AM",", AL",", AO",", AQ",", AR",", AS",", AT",", AU",", AW",", AX",", AZ",", BA",", BB",", BD",", BE",", BF",", BG",", BH",", BI",", BJ",", BL",", BM",", BO",", BQ",", BR",", BS",", BT",", BV",", BW",", BY",", BZ",", CA",", CC",", CD",", CF",", CG",", CH",", CI",", CK",", CL",", CM",", CN",", CO",", CR",", CU",", CV",", CW",", CX",", CY",", CZ",", DE",", DJ",", DK",", DM",", DO",", DZ",", EC",", EE",", EG",", EH",", ER",", ES",", ET",", FI",", FJ",", FK",", FM",", FO",", FR",", GA",", GB",", GD",", GE",", GF",", GG",", GH",", GI",", GL",", GM",", GN",", GP",", GQ",", GR",", GS",", GT",", GU",", GW",", GY",", HK",", HM",", HN",", HR",", HT",", HU",", ID",", IE",", IL",", IM",", IN",", IO",", IQ",", IR",", IS",", IT",", JE",", JM",", JO",", JP",", KE",", KG",", KH",", KI",", KM",", KN",", KP",", KR",", KW",", KY",", KZ",", LA",", LB",", LC",", LI",", LK",", LR",", LS",", LT",", LY",", MA",", MC",", MD",", ME",", MF",", MG",", MH",", MK",", ML",", MN",", MM",", MO",", MP",", MQ",", MR",", MS",", MT",", MU",", MV",", MW",", MX",", MY",", MZ",", NA",", NC",", NE",", NF",", NG",", NI",", NL",", NO",", NP",", NR",", NU",", NZ",", OM",", PA",", PE",", PF",", PG",", PH",", PK",", PL",", PM",", PN",", PR",", PS",", PT",", PW",", PY",", QA",", RE",", RO",", RS",", RU",", RW",", SA",", SB",", SC",", SD",", SE",", SG",", SH",", SI",", SJ",", SK",", SL",", SM",", SN",", SO",", SR",", SS",", ST",", SV",", SX",", SY",", SZ",", TC",", TD",", TF",", TG",", TH",", TJ",", TK",", TL",", TM",", TN",", TO",", TR",", TT",", TV",", TW",", TZ",", UA",", UG",", UK",", UM",", US",", UY",", UZ",", VA",", VC",", VE",", VG",", VI",", VN",", VU",", WF",", WS",", YE",", YT",", ZA",", ZM",", ZW",", EM"
            };
        }
    }

    public class Ipc
    {
        public string Classification { get; set; }
        public string Version { get; set; }
    }

    public class Inventor
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
    }

    public class Priority
    {
        public string Number { get; set; }
        public string Date { get; set; }
        public string Country { get; set; }
    }

    public class Applicant
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
    }

    public class Agent
    {
        public string Name { get; set; }
        public string Language { get; set; }
    }
}
