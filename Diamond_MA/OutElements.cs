using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diamond_MA
{
    class OutElements
    {
        public struct Priority
        {
            public string I31 { get; set; }
            public string I32 { get; set; }
            public string I33 { get; set; }
        }
        public struct IntClass
        {
            public string I51Number { get; set; }
            public string I51Date { get; set; }
        }
        public struct PersonalData
        {
            public string Name { get; set; }
            public string Address { get; set; }
            public string Country { get; set; }
        }
        public struct PctAndEpData
        {
            public string Number { get; set; }
            public string Date { get; set; }
        }
        public class Sub1
        {
            public string I11 { get; set; }
            public string I13 { get; set; }
            public string I21 { get; set; }
            public string I22 { get; set; }
            public Priority PRIO { get; set; }
            public List<IntClass> IntClasses { get; set; }
            public string I43 { get; set; }
            public string I54 { get; set; }
            public string I57 { get; set; }
            public List<PersonalData> I71 { get; set; }
            public List<string> I72 { get; set; }
            public List<PersonalData> I73 { get; set; }
            public string I74 { get; set; }
            public PctAndEpData I86 { get; set; }
            public PctAndEpData I87 { get; set; }
            public PctAndEpData I96 { get; set; }
            public PctAndEpData I97 { get; set; }
        }

        public class Sub2
        {
            public string AppNumber { get; set; }
            public string OwnerName { get; set; }
            public string LePatNumber { get; set; }
        }
    }
}
