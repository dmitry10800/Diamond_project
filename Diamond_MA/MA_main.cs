using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_MA
{
    class MA_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\MA\Test\");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> sub1Elements = null; // APPLICATIONS WITHDRAWN, REFUSED, TAKEN TO BE ABANDONED
            //List<XElement> sub2Elements = null; // PATENTS RENEWED UNDER SECTION 36


            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                /*TETML elements*/

                /*APPLICATIONS WITHDRAWN, REFUSED, TAKEN TO BE ABANDONED*/
                sub1Elements = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => e.Value != "APPLICATIONS WITHDRAWN, REFUSED, TAKEN TO BE ABANDONED")
                    .TakeWhile(e => e.Value != "APPLICATIONS MADE FOR RESTORATION UNDER SECTION 39")
                    .Where(e => e.Value != ""
                                && e.Value != "This index lists applications in ascending application number order"
                                && e.Value != "APPLICATIONS WITHDRAWN, REFUSED, TAKEN TO BE ABANDONED"
                    )
                    .ToList();

                Console.WriteLine("Lists of elements are prepared");
                /*APPLICATIONS WITHDRAWN, REFUSED, TAKEN TO BE ABANDONED*/
                if (sub1Elements != null && sub1Elements.Count() > 0)
                {
                    Process.Sub1.Run(sub1Elements);
                }
            }
        }
    }
}
