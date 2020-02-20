using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diamond_SI
{
    public class SubCode3Processing
    {
        public static Subcode3 ProcessSubCode3Element(string[] array)
        {
            Subcode3 subcode3 = new Subcode3();
            for (int i = 0; i < array.Length; i++)
            {
                if (i == 0)
                    subcode3.PublicationNumber = array[0];

                if (i == 1)
                    subcode3.DateField45 = Methods.DateNormalize(array[1]);

                if (i == 2)
                    subcode3.LegalEventDate = Methods.DateNormalize(array[2]);

            }
            return subcode3;
        }
    }
}
