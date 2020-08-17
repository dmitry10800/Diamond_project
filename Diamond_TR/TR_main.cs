using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Diamond_TR
{
    class TR_main
    {
        public static DirectoryInfo PathToTeml = new DirectoryInfo(@"C:\Users\Razrab\Desktop\TR\");
        public static FileInfo currentFile = null;
        public static string currentFileName = null;
        static void Main(string[] args)
        {
            var files = new List<string>();
            foreach (FileInfo file in PathToTeml.GetFiles("*.tetml", SearchOption.AllDirectories))
                files.Add(file.FullName);

            XElement elem = null;

            List<XElement> tmpList = new List<XElement>();
            List<XElement> sub10 = new List<XElement>();
            List<XElement> sub13 = new List<XElement>();
            List<XElement> sub16 = new List<XElement>();
            List<XElement> sub17 = new List<XElement>();
            List<XElement> sub30 = new List<XElement>();
            List<XElement> sub31 = new List<XElement>();
            List<XElement> sub37 = new List<XElement>();
            List<XElement> sub39 = new List<XElement>();

            foreach (var file in files)
            {
                currentFile = new FileInfo(file);
                currentFileName = Path.GetFileName(file);
                elem = XElement.Load(file);

                tmpList = elem.Descendants().Where(e => e.Name.LocalName == "Text")
                    .ToList();

                for (int i = 0; i < tmpList.Count; i++)
                {
                    var tmp = i;

                    if (tmpList[tmp].Value.Contains("YILLIK ÜCRETLERİN ÖDENMEMESİ NEDENİYLE GEÇERSİZ SAYILAN BAŞVURULAR"))
                    {
                        do
                        {
                            sub10.Add(tmpList[tmp]);
                            tmp++;
                        } while (tmp < tmpList.Count - 1 && !tmpList[tmp].Value.Contains("YENİDEN GEÇERLİLİK KAZANAN PATENT/FAYDALI MODELLER"));
                    }

                    if (tmpList[tmp].Value.Contains("YILLIK ÜCRETLERİN ÖDENMEMESİ NEDENİYLE HAKKI SONA EREN PATENT"))
                    {
                        do
                        {
                            sub13.Add(tmpList[tmp]);
                            tmp++;
                            //if (tmpList[tmp].Value.Contains("TARİFNAMESİNDE DEĞİŞİKLİK YAPILAN PATEN T/FAYDALI MODEL BAŞVURULARI"))
                            //    tmp = tmpList.Count;
                        } while (tmp < tmpList.Count - 1 && !tmpList[tmp].Value.Contains("YENİDEN GEÇERLİLİK KAZANAN PATENT"));
                    }

                    if (tmpList[tmp].Value.Contains("GEÇERSİZ SAYILAN"))
                    {
                        do
                        {
                            sub16.Add(tmpList[tmp]);
                            tmp++;
                        } while (tmp < tmpList.Count - 1 && !tmpList[tmp].Value.Contains("GERİ ÇEKİLEN PATENT"));
                    }

                    if (tmpList[tmp].Value.Contains("GERİ ÇEKMİŞ SAYILAN PATENT"))
                    {
                        do
                        {
                            sub17.Add(tmpList[tmp]);
                            tmp++;
                            //if (tmpList[tmp].Value.Contains("MÜLGA 551 SAYILI KHK"))
                            //    tmp = tmpList.Count;
                        } while (tmp < tmpList.Count - 1 && !tmpList[tmp].Value.Contains("MÜLGA 551 SAYILI KHK"));
                    }

                    if (tmpList[tmp].Value.Contains("6769 SAYILI SMK'NIN 96 NCI MADDE HÜKMÜ UYARINCA ARAŞTIRMA RAPORU"))
                    {
                        do
                        {
                            sub30.Add(tmpList[tmp]);
                            tmp++;
                            //if (tmpList[tmp].Value.Contains("6769 SAYILI SMK&apos;NIN 143 NCÜ MADDE HÜKMÜ UYARINCA"))
                            //    tmp = tmpList.Count;
                        } while (tmp < tmpList.Count - 1 && !tmpList[tmp].Value.Contains("6769 SAYILI SM KNIN 143 NCÜ MADDE HÜKMÜ UYARINCA ARAŞTIRMA RAPORU"));
                    }

                    if (tmpList[tmp].Value.Contains("6769 SAYILI SM KNIN 143 NCÜ MADDE HÜKMÜ UYARINCA ARAŞTIRMA RAPORU"))
                    {
                        do
                        {
                            sub31.Add(tmpList[tmp]);
                            ++tmp;
                            //if (tmpList[tmp].Value.Contains("MÜLGA 551 SAYILI KHK&apos;NİN 57 NCİ MADDE HÜK MÜ UYARINCA"))
                            //    tmp = tmpList.Count;
                        } while (tmp < tmpList.Count - 1 && !tmpList[tmp].Value.Contains("MÜLGA 551 SAYILI KHK"));
                    }

                    if (tmpList[tmp].Value.Contains("NCİ, VE 8 İNCİ FIKRALARI UYARINCA KULLANM A BEYANI VERİLEN BAŞVURU VEYA"))
                    {
                        do
                        {
                            sub37.Add(tmpList[tmp]);
                            ++tmp;
                            //if (tmpList[tmp].Value.Contains("6769 SAYILI SMK&apos;NIN UYGULANMASINA DAİR Y ÖNETMENLİĞİN 117 NCİ MADDESİ 7"))
                            //    tmp = tmpList.Count;
                        } while (tmp < tmpList.Count - 1 && !tmpList[tmp].Value.StartsWith("NCİ, VE 8 İNCİ FIKRALARI UYARINCA KULLAN MAMA BEYANI VERİLEN BAŞVURU"));
                    }

                    var value = tmpList[tmp].Value;

                    if (value.Contains("6769 SAYILI"))
                    {

                    }

                    if (tmpList[tmp].Value.Contains("6769 SAYILI SMK'NIN UYGULANMASINA DAİR YÖNETMENLİĞİN 117 NCİ MADDESİ 7 NCİ, VE 8 İNCİ FIKRALARI UYARINCA KULLANMA/KULLANMAMA"))
                    {
                        do
                        {
                            sub39.Add(tmpList[tmp]);
                            ++tmp;
                        } while (tmp < tmpList.Count - 1 && !tmpList[tmp].Value.StartsWith("YILLIK ÜCRETLERİN ÖDENMEMESİ NEDENİYLE GEÇERSİZ SAYILAN BAŞVURULAR"));
                    }
                }

                if (sub10.Count > 0)
                {
                    var proccessedRecords = Processing.AllSubsProcess(sub10);
                    var legalEvents = DiamondConverter.Sub10Convert(proccessedRecords);
                    DiamondSender.SendToDiamond(legalEvents);
                }

                if (sub13.Count > 0)
                {
                    var processedRecords = Processing.AllSubsProcess(sub13);
                    var legalEvents = DiamondConverter.Sub13Convert(processedRecords);
                    DiamondSender.SendToDiamond(legalEvents);
                }

                if (sub16.Count > 0)
                {
                    var processedRecords = Processing.AllSubsProcess(sub16);
                    var legalEvents = DiamondConverter.Sub16Convert(processedRecords);
                    DiamondSender.SendToDiamond(legalEvents);
                }

                if (sub17.Count > 0)
                {
                    var processedRecords = Processing.AllSubsProcess(sub17);
                    var legalEvents = DiamondConverter.Sub17Convert(processedRecords);
                    DiamondSender.SendToDiamond(legalEvents);
                }

                if (sub30.Count > 0)
                {
                    var processedRecords = Processing.AllSubsProcess(sub30);
                    var legalEvents = DiamondConverter.Sub30Convert(processedRecords);
                    DiamondSender.SendToDiamond(legalEvents);
                }

                if (sub31.Count > 0)
                {
                    var processedRecords = Processing.AllSubsProcess(sub31);
                    var legalEvents = DiamondConverter.Sub31Convert(processedRecords);
                    DiamondSender.SendToDiamond(legalEvents);
                }

                //if (sub37.Count > 0)
                //{
                //    var processedRecords = Processing.Sub37Process(sub37);
                //    var legalEvents = DiamondConverter.Sub37Convert(processedRecords);
                //    DiamondSender.SendToDiamond(legalEvents);
                //}

                if (sub39.Count > 0)
                {
                    var processedRecords = Processing.AllSubsProcess(sub39);
                    var legalEvents = DiamondConverter.Sub39Convert(processedRecords);
                    DiamondSender.SendToDiamond(legalEvents);
                }
            }
        }
    }
}
