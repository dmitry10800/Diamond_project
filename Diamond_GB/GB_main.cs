using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_GB
{
    class GB_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\GB\20200206");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> sub8 = null; // Applications Terminated before Publication under Section 16(1)
            List<XElement> sub9 = null; // Applications Terminated after Publication under Section 16(1)
            List<XElement> sub10 = null; // European Patents Revoked
            List<XElement> sub11 = null; // European Patents Expired
            List<XElement> sub12 = null; // UK Patents Expired
            List<XElement> sub42 = null; // European Patents Ceased
            List<XElement> sub43 = null; // UK Patents Ceased


            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                /*TETML elements*/

                /*Applications Terminated before Publication under Section 16(1)*/
                sub8 = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => e.Value != "Applications Terminated before Publication under Section 16(1)")
                    .TakeWhile(e => e.Value != "Applications Terminated after Publication under Section 16(1)" && e.Value != "Corrections" && e.Value != "Correction")
                    .Where(e => e.Value != ""
                    && e.Value != "This index lists applications which have been withdrawn, taken to be withdrawn, treated as having been withdrawn, refused,\nor treated as having been refused, before publication under Section 16(1)."
                    && e.Value != "Applications Terminated before Publication under Section 16(1)"
                    && e.Value != "Applications Terminated before Publication under Section 16(1) - cont"
                    && e.Value != "Applications Terminated after Publication under Section 16(1)"
                    && e.Value != "Applications Terminated before Publication under Section 16(1) - cont."
                    )
                    .ToList();

                /*Applications Terminated after Publication under Section 16(1)*/
                sub9 = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => e.Value != "Applications Terminated after Publication under Section 16(1)")
                    .TakeWhile(e => e.Value != "Applications Published" && e.Value != "Correction")
                    .Where(e => e.Value != ""
                    && e.Value != "This index lists applications which have been withdrawn, treated as having been withdrawn, refused, or treated as having\nbeen refused, after publication under Section 16(1)."
                    && e.Value != "Applications Published"
                    && e.Value != "Applications Terminated after Publication under Section 16(1)"
                    && e.Value != "Applications Terminated after Publication under Section 16(1) - cont."
                    )
                    .ToList();

                /*European Patents Revoked*/
                sub10 = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => e.Value != "European Patents Revoked")
                    .TakeWhile(e => e.Value != "European Patents Expired" && e.Value != "European Patents Ceased")
                    .Where(e => e.Value != ""
                    && !e.Value.StartsWith("Each entry is for European Patents designating the United Kingdom as a Contracting")
                    && e.Value != "European Patents Revoked"
                    && e.Value != "European Patents Expired"
                    )
                    .ToList();
                /*European Patents Expired*/
                sub11 = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => e.Value != "European Patents Expired")
                    .TakeWhile(e => e.Value != "UK Patents Expired" && e.Value != "UK Patents Ceased")
                    .Where(e => e.Value != ""
                    && e.Value != "Each entry shows European Patents designating the United Kingdom as a Contracting State which has expired after\ntermination of 20 years, the date of expiry is given."
                    && e.Value != "UK Patents Expired"
                    && e.Value != "European Patents Expired"
                    )
                    .ToList();
                /*UK Patents Expired */
                sub12 = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => e.Value != "UK Patents Expired")
                    .TakeWhile(e => e.Value != "Other Proceedings under the Patents Act 1977" && e.Value != "European Patents Ceased")
                    .Where(e => e.Value != ""
                    && !e.Value.StartsWith("Other Proceedings under the Patents Act 1977")
                    && e.Value != "This Section gives details of all"
                    && e.Value != "UK Patents Expired"
                    && e.Value != "This index lists UK patents which have expired after termination of 20 years."
                    )
                    .ToList();
                /*European Patents Ceased*/
                sub42 = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => e.Value != "European Patents Ceased")
                    .TakeWhile(e => e.Value != "European Patents Expired")
                    .Where(e => e.Value != ""
                    && !e.Value.StartsWith("Each entry is for European Patents designating the United Kingdom as a Contracting")
                    && e.Value != "European Patents Expired"
                    && e.Value != "European Patents Ceased - cont."
                    && e.Value != "European Patents Ceased")
                    .ToList();

                /*UK Patents Ceased*/
                sub43 = tet.Descendants().Where(d => (d.Name.LocalName == "Text"))
                    .SkipWhile(e => e.Value != "UK Patents Ceased")
                    .TakeWhile(e => e.Value != "UK Patents Expired" && e.Value != "Other Proceedings under the Patents Act 1977")
                    .Where(e => e.Value != ""
                    && e.Value != "This index lists UK patents which have ceased through non-payment of renewal fees."
                    && e.Value != "European Patents Expired"
                    && e.Value != "UK Patents Ceased - cont."
                    && e.Value != "European Patents Ceased"
                    && e.Value != "UK Patents Ceased"
                    )
                    .ToList();

                Console.WriteLine($"Sub 8 elements count: {sub8.Count}");
                Console.WriteLine($"Sub 9 elements count: {sub9.Count}");
                Console.WriteLine($"Sub 10 elements count: {sub10.Count}");
                Console.WriteLine($"Sub 11 elements count: {sub11.Count}");
                Console.WriteLine($"Sub 12 elements count: {sub12.Count}");
                Console.WriteLine($"Sub 42 elements count: {sub42.Count}");
                Console.WriteLine($"Sub 43 elements count: {sub43.Count}");
                Console.WriteLine("Lists of elements are prepared");

                /*Applications Terminated before Publication under Section 16(1)*/
                if (sub8 != null && sub8.Count() > 0)
                {
                    Process.Sub8.Run(sub8);
                }
                /*Applications Terminated after Publication under Section 16(1)*/
                if (sub9 != null && sub9.Count() > 0)
                {
                    Process.Sub9.Run(sub9);
                }
                /*European Patents Revoked*/
                if (sub10 != null && sub10.Count() > 0)
                {
                    Process.Sub10.Run(sub10);
                }
                /*European Patents Expired*/
                if (sub11 != null && sub11.Count() > 0)
                {
                    Process.Sub11.Run(sub11);
                }
                /*UK Patents Expired*/
                if (sub12 != null && sub12.Count() > 0)
                {
                    Process.Sub12.Run(sub12);
                }
                /*European Patents Ceased*/
                if (sub42 != null && sub42.Count() > 0)
                {
                    //Process.Sub42.Run(sub42);
                }
                /*UK Patents Ceased*/
                if (sub43 != null && sub43.Count() > 0)
                {
                    //Process.Sub43.Run(sub43);
                }
            }
        }
    }
}
