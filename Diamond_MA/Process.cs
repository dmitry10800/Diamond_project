using System.Collections.Generic;
using System.Xml.Linq;

namespace Diamond_MA
{
    class Process
    {
        /*APPLICATIONS WITHDRAWN, REFUSED, TAKEN TO BE ABANDONED*/
        public class Sub1
        {
            private static readonly string I11 = "(11) N° de publication :";
            private static readonly string I21 = "(21) N° Dépôt :";
            private static readonly string I22 = "(22) Date de Dépôt :";
            private static readonly string I30 = "(30) Données de Priorité :";
            private static readonly string I43 = "(43) Date de publication :";
            private static readonly string I51 = "(51) Cl. internationale :";
            private static readonly string I54 = "(54) Titre :";
            public static readonly string I57 = "(57)";
            private static readonly string I71 = "(71) Demandeur(s) :";
            private static readonly string I72 = "(72) Inventeur(s) :";
            private static readonly string I74 = "(74) Mandataire :";
            private static readonly string I86 = "(86) Données relatives à la demande internationale selon le PCT:";
            public static void Run(List<XElement> v)
            {
                List<OutElements.Sub1> elements = new List<OutElements.Sub1>();
            }
        }
        /*PATENTS RENEWED UNDER SECTION 36*/
        public class Sub2
        {
            public static void Run(List<XElement> v)
            {
                List<OutElements.Sub2> elements = new List<OutElements.Sub2>();
            }
        }
    }
}
