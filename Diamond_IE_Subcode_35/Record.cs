using System.Collections.Generic;

namespace Diamond_IE_Subcode_35
{
    class Record
    {
        public string pubNumber { get; set; }
        public string appNumber { get; set; }
        public string appDate { get; set; }
        public List<Person> owner { get; set; } = new List<Person>();
        public string note { get; set; }
    }

    class Person
    {
        public string name { get; set; }
    }
}
