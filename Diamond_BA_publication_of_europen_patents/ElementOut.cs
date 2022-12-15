using System.Collections.Generic;

namespace Diamond_BA_publication_of_europen_patents
{
    public class ElementOut
    {
        public string I11 { get; set; }
        public string I21 { get; set; }
        public string I22 { get; set; }
        public string I26 { get; set; }
        public string[] I31 { get; set; }
        public string[] I32 { get; set; }
        public string[] I33 { get; set; }
        public string[] I51N { get; set; }
        public string[] I51D { get; set; }
        public string I54 { get; set; }
        public string I57 { get; set; }
        public string[] I72N { get; set; }
        public string[] I72A { get; set; }
        public string[] I72C { get; set; }
        public string[] I73N { get; set; }
        public string[] I73A { get; set; }
        public string[] I73C { get; set; }
        public string I74N { get; set; }
        public string I74A { get; set; }
        public string I74C { get; set; }
        public string I96N { get; set; }
        public string I96D { get; set; }
        public string I97D { get; set; }
        public static List<ElementOut> ElementsOut = new List<ElementOut>();
    }
}
