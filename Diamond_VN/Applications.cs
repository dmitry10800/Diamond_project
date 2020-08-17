using System;
using System.Collections.Generic;
using System.Text;

namespace Diamond_VN
{
    class Applications
    {
        public string PubNumber { get; set; }
        public string PubDate { get; set; }
        public string Date43 { get; set; }
        public string Date45 { get; set; }
        public string AppNumber { get; set; }
        public string AppDate { get; set; }
        public string Title { get; set; }
        public string EventDate { get; set; }
        public string Note { get; set; }
        public string Date85 { get; set; }
        public string Translation { get; set; }
        public PCT84 Pct84 { get; set; }
        public PCT85 Pct85 { get; set; }
        public PCT86 Pct86 { get; set; }
        public PCT87 Pct87 { get; set; }
        public Abstract Abstract { get; set; }
        public List<ClassificationIpc> ClassificationIpcs { get; set; }
        public List<PartyMember> Inventors { get; set; }
        public List<PartyMember> Agents { get; set; }
        public List<PartyMember> Applicants { get; set; }
        public List<Priority> Priorities { get; set; }
        public List<PartyMember> GrantAssigOwner { get; set; }
        public List<PartyMember> InvAppGrant { get; set; }
        public List<PartyMember> InvOrApp { get; set; }
        public List<Related> Related { get; set; }
    }

    public class Related
    {
        public string Number { get; set; }
        public string Inid { get; set; }
    }

    public class PartyMember
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string TransName { get; set; }
        public string TransCountry { get; set; }
        public string Language { get; set; }
    }

    public class ClassificationIpc
    {
        public int? Edition { get; set; }
        public string Classification { get; set; }
    }

    public class Abstract
    {
        public string Language { get; set; }
        public string Text { get; set; }
    }

    public class PCT84
    {
        public string PubNumber { get; set; }
        public string PubDate { get; set; }
    }
    public class PCT85
    {
        public string PubNumber { get; set; }
        public string PubDate { get; set; }
    }
    public class PCT86
    {
        public string AppNumber { get; set; }
        public string AppDate { get; set; }
    }
    public class PCT87
    {
        public string PubNumber { get; set; }
        public string PubDate { get; set; }
        public string Kind { get; set; }
    }

    public class Priority
    {
        public string Number { get; set; }
        public string Date { get; set; }
        public string Country { get; set; }
    }
}
