﻿using System.Collections.Generic;
using System.Xml.Linq;

namespace Diamond_AU
{
    class Process
    {
        /*APPLICATIONS WITHDRAWN, REFUSED, TAKEN TO BE ABANDONED*/
        public class FirstList
        {
            public static List<OutElements.FirstList> Run(List<XElement> v)
            {
                return Methods.GetSplElements(v);
            }
        }
    }
}
