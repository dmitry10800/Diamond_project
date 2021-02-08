using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
            foreach (FileInfo file in PathToTetml.GetFiles("*.txt", SearchOption.AllDirectories))
                files.Add(file.FullName);

            XElement elem = null;

            List<XElement> paraList = new List<XElement>();
            List<string> subCode7List = new List<string>();

            foreach (var file in files)
            {
                currentFile = new FileInfo(file);
                currentFileName = Path.GetFileName(file);
                string text = File.ReadAllText(file);

                Regex regex = new Regex(@"(?<=\[[A-Z]{2}\]\s?\r\n)");
                List<string> splitText = regex.Split(text).Where(val => !string.IsNullOrEmpty(val)).ToList(); 

                subCode7List = splitText;
            }

            if (subCode7List.Count > 0)
            {
                List<Elements> processedRecords = Processing.SubCode7(subCode7List);

                var legalEvents = DiamondConverter.Sub7Convert(processedRecords);

                DiamondSender.SendToDiamond(legalEvents);
            }
        }
    }
}
