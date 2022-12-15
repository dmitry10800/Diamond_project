using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Integration;
using Newtonsoft.Json;

namespace Diamond_ID_patent_application
{
    class Diamond_ID_patent_application
    {
        private static readonly string I11 = "(11) No Pengumuman :";
        private static readonly string I13 = "(13)";
        private static readonly string I20 = "(20) RI Permohonan Paten";
        private static readonly string I21 = "(21) No. Permohonan Paten :";
        private static readonly string I22 = "(22) Tanggal Penerimaan Permohonan Paten :";
        private static readonly string I30 = "(30) Data Prioritas :";
        private static readonly string I43 = "(43) Tanggal Pengumuman Paten :";
        private static readonly string I51 = "(51) I.P.C :";
        private static readonly string I54 = "(54) Judul Invensi :";
        private static readonly string I57 = "(57) Abstrak :";
        private static readonly string I71 = "(71) Nama dan Alamat yang Mengajukan Permohonan Paten :";
        private static readonly string I72 = "(72) Nama Inventor :";
        private static readonly string I74 = "(74) Nama dan Alamat Konsultan Paten :";

        class ElementOut
        {
            public string I11 { get; set; }
            public string I13 { get; set; }
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string I43 { get; set; }
            public string I51 { get; set; }
            public string I54 { get; set; }
            public string I57 { get; set; }
            public string I71 { get; set; }
            public string[] I72N { get; set; }
            public string[] I72C { get; set; }
            public string I74 { get; set; }
            public string[] I31 { get; set; }
            public string[] I32 { get; set; }
            public string[] I33 { get; set; }
        }
        static string[] RecSplit(string recString)
        {
            string[] splittedRecord = null;
            string tempStrC = recString/*.Replace("\n", " ")*/;
            if (recString != "")
            {
                //if (recString.Contains("\n"))
                //{
                //    recString = recString.Replace("\n", " ");
                //}
                Regex regexPatOne = new Regex(@"\(\d{2}\)\s", RegexOptions.IgnoreCase);
                MatchCollection matchesClass = regexPatOne.Matches(recString);
                if (matchesClass.Count > 0)
                {
                    foreach (Match matchC in matchesClass)
                    {
                        tempStrC = tempStrC.Replace(matchC.Value, "***" + matchC.Value);
                    }
                }
                /*Splitting record*/
                splittedRecord = tempStrC.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries);
            }
            return splittedRecord;
        }
        /*Date*/
        static string DateNormalize(string tmpDate)
        {
            string swapDate = tmpDate;
            string[] splitDate = null;
            string month = null;
            if (tmpDate.Contains(" "))
            {
                splitDate = tmpDate.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (splitDate.Count() == 3)
                {
                    switch (splitDate[1].ToLower())
                    {
                        case "january":
                            month = "01";
                            break;
                        case "januari":
                            month = "01";
                            break;
                        case "february":
                            month = "02";
                            break;
                        case "march":
                            month = "03";
                            break;
                        case "maret":
                            month = "03";
                            break;
                        case "april":
                            month = "04";
                            break;
                        case "may":
                            month = "05";
                            break;
                        case "mei":
                            month = "05";
                            break;
                        case "june":
                            month = "06";
                            break;
                        case "juni":
                            month = "06";
                            break;
                        case "july":
                            month = "07";
                            break;
                        case "juli":
                            month = "07";
                            break;
                        case "august":
                            month = "08";
                            break;
                        case "agustus":
                            month = "08";
                            break;
                        case "september":
                            month = "09";
                            break;
                        case "october":
                            month = "10";
                            break;
                        case "oktober":
                            month = "10";
                            break;
                        case "november":
                            month = "11";
                            break;
                        case "december":
                            month = "12";
                            break;
                        case "desember":
                            month = "12";
                            break;
                        default:
                            month = "00";
                            break;
                    }
                }
                return swapDate = splitDate[2] + "-" + month + "-" + splitDate[0];
            }
            return tmpDate;
        }
        static List<ElementOut> ElementsOut = new List<ElementOut>();
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\ID\Gaz\");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            foreach (var tetFile in files)
            {
                ElementsOut.Clear();
                string FileName = tetFile;
                XElement tet = XElement.Load(FileName);
                var root = Directory.GetParent(FileName);
                string folderPath = Path.Combine(root.FullName);
                Directory.CreateDirectory(folderPath);
                /*TXT file for output information*/
                string path = Path.Combine(folderPath, FileName.Substring(0, FileName.IndexOf(".")) + ".txt"); //Output Filename
                StreamWriter sf = new StreamWriter(path);
                /*TETML elements*/
                var elements = tet.Descendants().Where(d => d.Name.LocalName == "Text")
                    .SkipWhile(e => !e.Value.StartsWith(I20))
                    .ToList();
                ElementOut currentElement = null;
                for (int i = 0; i < elements.Count; ++i)
                {
                    var element = elements[i];
                    string value = element.Value;
                    string tmpRecordValue = null;
                    string[] splittedRecord = null;
                    if (value.StartsWith(I20))
                    {
                        currentElement = new ElementOut();
                        ElementsOut.Add(currentElement);
                        int tmpInc = i;
                        tmpRecordValue = "";
                        do
                        {
                            tmpRecordValue += elements[tmpInc].Value + " ";
                            ++tmpInc;
                        } while (tmpInc < elements.Count()
                        && !elements[tmpInc].Value.StartsWith(I20));
                        if (tmpRecordValue != null)
                        {
                            splittedRecord = RecSplit(tmpRecordValue);
                            foreach (var inidCode in splittedRecord)
                            {
                                if (inidCode.StartsWith(I11))
                                {
                                    currentElement.I11 = inidCode.Replace(I11, "").Replace("\n", "").Trim();
                                }
                                if (inidCode.StartsWith(I13))
                                {
                                    currentElement.I13 = inidCode.Replace(I13, "").Replace("\n", "").Trim();
                                }
                                if (inidCode.StartsWith(I21))
                                {
                                    currentElement.I21 = inidCode.Replace(I21, "").Replace("\n", "").Trim();
                                }
                                if (inidCode.StartsWith(I22))
                                {
                                    currentElement.I22 = DateNormalize(inidCode.Replace(I22, "").Replace("\n", "").Trim());
                                }
                                if (inidCode.StartsWith(I43))
                                {
                                    currentElement.I43 = DateNormalize(inidCode.Replace(I43, "").Replace("\n", "").Trim());
                                }
                                if (inidCode.StartsWith(I51))
                                {
                                    string tmpIPC = inidCode.Replace(I51, "").Replace("\n", " ").Trim();
                                    //if (tmpIPC.Contains("/"))
                                    //{
                                    //    tmpIPC = inidCode.Substring(inidCode.IndexOf("/") + 1).Trim();
                                    //    if (tmpIPC.Contains("//"))
                                    //    {
                                    //        tmpIPC = tmpIPC.Remove(tmpIPC.IndexOf("//")).Trim();
                                    //    }
                                    //}
                                    currentElement.I51 = tmpIPC.Trim();
                                }
                                if (inidCode.StartsWith(I54))
                                {
                                    currentElement.I54 = inidCode.Replace(I54, "").Replace("\n", " ").Trim();
                                }
                                if (inidCode.StartsWith(I57))
                                {
                                    currentElement.I57 = inidCode.Replace(I57, "").Replace("\n", " ").Trim();
                                }
                                if (inidCode.StartsWith(I71))
                                {
                                    currentElement.I71 = inidCode.Replace(I71, "").Replace("\n", " ").Trim();
                                }
                                if (inidCode.StartsWith(I72))
                                {
                                    string tmpValue = inidCode.Replace(I72 + "\n", "").Trim();
                                    if (tmpValue.Contains("\n"))
                                    {
                                        string[] tmpValueSplitted = tmpValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                                        foreach (var item in tmpValueSplitted)
                                        {
                                            if (item.Contains(","))
                                            {
                                                currentElement.I72N = (currentElement.I72N ?? Enumerable.Empty<string>()).Concat(new string[] { item.Remove(item.IndexOf(",")).Trim() }).ToArray();
                                                currentElement.I72C = (currentElement.I72C ?? Enumerable.Empty<string>()).Concat(new string[] { item.Substring(item.LastIndexOf(",") + 1).Trim() }).ToArray();
                                            }
                                        }
                                    }
                                }
                                if (inidCode.StartsWith(I74))
                                {
                                    currentElement.I74 = inidCode.Replace(I74, "").Replace("\n", " ").Trim();
                                }
                            }
                        }
                    }
                }
                /*Output*/
                if (ElementsOut != null)
                {
                    int leCounter = 1;
                    /*list of record for whole gazette chapter*/
                    List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
                    /*Create a new event to fill*/
                    foreach (var record in ElementsOut)
                    {
                        Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();

                        legalEvent.GazetteName = Path.GetFileName(tetFile.Replace(".tetml", ".pdf"));
                        /*Setting subcode*/
                        legalEvent.SubCode = "1"; //1 - for Patent Application, 2 - for Simple Patent
                        /*Setting Section Code*/
                        legalEvent.SectionCode = "AZ";
                        /*Setting Country Code*/
                        legalEvent.CountryCode = "ID";
                        /*Setting File Name*/
                        legalEvent.Id = leCounter++; // creating uniq identifier
                        Biblio biblioData = new Biblio();
                        /*Elements output*/
                        biblioData.Publication.Number = record.I11;
                        biblioData.Publication.Kind = record.I13;
                        biblioData.Application.Number = record.I21;
                        biblioData.Application.Date = record.I22;
                        /*45 date of publication*/
                        biblioData.DOfPublication = new DOfPublication()
                        {
                            date = record.I43
                        };
                        /*---------------------*/
                        /*54 Title*/
                        Title title = new Title()
                        {
                            Language = "ID",
                            Text = record.I54
                        };
                        biblioData.Titles.Add(title);
                        /*--------*/
                        /*57 description*/
                        biblioData.Abstracts = new List<Abstract>();
                        Abstract description = new Abstract()
                        {
                            Language = "ID",
                            Text = record.I57
                        };
                        biblioData.Abstracts.Add(description);
                        /*--------------*/
                        /*71 name, address, country code*/
                        biblioData.Applicants = new List<PartyMember>();
                        PartyMember applicants = new PartyMember()
                        {
                            Name = record.I71
                        };
                        biblioData.Applicants.Add(applicants);
                        /*--------------*/
                        /*72 name, country code*/
                        if (record.I72C != null && record.I72N != null)
                        {
                            biblioData.Inventors = new List<PartyMember>();
                            for (int i = 0; i < record.I72N.Count(); i++)
                            {
                                PartyMember inventor = new PartyMember()
                                {
                                    Name = record.I72N[i],
                                    Country = record.I72C[i]
                                };
                                biblioData.Inventors.Add(inventor);
                            }
                        }
                        /*---------------------*/
                        /*74 name, address, cc*/
                        biblioData.Agents = new List<PartyMember>();
                        PartyMember agent = new PartyMember()
                        {
                            Name = record.I74
                        };
                        biblioData.Agents.Add(agent);
                        /*--------------------*/

                        legalEvent.Biblio = biblioData;
                        fullGazetteInfo.Add(legalEvent);
                    }

                    foreach (var rec in fullGazetteInfo)
                    {
                        string tmpValue = JsonConvert.SerializeObject(rec);
                        string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                        HttpClient httpClient = new HttpClient();
                        httpClient.BaseAddress = new Uri(url);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var content = new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json");
                        var result = httpClient.PostAsync("", content).Result;
                        var answer = result.Content.ReadAsStringAsync().Result;
                    }
                }
                //if (ElementsOut != null)
                //{
                //    foreach (var elemOut in ElementsOut)
                //    {
                //        if (elemOut.I21 != null)
                //        {
                //            sf.WriteLine("****");
                //            sf.WriteLine("11:\t" + elemOut.I11);
                //            sf.WriteLine("13:\t" + elemOut.I13);
                //            sf.WriteLine("21:\t" + elemOut.I21);
                //            sf.WriteLine("22:\t" + elemOut.I22);
                //            sf.WriteLine("43:\t" + elemOut.I43);
                //            sf.WriteLine("51:\t" + elemOut.I51);
                //            sf.WriteLine("54:\t" + elemOut.I54);
                //            sf.WriteLine("57:\t" + elemOut.I57);
                //            sf.WriteLine("71:\t" + elemOut.I71);
                //            if (elemOut.I72N != null && elemOut.I72N.Count() == elemOut.I72C.Count())
                //            {
                //                for (int i = 0; i < elemOut.I72N.Count(); i++)
                //                {
                //                    sf.WriteLine("72N:\t" + elemOut.I72N[i]);
                //                    sf.WriteLine("72C:\t" + elemOut.I72C[i]);
                //                }
                //            }
                //            sf.WriteLine("74:\t" + elemOut.I74);
                //            /*31,32,33 Priority*/
                //            //if (elemOut.I31 != null && elemOut.I31.Count() == elemOut.I32.Count() && elemOut.I31.Count() == elemOut.I33.Count())
                //            //{
                //            //    for (int i = 0; i < elemOut.I31.Count(); i++)
                //            //    {
                //            //        sf.WriteLine("31:\t" + elemOut.I31[i]);
                //            //        sf.WriteLine("32:\t" + elemOut.I32[i]);
                //            //        sf.WriteLine("33:\t" + elemOut.I33[i]);
                //            //    }
                //            //}
                //        }
                //    }
                //}
                //sf.Flush();
                //sf.Close();
            }
        }
    }
}
