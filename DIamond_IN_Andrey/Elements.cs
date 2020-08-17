using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIamond_IN_Andrey
{
    public class Elements
    {
        public string PubNumber { get; set; }
        public string AppNumber { get; set; }
        public string Kind { get; set; }
        public string PRIN { get; set; }
        public string PRID { get; set; }
        public string PRIC { get; set; }
        public PCT PCT { get; set; }
        public string WO { get; set; }
        public Related Related { get; set; }
    }

    public class Related
    {
        public string AppNumber { get; set; }
        public string AppDate { get; set; }
        public string PubNumber { get; set; }
        public string PubDate { get; set; }
        public string Inid { get; set; }
    }

    public class PCT
    {
        public string AppNumber { get; set; }
        public string DateOfFiling { get; set; }
    }
}
