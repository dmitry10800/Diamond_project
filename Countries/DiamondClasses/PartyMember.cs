using System.Collections.Generic;

namespace Integration
{
    public class PartyMember
    {
        public string Language { get; set; }

        public string Name { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address1 { get; set; }

        public string PostCode { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public List<Translation> Translations { get; set; }
    }
}