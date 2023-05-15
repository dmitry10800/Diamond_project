using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_PH
{
    class PH_main
    {
        public static DirectoryInfo PathToTetml = new DirectoryInfo(@"C:\Work\PH\PH_20201125_129");
        public static FileInfo currentFile = null;
        public static string currentFileName = null;

        static void Main(string[] args)
        {
            var files = new List<string>();
            foreach (var file in PathToTetml.GetFiles("*.txt", SearchOption.AllDirectories))
                files.Add(file.FullName);

            XElement elem = null;

            var paraList = new List<XElement>();
            var subCode7List = new List<string>();

            foreach (var file in files)
            {
                currentFile = new FileInfo(file);
                currentFileName = Path.GetFileName(file);
                var text = File.ReadAllText(file);

                var regex = new Regex(@"(?<=\[[A-Z]{2}\]\s?\r\n)");
                var splitText = regex.Split(text).Where(val => !string.IsNullOrEmpty(val)).ToList(); 

                subCode7List = splitText;
            }

            if (subCode7List.Count > 0)
            {
                var processedRecords = Processing.SubCode7(subCode7List);

                var legalEvents = DiamondConverter.Sub7Convert(processedRecords);

                DiamondSender.SendToDiamond(legalEvents);
            }
        }
    }
}
