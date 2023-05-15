using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace RU
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

    public class SubCode
    {
        public List<XDocument> Sub1 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>HZ9A")).ToList(); // HZ9A
        public List<XDocument> Sub2 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>PD4A")).ToList(); // PD4A
        public List<XDocument> Sub3 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>PC4A01")).ToList(); // PC4A01
        public List<XDocument> Sub4 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>PC9K01")).ToList(); // PC9K01
        public List<XDocument> Sub5 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>PC4A03")).ToList(); // PC4A03
        public List<XDocument> Sub6 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>PC9K02")).ToList(); // PC9K02
        public List<XDocument> Sub7 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>PD9K")).ToList(); // PD9K
        public List<XDocument> Sub8 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>HC9A")).ToList(); // HC9A
        public List<XDocument> Sub9 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>TC4A")).ToList(); // TC4A
        public List<XDocument> Sub10 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>TC9K")).ToList(); // TC9K
        public List<XDocument> Sub11 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>HE9A")).ToList(); // HE9A
        public List<XDocument> Sub12 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>TE4A")).ToList(); // TE4A
        public List<XDocument> Sub13 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>TE9K")).ToList(); // TE9K
        public List<XDocument> Sub14 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>FA9A01")).ToList(); // FA9A01
        public List<XDocument> Sub15 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>FA9A02")).ToList(); // FA9A02
        public List<XDocument> Sub16 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>FZ9A")).ToList(); // FZ9A
        public List<XDocument> Sub17 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>MZ4A")).ToList(); // MZ4A
        public List<XDocument> Sub18 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>MM4A")).ToList(); // MM4A
        public List<XDocument> Sub19 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>MF4A01")).ToList(); // MF4A01
        public List<XDocument> Sub20 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>MF4A02")).ToList(); // MF4A02
        public List<XDocument> Sub21 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>MZ9K")).ToList(); // MZ9K
        public List<XDocument> Sub22 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>MM9K")).ToList(); // MM9K
        public List<XDocument> Sub23 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>MG9K")).ToList(); // MG9K
        public List<XDocument> Sub24 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>MF9K01")).ToList(); // MF9K01
        public List<XDocument> Sub25 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>NG4A")).ToList(); // NG4A
        public List<XDocument> Sub26 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>NF4A")).ToList(); // NF4A
        public List<XDocument> Sub27 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>NG9K")).ToList(); // NG9K
        public List<XDocument> Sub28 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>NF9K")).ToList(); // NF9K
        public List<XDocument> Sub29 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>QC4A01")).ToList(); // QC4A01
        public List<XDocument> Sub30 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>QC9K01")).ToList(); // QC9K01
        public List<XDocument> Sub31 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>FA9A04")).ToList(); // FA9A04
        public List<XDocument> Sub32 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>FA9A03")).ToList(); // FA9A03
        //public List<XDocument> Sub33 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>PD4A")).ToList();
        //public List<XDocument> Sub34 = RU_main.AllXmlDocs.Where(x => x.Root.ToString().Contains(@"<ru-b903i>PD4A")).ToList();
    }

    class ProcessSubcodes
    {
        public static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                string tmpValue = JsonConvert.SerializeObject(rec);
                var url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event"; //STAGING
                //string url = @"https://diamond.lighthouseip.online/external-api/import/legal-event"; //PRODUCTION
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }

        struct XmlFileRecords
        {
            public XmlNode Sdopi { get; set; }
            public XmlNodeList RuNotification { get; set; }
        }

        /*Получение нужных нодов из XML*/
        private static XmlFileRecords GetNodesFromXml(XDocument xmlFile)
        {
            var extractedNodes = new XmlFileRecords();
            var xmlDocument = xmlFile.ToXmlDocument();
            extractedNodes.Sdopi = xmlDocument.SelectSingleNode("ru-patent-document/SDOBI");
            extractedNodes.RuNotification = xmlDocument.SelectNodes("ru-patent-document/ru-notification");
            return extractedNodes;
        }

        private static void PrepareSubCodesReport(List<XDocument> elements)
        {
            var uniqSubcodes = elements.Select(x => Regex.Match(x.Root.ToString(), @"<ru-b903i>(?<value>.*)</ru-b903i>").Groups["value"].Value).Distinct().ToList();
            //var emptySubcodes =  elements.Where(x => Regex.Match(x.Root.ToString(), @"<ru-b903i>(?<value>.*)</ru-b903i>").Groups["value"].Value.Length < 4).ToList();
            foreach (var item in uniqSubcodes)
            {
                switch (item)
                {
                    case "HZ9A": Console.WriteLine("1"); break;
                    case "PD4A": Console.WriteLine("2"); break;
                    case "PC4A01": Console.WriteLine("3"); break;
                    case "PC9K01": Console.WriteLine("4"); break;
                    case "PC4A03": Console.WriteLine("5"); break;
                    case "PC9K02": Console.WriteLine("6"); break;
                    case "PD9K": Console.WriteLine("7"); break;
                    case "HC9A": Console.WriteLine("8"); break;
                    case "TC4A": Console.WriteLine("9"); break;
                    case "TC9K": Console.WriteLine("10"); break;
                    case "HE9A": Console.WriteLine("11"); break;
                    case "TE4A": Console.WriteLine("12"); break;
                    case "TE9K": Console.WriteLine("13"); break;
                    case "FA9A01": Console.WriteLine("14"); break;
                    case "FA9A02": Console.WriteLine("15"); break;
                    case "FZ9A": Console.WriteLine("16"); break;
                    case "MZ4A": Console.WriteLine("17"); break;
                    case "MM4A": Console.WriteLine("18"); break;
                    case "MF4A01": Console.WriteLine("19"); break;
                    case "MF4A02": Console.WriteLine("20"); break;
                    case "MZ9K": Console.WriteLine("21"); break;
                    case "MM9K": Console.WriteLine("22"); break;
                    case "MG9K": Console.WriteLine("23"); break;
                    case "MF9K01": Console.WriteLine("24"); break;
                    case "NG4A": Console.WriteLine("25"); break;
                    case "NF4A": Console.WriteLine("26"); break;
                    case "NG9K": Console.WriteLine("27"); break;
                    case "NF9K": Console.WriteLine("28"); break;
                    case "QC4A01": Console.WriteLine("29"); break;
                    case "QC9K01": Console.WriteLine("30"); break;
                    case "FA9A04": Console.WriteLine("31"); break;
                    case "FA9A03": Console.WriteLine("32"); break;
                    default: Console.WriteLine(item); break;
                }
            }
        }

        public static void SubCodesProcessing()
        {
            PrepareSubCodesReport(RU_main.AllXmlDocs);
            var subCodes = new SubCode();
            if (subCodes.Sub1.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub1)));
            if (subCodes.Sub2.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub2)));
            if (subCodes.Sub3.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub3)));
            if (subCodes.Sub4.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub4)));
            if (subCodes.Sub5.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub5)));
            if (subCodes.Sub6.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub6)));
            if (subCodes.Sub7.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub7)));
            if (subCodes.Sub8.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub8)));
            if (subCodes.Sub9.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub9)));
            if (subCodes.Sub10.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub10)));
            if (subCodes.Sub11.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub11)));
            if (subCodes.Sub12.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub12)));
            if (subCodes.Sub13.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub13)));
            if (subCodes.Sub14.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub14)));
            if (subCodes.Sub15.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub15)));
            if (subCodes.Sub16.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub16)));
            if (subCodes.Sub17.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub17)));
            if (subCodes.Sub18.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub18)));
            if (subCodes.Sub19.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub19)));
            if (subCodes.Sub20.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub20)));
            if (subCodes.Sub21.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub21)));
            if (subCodes.Sub22.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub22)));
            if (subCodes.Sub23.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub23)));
            if (subCodes.Sub24.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub24)));
            if (subCodes.Sub25.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub25)));
            if (subCodes.Sub26.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub26)));
            if (subCodes.Sub27.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub27)));
            if (subCodes.Sub28.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub28)));
            if (subCodes.Sub29.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub29)));
            if (subCodes.Sub30.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub30)));
            if (subCodes.Sub31.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub31)));
            if (subCodes.Sub32.Count > 0)
                SendToDiamond(ConvertToDiamondFormat.Sub2ToDiamond(Sub2Mapper(subCodes.Sub32)));
        }
        private static List<RecordElements.SudCode2> Sub2Mapper(List<XDocument> sub2Elements)
        {
            var sub2output = new List<RecordElements.SudCode2>();
            foreach (var xmlFile in sub2Elements)
            {
                var extractedNodes = GetNodesFromXml(xmlFile);
                var headerElements = new RecordElements.SudCode2
                {
                    B110 = Regex.Match(extractedNodes.Sdopi.InnerXml, @"<B110>(?<value>.*)</B110>")?.Groups["value"].Value,
                    B130 = Regex.Match(extractedNodes.Sdopi.InnerXml, @"<B130>(?<value>.*)</B130>")?.Groups["value"].Value,
                    B140 = Regex.Match(extractedNodes.Sdopi.InnerXml, @"<B140><date>(?<value>.*)</date></B140>")?.Groups["value"].Value,
                    B190 = Regex.Match(extractedNodes.Sdopi.InnerXml, @"<B190>(?<value>.*)</B190>")?.Groups["value"].Value,
                    B210 = Regex.Match(extractedNodes.Sdopi.InnerXml, @"<B210>(?<value>.*)</B210>")?.Groups["value"].Value,
                    B220 = Regex.Match(extractedNodes.Sdopi.InnerXml, @"<B220><date>(?<value>.*)</date></B220>")?.Groups["value"].Value
                };

                var bodyElements = new List<RecordElements.SudCode2>();
                foreach (XmlNode rec in extractedNodes.RuNotification)
                {

                    bodyElements.Add(new RecordElements.SudCode2
                    {
                        B110 = Regex.Match(rec.InnerXml, @"<B110>(?<value>.*)</B110>", RegexOptions.IgnoreCase)?.Groups["value"].Value,
                        B405i = Regex.Match(rec.InnerXml, @"<ru-B405i>(?<value>.*)</ru-B405i>", RegexOptions.IgnoreCase)?.Groups["value"].Value,
                        B460i = Regex.Match(rec.InnerXml, @"<ru-B460i><date>(?<value>.*)</date></ru-B460i>", RegexOptions.IgnoreCase)?.Groups["value"].Value,
                        B731i = Regex.Matches(rec.InnerXml, @"<ru-B731i><ru-name-text>(?<value>[^</]+)</ru-name-text></ru-B731i>", RegexOptions.IgnoreCase).Cast<Match>().Select(x => x.Groups["value"].Value).ToList(),
                        B903i = Regex.Match(rec.InnerXml, @"<ru-B903i>(?<value>.*)</ru-B903i>", RegexOptions.IgnoreCase)?.Groups["value"].Value,
                        B980i = Regex.Match(rec.InnerXml, @"<ru-B980i>(?<value>.*)</ru-B980i>", RegexOptions.IgnoreCase)?.Groups["value"].Value
                    });
                }

                foreach (var rec in bodyElements)
                {
                    var completeRecord = new RecordElements.SudCode2();

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

                    completeRecord.B405i = rec.B405i;
                    completeRecord.B460i = rec.B460i;
                    completeRecord.B731i = rec.B731i;
                    completeRecord.B903i = rec.B903i;
                    completeRecord.B980i = rec.B980i;

                    if (completeRecord.B903i == "PD4A")
                        sub2output.Add(completeRecord);
                }
            }
            return sub2output.Count > 0 ? sub2output : null;
        }
        private static List<RecordElements.SudCode7> Sub7Mapper(List<XDocument> sub7Elements)
        {
            var sub7output = new List<RecordElements.SudCode7>();
            foreach (var xmlFile in sub7Elements)
            {
                var extractedNodes = GetNodesFromXml(xmlFile);
                var headerElements = new RecordElements.SudCode7
                {
                    B110 = Regex.Match(extractedNodes.Sdopi.InnerXml, @"<B110>(?<value>.*)</B110>")?.Groups["value"].Value,
                    B130 = Regex.Match(extractedNodes.Sdopi.InnerXml, @"<B130>(?<value>.*)</B130>")?.Groups["value"].Value,
                    B140 = Regex.Match(extractedNodes.Sdopi.InnerXml, @"<B140><date>(?<value>.*)</date></B140>")?.Groups["value"].Value,
                    B190 = Regex.Match(extractedNodes.Sdopi.InnerXml, @"<B190>(?<value>.*)</B190>")?.Groups["value"].Value,
                    B210 = Regex.Match(extractedNodes.Sdopi.InnerXml, @"<B210>(?<value>.*)</B210>")?.Groups["value"].Value,
                    B220 = Regex.Match(extractedNodes.Sdopi.InnerXml, @"<B220><date>(?<value>.*)</date></B220>")?.Groups["value"].Value
                };

                var bodyElements = new List<RecordElements.SudCode7>();
                foreach (XmlNode rec in extractedNodes.RuNotification)
                {

                    bodyElements.Add(new RecordElements.SudCode7
                    {
                        B110 = Regex.Match(rec.InnerXml, @"<B110>(?<value>.*)</B110>", RegexOptions.IgnoreCase)?.Groups["value"].Value,
                        B405i = Regex.Match(rec.InnerXml, @"<ru-B405i>(?<value>.*)</ru-B405i>", RegexOptions.IgnoreCase)?.Groups["value"].Value,
                        B460i = Regex.Match(rec.InnerXml, @"<ru-B460i><date>(?<value>.*)</date></ru-B460i>", RegexOptions.IgnoreCase)?.Groups["value"].Value,
                        B731i = Regex.Matches(rec.InnerXml, @"<ru-B731i><ru-name-text>(?<value>[^</]+)</ru-name-text></ru-B731i>", RegexOptions.IgnoreCase).Cast<Match>().Select(x => x.Groups["value"].Value).ToList(),
                        B903i = Regex.Match(rec.InnerXml, @"<ru-B903i>(?<value>.*)</ru-B903i>", RegexOptions.IgnoreCase)?.Groups["value"].Value,
                        B980i = Regex.Match(rec.InnerXml, @"<ru-B980i>(?<value>.*)</ru-B980i>", RegexOptions.IgnoreCase)?.Groups["value"].Value
                    });
                }

                foreach (var rec in bodyElements)
                {
                    var completeRecord = new RecordElements.SudCode7();

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

                    completeRecord.B405i = rec.B405i;
                    completeRecord.B460i = rec.B460i;
                    completeRecord.B731i = rec.B731i;
                    completeRecord.B903i = rec.B903i;
                    completeRecord.B980i = rec.B980i;

                    if (completeRecord.B903i == "PD9K")
                        sub7output.Add(completeRecord);
                }
            }
            return sub7output.Count > 0 ? sub7output : null;
        }
    }
}
