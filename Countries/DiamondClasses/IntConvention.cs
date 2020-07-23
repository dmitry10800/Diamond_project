using System;
using System.Collections.Generic;

namespace Integration
{
    public class IntConvention
    {
        public string PctNationalDate { get; set; }
        public string PctApplNumber { get; set; }
        public string PctApplKind { get; set; }
        public string PctApplDate { get; set; }
        public string PctApplCountry { get; set; }
        public string PctPublNumber { get; set; }
        public string PctPublKind { get; set; }
        public string PctPublDate { get; set; }
        public string PctPublCountry { get; set; }
        public string PctSearchDate { get; set; }

        public List<string> DesignatedStates { get; set; }
    }
}