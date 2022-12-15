using System.Collections.Generic;

namespace Dimond_EE
{
    class Patent
    {
        public string newspaperName { get; set; }
        public List<(string, string)> i51 { get; set; } = new List<(string, string)>();
        public string i11 { get; set; }
        public string i13 { get; set; }
        public List<Priorities> i30 { get; set; } = new List<Priorities>();
        public string i96appNumber { get; set; }
        public string i96appDate { get; set; }
        public List<Priorities> i97 { get; set; } = new List<Priorities>();
        public string i54 { get; set; }
        public List<Person> i73 { get; set; } = new List<Person>();
        public List<Person> i72 { get; set; } = new List<Person>();
        public List<Person> i74 { get; set; } = new List<Person>();
        public List<string> note { get; set; } = new List<string>();

    }

    class Priorities
    {
        public string number { get; set; }
        public string kind { get; set; }
        public string date { get; set; }
    }

    class Person
    {
        public string name { get; set; }
        public string adress { get; set; }
        public string country { get; set; }
    }
}
