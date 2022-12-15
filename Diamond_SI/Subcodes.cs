using System.Collections.Generic;

namespace Diamond_SI
{
    public class Subcode3
    {
        public string PublicationNumber { get; set; }
        public string DateField45 { get; set; }
        public string LegalEventDate { get; set; }
    }

    public class Subcode4
    {
        public string PublicationNumber { get; set; }
        public string DateField46 { get; set; }
        public string LegalEventDate { get; set; }
    }

    public class Subcode20
    {
        public List<string> Classification { get; set; } 
        public string PublicationNumber { get; set; }
        public string PublicationKind { get; set; }
        public string ApplicationNumber { get; set; }
        public string ApplicationDate { get; set; }
        //public List<Field_86> Field_86 { get; set; }
        //public List<Field_87> Field_87 { get; set; }
        //public List<Field_96> Field_96 { get; set; }
        //public List<Field_97> Field_97 { get; set; }

        public List<Fields_86_87_96_97> Field_86 { get; set; }
        public List<Fields_86_87_96_97> Field_87 { get; set; }
        public List<Fields_86_87_96_97> Field_96 { get; set; }
        public List<Fields_86_87_96_97> Field_97 { get; set; }

        public string DateField46 { get; set; }
        public List<PriorityInformation> PriorityInformation { get; set; }
        public List<PersonInformation> InventorInformation { get; set; }
        public List<PersonInformation> Grantee_Assignee_OwnerInformation { get; set; }
        public List<PersonInformation> AgentInformation { get; set; }
        public Title Title { get; set; }

        public Subcode20()
        {
            Classification = new List<string>();
            //Field_86 = new List<Field_86>();
            //Field_87 = new List<Field_87>();
            //Field_96 = new List<Field_96>();
            //Field_97 = new List<Field_97>();

            Field_86 = new List<Fields_86_87_96_97>();
            Field_87 = new List<Fields_86_87_96_97>();
            Field_96 = new List<Fields_86_87_96_97>();
            Field_97 = new List<Fields_86_87_96_97>();
            PriorityInformation = new List<PriorityInformation>();
            InventorInformation = new List<PersonInformation>();
            Grantee_Assignee_OwnerInformation = new List<PersonInformation>();
            AgentInformation = new List<PersonInformation>();
        }

    }

    public class Fields_86_87_96_97
    {
        public string Number { get; set; }
        public string Date { get; set; }
        public string Country { get; set; }
        public string kind { get; set; }
    }

    public class Field_86
    {
        public string PCT_ApplicationNumber { get; set; }
        public string PCT_ApplicationDate { get; set; }
    }

    public class Field_87
    {
        public string PublicationNumber { get; set; }
        public string PublicationDate { get; set; }
    }

    public class Field_96
    {
        public string ApplicationCountry { get; set; }
        public string ApplicationNumber { get; set; }
        public string ApplicationDate { get; set; }
    }

    public class Field_97
    {
        public string PublicationCountry { get; set; }
        public string PublicationNumber { get; set; }
        public string PublicationKind { get; set; }
        public string PublicationDate { get; set; }
    }

    public class PriorityInformation
    {
        public string PriorityNumber { get; set; }
        public string PriorityDate { get; set; }
        public string PriorityCountryCode { get; set; }
    }

    public class PersonInformation
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
    }

    public class Title
    {
        public string Language { get; set; }
        public string Text { get; set; }
    }
}
