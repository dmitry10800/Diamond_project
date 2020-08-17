using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Diamond_AR_Subcodes_2_3
{
    class AR_main
    {
        public static DirectoryInfo PathToTeml = new DirectoryInfo(@"C:\Users\Razrab\Desktop\AR\");
        public static FileInfo currentFile = null;
        public static string currentFileName = null;

        static void Main(string[] args)
        {
            var files = new List<string>();
            foreach (FileInfo file in PathToTeml.GetFiles("*.tetml", SearchOption.AllDirectories))
                files.Add(file.FullName);

            XElement elem = null;

            List<XElement> tmpList = new List<XElement>();
            List<XElement> sub3 = new List<XElement>();
            List<XElement> sub2 = new List<XElement>();

            foreach (var file in files)
            {
                currentFile = new FileInfo(file);
                currentFileName = Path.GetFileName(file);
                elem = XElement.Load(file);

                tmpList = elem.Descendants().Where(e => e.Name.LocalName == "Text")
                    .ToList();


                if (tmpList.Count > 0)
                {
                    var processedRecords = Processing.Sub2
                        (tmpList);
                    var legalEvents = DiamondConverter.Sub2And3(processedRecords);
                    DiamondSender.SendToDiamond(legalEvents);
                }
            }
        }
    }
}
