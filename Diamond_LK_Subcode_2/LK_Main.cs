using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_LK_Subcode_2
{
    class LK_Main
    {
        public static DirectoryInfo PathToTetml = new DirectoryInfo(@"C:\Users\Razrab\Desktop\LK\");
        public static FileInfo currentFile = null;
        public static Regex pattern = new Regex(@"\d{5} \d{2}(\.|\/)\d{2}(\.|\/)\d{4}");

        static void Main(string[] args)
        {
            var files = new List<string>();
            foreach (var file in PathToTetml.GetFiles("*.tetml", SearchOption.AllDirectories))
                files.Add(file.FullName);

            XElement elem = null;

            var finalParaList = new List<XElement>();
            var paraList = new List<XElement>();

            var myList = new List<string>();

            foreach (var file in files)
            {
                currentFile = new FileInfo(file);
                elem = XElement.Load(file);
                paraList = elem.Descendants().Where(e => e.Name.LocalName == "Para").ToList();
                var tmp = 0;

                for (var i = 0; i < paraList.Count; i++)
                {
                    tmp = i;
                    if (paraList[i].Value.Contains("Patents Publication of Grant"))
                    {
                        do
                        {
                            finalParaList.Add(paraList[tmp]);
                            tmp++;
                            if (paraList[tmp].Value.Contains("INTELLECTUAL PROPERTY"))
                            {
                                tmp = paraList.Count;
                            }
                        } while (tmp < paraList.Count);
                    }
                }

                for (var i = 0; i < finalParaList.Count; i++)
                {
                    if (pattern.Match(finalParaList[i].ToString()).Success)
                    {
                        var tmpValue = i + 1;
                        var text = "";
                        var matchesNumbers = Regex.Matches(finalParaList[i].ToString(), "<Text>.*?</Text>");
                        var matchesTitle = Regex.Matches(finalParaList[tmpValue].ToString(), "<Text>.*?</Text>");

                        if (pattern.Match(matchesNumbers[0].Value).Success && !pattern.Match(matchesTitle[0].Value).Success)
                        {
                            foreach (Match number in matchesNumbers)
                            {
                                text += number.Value.Replace("<Text>", "").Replace("</Text>", "") + " ";
                            }
                            foreach (Match title in matchesTitle)
                            {
                                text += "\n" + title.Value.Replace("<Text>", "").Replace("</Text>", "");
                            }
                        }
                        else if (pattern.Match(matchesNumbers[0].Value).Success && pattern.Match(matchesTitle[0].Value).Success)
                        {
                            foreach (Match number in matchesNumbers)
                            {
                                text += number.Value.Replace("<Text>", "").Replace("</Text>", "") + "\n";
                            }
                        }

                        if (string.IsNullOrEmpty(text))
                        {
                            var exp = Regex.Match(finalParaList[i].Value, @"\d{5} \d{2}(\.|\/)\d{2}(\.|\/)\d{4}");
                            var txt = Path.Combine(PathToTetml.FullName + $"{file.Split('\\').Last().Replace(".tetml", "")}_ErrorsLog.txt");
                            using (var sf = File.AppendText(txt))
                            {
                                sf.WriteLineAsync($"{exp.Value.Split(' ').First()} is empty");
                                sf.Flush();
                                sf.Close();
                            }
                        }

                        myList.Add(text);
                    }
                }

                if (myList.Count > 0)
                {
                    var processedRecords = Processing.Applications(myList);
                    var legalEvents = ConvertToDiamond.Sub2Convert(processedRecords);
                    DiamondSender.SendToDiamond(legalEvents);
                }
            }
        }
    }
}
