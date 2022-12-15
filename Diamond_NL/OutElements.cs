using System.Collections.Generic;

namespace Diamond_NL
{
    class OutElements
    {
        public struct NameAddressCountry
        {
            public string Name { get; set; }
            public string Address { get; set; }
            public string Country { get; set; }
        }
        public class SubCombo
        {
            public string PublNumber { get; set; }
            public string LePatNumber { get; set; }
            public string DateI24 { get; set; }
            public string LeNoteValue { get; set; }
        }
        public class Sub26
        {
            public string PublNumber { get; set; }
            public string LePatNumber { get; set; }
            public string DateI24 { get; set; }
            public List<string> IntClass { get; set; }
            public string New73Assignee { get; set; }
            public string New71Applicant { get; set; }
            public string LeNoteValue { get; set; }
        }
    }
}
