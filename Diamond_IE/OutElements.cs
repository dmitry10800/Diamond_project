using System.Collections.Generic;

namespace Diamond_IE
{
    class OutElements
    {
        public class FirstList
        {
            public string PatNumber { get; set; }
            public string IpcVersion { get; set; }
            public string Title { get; set; }
            public List<string> IpcClass { get; set; }
            public string AppName { get; set; }
        }
    }
}
