using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_MK_Andrey
{
    class MK_Processing
    {
        public const string _pathToTetml = @"C:\Users\andre\Desktop\MK\";
        public static List<XElement> _sub1Elements;
        public static XElement _tet;
        public static string _currentFileName;

        static void Main(string[] args)
        {
            var tetmlFiles = Methods.GetTetmlFiles(_pathToTetml);
            foreach (var tetml in tetmlFiles)
            {
                _sub1Elements = new List<XElement>();
                _tet = XElement.Load(tetml.FullName);
                _currentFileName = tetml.FullName;
                var currentFileName = Path.GetFileNameWithoutExtension(tetml.FullName);

                _sub1Elements = _tet.Descendants().Where(x => x.Name.LocalName == "Text")
                    .SkipWhile(x => !x.Value.Contains("(51) C 07J 1/00, C 07J 7/00, C 07J 51/00, C"))
                    .TakeWhile(x => !x.Value.Contains("ПРЕГЛЕД НА ПАТЕНТИ СПОРЕД МЕЃУНАРОДНАТА КЛАСИФИКАЦИЈА НА"))
                    .ToList();

                if (_sub1Elements.Count > 0)
                {
                    var processedRecords = Subcodes.Sub3Process(_sub1Elements);
                    var legalEvents = ConvertToDiamond.ConvertSub3(processedRecords);
                    Methods.SendToDiamond(legalEvents);
                }
            }
        }
    }
}
