using Integration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiamondProjectClasses
{
    public class Claim
    {
        public string Language { get; set; }
        public string Number { get; set; }
        public string Text { get; set; }
        public List<Translation> Translations { get; set; }
        public List<ScreenShot> ScreenShots { get; set; }
    }
}
