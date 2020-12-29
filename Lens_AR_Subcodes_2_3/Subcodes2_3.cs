using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lens_AR_Subcodes_2_3
{
    public class Subcodes2_3
    {
        public string PlainLanguageDesignation { get; set; }
        public string PublicationNumber { get; set; }
        public string PublicationKind { get; set; }
        public string ApplicationNumber { get; set; }
        public string ApplicationDate { get; set; }
        public string EffectiveDate { get; set; }
        public List<LegalStatusEvents> LegalStatusEvents { get; set; }
        public List<PriorityInformation> PriorityInformation { get; set; }
        public string Date_Field47 { get; set; }
        public string Date_Field45 { get; set; }
        public List<Classification_Field51> ClassificationInformation { get; set; }
        public TitleAbstaractInformation Title { get; set; }
        public TitleAbstaractInformation Abstaract { get; set; }
        public List<PersonInformation> ApplicantInformation { get; set; }
        public List<PersonInformation> AgentInformation { get; set; }
        public List<PersonInformation> InventorInformation { get; set; }
    }

    public class PersonInformation
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string CountryCode { get; set; }
    }

    public class TitleAbstaractInformation
    {
        public string Language { get; set; }
        public string Text { get; set; }
    }


    public class Classification_Field51
    {
        public string Classification { get; set; }
        public string IPC_Version { get; set; }
    }

    public class LegalStatusEvents
    {
        public string Language { get; set; }
        public string Note { get; set; }
        public NextLanguageField NextLanguageField { get; set; }
    }

    public class NextLanguageField
    {
        public string Language { get; set; }
        public string Note { get; set; }
    }

    public class PriorityInformation
    {
        public string PriorityNumber { get; set; }
        public string PriorityDate { get; set; }
        public string PriorityCountryCode { get; set; }
    }
}
