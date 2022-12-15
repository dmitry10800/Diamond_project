using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_RU
{
    class RU_main
    {
        public static string PathToFolder { get; set; } = @"D:\TET_DEV\Diamond\RU\Gazette\xrbi201933\DOC";

        public static string NameArchive { get; set; } = @"RU_20191127_33.pdf";

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
