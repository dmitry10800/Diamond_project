using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PL
{
    class PLProcessing
    {
        public const string _pathToTetml = @"D:\_DFA_main\_Patents\PL\Test";
        public static bool _isStaging = true;
        public static FileInfo _currentFileInProcess;
        public static XElement _tet;
        public static List<XElement> _sub10Elements; 
        public static List<XElement> _sub25Elements;
        public static List<XElement> _sub32Elements;
        public static string _chapterBeginning;
        public static string _chapterEnding;

        static void Main()
        {
            var tetmlFiles = Methods.GetTetmlFiles(_pathToTetml);
            foreach (var tetmlFile in tetmlFiles)
            {
                _sub10Elements = new List<XElement>();
                _sub25Elements = new List<XElement>();
                _sub32Elements = new List<XElement>();
                _currentFileInProcess = tetmlFile;
                _tet = XElement.Load(tetmlFile.FullName);
                string currentFileName = Path.GetFileNameWithoutExtension(tetmlFile.FullName);

                _sub32Elements = _tet.Descendants().Where(x => x.Name.LocalName == "Text")
                .SkipWhile(x => x.Value != "Poniższe zestawienie zawiera kolejno: numer wygasłego patentu\neuropejskiego, datę wygaśnięcia oraz zakres wygaśnięcia.") //Sub12 - chapter title of beginning
                .TakeWhile(x => x.Value != "Wpisy i zmiany w rejestrze nieuwzględnione\nw innych samodzielnych ogłoszeniach") //Sub12 - chapter title of ending
                .ToList();
                _sub25Elements = _tet.Descendants().Where(x => x.Name.LocalName == "Text")
                .SkipWhile(x => x.Value != @"§¬n yªu cÇu cÊp b»ng ®éc quyÒn S¸ng chÕ") //Sub12 - chapter title of beginning
                .TakeWhile(x => x.Value != @"PHÇN II") //Sub12 - chapter title of ending
                .ToList();
                _sub10Elements = _tet.Descendants().Where(x => x.Name.LocalName == "Text")
                .SkipWhile(x => x.Value != @"§¬n yªu cÇu cÊp b»ng ®éc quyÒn S¸ng chÕ") //Sub12 - chapter title of beginning
                .TakeWhile(x => x.Value != @"PHÇN II") //Sub12 - chapter title of ending
                .ToList();

                if (_sub32Elements.Count > 0)
                {
                    var sub10records = Subcodes.ProcessSub_32(_sub32Elements, "10", "MK", currentFileName + ".pdf");
                    //DiamondSender.SendToDiamond(sub12records, _isStaging);
                }
                if (_sub25Elements.Count > 0)
                {
                    //var sub13records = Subcodes.ProcessSub_25(_sub25Elements, "25", "TZ", currentFileName + ".pdf");
                    //DiamondSender.SendToDiamond(sub13records, _isStaging);
                }
                if (_sub10Elements.Count > 0)
                {
                    //var sub14records = Subcodes.ProcessSub_32(_sub32Elements, "32", "MZ", currentFileName + ".pdf");
                    //DiamondSender.SendToDiamond(sub14records, _isStaging);
                }
            }
        }
    }
}
