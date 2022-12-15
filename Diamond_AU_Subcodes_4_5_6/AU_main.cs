using AU;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_AU_Subcodes_4_5_6
{
    class AU_main
    {
        public static DirectoryInfo PathToTetml = new DirectoryInfo(@"D:\DIAMOND\AU\20200227\AU_20200227_08");
        public static FileInfo currentFile = null;
        public static string currentFileName = null;

        static void Main(string[] args)
        {
            var files = new List<string>();
            foreach (FileInfo file in PathToTetml.GetFiles("*.tetml", SearchOption.AllDirectories))
                files.Add(file.FullName);

            XElement elem = null;

            List<XElement> textList = new List<XElement>();

            foreach (var file in files)
            {
                currentFile = new FileInfo(file);
                currentFileName = Path.GetFileName(file);
                elem = XElement.Load(file);
                var tmp = 0;
                textList = elem.Descendants().Where(e => e.Name.LocalName == "Text").ToList();

                if (textList.Count > 0)
                {
                    //var subCode4 = Processing.SubCode4(textList);
                    //var subCode5 = Processing.SubCode5(textList);
                    var subCode6 = Processing.SubCode6(textList);
                    //var legal4 = DiamondConverter.Sub4Convert(subCode4);
                    //var legal5 = DiamondConverter.Sub5Convert(subCode5);
                    var legal6 = DiamondConverter.Sub6Convert(subCode6);
                    DiamondSender.SendToDiamond(legal6);
                }
            }
        }
    }
}
