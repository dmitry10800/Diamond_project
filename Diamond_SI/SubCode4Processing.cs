using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diamond_SI
{
    public class SubCode4Processing
    {
        public static Subcode4 ProcessSubCode4Element(string[] array)
        {
            Subcode4 subcode4 = new Subcode4();
            for (int i = 0; i < array.Length; i++)
            {
                if (i == 0)
                    subcode4.PublicationNumber = array[0];

                if (i == 1)
                    subcode4.DateField46 = Methods.DateNormalize(array[1]);

                if (i == 2)
                    subcode4.LegalEventDate = Methods.DateNormalize(array[2]);
            }
            return subcode4;
        }
    }
}
