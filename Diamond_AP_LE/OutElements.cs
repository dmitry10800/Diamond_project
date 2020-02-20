using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diamond_AP_LE
{
    class OutElements
    {
        public struct NameAddressCountry
        {
            public string Name { get; set; }
            public string Address { get; set; }
            public string Country { get; set; }
        }
        public class Sub10
        {
            public string AppNumber { get; set; }
            public string DateFeePaid { get; set; }
            public string ValidUntil { get; set; }
            public string Anniversary { get; set; }
        }
        public class Sub7
        {
            public string AppNumber { get; set; }
            public string PatNumber { get; set; }
            public string DateFeePaid { get; set; }
            public string ValidUntil { get; set; }
            public string Anniversary { get; set; }
        }
    }
}
