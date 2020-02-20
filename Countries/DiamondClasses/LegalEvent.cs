using System;
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

        /*translations*/
        public List<NoteTranslation> Translations = new List<NoteTranslation>();

    }
}
