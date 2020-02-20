using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diamond_NZ_Subcodes_1_2_4
{
    public class SubCode1
    {
        public string PublicationNumber { get; set; }
        public List<ApplicantInformation> ApplicantInformation { get; set; } // (71)
        public List<ApplicantInformation> ApplicantInformationNew { get; set; } // (71) new
        public AgentInformation AgentInformation { get; set; } // (74)
        public AgentInformation AgentInformationNew { get; set; } // (74) new
        public string LegalEventDate { get; set; }

    }

    public class SubCode2
    {
        public string PublicationNumber { get; set; }
        public string LegalEventDate { get; set; }
    }

    public class SubCode4
    {
        public string PublicationNumber { get; set; }
        public string LegalEventDate { get; set; }
    }

    public class ApplicantInformation
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; } // country - country code (CC)
    }

    public class AgentInformation
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; } // country - country code (CC) 
    }
}
