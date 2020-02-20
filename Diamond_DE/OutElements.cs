using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diamond_DE
{
    class OutElements
    {
        public struct NameAddressCountry
        {
            public string Name { get; set; }
            public string Address { get; set; }
            public string Country { get; set; }
        }
        public class Sub1
        {
            public string AppNumber { get; set; }
            public string LePatNumber { get; set; }
            public string DateI43 { get; set; }
            public List<NameAddressCountry> New71Applicant { get; set; }
            public List<NameAddressCountry> New74Agent { get; set; }
        }
        public class Sub3
        {
            public string AppNumber { get; set; }
            public string LePatNumber { get; set; }
            public string DateI45 { get; set; }
            public List<NameAddressCountry> New73Assignee { get; set; }
            public List<NameAddressCountry> New74Agent { get; set; }
        }

        public class Sub6
        {
            public string AppNumber { get; set; }
            public string IpcClass { get; set; }
            public string DateI43 { get; set; }
        }
        public class Sub7
        {
            public string AppNumber { get; set; }
            public string IpcClass { get; set; }
            public string DateI45 { get; set; }
        }
    }
}
