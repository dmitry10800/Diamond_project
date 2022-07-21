using DiamondProjectClasses;
using System;
using System.Collections.Generic;

namespace Integration
{
    public class Biblio
    {
        public Publication Publication { get; set; }

        public Application Application { get; set; }

        public List<Priority> Priorities { get; set; }

        public DOfPublication DOfPublication { get; set; }

        public List<Title> Titles { get; set; }

        public List<Abstract> Abstracts { get; set; }

        public List<PartyMember> Applicants { get; set; }

        public List<PartyMember> Inventors { get; set; }

        public List<PartyMember> Assignees { get; set; }

        public List<PartyMember> Agents { get; set; }

        public List<PartyMember> InvOrApps { get; set; }

        public List<PartyMember> InvAppGrants { get; set; }

        public List<Ipc> Ipcs { get; set; }

        public List<Ipcr> Ipcrs { get; set; }

        public List<Cpc> Cpcs { get; set; }

        public IntConvention IntConvention { get; set; }

        public List<PatentCitation> PatentCitations { get; set; }

        public List<NonPatentCitation> NonPatentCitations { get; set; }

        public List<RelatedDocument> Related { get; set; }

        public List<EuropeanPatent> EuropeanPatents { get; set; }

        public Spc Spc { get; set; }

        public List<Claim> Claims { get; set; }

        public List<ScreenShot> ScreenShots { get; set; }

        public List<AbstractImage> Images { get; set; }

        public Biblio()
        {
            Publication = new Publication();
            Application = new Application();
            Priorities = new List<Priority>();
            Titles = new List<Title>();
            Abstracts = new List<Abstract>();
            Claims = new List<Claim>();
            Applicants = new List<PartyMember>();
            Inventors = new List<PartyMember>();
            Agents = new List<PartyMember>();
            Ipcs = new List<Ipc>();
            Ipcrs = new List<Ipcr>();
            Cpcs = new List<Cpc>();
            IntConvention = new IntConvention();
            Assignees = new List<PartyMember>();
            //DesignatedStates = new List<string>();
            PatentCitations = new List<PatentCitation>();
            NonPatentCitations = new List<NonPatentCitation>();
            Related = new List<RelatedDocument>();
            EuropeanPatents = new List<EuropeanPatent>();
        }
    }

    public class Spc
    {
        public string Date { get; set; }

        public string Number { get; set; }

        public string Patent { get; set; }

        public string ExpiredDate { get; set; }
    }

    public class EuropeanPatent
    {
        public string AppNumber { get; set; }

        public string AppDate { get; set; }

        public string AppKind { get; set; }

        public string AppCountry { get; set; }

        public string PubNumber { get; set; }

        public string PubDate { get; set; }

        public string PubKind { get; set; }

        public string PubCountry { get; set; }

        public string Date { get; set; }

        public string Number { get; set; }

        public string Patent { get; set; }

        public string Spc92Number { get; set; }

        public string Spc92Date { get; set; }

        public string Spc92Country { get; set; }

        public string SpcDate { get; set; }

        public string ExpiredDate { get; set; }

        public string Country { get; set; }
    }

    public class DOfPublication
    {
        public string date_41 { get; set; }

        public string date_42 { get; set; }

        public string date { get; set; }

        public string date_44 { get; set; }

        public string date_45 { get; set; }

        public string date_46 { get; set; }

        public string date_47 { get; set; }

        public string date_48 { get; set; }
    }

    public class AbstractImage
    {
        public string Id { get; set; }

        public string Data { get; set; }
    }

    public class ScreenShot
    {
        public string Id { get; set; }

        public string Data { get; set; }
    }
}