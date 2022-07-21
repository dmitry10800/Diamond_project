using System;
using System.Collections.Generic;
using System.Text;
using Integration;

namespace DiamondProjectClasses
{
    public class LicenseInformation
    {
        public List<PartyMember> LicensorInformation { get; set; }
        public List<LicenseeInformation> LicenseeInformation { get; set; }

        public LicenseInformation()
        {
            LicenseeInformation = new List<LicenseeInformation>();
            LicensorInformation = new List<PartyMember>();
        }
    }
}
