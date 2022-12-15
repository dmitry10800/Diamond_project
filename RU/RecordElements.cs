using System.Collections.Generic;

namespace RU
{
    class RecordElements
    {
        public struct SudCode2
        {
            public string B110 { get; set; } // 11 pub number
            public string B130 { get; set; } // kind code
            public string B140 { get; set; } // pub date
            public string B190 { get; set; } // 19 country code
            public string B210 { get; set; } // 21 patent app number
            public string B220 { get; set; } // 22 patent app date
            public string B405i { get; set; } // LE Note - bulleting number
            public string B460i { get; set; } // LE Date - bulletin publication date
            public List<string> B731i { get; set; }// 73 patent owner (Tag ru-name-text)
            public string B903i { get; set; } // 12 bulletin chapter (SectionCode)
            public string B980i { get; set; } // 73 patent owner address
        }
        public struct SudCode7
        {
            public string B110 { get; set; } // 11 pub number
            public string B130 { get; set; } // kind code
            public string B140 { get; set; } // pub date
            public string B190 { get; set; } // 19 country code
            public string B210 { get; set; } // 21 patent app number
            public string B220 { get; set; } // 22 patent app date
            public string B405i { get; set; } // LE Note - bulleting number
            public string B460i { get; set; } // LE Date - bulletin publication date
            public List<string> B731i { get; set; }// 73 patent owner (Tag ru-name-text)
            public string B903i { get; set; } // 12 bulletin chapter (SectionCode)
            public string B980i { get; set; } // 73 patent owner address
        }
    }
}
