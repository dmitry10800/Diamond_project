using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_AP
{
    class Diamond_AP_main
    {
        public static string CurrentFileName;

        private static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\AP\20190829");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            XElement tet;
            List<XElement> applicationsFiled = null;
            List<XElement> applicationsPendingGrant = null;
            List<XElement> utilityModelAppFiled = null;
            List<XElement> patentsAssigned = null;
            List<XElement> patentAppAssigned = null;
            List<XElement> utilityModelAppPendingReg = null;

            foreach (var tetFile in files)
            {
                CurrentFileName = tetFile;
                tet = XElement.Load(tetFile);
                //currentElement = null;

                /*TETML elements*/
                /*Patent Applications Filed*/
                applicationsFiled = tet.Descendants().Where(d => d.Name.LocalName == "Text" && d.Value != "PATENTS")
                    .SkipWhile(e => !e.Value.StartsWith("Patent\nApplications Filed"))
                    .TakeWhile(e => !e.Value.StartsWith("■") && !e.Value.StartsWith("Patent Applications Renewed"))
                    .Where(e => e.Value != "Patent\nApplications Filed"
                    && e.Value != "Patent\nApplications\nFiled(Contd.)"
                    && e.Value != "Patent\nApplications\nFiled (Contd.)"
                    && !e.Value.StartsWith("Page")
                    && !e.Value.StartsWith("AP; ARIPO Journal; Vol.")
                    && e.Value != "▶")
                    .ToList();
                /*Patent Applications Pending Grant*/
                applicationsPendingGrant = tet.Descendants().Where(d => d.Name.LocalName == "Text" && d.Value != "PATENTS")
                    .SkipWhile(e => !e.Value.StartsWith("Patent\nApplications\nPending Grant"))
                    .TakeWhile(e => !e.Value.StartsWith("■") && !e.Value.StartsWith("Patents Granted"))
                    .Where(e => e.Value != "Patent\nApplications\nPending Grant" && e.Value != "Patent\nApplications\nPending Grant\n(Contd.)"
                    && !e.Value.StartsWith("Page") && !e.Value.StartsWith("AP; ARIPO Journal; Vol."))
                    .ToList();
                /*Utility Model Applications Filed*/
                utilityModelAppFiled = tet.Descendants().Where(d => d.Name.LocalName == "Text" && d.Value != "UTILITY MODELS")
                    .SkipWhile(e => !e.Value.StartsWith("Utility Model\nApplications Filed"))
                    .TakeWhile(e => !e.Value.StartsWith("■") && !e.Value.StartsWith("Utility Model Applications Renewed"))
                    .Where(e => e.Value != "Utility Model\nApplications Filed" && e.Value != "Utility Model\nApplications Filed\n(Contd.)"
                    && !e.Value.StartsWith("Page") && !e.Value.StartsWith("AP; ARIPO Journal; Vol."))
                    .ToList();
                /*Patents Assigned*/
                patentsAssigned = tet.Descendants().Where(d => d.Name.LocalName == "Text" && d.Value != "UTILITY MODELS")
                    .SkipWhile(e => !e.Value.StartsWith("Patents\nAssigned"))
                    .TakeWhile(e => !e.Value.StartsWith("■") && !e.Value.StartsWith("UTILITY MODELS"))
                    .Where(e => e.Value != "Patents\nAssigned" && e.Value != "Patents\nAssigned\n(Contd.)"
                    && !e.Value.StartsWith("Page") && !e.Value.StartsWith("AP; ARIPO Journal; Vol."))
                    .ToList();
                /*Patent Applications Assigned*/
                patentAppAssigned = tet.Descendants().Where(d => d.Name.LocalName == "Text")
                    .SkipWhile(e => !e.Value.StartsWith("Patent\nApplications\nAssigned"))
                    .TakeWhile(e => !e.Value.StartsWith("■") && !e.Value.StartsWith("Patent\nApplications\nPending Grant") && !e.Value.StartsWith("Patents Granted"))
                    .Where(e => e.Value != "Patent\nApplications\nAssigned" && e.Value != "Patent\nApplications\nAssigned\n(Contd.)"
                    && !e.Value.StartsWith("Page") && !e.Value.StartsWith("AP; ARIPO Journal; Vol."))
                    .ToList();
                /*Utility Model Applications Pending Registration*/
                utilityModelAppPendingReg = tet.Descendants().Where(d => d.Name.LocalName == "Text")
                    .SkipWhile(e => !e.Value.StartsWith("Utility Model\nApplicationsn\nPending\nRegistration"))
                    .TakeWhile(e => !e.Value.StartsWith("■") && !e.Value.StartsWith("Patent\nApplications\nPending Grant") && !e.Value.StartsWith("Utility Model Applications Renewed"))
                    .Where(e => e.Value != "Utility Model\nApplicationsn\nPending\nRegistration" && e.Value != "Utility Model\nApplicationsn\nPending\nRegistration\n(Contd.)"
                    && !e.Value.StartsWith("Page") && !e.Value.StartsWith("AP; ARIPO Journal; Vol."))
                    .ToList();
                Console.WriteLine("lal");
            }
            /*Chapter Applications Filed processing*/
            if (applicationsFiled != null)
            {
                ProcessAppFiled appFiled = new ProcessAppFiled();
                List<ProcessAppFiled.ElementOut> el = appFiled.OutputValue(applicationsFiled);
                var legalStatusEvents = ConvertToDiamond.AppFiledConvertation(el);
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
            /*Chapter Applications Pending Grant*/
            if (applicationsPendingGrant != null)
            {
                ProcessAppPendingGrant appPendingGrand = new ProcessAppPendingGrant();
                List<ProcessAppPendingGrant.ElementOut> el = appPendingGrand.OutputValue(applicationsPendingGrant);
                var legalStatusEvents = ConvertToDiamond.AppPendingGrantConvertation(el);
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
            /*Chapter Utility Model Applications Filed*/
            if (utilityModelAppFiled != null)
            {
                ProcessUtilityModelAppFiled umAppFiled = new ProcessUtilityModelAppFiled();
                List<ProcessUtilityModelAppFiled.ElementOut> el = umAppFiled.OutputValue(utilityModelAppFiled);
                var legalStatusEvents = ConvertToDiamond.UmAppFiledConvertation(el);
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
            /*Chapter Patents Assigned*/
            if (patentsAssigned != null)
            {
                ProcessPatentsAssigned patAssigned = new ProcessPatentsAssigned();
                List<ProcessPatentsAssigned.ElementOut> el = patAssigned.OutputValue(patentsAssigned);
                var legalStatusEvents = ConvertToDiamond.PatentAssignedConvertation(el);
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
            /*Chapter Patent Applications Assigned*/
            if (patentAppAssigned != null)
            {
                ProcessPatentAppAssigned patAppAssigned = new ProcessPatentAppAssigned();
                List<ProcessPatentAppAssigned.ElementOut> el = patAppAssigned.OutputValue(patentAppAssigned);
                var legalStatusEvents = ConvertToDiamond.PatentAppAssignedConvertation(el);
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
            /*Chapter Utility Model Applications Pending Registration*/
            if (utilityModelAppPendingReg != null)
            {
                ProcessUtilityModelAppPendingReg umAppPendingReg = new ProcessUtilityModelAppPendingReg();
                List<ProcessUtilityModelAppPendingReg.ElementOut> el = umAppPendingReg.OutputValue(utilityModelAppPendingReg);
                var legalStatusEvents = ConvertToDiamond.UmAppPendingRegConvertation(el);
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
