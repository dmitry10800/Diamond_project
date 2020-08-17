using System;
using Integration;

namespace Diamond.Core.Models
{
    /// <summary>
    /// Represents one event which is keyed by operator. 
    /// It can be a patent (an application, a grant, etc) or other event (invalidation, fee paid, etc)
    /// </summary>
    public class LegalStatusEvent
    {
        public string SubCode { get; set; }//+

        public string SectionCode { get; set; }//+

        public Biblio Biblio { get; set; }

        public Biblio NewBiblio { get; set; }

        public LegalEvent LegalEvent { get; set; }

        public string CountryCode { get; set; }

        public int Id { get; set; }

        public string GazetteName { get; set; }
    }
}
