using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Diamond_RU
{
    /*Расширение для XmlDocument и XDocument, позволяет конвертировать один объект в другой*/
    public static class DocumentExtensions
    {
        public static XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }
    }
    class ProcessSubcodes
    {
        private static readonly List<XDocument> Sub2Elements = GetSubcode2Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub7Elements = GetSubcode7Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub1Elements = GetSubcode1Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub4Elements = GetSubcode4Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub3Elements = GetSubcode3Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub5Elements = GetSubcode5Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub6Elements = GetSubcode6Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub8Elements = GetSubcode8Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub9Elements = GetSubcode9Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub11Elements = GetSubcode11Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub12Elements = GetSubcode12Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub13Elements = GetSubcode13Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub10Elements = GetSubcode10Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub14Elements = GetSubcode14Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub15Elements = GetSubcode15Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub16Elements = GetSubcode16Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub17Elements = GetSubcode17Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub18Elements = GetSubcode18Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub19Elements = GetSubcode19Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub20Elements = GetSubcode20Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub21Elements = GetSubcode21Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub22Elements = GetSubcode22Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub26Elements = GetSubcode26Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub28Elements = GetSubcode28Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub25Elements = GetSubcode25Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub24Elements = GetSubcode24Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub23Elements = GetSubcode23Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub31Elements = GetSubcode31Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub32Elements = GetSubcode32Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub29Elements = GetSubcode29Files(RU_main.AllXmlDocs);

        private static readonly List<XDocument> Sub30Elements = GetSubcode30Files(RU_main.AllXmlDocs);


        struct XmlFileRecords
        {
            public XmlNode Sdopi { get; set; }
            public XmlNodeList RuNotification { get; set; }
        }

        /*Получение нужных нодов из XML*/
        private static XmlFileRecords GetNodesFromXml(XDocument xmlFile)
        {
            XmlFileRecords extractedNodes = new XmlFileRecords();
            var xmlDocument = xmlFile.ToXmlDocument();
            extractedNodes.Sdopi = xmlDocument.SelectSingleNode("ru-patent-document/SDOBI");
            extractedNodes.RuNotification = xmlDocument.SelectNodes("ru-patent-document/ru-notification");
            return extractedNodes;
        }

        private static List<XDocument> GetSubcode2Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>PD4A")).ToList();
        }

        private static List<XDocument> GetSubcode7Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>PD9K")).ToList();
        }

        private static List<XDocument> GetSubcode1Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>HZ9A")).ToList();
        }

        private static List<XDocument> GetSubcode4Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>PC9K01")).ToList();
        }

        private static List<XDocument> GetSubcode3Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>PC4A01")).ToList();
        }
        private static List<XDocument> GetSubcode5Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>PC4A03")).ToList();
        }
        private static List<XDocument> GetSubcode6Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>PC9K02")).ToList();
        }
        private static List<XDocument> GetSubcode8Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>HC9A")).ToList();
        }
        private static List<XDocument> GetSubcode9Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>TC4A")).ToList();
        }
        private static List<XDocument> GetSubcode11Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>HE9A")).ToList();
        }
        private static List<XDocument> GetSubcode12Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>TE4A")).ToList();
        }
        private static List<XDocument> GetSubcode13Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>TE9K")).ToList();
        }
        private static List<XDocument> GetSubcode10Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>TC9K")).ToList();
        }
        private static List<XDocument> GetSubcode14Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>FA9A01")).ToList();
        }
        private static List<XDocument> GetSubcode15Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>FA9A02")).ToList();
        }
        private static List<XDocument> GetSubcode16Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>FZ9A")).ToList();
        }
        private static List<XDocument> GetSubcode17Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>MZ4A")).ToList();
        }
        private static List<XDocument> GetSubcode18Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>MM4A")).ToList();
        }
        private static List<XDocument> GetSubcode19Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>MF4A01")).ToList();
        }
        private static List<XDocument> GetSubcode20Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>MF4A02")).ToList();
        }
        private static List<XDocument> GetSubcode21Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>MZ9K")).ToList();
        }
        private static List<XDocument> GetSubcode22Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>MM9K")).ToList();
        }
        private static List<XDocument> GetSubcode26Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>NF4A")).ToList();
        }
        private static List<XDocument> GetSubcode28Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>NF9K")).ToList();
        }
        private static List<XDocument> GetSubcode25Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>NG4A")).ToList();
        }
        private static List<XDocument> GetSubcode24Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>MF9K01")).ToList();
        }
        private static List<XDocument> GetSubcode23Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>MG9K")).ToList();
        }
        private static List<XDocument> GetSubcode31Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>FA9A04")).ToList();
        }
        private static List<XDocument> GetSubcode32Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>FA9A03")).ToList();
        }
        private static List<XDocument> GetSubcode29Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>QC4A01")).ToList();
        }
        private static List<XDocument> GetSubcode30Files(List<XDocument> elements)
        {
            return elements.Where(x => x.Root.ToString().Contains(@"<ru-b903i>QC9K01")).ToList();
        }

        public static void SubCodesProcessing()
        {

            if (Sub1Elements.Count > 0)
            {
                List<RecordElements.SubCode> sub1outTEST = SubCodeMapper(Sub1Elements, 1, "HZ9A");
                var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub1outTEST, 1);
                Console.WriteLine("Сабкод 1 конвертирвоан успешно.");
                DiamondSender.SendToDiamond(convertedTEST);
                Console.WriteLine("Сабкод 1 отправлен на Diamond успешно...");
            }

            /* if (Sub2Elements.Count > 0)
             {
                 List<RecordElements.SubCode> sub2outputTEST = SubCodeMapper(Sub2Elements, 2, "PD4A");
                 var convertedElements = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub2outputTEST, 2);
                 Console.WriteLine("Сабкод 2 конвертирвоан успешно.");
                 DiamondSender.SendToDiamond(convertedElements);
                 Console.WriteLine("Сабкод 2 отправлен на Diamond успешно...");
            }
             
             if (Sub3Elements.Count > 0) 
             {
                 List<RecordElements.SubCode> sub3outTEST = SubCodeMapper(Sub3Elements, 3, "PC4A01");
                 var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub3outTEST, 3);
                 Console.WriteLine("Сабкод 3 конвертирвоан успешно.");
                 DiamondSender.SendToDiamond(convertedTEST);
                 Console.WriteLine("Сабкод 3 отправлен на Diamond успешно...");
            }
             
             if (Sub4Elements.Count > 0)
             {
                  List<RecordElements.SubCode> sub4outTEST = SubCodeMapper(Sub4Elements, 4, "PC9K01");
                  var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub4outTEST, 4);
                  Console.WriteLine("Сабкод 4 конвертирвоан успешно.");
                  DiamondSender.SendToDiamond(convertedTEST);
                  Console.WriteLine("Сабкод 4 отправлен на Diamond успешно...");

            }
            
            if (Sub5Elements.Count > 0)
            {
                List<RecordElements.SubCode> sub5outTEST = SubCodeMapper(Sub5Elements, 5, "PC4A03");
                var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub5outTEST, 5);
                Console.WriteLine("Сабкод 5 конвертирвоан успешно.");
                DiamondSender.SendToDiamond(convertedTEST);
                Console.WriteLine("Сабкод 5 отправлен на Diamond успешно...");
            }
            
            if (Sub6Elements.Count > 0)
            {
                 List<RecordElements.SubCode> sub6outTEST = SubCodeMapper(Sub6Elements, 6, "PC9K02");
                 var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub6outTEST, 6);
                 Console.WriteLine("Сабкод 6 конвертирвоан успешно.");
                 DiamondSender.SendToDiamond(convertedTEST);
                 Console.WriteLine("Сабкод 6 отправлен на Diamond успешно...");
            }

             if (Sub7Elements.Count > 0)
             {
                List<RecordElements.SubCode> sub7outTEST = SubCodeMapper(Sub7Elements, 7, "PD9K");
                var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub7outTEST, 7);
                Console.WriteLine("Сабкод 7 конвертирвоан успешно.");
                DiamondSender.SendToDiamond(convertedTEST);
                Console.WriteLine("Сабкод 7 отправлен на Diamond успешно...");
            }*/

            if (Sub8Elements.Count > 0)
            {
                List<RecordElements.SubCode> sub9outTEST = SubCodeMapper(Sub8Elements, 8, "HC9A");
                var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub9outTEST, 8);
                Console.WriteLine("Сабкод 8 конвертирвоан успешно.");
                DiamondSender.SendToDiamond(convertedTEST);
                Console.WriteLine("Сабкод 8 отправлен на Diamond успешно...");

            }
            /*
            if (Sub9Elements.Count > 0)
            {
                List<RecordElements.SubCode> sub9outTEST = SubCodeMapper(Sub9Elements, 9, "TC4A");
                var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub9outTEST, 9);
                Console.WriteLine("Сабкод 9 конвертирвоан успешно.");
                DiamondSender.SendToDiamond(convertedTEST);
                Console.WriteLine("Сабкод 9 отправлен на Diamond успешно...");
            }

            if (Sub10Elements.Count > 0)
            {
                List<RecordElements.SubCode> sub10outTEST = SubCodeMapper(Sub10Elements, 10, "TC9K");
                var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub10outTEST, 10);
                Console.WriteLine("Сабкод 10 конвертирвоан успешно.");
                DiamondSender.SendToDiamond(convertedTEST);
                Console.WriteLine("Сабкод 10 отправлен на Diamond успешно...");
            }

            if (Sub11Elements.Count > 0)
            {
                List<RecordElements.SubCode> sub11outTEST = SubCodeMapper(Sub11Elements, 11, "HE9A");
                var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub11outTEST, 11);
                Console.WriteLine("Сабкод 11 конвертирвоан успешно.");
                DiamondSender.SendToDiamond(convertedTEST);
                Console.WriteLine("Сабкод 11 отправлен на Diamond успешно...");
            }

            if (Sub12Elements.Count > 0)
            {
                List<RecordElements.SubCode> sub12outTEST = SubCodeMapper(Sub12Elements, 12, "TE4A");
                var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub12outTEST, 12);
                Console.WriteLine("Сабкод 12 конвертирвоан успешно.");
                DiamondSender.SendToDiamond(convertedTEST);
                Console.WriteLine("Сабкод 12 отправлен на Diamond успешно...");

            }

            if (Sub13Elements.Count > 0)
            {
                List<RecordElements.SubCode> sub13outTEST = SubCodeMapper(Sub13Elements, 13, "TE9K");
                var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub13outTEST, 13);
                Console.WriteLine("Сабкод 13 конвертирвоан успешно.");
                DiamondSender.SendToDiamond(convertedTEST);
                Console.WriteLine("Сабкод 13 отправлен на Diamond успешно...");
            }



            if (Sub14Elements.Count > 0)
            {
                List<RecordElements.SubCode> sub14outTEST = SubCodeMapper(Sub14Elements, 14, "FA9A01");
                var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub14outTEST, 14);
                Console.WriteLine("Сабкод 14 конвертирвоан успешно.");
                DiamondSender.SendToDiamond(convertedTEST);
                Console.WriteLine("Сабкод 14 отправлен на Diamond успешно...");
            }

            if (Sub15Elements.Count > 0)
            {
                List<RecordElements.SubCode> sub15outTEST = SubCodeMapper(Sub15Elements, 15, "FA9A02");
                var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub15outTEST, 15);
                Console.WriteLine("Сабкод 15 конвертирвоан успешно.");
                DiamondSender.SendToDiamond(convertedTEST);
                Console.WriteLine("Сабкод 15 отправлен на Diamond успешно...");
            }

            if (Sub16Elements.Count > 0)
            {
                List<RecordElements.SubCode> sub16outTEST = SubCodeMapper(Sub16Elements, 16, "FZ9A");
                var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub16outTEST, 16);
                Console.WriteLine("Сабкод 16 конвертирвоан успешно.");
                DiamondSender.SendToDiamond(convertedTEST);
                Console.WriteLine("Сабкод 16 отправлен на Diamond успешно...");
            }

            if (Sub17Elements.Count > 0)
            {
                List<RecordElements.SubCode> sub17outTEST = SubCodeMapper(Sub17Elements, 17, "MZ4A");
                var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub17outTEST, 17);
                Console.WriteLine("Сабкод 17 конвертирвоан успешно.");
                DiamondSender.SendToDiamond(convertedTEST);
                Console.WriteLine("Сабкод 17 отправлен на Diamond успешно...");
            }

            if (Sub18Elements.Count > 0)
            {
                List<RecordElements.SubCode> sub18outTEST = SubCodeMapper(Sub18Elements, 18, "MM4A");
                var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub18outTEST, 18);
                Console.WriteLine("Сабкод 18 конвертирвоан успешно.");
                DiamondSender.SendToDiamond(convertedTEST);
                Console.WriteLine("Сабкод 18 отправлен на Diamond успешно...");
            }

            if (Sub19Elements.Count > 0)
            {
                List<RecordElements.SubCode> sub19outTEST = SubCodeMapper(Sub19Elements, 19, "MF4A01");
                var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub19outTEST, 19);
                Console.WriteLine("Сабкод 19 конвертирвоан успешно.");
                DiamondSender.SendToDiamond(convertedTEST);
                Console.WriteLine("Сабкод 19 отправлен на Diamond успешно...");
            }

            if (Sub20Elements.Count > 0)
            {
                List<RecordElements.SubCode> sub20outTEST = SubCodeMapper(Sub20Elements, 20, "MF4A02");
                var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub20outTEST, 20);
                Console.WriteLine("Сабкод 20 конвертирвоан успешно.");
                DiamondSender.SendToDiamond(convertedTEST);
                Console.WriteLine("Сабкод 20 отправлен на Diamond успешно...");
            }

            if (Sub21Elements.Count > 0)
            {
                List<RecordElements.SubCode> sub21outTEST = SubCodeMapper(Sub21Elements, 21, "MZ9K");
                var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub21outTEST, 21);
                Console.WriteLine("Сабкод 21 конвертирвоан успешно.");
                DiamondSender.SendToDiamond(convertedTEST);
                Console.WriteLine("Сабкод 21 отправлен на Diamond успешно...");
            }

            if (Sub22Elements.Count > 0)  
            {
                List<RecordElements.SubCode> sub22outTEST = SubCodeMapper(Sub22Elements, 22, "MM9K");
                var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub22outTEST, 22);
                Console.WriteLine("Сабкод 22 конвертирвоан успешно.");
                DiamondSender.SendToDiamond(convertedTEST);
                Console.WriteLine("Сабкод 22 отправлен на Diamond успешно...");
            }

            if (Sub23Elements.Count > 0)
            {
                List<RecordElements.SubCode> sub23outTEST = SubCodeMapper(Sub23Elements, 23, "MG9K");
                var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub23outTEST, 23);
                Console.WriteLine("Сабкод 23 конвертирвоан успешно.");
                DiamondSender.SendToDiamond(convertedTEST);
                Console.WriteLine("Сабкод 23 отправлен на Diamond успешно...");
            }

            if (Sub24Elements.Count > 0)
            {
                List<RecordElements.SubCode> sub24outTEST = SubCodeMapper(Sub24Elements, 24, "MF9K01");
                var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub24outTEST, 24);
                Console.WriteLine("Сабкод 24 конвертирвоан успешно.");
                DiamondSender.SendToDiamond(convertedTEST);
                Console.WriteLine("Сабкод 24 отправлен на Diamond успешно...");
            }

            if (Sub25Elements.Count > 0)
            {
                List<RecordElements.SubCode> sub25outTEST = SubCodeMapper(Sub25Elements, 25, "NG4A");
                var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub25outTEST, 25);
                Console.WriteLine("Сабкод 25 конвертирвоан успешно.");
                DiamondSender.SendToDiamond(convertedTEST);
                Console.WriteLine("Сабкод 25 отправлен на Diamond успешно...");
            }

            if (Sub26Elements.Count > 0)
            { 
                List<RecordElements.SubCode> sub26outTEST = SubCodeMapper(Sub26Elements, 26, "NF4A");
                var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub26outTEST, 26);
                Console.WriteLine("Сабкод 26 конвертирвоан успешно.");
                DiamondSender.SendToDiamond(convertedTEST);
                Console.WriteLine("Сабкод 26 отправлен на Diamond успешно...");
            }

            if (Sub28Elements.Count > 0)
            {
                List<RecordElements.SubCode> sub28outTEST = SubCodeMapper(Sub28Elements, 28, "NF9K");
                var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub28outTEST, 28);
                Console.WriteLine("Сабкод 28 конвертирвоан успешно.");
                DiamondSender.SendToDiamond(convertedTEST);
                Console.WriteLine("Сабкод 28 отправлен на Diamond успешно...");
            }

            if (Sub29Elements.Count > 0)
            {
                 List<RecordElements.SubCode> sub29outTEST = SubCodeMapper(Sub29Elements, 29, "QC4A01");
                 var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub29outTEST, 29);
                 Console.WriteLine("Сабкод 29 конвертирвоан успешно.");
                 DiamondSender.SendToDiamond(convertedTEST);
                 Console.WriteLine("Сабкод 29 отправлен на Diamond успешно...");
            }

            if (Sub30Elements.Count > 0)
            {
                 List<RecordElements.SubCode> sub30outTEST = SubCodeMapper(Sub30Elements, 30, "QC9K01");
                 var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub30outTEST, 30);
                 Console.WriteLine("Сабкод 30 конвертирвоан успешно.");
                 DiamondSender.SendToDiamond(convertedTEST);
                 Console.WriteLine("Сабкод 30 отправлен на Diamond успешно...");
            }

            if (Sub31Elements.Count > 0)
            {
                List<RecordElements.SubCode> sub31outTEST = SubCodeMapper(Sub31Elements, 31, "FA9A04");
                var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub31outTEST, 31);
                Console.WriteLine("Сабкод 31 конвертирвоан успешно.");
                DiamondSender.SendToDiamond(convertedTEST);
                Console.WriteLine("Сабкод 31 отправлен на Diamond успешно...");
            }

            if (Sub32Elements.Count > 0)
            {
                List<RecordElements.SubCode> sub32outTEST = SubCodeMapper(Sub32Elements, 32, "FA9A03");
                var convertedTEST = ConvertToDiamondFormat.ConvertSubCodeToDiamondFormat(sub32outTEST, 32);
                Console.WriteLine("Сабкод 32 конвертирвоан успешно.");
                DiamondSender.SendToDiamond(convertedTEST);
                Console.WriteLine("Сабкод 32 отправлен на Diamond успешно...");
            }*/

            Console.WriteLine(
                "\n\n\n----------------------------------------------------------//--------------------------------------------------");
            Console.WriteLine("\t\t\t\tОтправка завершена.");
            Console.ReadKey();
        }

        private static List<RecordElements.SubCode> SubCodeMapper(List<XDocument> subCodeElements, int numberSubcode, string B903i_Value)
        {
            List<RecordElements.SubCode> subCodeoutput = new List<RecordElements.SubCode>();
            foreach (var xmlFile in subCodeElements)
            {
                var extractedNodes = GetNodesFromXml(xmlFile);
                var headerElements = new RecordElements.SubCode
                {
                    B110 = Regex.Match(extractedNodes.Sdopi.InnerXml, @"<B110>(?<value>.*)</B110>")?.Groups["value"].Value,
                    B130 = Regex.Match(extractedNodes.Sdopi.InnerXml, @"<B130>(?<value>.*)</B130>")?.Groups["value"].Value,
                    B140 = Regex.Match(extractedNodes.Sdopi.InnerXml, @"<B140><date>(?<value>.*)</date></B140>")?.Groups["value"].Value,
                    B190 = Regex.Match(extractedNodes.Sdopi.InnerXml, @"<B190>(?<value>.*)</B190>")?.Groups["value"].Value,
                    B210 = Regex.Match(extractedNodes.Sdopi.InnerXml, @"<B210>(?<value>.*)</B210>")?.Groups["value"].Value,
                    B220 = Regex.Match(extractedNodes.Sdopi.InnerXml, @"<B220><date>(?<value>.*)</date></B220>")?.Groups["value"].Value
                };

                RecordElements.AgentInfo agentInfo;
                RecordElements.TitleAndOwner titleAndOwner;
                var bodyElements = new List<RecordElements.SubCode>();
                List<string> outList = new List<string>();
                List<string> AssigneeList = new List<string>();
                foreach (XmlNode rec in extractedNodes.RuNotification)
                {
                    agentInfo = new RecordElements.AgentInfo();
                    titleAndOwner = new RecordElements.TitleAndOwner();

                    agentInfo.Address =
                        Regex.Match(rec.InnerXml, @"<ru-B980i>(?<value>.*)</ru-B980i>", RegexOptions.IgnoreCase)
                            ?.Groups["value"].Value;

                    if (numberSubcode == 24)
                    {
                        var result = Regex.Match(rec.InnerXml, @"<ru-B909i>.*(?<value>\d{2}\.\d{2}\.\d{4}).*</ru-B909i>", RegexOptions.IgnoreCase);
                        if (titleAndOwner != null)
                        {
                            titleAndOwner.Title = result.Groups["value"].Value;
                        }
                    }
                    else
                    {
                        var result = Regex.Match(rec.InnerXml, "<ru-B909i>.*\"(?<title>.*)\".*Патентообладателем изобретения признано?(?<patentOwner>.*)\\.</ru-B909i>", RegexOptions.IgnoreCase);
                        if (titleAndOwner != null)
                        {
                            titleAndOwner.Title = result.Groups["title"].Value;
                            titleAndOwner.Patent_Owner = result.Groups["patentOwner"].Value;
                        }
                    }

                    bodyElements.Add(new RecordElements.SubCode
                    {
                        B110 = Regex.Match(rec.InnerXml, @"<B110>(?<value>.*)</B110>", RegexOptions.IgnoreCase)?.Groups["value"].Value,
                        B110j = Regex.Match(rec.InnerXml, @"<ru-B110j>(?<value>.*)</ru-B110j>", RegexOptions.IgnoreCase)?.Groups["value"].Value,
                        B405i = Regex.Match(rec.InnerXml, @"<ru-B405i>(?<value>.*)</ru-B405i>", RegexOptions.IgnoreCase)?.Groups["value"].Value,
                        B460 = Regex.Match(rec.InnerXml, @"<B460><date>(?<value>.*)</date></B460>", RegexOptions.IgnoreCase)?.Groups["value"].Value,
                        B460i = Regex.Match(rec.InnerXml, @"<ru-B460i><date>(?<value>.*)</date></ru-B460i>", RegexOptions.IgnoreCase)?.Groups["value"].Value,
                        B711 = Regex.Matches(rec.InnerXml, @"<B711><ru-name-text>(?<value>[^</]+)</ru-name-text></B711>", RegexOptions.IgnoreCase).Cast<Match>().Select(x => x.Groups["value"].Value).ToList(),
                        B721 = Regex.Matches(rec.InnerXml, @"<B721><ru-name-text>(?<value>[^</]+)</ru-name-text></B721>", RegexOptions.IgnoreCase).Cast<Match>().Select(x => x.Groups["value"].Value).ToList(),
                        B731i = Regex.Matches(rec.InnerXml, @"<ru-B731i><ru-name-text>(?<value>[^</]+)</ru-name-text></ru-B731i>", RegexOptions.IgnoreCase).Cast<Match>().Select(x => x.Groups["value"].Value).ToList(),
                        B903i = Regex.Match(rec.InnerXml, @"<ru-B903i>(?<value>.*)</ru-B903i>", RegexOptions.IgnoreCase)?.Groups["value"].Value,
                        B919i = Regex.Match(rec.InnerXml, @"<ru-B919i>(?<value>.*)</ru-B919i>", RegexOptions.IgnoreCase)?.Groups["value"].Value,
                        B920i = Regex.Match(rec.InnerXml, @"<ru-B920i><date>(?<value>.*)</date></ru-B920i>", RegexOptions.IgnoreCase)?.Groups["value"].Value,
                        B236 = Regex.Match(rec.InnerXml, @"<B236><date>(?<value>.*)</date></B236>", RegexOptions.IgnoreCase)?.Groups["value"].Value,
                        B238 = Regex.Match(rec.InnerXml, @"<B238><date>(?<value>.*)</date></B238>", RegexOptions.IgnoreCase)?.Groups["value"].Value,
                        B994i = Regex.Match(rec.InnerXml, @"<ru-B994i><date>(?<value>.*)</date></ru-B994i>", RegexOptions.IgnoreCase)?.Groups["value"].Value,
                        B908i = Regex.Match(rec.InnerXml, @"<ru-B908i>(?<value>.*[^\d])(?<LegalEventDate>\d{2}\.\d{2}\.\d{4}).*</ru-B908i>", RegexOptions.IgnoreCase)?.Groups["LegalEventDate"].Value,
                        B247i = Regex.Match(rec.InnerXml, @"<ru-B247i><date>(?<value>.*)</date></ru-B247i>", RegexOptions.IgnoreCase)?.Groups["value"].Value,
                        B733i = Regex.Matches(rec.InnerXml, @"<ru-B733i><ru-name-text>(?<value>[^</]+)</ru-name-text></ru-B733i>", RegexOptions.IgnoreCase).Cast<Match>().Select(x => x.Groups["value"].Value).ToList(),
                        B791 = Regex.Match(rec.InnerXml, @"<B791>.*<ru-name-text>(?<value>.*)</ru-name-text></B791>", RegexOptions.IgnoreCase)?.Groups["value"].Value,
                        B919ic = Regex.Match(rec.InnerXml, @"<ru-B919ic>(?<value>.*)</ru-B919ic>", RegexOptions.IgnoreCase)?.Groups["value"].Value,
                        B920ic = Regex.Match(rec.InnerXml, @"<ru-B920ic><date>(?<value>.*)</date></ru-B920ic>", RegexOptions.IgnoreCase)?.Groups["value"].Value,
                        B909i = titleAndOwner,
                        B980i = agentInfo
                    });

                    Regex reg = new Regex(@"<ru-b734i><ru-name-text>.*</ru-name-text></ru-b734i>");
                    MatchCollection matches = reg.Matches(rec.InnerXml);
                    if (matches.Count > 0)
                    {
                        foreach (Match match in matches)
                        {
                            outList.Add(match.Value);
                        }
                    }

                    Regex reg2 = new Regex(@"<B731><ru-name-text>.*</ru-name-text></B731>");
                    MatchCollection matches2 = reg2.Matches(rec.InnerXml);
                    if (matches2.Count > 0)
                    {
                        foreach (Match match in matches2)
                        {
                            AssigneeList.Add(match.Value);
                        }
                    }
                }

                foreach (var rec in bodyElements)
                {
                    var completeRecord = new RecordElements.SubCode();
                    if (outList != null)
                    {
                        if (outList.Count == 1)
                        {
                            string[] splittedList = outList[0].Split(new string[] { "</ru-b734i>" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                            for (int i = 0; i < splittedList.Length; i++)
                            {
                                splittedList[i] = splittedList[i].Replace("<ru-b734i>", "")
                                    .Replace("<ru-name-text>", "").Replace("</ru-name-text>", "")
                                    .Replace("</ru-b734i>", "").Trim();
                            }

                            if (splittedList != null)
                                completeRecord.B734i = splittedList.ToList();
                            else
                                completeRecord.B734i = outList;
                        }

                    }

                    if (AssigneeList != null)
                    {
                        if (AssigneeList.Count == 1)
                        {
                            string[] splittedList = AssigneeList[0].Split(new string[] { "</B731>" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                            for (int i = 0; i < splittedList.Length; i++)
                            {
                                splittedList[i] = splittedList[i].Replace("<B731>", "")
                                    .Replace("<ru-name-text>", "").Replace("</ru-name-text>", "")
                                    .Replace("</B731>", "").Trim();
                            }

                            if (splittedList != null)
                                completeRecord.B731 = splittedList.ToList();
                            else
                                completeRecord.B731 = AssigneeList;
                        }

                    }

                    if (completeRecord.Field12 == null)
                    {
                        RecordElements.Field12 field12 = new RecordElements.Field12();
                        var result = Methods.GetNameChapterForName(B903i_Value);
                        field12.versionRU = result.Item1;
                        field12.versionEN = result.Item2;
                        completeRecord.Field12 = field12;
                    }

                    if (!string.IsNullOrEmpty(headerElements.B110))
                        completeRecord.B110 = headerElements.B110;
                    else if (!string.IsNullOrEmpty(rec.B110))
                        completeRecord.B110 = rec.B110;
                    else
                        Console.WriteLine("Element 110 was to found");

                    if (!string.IsNullOrEmpty(headerElements.B130))
                        completeRecord.B130 = headerElements.B130;
                    else if (!string.IsNullOrEmpty(rec.B130))
                        completeRecord.B130 = rec.B130;
                    else
                        Console.WriteLine("Element B130 was to found");

                    if (!string.IsNullOrEmpty(headerElements.B140))
                        completeRecord.B140 = headerElements.B140;
                    else if (!string.IsNullOrEmpty(rec.B110))
                        completeRecord.B140 = rec.B140;
                    else
                        Console.WriteLine("Element B140 was to found");

                    if (!string.IsNullOrEmpty(headerElements.B190))
                        completeRecord.B190 = headerElements.B190;
                    else if (!string.IsNullOrEmpty(rec.B190))
                        completeRecord.B190 = rec.B190;
                    else
                        Console.WriteLine("Element B190 was to found");

                    if (!string.IsNullOrEmpty(headerElements.B210))
                        completeRecord.B210 = headerElements.B210;
                    else if (!string.IsNullOrEmpty(rec.B210))
                        completeRecord.B210 = rec.B210;
                    else
                        Console.WriteLine("Element B210 was to found");

                    if (!string.IsNullOrEmpty(headerElements.B220))
                        completeRecord.B220 = headerElements.B220;
                    else if (!string.IsNullOrEmpty(rec.B220))
                        completeRecord.B220 = rec.B220;
                    else
                        Console.WriteLine("Element B220 was to found");

                    if (!string.IsNullOrEmpty(rec.B405i))
                    {
                        completeRecord.B405i = Regex.Replace(rec.B405i, @"^\d{4}", "").Trim();
                    }
                    if (!string.IsNullOrEmpty(rec.B460i))
                    {
                        completeRecord.B460i = rec.B460i;
                    }
                    if (!string.IsNullOrEmpty(rec.B460i))
                    {
                        completeRecord.B460i = rec.B460i;
                    }
                    if (!string.IsNullOrEmpty(rec.B460i))
                    {
                        completeRecord.B460i = rec.B460i;
                    }
                    if (rec.B711 != null)
                    {
                        completeRecord.B711 = rec.B711;
                    }
                    if (rec.B721 != null)
                    {
                        completeRecord.B721 = rec.B721;
                    }
                    if (rec.B731 != null)
                    {
                        completeRecord.B731 = rec.B731;
                    }
                    if (rec.B731i != null)
                    {
                        completeRecord.B731i = rec.B731i;
                    }
                    if (rec.B734i != null)
                    {
                        foreach (var item in rec.B734i)
                        {
                            completeRecord.B734i.Add(item);
                        }
                    }
                    if (!string.IsNullOrEmpty(rec.B903i))
                    {
                        completeRecord.B903i = rec.B903i;
                    }
                    if (!string.IsNullOrEmpty(rec.B919i))
                    {
                        completeRecord.B919i = rec.B919i;
                    }
                    if (!string.IsNullOrEmpty(rec.B920i))
                    {
                        completeRecord.B920i = rec.B920i;
                    }

                    if (!string.IsNullOrEmpty(rec.B236))
                    {
                        completeRecord.B236 = rec.B236;
                    }

                    if (!string.IsNullOrEmpty(rec.B238))
                    {
                        completeRecord.B238 = rec.B238;
                    }

                    if (!string.IsNullOrEmpty(rec.B994i))
                    {
                        completeRecord.B994i = rec.B994i;
                    }

                    if (!string.IsNullOrEmpty(rec.B908i))
                    {
                        completeRecord.B908i = rec.B908i;
                    }

                    if (numberSubcode == 20)
                    {
                        if (rec.B909i != null)
                        {
                            completeRecord.B909i = rec.B909i;
                        }
                    }

                    if (numberSubcode == 24)
                    {
                        if (rec.B909i != null)
                        {
                            completeRecord.B909i = rec.B909i;
                        }
                    }

                    if (!string.IsNullOrEmpty(rec.B247i))
                    {
                        completeRecord.B247i = rec.B247i;
                    }

                    if (numberSubcode == 25)
                    {
                        if (!string.IsNullOrEmpty(rec.B110j))
                            completeRecord.B110j = rec.B110j;

                        if (!string.IsNullOrEmpty(rec.B460))
                            completeRecord.B460 = rec.B460;
                    }

                    if (!string.IsNullOrEmpty(rec.B110j))
                    {
                        completeRecord.B110j = rec.B110j;
                    }

                    if (rec.B733i != null)
                    {
                        completeRecord.B733i = rec.B733i;
                    }

                    if (!string.IsNullOrEmpty(rec.B791))
                    {
                        completeRecord.B791 = rec.B791;
                    }

                    if (!string.IsNullOrEmpty(rec.B919ic))
                    {
                        completeRecord.B919ic = rec.B919ic;
                    }

                    if (!string.IsNullOrEmpty(rec.B920ic))
                    {
                        completeRecord.B920ic = rec.B920ic;
                    }

                    if (numberSubcode == 13 || numberSubcode == 12 || numberSubcode == 11)
                    {
                        if (rec.B980i.Name == null && rec.B980i.Address != null)
                        {
                            string fullStr = rec.B980i.Address;

                            RecordElements.AgentInfo agent;
                            string[] splittedStr = fullStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                            if (splittedStr.Length > 0)
                            {
                                agent = new RecordElements.AgentInfo();
                                agent.Name = splittedStr.Last();
                                agent.Address = fullStr.Replace(agent.Name, "").TrimEnd(',').Trim();
                                completeRecord.B980i = agent;
                            }
                            else
                            {
                                agent = new RecordElements.AgentInfo();
                                agent.Address = rec.B980i.Address.TrimEnd(',').Trim();
                                completeRecord.B980i = agent;
                            }
                        }
                    }
                    else
                    {
                        if (rec.B980i != null)
                        {
                            completeRecord.B980i = rec.B980i;
                        }
                    }

                    if (numberSubcode == 3 || numberSubcode == 5 || numberSubcode == 6)
                    {
                        if (completeRecord.B734i != null)
                        {
                            var searchLine = Methods.ReplaceNameOrganization(completeRecord.B734i[0]);

                            if (searchLine != null)
                            {
                                if (completeRecord.B980i.Address.Contains(searchLine))
                                {
                                    completeRecord.isField73 = true;
                                }
                                else
                                {
                                    completeRecord.isField73 = false;
                                }
                            }
                        }
                        else
                        {
                            completeRecord.isField73 = false;
                        }

                    }

                    if (completeRecord.B903i == B903i_Value)
                        subCodeoutput.Add(completeRecord);
                }
            }
            return subCodeoutput.Count > 0 ? subCodeoutput : null;
        }
    }
}
