using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diamond_PH
{
    public class Elements
    {
        public string AppNumber { get; set; }
        public string Title { get; set; }
        public string AppDate { get; set; }
        public string EventDate { get; set; }
        public List<Owner> Owner { get; set; }

        public override string ToString()
        {
            return $"{ AppNumber}";
        }

    }

    public class Owner
    {
        public string Name { get; set; }
        public string Country { get; set; }
    }
}
