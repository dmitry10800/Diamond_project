using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_IE
{
    class Process
    {
        public class FirstList
        {
            public static List<OutElements.FirstList> Run(List<XElement> v)
            {
                List<OutElements.FirstList> elements = new List<OutElements.FirstList>();
                var str = Methods.XElemToString(v);
                var spl = Methods.SplitByNumberAndName(str);
                return Methods.GetSplElements(spl);
            }
        }
    }
}
