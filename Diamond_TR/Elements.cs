using System;
using System.Collections.Generic;
using System.Text;

namespace Diamond_TR
{
    class Elements
    {
        public string PubNumber { get; set; }
        public string EventDate { get; set; }
        public Title Title { get; set; }
        public List<Applicant> Applicants { get; set; }
        public int id { get; set; }
    }

    public class Title
    {
        public string Text { get; set; }
        public string Language { get; set; }
    }

    public class Applicant
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
    }
}
