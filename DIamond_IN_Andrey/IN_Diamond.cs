using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace DIamond_IN_Andrey
{
    class IN_Diamond
    {
        public static DirectoryInfo PathToTeml = new DirectoryInfo(@"C:\Users\Razrab\Desktop\IN2\");
        public static FileInfo currentFile = null;
        public static string currentFileName = null;

        static void Main(string[] args)
        {
            var files = new List<string>();
            foreach (var file in PathToTeml.GetFiles("*.tetml", SearchOption.AllDirectories))
                files.Add(file.FullName);

            XElement elem = null;

            var tmpList = new List<XElement>();
            var sub1 = new List<XElement>();
            var sub2 = new List<XElement>();
            var sub3 = new List<XElement>();

            foreach (var file in files)
            {
                currentFile = new FileInfo(file);
                currentFileName = Path.GetFileName(file);
                elem = XElement.Load(file);

                tmpList = elem.Descendants().Where(e => e.Name.LocalName == "Text" /*|| e.Name.LocalName == "PlacedImage" || e.Name.LocalName == "Page"*/)
                    .ToList();

                for (var i = 0; i < tmpList.Count; i++)
                {
                    var tmp = i;
                    if (currentFileName.Contains("(1)"))
                    {
                        if (tmpList[tmp].Value.Contains("CONTINUED FROM PART"))
                        {
                            do
                            {
                                sub3.Add(tmpList[tmp]);
                                tmp++;
                                if (tmpList[tmp].Value.Contains("Publication Under Section 43(2)"))
                                    tmp = tmpList.Count - 1;
                            } while (tmp < tmpList.Count - 1);
                        }

                        tmp = i;
                        if (tmpList[i].Value.Contains("Publication Under Section 43(2)"))
                        {
                            do
                            {
                                sub2.Add(tmpList[tmp]);
                                tmp++;
                            } while (tmp < tmpList.Count - 1);
                        }
                    }
                }

                for (var i = 0; i < tmpList.Count; i++)
                {
                    var tmp = i;
                    if (tmpList[i].Value.Contains("Early Publication:"))
                    {
                        do
                        {
                            sub1.Add(tmpList[tmp]);
                            tmp++;
                            if (tmpList[tmp].Value.Contains("Publication After 18 Months:"))
                                tmp = tmpList.Count;
                        } while (tmp < tmpList.Count);
                    }

                    if (tmpList[i].Value.Contains("Publication After 18 Months:"))
                    {
                        do
                        {
                            sub3.Add(tmpList[tmp]);
                            tmp++;
                            if (tmpList[tmp].Value.Contains("PUBLISHED WEEKLY ISSUED FER"))
                                tmp = tmpList.Count;
                        } while (tmp < tmpList.Count - 1);
                    }
                }

                if (sub1.Count > 0)
                {
                    var processedRecords = Processing.BiblioProcess(sub1);
                    var legalEvents = DiamondConverter.BiblioConvert1(processedRecords);
                    //DiamondSender.SendToDiamond(legalEvents);
                }

                if (sub2.Count > 0)
                {
                    var processedRecords = Processing.BiblioProcess2(sub2);
                }

                if (sub3.Count > 0)
                {
                    var processedRecords = Processing.BiblioProcess(sub3);
                    var legalEvents = DiamondConverter.BiblioConvert3(processedRecords);
                    DiamondSender.SendToDiamond(legalEvents);
                }
            }
        }
    }
}
