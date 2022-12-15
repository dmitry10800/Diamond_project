using Integration;
using System.Collections.Generic;

namespace DiamondProjectClasses
{
    public class Claim
    {
        public string Language { get; set; }
        public string Number { get; set; }
        public string Text { get; set; }
        public List<Translation> Translations { get; set; }
        public List<ScreenShot> ScreenShots { get; set; }

        public Claim()
        {
            Translations = new List<Translation>();
            ScreenShots = new List<ScreenShot>();
        }
    }
}
