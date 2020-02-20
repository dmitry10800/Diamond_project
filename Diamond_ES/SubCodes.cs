using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diamond_ES
{
    public class SubCodes
    {
    }

    public class SubCode3
    {
        public string PublicationNumber { get; set; }
        public string PublicationKind { get; set; }
        public string ApplicationNumber { get; set; }
        public string ApplicationDate { get; set; }
        public List<RelatedPublicationInformation> RelatedPublicationInformation { get; set; }
        public string TitleText { get; set; }
        public List<string> GranteeAssigneeOwnerInformation { get; set; }
        public List<string> AgentName { get; set; }
        public string LegalStatusEvents_EventDate { get; set; }
        public List<LegalNote> LegalStatusEvents_Note { get; set; }
    }

    public class LegalNote
    {
        public string Note { get; set; }
        public string Language { get; set; }
    }

    public class RelatedPublicationInformation
    {
        public string Number { get; set; }
        public string Date { get; set; }
        public string InidNumber { get; set; }
    }

    public class SubCode11
    {
        public string PublicationNumber { get; set; }
        public string PublicationKind { get; set; }
        public string ApplicationNumber { get; set; }
        public string ApplicationDate { get; set; }
        public string TitleText { get; set; }
        public List<string> GranteeAssigneeOwnerInformation { get; set; }
        public List<string> AgentName { get; set; }
        public string LegalStatusEvents_EventDate { get; set; }
        public List<LegalNote> LegalStatusEvents_Note { get; set; }
    }

    public class SubCode12
    {
        public string PublicationNumber { get; set; }
        public string PublicationKind { get; set; }
        public string ApplicationNumber { get; set; }
        public string ApplicationDate { get; set; }
        public string TitleText { get; set; }
        public List<string> GranteeAssigneeOwnerInformation { get; set; }
        public List<string> AgentName { get; set; }
        public string LegalStatusEvents_EventDate { get; set; }
        public List<LegalNote> LegalStatusEvents_Note { get; set; }


    }
}
