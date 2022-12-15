using System.Collections.Generic;

namespace Diamond_RU
{
    class RecordElements
    {
        public struct SubCode
        {
            public string B110 { get; set; }
            public string B110j { get; set; }
            public string B130 { get; set; }
            public string B140 { get; set; }
            public string B190 { get; set; }
            public string B210 { get; set; }
            public string B220 { get; set; }
            public string B236 { get; set; }
            public string B238 { get; set; }
            public string B247i { get; set; }
            public string B405i { get; set; }
            public string B460 { get; set; }
            public string B460i { get; set; }
            public List<string> B711 { get; set; }
            public List<string> B721 { get; set; }
            public List<string> B731 { get; set; }
            public List<string> B731i { get; set; }
            public List<string> B734i { get; set; }
            public string B903i { get; set; }
            public string B908i { get; set; }
            public TitleAndOwner B909i { get; set; }
            public string B919i { get; set; }
            public string B920i { get; set; }
            public string B920ic { get; set; }
            public string B919ic { get; set; }
            public AgentInfo B980i { get; set; }
            public string B994i { get; set; }
            public List<string> B733i { get; set; }
            public string B791 { get; set; }
            public bool isField73 { get; set; }
            public Field12 Field12 { get; set; }
        }

        public class AgentInfo
        {
            public string Address { get; set; }
            public string Name { get; set; }
        }

        public class Field12
        {
            public string versionRU { get; set; }
            public string versionEN { get; set; }
        }

        public class TitleAndOwner
        {
            public string Title { get; set; }
            public string Patent_Owner { get; set; }
        }
    }
}
