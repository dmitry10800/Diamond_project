using System.Collections.Generic;

namespace Integration
{
    /// <summary>
    /// Represents fields for other events where INID codes are not available
    /// </summary>
    public class LegalEvent
    {
        public string Number { get; set; }

        public string Date { get; set; }

        public string Note { get; set; }

        public string Language { get; set; }

        public List<NoteTranslation> Translations { get; set; }

        public LegalEvent()
        {
            Translations = new List<NoteTranslation>();
        }
    }

    public class NoteTranslation
    {
        public string Tr { get; set; }

        public string Type { get; set; }

        public string Language { get; set; }
    }
}