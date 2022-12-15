using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_RS
{
    class RS_main
    {
        public static DirectoryInfo PathToTeml = new DirectoryInfo(@"C:\Users\Razrab\Desktop\RS\");
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

            foreach (var file in files)
            {
                currentFile = new FileInfo(file);
                currentFileName = Path.GetFileName(file);
                elem = XElement.Load(file);

                tmpList = elem.Descendants().Where(e => e.Name.LocalName == "Text")
                    .ToList();

                for (int i = 0; i < tmpList.Count; i++)
                {
                    var tmp = i;
                    if (tmpList[tmp].Value.Contains("РЕГИСТРОВАНИ ПА ТЕНТИ / Patents granted"))
                    {
                        do
                        {
                            sub3.Add(tmpList[tmp]);
                            tmp++;
                            if (tmpList[tmp].Value.Contains("OБЈАВА ПАТЕНАТА"))
                                tmp = tmpList.Count;
                        } while (tmp < tmpList.Count - 1);
                    }
                }

                if (sub3.Count > 0)
                {
                    var processedRecords = Processing.Sub3Process(sub3);
                }
            }
        }
    }
}
