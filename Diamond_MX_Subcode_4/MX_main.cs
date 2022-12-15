using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_MX_Subcode_4
{
    class MX_main
    {
        public static DirectoryInfo PathToTetml = new DirectoryInfo(@"D:\DIAMOND\MX\20200302\MX_20200221_01(LT)");
        public static FileInfo currentFile = null;
        public static string currentFileName = null;

        static void Main(string[] args)
        {
            var files = new List<string>();
            foreach (FileInfo file in PathToTetml.GetFiles("*.tetml", SearchOption.AllDirectories))
                files.Add(file.FullName);

            XElement elem = null;

            List<XElement> textList = new List<XElement>();
            List<XElement> subCode4List = new List<XElement>();

            foreach (var file in files)
            {
                currentFile = new FileInfo(file);
                currentFileName = Path.GetFileName(file);
                elem = XElement.Load(file);
                var tmp = 0;
                textList = elem.Descendants().Where(e => e.Name.LocalName == "Text").ToList();

                for (int i = 0; i < textList.Count; i++)
                {
                    tmp = i;
                    if (textList[i].Value == "Cambio de Razón Socia")
                    {
                        do
                        {
                            subCode4List.Add(textList[tmp]);
                            tmp++;
                            if (textList[tmp].Value.Contains("Cambios en Patente"))
                            {
                                tmp = textList.Count;
                            }
                        } while (tmp < textList.Count);
                    }
                }

                if (subCode4List.Count > 0)
                {
                    var processedRecords = Processing.SubCode4(subCode4List);
                    var legalEvents = DiamondConverter.Sub4Convert(processedRecords);
                    DiamondSender.SendToDiamond(legalEvents);
                }
            }
        }
    }
}
