using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diamond_BE
{
    public class Elements
    {
        public string PubNumber { get; set; }
        public string AppNumber { get; set; }
        public string Date45 { get; set; }
        public string Date47 { get; set; }
        public Agent Agent { get; set; }
        public string Title { get; set; }
        public string PubLang { get; set; }
        public Owner Owner { get; set; }
        public string EventDate { get; set; }
    }

    public class Agent
    {
        public string Name { get; set; }
    }

    public class Owner
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
    }
}
