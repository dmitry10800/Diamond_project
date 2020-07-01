using System;
using System.Collections.Generic;
using System.Text;

namespace Diamond_RS
{
    class Elements
    {
        public string AppNumber { get; set; }
        public string AppDate { get; set; }
        public Publication Publication { get; set; }
        public List<ClassificationIpc> Ipcs { get; set; }
        public Priority Priority { get; set; }
        public EuropeanPatents EuropeanPatents { get; set; }
        public Title Title { get; set; }
        public List<PartyMember> GranteeAssigneeOwner { get; set; }
        public List<PartyMember> Inventors { get; set; }
        public List<PartyMember> Agents { get; set; }
        public PCT Pct { get; set; }
    }

    public class Publication
    {
        public string PubNumber { get; set; }
        public string Kind { get; set; }
    }

    public class ClassificationIpc
    {
        public string Classification { get; set; }
    }

    public class Priority
    {
        public string Number { get; set; }
        public string Date { get; set; }
        public string Country { get; set; }
    }

    public class PCT
    {
        public string AppNumber { get; set; }
        public string AppDate { get; set; }
        public string PubNumber { get; set; }
        public string PubDate { get; set; }
    }

    public class EuropeanPatents
    {
        public string AppNumber { get; set; }
        public string AppDate { get; set; }
        public string PubNumber { get; set; }
        public string PubDate { get; set; }
        public string Country { get; set; }
    }

    public class Title
    {
        public string Text { get; set; }
        public string Language { get; set; }
    }

    public class PartyMember
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
    }
}
