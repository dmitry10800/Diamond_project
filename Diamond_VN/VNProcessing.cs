using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_VN
{
    class VNProcessing
    {
        public const string _pathToTetml = @"D:\_DFA_main\_Patents\VN\20201130";
        public static bool _isStaging = false; //если false, то данные будут загружены в продакшн, если true - то данные будут загружены на стэйджинг
        public static FileInfo _currentFileInProcess;
        public static XElement _tet;
        public static List<XElement> _sub12Elements;
        public static List<XElement> _sub13Elements;
        public static List<XElement> _sub14Elements;
        public static List<XElement> _sub15Elements;
        public static string _chapterBeginning;
        public static string _chapterEnding;

        static void Main()
        {
            var tetmlFiles = Methods.GetTetmlFiles(_pathToTetml);
            foreach (var tetmlFile in tetmlFiles)
            {
                _sub12Elements = new List<XElement>();
                _sub13Elements = new List<XElement>();
                _sub14Elements = new List<XElement>();
                _sub15Elements = new List<XElement>();
                _currentFileInProcess = tetmlFile;
                _tet = XElement.Load(tetmlFile.FullName);
                string currentFileName = Path.GetFileNameWithoutExtension(tetmlFile.FullName);

                if (currentFileName.EndsWith("A"))
                {
                    _sub12Elements = _tet.Descendants().Where(x => x.Name.LocalName == "Text")
                        .SkipWhile(x => x.Value != @"§¬n yªu cÇu cÊp b»ng ®éc quyÒn S¸ng chÕ"
                            && x.Value != "§¬n yªu cÇu cÊp b»ng ®éc quyÒn s¸ng chÕ"
                            && x.Value != "§¬n yªu cÇu ®−îc cÊp b»ng ®éc quyÒn s¸ng chÕ"
                            && x.Value != "§¬n yªu cÇu ®−îc cÊp b»ng ®éc quyÒn s¸ng chÕ"
                            && x.Value != "§¥N Y£U CÇU CÊP B»NG §éc quyÒn s¸ng chÕ") //Sub12 - chapter title of beginning
                        .TakeWhile(x => x.Value != @"PHÇN II"
                            && x.Value != "PHÇN II"
                            && x.Value != "§¥N Y£U CÇU CÊP B»NG ®éc quyÒn gi¶I ph¸p h÷u Ých"
                            && x.Value != @"§¥N Y£U CÇU CÊP B»NG gi¶I ph¸p h÷u Ých") //Sub12 - chapter title of ending
                        .ToList();
                    _sub13Elements = _tet.Descendants().Where(x => x.Name.LocalName == "Text")  //Sub13 - chapter title of beginning
                        .SkipWhile(x => x.Value != @"§¥N Y£U CÇU CÊP B»NG ®éc quyÒn gi¶I ph¸p h÷u Ých")  //Sub13 - chapter title of ending
                        .TakeWhile(x => x.Value != @"PhÇn iii" &&
                        x.Value != @"Y£U CÇU thÈm ®Þnh NéI DUNG" &&
                        x.Value != @"PHẦN III")
                        .ToList();
                }
                else if (currentFileName.EndsWith("B"))
                {
                    _sub14Elements = _tet.Descendants().Where(x => x.Name.LocalName == "Text")
                        .SkipWhile(x => x.Value != @"S¸ng chÕ ®−îc cÊp b»ng ®éc quyÒn"
                        && x.Value != "SÁNG CHẾ Đ¦ỢC CẤP BẰNG ĐỘC QUYỀN"
                        && x.Value != "S¸NG CHÕ ®−îc cÊp b»ng ®éc quyÒn") //Sub14 - chapter title of beginning
                        .TakeWhile(x => x.Value != @"PhÇn iI"
                        && x.Value != @"Gi¶I ph¸p h÷u Ých ®−îc cÊp b»ng ®éc quyÒn"
                        && x.Value != @"PHÇN II"
                        && x.Value != "PHẦN II") //Sub14 - chapter title of ending
                        .ToList();
                    _sub15Elements = _tet.Descendants().Where(x => x.Name.LocalName == "Text")
                        .SkipWhile(x => x.Value != @"Gi¶I ph¸p h÷u Ých ®−îc cÊp b»ng ®éc quyÒn"
                        && x.Value != "GIẢI PHÁP HỮU ÍCH ĐƯỢC CẤP BẰNG ĐỘC QUYỀN"
                        && x.Value != "GI¶I PH¸P H÷U ÝCH ®−îc cÊp b»ng ®éc quyÒn") //Sub15 - chapter title of beginning
                        .TakeWhile(x => x.Value != @"PHÇN II" &&
                        x.Value != "PHẦN II" &&
                        x.Value != @"§¥N Y£U CÇU CÊP B»NG gi¶I ph¸p h÷u Ých" &&
                        x.Value != @"PhÇn iii" &&
                        x.Value != @"PhÇn iii" &&
                        x.Value != @"PHẦN III") //Sub15 - chapter title of ending
                        .ToList();
                }
                else
                {
                    Console.WriteLine($"Wrong pdf name format of {tetmlFile.FullName} -> filename should end with letter A or letter B");
                }

                if (_sub12Elements.Count > 0)
                {
                    var sub12records = Subcodes.ProcessSubCodes(_sub12Elements, "12", "AA", currentFileName + ".pdf");
                    DiamondSender.SendToDiamond(sub12records, _isStaging);
                }
                if (_sub13Elements.Count > 0)
                {
                    var sub13records = Subcodes.ProcessSubCodes(_sub13Elements, "13", "AA", currentFileName + ".pdf");
                    DiamondSender.SendToDiamond(sub13records, _isStaging);
                }
                if (_sub14Elements.Count > 0)
                {
                    var sub14records = Subcodes.ProcessSubCodes(_sub14Elements, "14", "FG", currentFileName + ".pdf");
                    DiamondSender.SendToDiamond(sub14records, _isStaging);
                }
                if (_sub15Elements.Count > 0)
                {
                    var sub15records = Subcodes.ProcessSubCodes(_sub15Elements, "15", "FG", currentFileName + ".pdf");
                    DiamondSender.SendToDiamond(sub15records, _isStaging);
                }
            }
        }
    }
}