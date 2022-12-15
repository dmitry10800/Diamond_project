namespace Diamond_BG_Subcode_21
{
    public class Subcode21
    {
        public string PublicationNumber { get; set; }
        public string PublicationKind { get; set; }
        public LegalStatusEvents LegalStatusEvents { get; set; }
    }

    public class LegalStatusEvents
    {
        public string PatentNumber { get; set; }
        public string EventDate { get; set; }
    }
}
