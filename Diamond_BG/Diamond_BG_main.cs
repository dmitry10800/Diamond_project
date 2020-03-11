using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_BG
{
    class Diamond_BG_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\TET_DEV\Diamond\BG\20200311\BG_20200228_02(2)");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> applEuropeanPatents = null;
            List<XElement> grantedEuropeanPatents = null;
            List<XElement> publishedAppForInventions = null;
            List<XElement> grantedPatentsForInventions = null;
            List<XElement> grantedCertForUM = null;

            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                //currentElement = null;

                /*TETML elements*/
                /*Granted European patents valid in the Republic of Bulgaria 5 subcode*/
                grantedEuropeanPatents = tet.Descendants().Where(d => d.Name.LocalName == "Text" && d.Value != "PATENTS")
                    .SkipWhile(e => !e.Value.StartsWith("ИЗДАДЕНИ ЕВРОПЕЙСКИ ПАТЕНТИ, ЗА КОИТО\nЕ ПРЕДСТАВЕН ПРЕВОД НА ОПИСАНИЕТО"))
                    .TakeWhile(e => !e.Value.StartsWith("ПУБЛИКУВАНЕ НА ОПИСАНИЯ НА ИЗДАДЕНИ\nЕВРОПЕЙСКИ ПАТЕНТИ СЛЕД ПРОЦЕДУРА ПО\nОПОЗИЦИЯ, СЪГЛАСНО ЧЛ. 103 ОТ ЕПК")
                    && !e.Value.StartsWith("ПУБЛИКУВАНЕ НА ОПИСАНИЯ НА ИЗДАДЕНИ\nЕВРОПЕЙСКИ ПАТЕНТИ, С ДЕЙСТВИЕ НА")
                    && !e.Value.StartsWith("ПУБЛИКУВАНЕ НА ОПИСАНИЯ КЪМ СВИДЕТЕЛСТВА\nЗА РЕГИСТРАЦИЯ НА ПОЛЕЗНИ МОДЕЛИ")
                    && !e.Value.StartsWith("СЕРТИФИКАТИ ЗА ДОПЪЛНИТЕЛНА\nЗАКРИЛА НА ЛЕКАРСТВЕНИ\nПРОДУКТИ И ПРОДУКТИ ЗА\nРАСТИТЕЛНА ЗАЩИТА")
                    )
                    .Where(e => !e.Value.StartsWith("ИЗДАДЕНИ ЕВРОПЕЙСКИ ПАТЕНТИ, ЗА КОИТО\nЕ ПРЕДСТАВЕН ПРЕВОД НА ОПИСАНИЕТО")
                    && e.Value != "ЗАЯВКИ ЗА ЕВРОПЕЙСКИ ПАТЕНТ И\nИЗДАДЕНИ ЕВРОПЕЙСКИ ПАТЕНТИ,\nС ДЕЙСТВИЕ НА ТЕРИТОРИЯТА НА\nРЕПУБЛИКА БЪЛГАРИЯ"
                    && !e.Value.StartsWith("РАЗДЕЛ")
                    )
                    .ToList();
                /*4 subcode*/
                applEuropeanPatents = tet.Descendants().Where(d => d.Name.LocalName == "Text" && d.Value != "PATENTS")
                    .SkipWhile(e => !e.Value.StartsWith("ЗАЯВКИ ЗА ЕВРОПЕЙСКИ ПАТЕНТ, ЗА КОИТО Е\nПРЕДСТАВЕН ПРЕВОД НА ПАТЕНТНИТЕ ПРЕТЕНЦИ\nИ НА БЪЛГАРСКИ ЕЗИК"))
                    .TakeWhile(e => !e.Value.StartsWith("ИЗДАДЕНИ ЕВРОПЕЙСКИ ПАТЕНТИ, ЗА КОИТО")
                    && !e.Value.StartsWith("ПУБЛИКУВАНЕ НА ОПИСАНИЯ НА ИЗДАДЕНИ"))
                    .Where(e => e.Value != "ЗАЯВКИ ЗА ЕВРОПЕЙСКИ ПАТЕНТ, ЗА КОИТО Е\nПРЕДСТАВЕН ПРЕВОД НА ПАТЕНТНИТЕ ПРЕТЕНЦИ\nИ НА БЪЛГАРСКИ ЕЗИК"
                    && e.Value != "ЗАЯВКИ ЗА ЕВРОПЕЙСКИ ПАТЕНТ И\nИЗДАДЕНИ ЕВРОПЕЙСКИ ПАТЕНТИ,\nС ДЕЙСТВИЕ НА ТЕРИТОРИЯТА НА\nРЕПУБЛИКА БЪЛГАРИЯ"
                    && !e.Value.StartsWith("РАЗДЕЛ"))
                    .ToList();
                
                /*Published applications for inventions 1 subcode*/
                publishedAppForInventions = tet.Descendants().Where(d => d.Name.LocalName == "Text" && d.Value != "PATENTS")
                    .SkipWhile(e => !e.Value.StartsWith("ПУБЛИКУВАНИ ЗАЯВКИ ЗА ИЗОБРЕТЕНИЯ"))
                    .TakeWhile(e => !e.Value.StartsWith("ИЗДАДЕНИ ПАТЕНТИ ЗА ИЗОБРЕТЕНИЯ")
                    && !e.Value.StartsWith("Patent Applications Renewed")
                    && !e.Value.StartsWith("ПУБЛИКУВАНЕ НА ОПИСАНИЯ НА ИЗДАДЕНИ\nПАТЕНТИ ЗА ИЗОБРЕТЕНИЯ")
                    && !e.Value.StartsWith("СЕРТИФИКАТИ ЗА ДОПЪЛНИТЕЛНА\nЗАКРИЛА НА ЛЕКАРСТВЕНИ\nПРОДУКТИ И ПРОДУКТИ ЗА\nРАСТИТЕЛНА ЗАЩИТА"))
                    .Where(e => e.Value != "ПУБЛИКУВАНИ ЗАЯВКИ ЗА ИЗОБРЕТЕНИЯ"
                    && e.Value != "И З О Б Р Е Т Е Н И Я"
                    && !e.Value.StartsWith("РАЗДЕЛ"))
                    .ToList();

                /*3 subcode*/
                grantedPatentsForInventions = tet.Descendants().Where(d => d.Name.LocalName == "Text" && d.Value != "PATENTS")
                    .SkipWhile(e => !e.Value.StartsWith("ИЗДАДЕНИ ПАТЕНТИ ЗА ИЗОБРЕТЕНИЯ"))
                    .TakeWhile(e => !e.Value.StartsWith("ПУБЛИКУВАНЕ НА ОПИСАНИЯ НА ИЗДАДЕНИ\nПАТЕНТИ ЗА ИЗОБРЕТЕНИЯ")
                    && !e.Value.StartsWith("ПРОМИШЛЕНИ ДИЗАЙНИ"))
                    .Where(e => e.Value != "ИЗДАДЕНИ ПАТЕНТИ ЗА ИЗОБРЕТЕНИЯ"
                    && e.Value != "И З О Б Р Е Т Е Н И Я"
                    && !e.Value.StartsWith("РАЗДЕЛ"))
                    .ToList();

                /*7 subcode*/
                grantedCertForUM = tet.Descendants().Where(d => d.Name.LocalName == "Text" && d.Value != "PATENTS")
                    .SkipWhile(e => !e.Value.StartsWith("ИЗДАДЕНИ СВИДЕТЕЛСТВА ЗА РЕГИСТРАЦИЯ НА\nПОЛЕЗНИ МОДЕЛИ"))
                    .TakeWhile(e => !e.Value.StartsWith("ПУБЛИКУВАНЕ НА ОПИСАНИЯ КЪМ СВИДЕТЕЛСТВА\nЗА РЕГИСТРАЦИЯ НА ПОЛЕЗНИ МОДЕЛИ"))
                    .Where(e => !e.Value.StartsWith("ИЗДАДЕНИ СВИДЕТЕЛСТВА ЗА\nРЕГИСТРАЦИЯ НА ПОЛЕЗНИ МОДЕЛИ")
                    && e.Value != "И З О Б Р Е Т Е Н И Я"
                    && !e.Value.StartsWith("РАЗДЕЛ"))
                    .ToList();

                //Console.WriteLine("lal");

                /*Applications 4 subcode*/
               /* if (applEuropeanPatents != null && applEuropeanPatents.Count() > 0)
                {
                    ProcessEuropeanPatentApplications appl = new ProcessEuropeanPatentApplications();
                    List<ProcessEuropeanPatentApplications.ElementOut> el = appl.OutputValue(applEuropeanPatents);
                    var legalStatusEvents = ConvertToDiamond.ApplEuropeanPatentsConvertation(el);
                    try
                    {
                        //Methods.SendToDiamond(legalStatusEvents);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Sending error");
                        throw;
                    }
                }*/

                /*Granted Patents For Inventions 3 subcode*/
                if (grantedPatentsForInventions != null && grantedPatentsForInventions.Count() > 0)
                {
                    ProcessGrantedPatentsForInventions grantedEP = new ProcessGrantedPatentsForInventions();
                    List<ProcessGrantedPatentsForInventions.ElementOut> el = grantedEP.OutputValue(grantedPatentsForInventions);
                    var legalStatusEvents = ConvertToDiamond.GrantedPatentsForInventionsConvertation(el);
                    try
                    {
                        Methods.SendToDiamond(legalStatusEvents);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Sending error");
                        throw;
                    }
                }

                /*Granted Certificates for Register Utility Models 7 subcode*/
                if (grantedCertForUM != null && grantedCertForUM.Count() > 0)
                {
                    ProcessGrantedCertificatesForRegUtilityModels grantedCertUM = new ProcessGrantedCertificatesForRegUtilityModels();
                    List<ProcessGrantedCertificatesForRegUtilityModels.ElementOut> el = grantedCertUM.OutputValue(grantedCertForUM);
                    var legalStatusEvents = ConvertToDiamond.GrantedCertificatesForRegUMConvertation(el);
                    try
                    {
                        Methods.SendToDiamond(legalStatusEvents);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Sending error");
                        throw;
                    }
                }

                /*Granted European patents valid in the Republic of Bulgaria 5 subcode*/
                if (grantedEuropeanPatents != null && grantedEuropeanPatents.Count() > 0)
                {
                    ProcessGrantedEuropeanPatents grantedEP = new ProcessGrantedEuropeanPatents();
                    List<ProcessGrantedEuropeanPatents.ElementOut> el = grantedEP.OutputValue(grantedEuropeanPatents);
                    var legalStatusEvents = ConvertToDiamond.GrantedEuropeanPatentsConvertation(el);
                    try
                    {
                        Methods.SendToDiamond(legalStatusEvents);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Sending error");
                        throw;
                    }
                }

                /*Granted European patents valid in the Republic of Bulgaria 1 subcode*/
                if (publishedAppForInventions != null && publishedAppForInventions.Count() > 0)
                {
                    ProcessPublishedApplicationsForInventions publApp = new ProcessPublishedApplicationsForInventions();
                    List<ProcessPublishedApplicationsForInventions.ElementOut> el = publApp.OutputValue(publishedAppForInventions);
                    var legalStatusEvents = ConvertToDiamond.PublishedAppForInventionConvertation(el);
                    try
                    {
                        Methods.SendToDiamond(legalStatusEvents);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Sending error");
                        throw;
                    }
                }
            }
        }
    }
}
