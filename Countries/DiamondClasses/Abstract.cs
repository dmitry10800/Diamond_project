using System;
using System.Collections.Generic;

namespace Integration
{
    public class Abstract
    {
        public string Language { get; set; }

        public string Text { get; set; }
        public List<Translation> Translations { get; set; }

        public Abstract()
        {
            this.Translations = new List<Translation>();
        }
    }
}