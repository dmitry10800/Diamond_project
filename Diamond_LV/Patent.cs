using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diamond_LV
{
    class Patent
    {
        public string newspaperName { get; set; }
        public List<(string, string)> i51 { get; set; } = new List<(string, string)>();
        public string i11 { get; set; }
        public string i13 { get; set; }
        public string i21 { get; set; }
        public string i22 { get; set; }
        public string i43 { get; set; }
        public string i45 { get; set; }
        public List<string> i31 { get; set; }
        public List<string> i32 { get; set; }
        public List<string> i33 { get; set; }
        public string i86PCTappNumber { get; set; }
        public string i86PCTappDate { get; set; }
        public string i87PCTpubNumber { get; set; }
        public string i87PCTpubDate { get; set; }
        public List<string> i54 { get; set; } = new List<string>();
        public List<string> i62 { get; set; }
        public List<Person> i73 { get; set; } = new List<Person>();
        public List<Person> i72 { get; set; } = new List<Person>();
        public List<Person> i74 { get; set; } = new List<Person>();
        public string i57 { get; set; }
        public string note { get; set; }

    }

    class Person
    {
        public string name { get; set; }
        public string adress { get; set; }
        public string country { get; set; }
    }
}
