using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_VN
{
    class VN_main
    {
        public static DirectoryInfo PathToTeml = new DirectoryInfo(@"C:\Users\Razrab\Desktop\VN4\");
        public static FileInfo currentFile = null;
        public static string currentFileName = null;

        static void Main(string[] args)
        {
            var files = new List<string>();
            foreach (FileInfo file in PathToTeml.GetFiles("*.tetml", SearchOption.AllDirectories))
                files.Add(file.FullName);

            XElement elem = null;

            List<XElement> subCode12 = new List<XElement>();
            List<XElement> tmpList = new List<XElement>();

            foreach (var file in files)
            {
                currentFile = new FileInfo(file);
                currentFileName = Path.GetFileName(file);
                elem = XElement.Load(file);
                var tmp = 0;
                var text = /*Methods.ConvertText(*/currentFile.OpenText().ReadToEnd().Normalize(System.Text.NormalizationForm.FormKC);//);
                var subcode12Index = text.IndexOf("<Text>Đơn yêu cầu cấp bằng độc quyền Sáng chế</Text>", StringComparison.OrdinalIgnoreCase);
                var subcode13Index = text.IndexOf("<Text>ĐƠN YÊU CầU CấP BằNG giảI pháp hữu ích</Text>", StringComparison.OrdinalIgnoreCase);
                var subcode14Index = text.IndexOf("<Text>Sáng chế được cấp Bằng độc quy", StringComparison.OrdinalIgnoreCase);
                var subcode15Index = text.IndexOf("<Text>Giải pháp hữu ích được cấp Bằng độc quy", StringComparison.OrdinalIgnoreCase);
                var subcode12Text = "";
                var subcode13Text = "";
                var subcode14Text = "";
                var subcode15Text = "";

                //if (subcode12Index > -1)
                //{
                //subcode12Text = text.Substring(subcode12Index);
                if (currentFileName.Replace(".tetml", "").Trim().EndsWith("A"))
                {
                    var matchPhan1 = Regex.Match(text, @"<Text>Ph(ầ|Ç)n\s*I<\/Text>", RegexOptions.IgnoreCase);
                    var matchEnd = Regex.Match(text, @"<Text>Ph(ầ|Ç)n II<\/Text>", RegexOptions.IgnoreCase);
                    var endIndex1 = text.IndexOf(matchEnd.Value);
                    subcode12Text = text.Substring(text.IndexOf(matchPhan1.Value), endIndex1 - text.IndexOf(matchPhan1.Value));

                    var matchPhan = Regex.Match(text, @"<Text>Ph(ầ|Ç)n II<\/Text>", RegexOptions.IgnoreCase);
                    var endMatch1 = Regex.Match(text, @"<Text>Ph(ầ|Ç)n III<\/Text>", RegexOptions.IgnoreCase);
                    var endIndex = text.IndexOf(endMatch1.Value);
                    subcode13Text = text.Substring(text.IndexOf(matchPhan.Value), endIndex - text.IndexOf(matchPhan.Value));
                }



                //}

                //if(subcode13Index > -1)
                //{
                //subcode13Text = text.Substring(subcode13Index);

                //}

                //if(subcode14Index > -1)
                // {
                //subcode14Text = text.Substring(subcode14Index);
                if (currentFileName.Replace(".tetml", "").Trim().EndsWith("B"))
                {
                    var matchPhan2 = Regex.Match(text, @"<Text>Ph(ầ|Ç|Ẩ)n ?I<\/Text>", RegexOptions.IgnoreCase);
                    var matchEnd2 = Regex.Match(text, @"<Text>Ph(ầ|Ç|Ẩ)n ?II<\/Text>", RegexOptions.IgnoreCase);
                    var endIndex2 = text.IndexOf(matchEnd2.Value);
                    subcode14Text = text.Substring(text.IndexOf(matchPhan2.Value), endIndex2 - text.IndexOf(matchPhan2.Value));

                    var matchPhan = Regex.Match(text, @"<Text>Ph(ầ|Ç|Ẩ)n ?II<\/Text>", RegexOptions.IgnoreCase);
                    var matchEnd = Regex.Match(text, @"<Text>Ph(ầ|Ç|Ẩ)n ?(III|ĩĩĩ)<\/Text>", RegexOptions.IgnoreCase);
                    var endIndex = text.IndexOf(matchEnd.Value);
                    subcode15Text = text.Substring(text.IndexOf(matchPhan.Value), endIndex - text.IndexOf(matchPhan.Value));
                }

                //}

                if (subcode15Index > -1)
                {
                    //subcode15Text = text.Substring(subcode15Index);
                    var matchPhan3 = Regex.Match(subcode15Text, @"<Text>Phần ?.{1,3}<\/Text>", RegexOptions.IgnoreCase);
                    var endIndex3 = subcode15Text.IndexOf(matchPhan3.Value);
                    subcode15Text = subcode15Text.Substring(0, endIndex3);
                }

                var matchesTextSub12 = Regex.Matches(subcode12Text, @"<Text>.*?<\/Text>");
                var matchesTextSub13 = Regex.Matches(subcode13Text, @"<Text>.*?<\/Text>");
                var matchesTextSub14 = Regex.Matches(subcode14Text, @"<Text>.*?<\/Text>");
                var matchesTextSub15 = Regex.Matches(subcode15Text, @"<Text>.*?<\/Text>");

                if (matchesTextSub12.Count > 0)
                {
                    //var processedRecords = Processing.SubCode12Process(matchesTextSub12);
                    //var legalEvents = DiamondConverter.Sub12Convert(processedRecords);
                    //DiamondSender.SendToDiamond(legalEvents);
                    //Output.Test(processedRecords);
                    //DiamondSender.SendToDiamond(legalEvents);
                }

                if (matchesTextSub13.Count > 0)
                {
                    var processedRecords = Processing.SubCode13Process(matchesTextSub13);
                    var legalEvents = DiamondConverter.Sub13Convert(processedRecords);
                    DiamondSender.SendToDiamond(legalEvents);
                }

                if (matchesTextSub14.Count > 0)
                {
                    var processedRecords = Processing.SubCode14Process(matchesTextSub14);
                    var legalEvents = DiamondConverter.Sub14Convert(processedRecords);
                    DiamondSender.SendToDiamond(legalEvents);
                }

                //if (matchesTextSub15.Count > 0)
                //{
                //    var processedRecords = Processing.SubCode15Process(matchesTextSub15);
                //    var legalEvents = DiamondConverter.Sub15Convert(processedRecords);
                //    DiamondSender.SendToDiamond(legalEvents);
                //}
            }
        }
    }
}
