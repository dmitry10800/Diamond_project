using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_SG
{
    class Process
    {
        /*APPLICATIONS WITHDRAWN, REFUSED, TAKEN TO BE ABANDONED*/
        public class FirstList
        {
            public static List<OutElements.FirstList> Run(List<XElement> v)
            {
                List<OutElements.FirstList> elements = new List<OutElements.FirstList>();
                var str = Methods.XElemToString(v);
                var splStr = str.Replace("\n", " ").Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var rec in splStr)
                {
                    elements.Add(new OutElements.FirstList { AppNumber = rec, LePatNumber = rec });
                }
                return elements;
            }
        }
        /*PATENTS RENEWED UNDER SECTION 36*/
        public class SecondList
        {
            public static List<OutElements.SecondList> Run(List<XElement> v)
            {
                List<OutElements.SecondList> elements = new List<OutElements.SecondList>();
                var str = Methods.XElemToString(v);
                var spl = Methods.SplitByNumberAndName(str);
                return Methods.GetSplElements(spl);
            }
        }
    }
}
