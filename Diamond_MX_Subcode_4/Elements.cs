using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diamond_MX_Subcode_4
{
    public class Elements
    {
        public string PubNumber { get; set; }
        public string AppNumber { get; set; }
        public string EventDate { get; set; }
        public Owner Owner { get; set; }
    }

    public class Owner
    {
        public string Name { get; set; }
    }
}
