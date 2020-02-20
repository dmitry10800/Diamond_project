using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RU
{
    class RU_main
    {
        public static string PathToFolder { get; set; } = @"D:\_DFA_main\_Patents\RU\xrbi201933\xrbi201933\DOC";

        public static List<string> GetDocumentList(string path)
        {
            return Directory.GetFiles(path, @"*.XML", SearchOption.AllDirectories).ToList();
        }

        public static List<XDocument> AllXmlDocs { get; set; } = GetDocumentList(PathToFolder).Select(x => XDocument.Load(x)).ToList();
        static void Main(string[] args)
        {
            //var documents = GetDocumentList(PathToFolder);

            //ProcessSubcodes.AllXmlDocs = documents.Select(x => XDocument.Load(x)).ToList();

            ProcessSubcodes.SubCodesProcessing();
        }
    }
}
