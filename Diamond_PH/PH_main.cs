using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_PH
{
    class PH_main
    {
        public static DirectoryInfo PathToTetml = new DirectoryInfo(@"C:\Users\Razrab\Desktop\PH\");
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
                var text = File.ReadAllText(file);

                var splitText = text.Split("\r\n\r\n".ToCharArray());
                subCode7List = splitText.ToList();
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
