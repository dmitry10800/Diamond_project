using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_BE
{
    class BE_main
    {
        public static DirectoryInfo PathToTetml = new DirectoryInfo(@"C:\Users\Admin\Desktop\BE\");
        public static FileInfo currentFile = null;
        public static string currentFileName = null;

        static void Main(string[] args)
        {
            var files = new List<string>();
            foreach (FileInfo file in PathToTetml.GetFiles("*.tetml", SearchOption.AllDirectories))
                files.Add(file.FullName);

            XElement elem = null;

            List<XElement> paraList = new List<XElement>();
            List<XElement> subCode4List = new List<XElement>();
            List<string> finalList = new List<string>();

            foreach (var file in files)
            {
                currentFile = new FileInfo(file);
                currentFileName = Path.GetFileName(file);
                elem = XElement.Load(file);
                paraList = elem.Descendants().Where(e => e.Name.LocalName == "Text").ToList();
                var tmp = 0;

                for (int i = 0; i < paraList.Count; i++)
                {
                    tmp = i;
                    if (paraList[i].Value.Contains("Contains data concerning lapse , annulment or rejection of patent(s) (applications) and SPC"))
                    {
                        do
                        {
                            subCode4List.Add(paraList[tmp]);
                            tmp++;
                            if (paraList[tmp].Value.Contains("VRB: Contient des données concernant les documents de (demandes de) brevet et de CCP"))
                            {
                                tmp = paraList.Count;
                            }
                        } while (tmp < paraList.Count);
                    }
                }
            }

            if (subCode4List.Count > 0)
            {
                var processedRecords = Processing.SubCode4(subCode4List);
                var legalEvents = ConvertToDiamond.Sub4Convert(processedRecords);
                DiamondSender.SendToDiamond(legalEvents);
            }
        }
    }
}
