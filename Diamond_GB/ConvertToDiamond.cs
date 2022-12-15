using System;
using System.Collections.Generic;
using System.IO;
using Integration;

namespace Diamond_GB
{
    class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub8(List<OutElements.Sub8> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            string dateFromName = Methods.GetDateFromGazette(GB_main.CurrentFileName);
            if (elementOuts != null)
            {
                int leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = Path.GetFileName(GB_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "8";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "FA";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "GB";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Application.Number = record.AppNumber;
                    /*Notes*/
                    legalEvent.LegalEvent = new LegalEvent { Date = dateFromName };
                    /**********************/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                    Console.WriteLine("Record\t" + legalEvent.Biblio.Publication.Number + "\t:\tconverted successfully.");
                }
            }
            Console.WriteLine("Subcode 8 converted successfully.\tPatents:\t" + fullGazetteInfo.Count + "\n");
            return fullGazetteInfo;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub9(List<OutElements.Sub9> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            string dateFromName = Methods.GetDateFromGazette(GB_main.CurrentFileName);
            if (elementOuts != null)
            {
                int leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = Path.GetFileName(GB_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "9";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "FA";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "GB";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Application.Number = record.AppNumber;
                    biblioData.Publication.Number = record.PubNumber;
                    /*Notes*/
                    legalEvent.LegalEvent = new LegalEvent { Date = dateFromName };
                    /**********************/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                    Console.WriteLine("Record\t" + legalEvent.Biblio.Publication.Number + "\t:\tconverted successfully.");
                }
            }
            Console.WriteLine("Subcode 9 converted successfully.\tPatents:\t" + fullGazetteInfo.Count + "\n");
            return fullGazetteInfo;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub10(List<OutElements.Sub10> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            if (elementOuts != null)
            {
                int leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = Path.GetFileName(GB_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "10";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "MF";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "GB";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Application.Number = record.AppNumber;
                    biblioData.Publication.Number = record.PubNumber;
                    /*Notes*/
                    legalEvent.LegalEvent = new LegalEvent { Date = record.LeDate };
                    /**********************/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                    Console.WriteLine("Record\t" + legalEvent.Biblio.Publication.Number + "\t:\tconverted successfully.");
                }
            }
            Console.WriteLine("Subcode 10 converted successfully.\tPatents:\t" + fullGazetteInfo.Count + "\n");
            return fullGazetteInfo;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub11(List<OutElements.Sub11> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            if (elementOuts != null)
            {
                int leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = Path.GetFileName(GB_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "11";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "MK";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "GB";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.PubNumber;
                    /*Notes*/
                    legalEvent.LegalEvent = new LegalEvent { Date = record.LeDate };
                    /**********************/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                    Console.WriteLine("Record\t" + legalEvent.Biblio.Publication.Number + "\t:\tconverted successfully.");
                }
            }
            Console.WriteLine("Subcode 11 converted successfully.\tPatents:\t" + fullGazetteInfo.Count + "\n");
            return fullGazetteInfo;
        }
        public static List<Diamond.Core.Models.LegalStatusEvent> Sub12(List<OutElements.Sub12> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            if (elementOuts != null)
            {
                int leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = Path.GetFileName(GB_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "12";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "MK";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "GB";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.PubNumber;
                    /*Notes*/
                    legalEvent.LegalEvent = new LegalEvent { Date = record.LeDate };
                    /**********************/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                    Console.WriteLine("Record\t" + legalEvent.Biblio.Publication.Number + "\t:\tconverted successfully.");
                }
            }
            Console.WriteLine("Subcode 12 converted successfully.\tPatents:\t" + fullGazetteInfo.Count + "\n");
            return fullGazetteInfo;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub42(List<OutElements.Sub42> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            if (elementOuts != null)
            {
                int leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = Path.GetFileName(GB_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "42";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "MM";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "GB";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.PubNumber;
                    /*Notes*/
                    legalEvent.LegalEvent = new LegalEvent { Date = record.LeDate };
                    /**********************/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                    Console.WriteLine("Record\t" + legalEvent.Biblio.Publication.Number + "\t:\tconverted successfully.");
                }
            }
            Console.WriteLine("Subcode 42 converted successfully.\tPatents:\t"+fullGazetteInfo.Count+"\n");
            return fullGazetteInfo;
        }

        public static List<Diamond.Core.Models.LegalStatusEvent> Sub43(List<OutElements.Sub43> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            if (elementOuts != null)
            {
                int leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    legalEvent.GazetteName = Path.GetFileName(GB_main.CurrentFileName.Replace(".tetml", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "43";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "MM";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "GB";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    Biblio biblioData = new Biblio();
                    /*Elements output*/
                    biblioData.Publication.Number = record.PubNumber;
                    /*Notes*/
                    legalEvent.LegalEvent = new LegalEvent { Date = record.LeDate };
                    /**********************/
                    legalEvent.Biblio = biblioData;
                    fullGazetteInfo.Add(legalEvent);
                    Console.WriteLine("Record\t" + legalEvent.Biblio.Publication.Number + "\t:\tconverted successfully.");
                }
            }
            Console.WriteLine("Subcode 43 converted successfully.\tPatents:\t" + fullGazetteInfo.Count + "\n");
            return fullGazetteInfo;
        }
    }
}
