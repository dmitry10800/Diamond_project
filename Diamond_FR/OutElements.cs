using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diamond_FR
{
    class OutElements
    {
        public class FirstList
        {
            public string AppNumber { get; set; }
            public string LePatNumber { get; set; }
            public string LeDate { get; set; } //Gazette date
            public string LeNoteNumber { get; set; } //Note reg number
            public string LeNoteCountry { get; set; } //Note - nature of applicant
        }

        public class SecondList
        {
            public string AppNumber { get; set; }
            public string RegNumber { get; set; }
            public string NatureOfApplication { get; set; }
        }
    }
}
