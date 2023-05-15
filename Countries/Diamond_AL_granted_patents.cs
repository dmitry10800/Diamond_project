using Newtonsoft.Json;
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

namespace Diamond_AL_granted_patents
{
    class Diamond_AL_granted_patents
    {
        private static readonly string I11 = "( 11 )";
        private static readonly string I11A = "( 11)";
        private static readonly string I18 = "( 18 )";
        private static readonly string I18A = "( 18)";
        private static readonly string I21 = "( 21 )";
        private static readonly string I21A = "( 21)";
        private static readonly string I22 = "( 22 )";
        private static readonly string I22A = "( 22)";
        private static readonly string I30 = "( 30 )";
        private static readonly string I30A = "( 30)";
        private static readonly string I54 = "( 54 )";
        private static readonly string I54A = "( 54)";
        private static readonly string I57 = "( 57 )";
        private static readonly string I57A = "( 57)";
        private static readonly string I71 = "( 71 )";
        private static readonly string I71A = "( 71)";
        private static readonly string I72 = "( 72 )";
        private static readonly string I72A = "( 72)";
        private static readonly string I73 = "( 73 )";
        private static readonly string I73A = "( 73)";
        private static readonly string I74 = "( 74 )";
        private static readonly string I74A = "( 74)";
        private static readonly string I96 = "( 96 )";
        private static readonly string I96A = "( 96)";
        private static readonly string I97 = "( 97 )";
        private static readonly string I97A = "( 97)";

        class ElementOut
        {
            public string I11 { get; set; }
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string[] I30N { get; set; }
            public string[] I30D { get; set; }
            public string[] I30C { get; set; }
            public string I45 { get; set; }
            public string I54 { get; set; }
            public string I57 { get; set; }
            public string I71N { get; set; }
            public string I71A { get; set; }
            public string I71C { get; set; }
            //public string I71MN { get; set; }
            //public string I71MA { get; set; }
            //public string I71MC { get; set; }
            //public string I73N { get; set; }
            //public string I73A { get; set; }
            public string[] I72N { get; set; }
            public string[] I72A { get; set; }
            public string I74N { get; set; }
            public string I74A { get; set; }
            public string I74C { get; set; }
            public string I96N { get; set; }
            public string I96D { get; set; }
            public string I97N { get; set; }
            public string I97D { get; set; }
        }

        /*72 split*/
        static (string[] ownerName, string[] ownerAdr) OwnerSplit(string tmpOwnerValue)
        {
            string[] tmpMultiOwn = null;
            string[] ownerName = null;
            string[] ownerAdr = null;
            string tmpOwnName = null;
            string tmpOwnAdr = null;
            /*If MultiOwner*/
            /*Split by ";" char*/
            if (tmpOwnerValue.Contains(";"))
            {
                tmpMultiOwn = tmpOwnerValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToArray();

                if (tmpMultiOwn != null && tmpMultiOwn.Count() > 0)
                {
                    foreach (var ownerRec in tmpMultiOwn)
                    {
                        if (ownerRec.Contains("("))
                        {
                            tmpOwnName = ownerRec.Remove(ownerRec.IndexOf("(")).Trim();
                            tmpOwnAdr = ownerRec.Replace(tmpOwnName, "").Replace("(", "").Replace(")", "").Trim();
                        }
                        if (tmpOwnName != null && tmpOwnAdr != null)
                        {
                            ownerName = (ownerName ?? Enumerable.Empty<string>()).Concat(new[] { tmpOwnName }).ToArray();
                            ownerAdr = (ownerAdr ?? Enumerable.Empty<string>()).Concat(new[] { tmpOwnAdr }).ToArray();
                        }
                    }
                }
            }
            else if (tmpOwnerValue.Contains("("))
            {
                tmpOwnName = tmpOwnerValue.Remove(tmpOwnerValue.IndexOf("(")).Trim();
                tmpOwnAdr = tmpOwnerValue.Replace(tmpOwnName, "").Replace("(", "").Replace(")", "").Trim();
                if (tmpOwnName != null && tmpOwnAdr != null)
                {
                    ownerName = (ownerName ?? Enumerable.Empty<string>()).Concat(new[] { tmpOwnName }).ToArray();
                    ownerAdr = (ownerAdr ?? Enumerable.Empty<string>()).Concat(new[] { tmpOwnAdr }).ToArray();
                }
            }
            return (ownerName, ownerAdr);
        }

        /*Priority split*/
        static (string[] prioNumber, string[] prioCountry, string[] prioDate) PrioritySplit(string tmpPriorityValue)
        {
            string[] tmpMultiPrio = null;
            string[] prioNumber = null;
            string[] prioCountry = null;
            string[] prioDate = null;
            var datePattern = @"\s\d{2}\/\d{2}\/\d{4}\s";
            string tmpDateValue = null;
            string[] tmpPrioRecordValue = null;
            tmpPriorityValue = tmpPriorityValue.Replace(" and ", ";").Replace(" AND ", ";");
            /*If MultiPriority*/
            /*Split by ";" char*/
            if (tmpPriorityValue.Contains(";"))
            {
                tmpMultiPrio = tmpPriorityValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                if (tmpMultiPrio != null && tmpMultiPrio.Count() > 0)
                {
                    foreach (var prioRecord in tmpMultiPrio)
                    {
                        tmpDateValue = Regex.Match(prioRecord.Trim(), datePattern).Value;
                        if (tmpDateValue != null)
                        {
                            tmpPrioRecordValue = prioRecord.Split(new string[] { tmpDateValue }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                            if (tmpPrioRecordValue != null && tmpPrioRecordValue.Count() == 2)
                            {
                                prioNumber = (prioNumber ?? Enumerable.Empty<string>()).Concat(new[] { tmpPrioRecordValue[0].Trim() }).ToArray();
                                prioDate = (prioDate ?? Enumerable.Empty<string>()).Concat(new[] { DateNormalize(tmpDateValue.Trim()) }).ToArray();
                                prioCountry = (prioCountry ?? Enumerable.Empty<string>()).Concat(new[] { tmpPrioRecordValue[1].Trim() }).ToArray();
                            }
                        }
                    }
                }
            }
            else
            {
                tmpDateValue = Regex.Match(tmpPriorityValue.Trim(), datePattern).Value;
                if (tmpDateValue != null)
                {
                    tmpPrioRecordValue = tmpPriorityValue.Split(new string[] { tmpDateValue }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (tmpPrioRecordValue != null && tmpPrioRecordValue.Count() == 2)
                    {
                        prioNumber = (prioNumber ?? Enumerable.Empty<string>()).Concat(new[] { tmpPrioRecordValue[0].Trim() }).ToArray();
                        prioDate = (prioDate ?? Enumerable.Empty<string>()).Concat(new[] { DateNormalize(tmpDateValue.Trim()) }).ToArray();
                        prioCountry = (prioCountry ?? Enumerable.Empty<string>()).Concat(new[] { tmpPrioRecordValue[1].Trim() }).ToArray();
                    }
                }
            }
            return (prioNumber, prioCountry, prioDate);
        }

        static string[] RecSplit(string recString)
        {
            string[] splittedRecord = null;
            var tempStrC = recString/*.Replace("\n", " ")*/;
            if (recString != "")
            {
                //if (recString.Contains("\n"))
                //{
                //    recString = recString.Replace("\n", " ");
                //}
                var regexPatOne = new Regex(@"\(\s+\d{2}\s*\)", RegexOptions.IgnoreCase);
                var matchesClass = regexPatOne.Matches(recString);
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
            var swapDate = tmpDate;
            string[] splitDate = null;
            if (tmpDate.Contains("/"))
            {
                splitDate = tmpDate.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                if (splitDate.Count() == 3)
                {
                    return swapDate = splitDate[2] + "-" + splitDate[1] + "-" + splitDate[0];
                }
            }
            return tmpDate;
        }
        static List<ElementOut> ElementsOut = new List<ElementOut>();
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\AL\Reg\");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (var file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            foreach (var tetFile in files)
            {
                ElementsOut.Clear();
                var FileName = tetFile;
                var tet = XElement.Load(FileName);
                var root = Directory.GetParent(FileName);
                var folderPath = Path.Combine(root.FullName);
                Directory.CreateDirectory(folderPath);
                /*TXT file for output information*/
                var path = Path.Combine(folderPath, FileName.Substring(0, FileName.IndexOf(".")) + ".txt"); //Output Filename
                var sf = new StreamWriter(path);
                /*TETML elements*/
                var elements = tet.Descendants().Where(d => d.Name.LocalName == "Text")
                    .SkipWhile(e => !e.Value.Contains("PATENTA TË LËSHUARA"))
                    .TakeWhile(e => !e.Value.Contains("NDRYSHIMI I EMRIT TË PRONARIT"))
                    .ToList();
                ElementOut currentElement = null;
                for (var i = 0; i < elements.Count; ++i)
                {
                    var element = elements[i];
                    var value = element.Value;
                    string tmpRecordValue = null;
                    string[] splittedRecord = null;
                    if (value.StartsWith(I11) || value.StartsWith(I11A))
                    {
                        currentElement = new ElementOut();
                        ElementsOut.Add(currentElement);
                        var tmpInc = i;
                        tmpRecordValue = "";
                        do
                        {
                            tmpRecordValue += elements[tmpInc].Value + "\n";
                            ++tmpInc;
                        } while (tmpInc < elements.Count()
                        && !elements[tmpInc].Value.StartsWith(I11)
                        && !elements[tmpInc].Value.StartsWith(I11A));
                        if (tmpRecordValue != null)
                        {
                            splittedRecord = RecSplit(tmpRecordValue);
                            foreach (var inidCode in splittedRecord)
                            {
                                if (inidCode.StartsWith(I11) || inidCode.StartsWith(I11A))
                                {
                                    currentElement.I11 = inidCode.Replace(I11, "").Replace(I11A, "").Replace("\n", "").Trim();
                                }
                                if (inidCode.StartsWith(I21) || inidCode.StartsWith(I21A))
                                {
                                    currentElement.I21 = inidCode.Replace(I21, "").Replace(I21A, "").Replace("\n", "").Replace(" ", "").Trim();
                                }
                                if (inidCode.StartsWith(I22) || inidCode.StartsWith(I22A))
                                {
                                    var tmpDateValue = inidCode.Replace(I22, "").Replace(I22A, "").Replace("\n", "").Trim();
                                    currentElement.I22 = DateNormalize(tmpDateValue);
                                }
                                /*Priority*/
                                if (inidCode.StartsWith(I30) || inidCode.StartsWith(I30A))
                                {
                                    var (prioNumber, prioCountry, prioDate) = PrioritySplit(inidCode.Replace(I30, "").Replace(I30A, "").Replace("\n", " ").Trim());
                                    currentElement.I30N = prioNumber;
                                    currentElement.I30D = prioDate;
                                    currentElement.I30C = prioCountry;
                                }
                                /*Title*/
                                if (inidCode.StartsWith(I54) || inidCode.StartsWith(I54A))
                                {
                                    var tmpTitleValue = inidCode.Replace(I54, "").Replace(I54A, "").Replace("\n", " ").Trim();
                                    var datePattern = @"\d{2}\/\d{2}\/\d{4}";
                                    string tmpDateValue = null;
                                    tmpDateValue = Regex.Match(tmpTitleValue, datePattern).Value;
                                    if (tmpDateValue != null) { currentElement.I45 = DateNormalize(tmpDateValue); }
                                    currentElement.I54 = tmpTitleValue.Replace(tmpDateValue, "").Trim();
                                }
                                /*Description*/
                                if (inidCode.StartsWith(I57) || inidCode.StartsWith(I57A))
                                {
                                    var tmpTitleValue = inidCode.Replace(I57, "").Replace(I57A, "").Replace("\n", " ").Trim();
                                    currentElement.I57 = tmpTitleValue;
                                }
                                /*71*/
                                if (inidCode.StartsWith(I71) || inidCode.StartsWith(I71A))
                                {
                                    var tmpDescValue = inidCode.Replace(I71, "").Replace(I71A, "").Trim();
                                    string[] tmpDateValueSpl = null;
                                    string tmpNameValue = null;
                                    string tmpAdrValue = null;
                                    string tmpCountryCode = null;
                                    /*Test splitting*/
                                    //if (tmpDescValue.Contains(" and ") || tmpDescValue.Contains(";"))
                                    //{
                                    //    List<string> ownersList = new List<string>();
                                    //    string[] tmpDescReplaced = tmpDescValue.Replace(" and ", ";").Replace("\n", " ").Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    //    foreach (var record in tmpDescReplaced)
                                    //    {
                                    //        if (!Regex.IsMatch(record, @"\,\s*[A-Z]{2}$"))
                                    //        {
                                    //            ownersList.Add(record);
                                    //        }
                                    //        if (!Regex.IsMatch(record, @"\,\s*[A-Z]{2}$"))
                                    //        {
                                    //        }
                                    //    }
                                    //}
                                    /*************************************/
                                    if (tmpDescValue.Contains("\n"))
                                    {
                                        tmpDateValueSpl = tmpDescValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                        if (tmpDateValueSpl != null && tmpDateValueSpl.Count() > 1)
                                        {
                                            string tmpAdrCC = null;
                                            tmpAdrCC = Regex.Match(tmpDescValue, @"[A-Z]{2}$").Value;
                                            if (tmpAdrCC != null)
                                            {
                                                tmpCountryCode = tmpAdrCC.Trim();
                                            }
                                            tmpNameValue = tmpDateValueSpl[0];
                                            if (tmpAdrCC != null)
                                            {
                                                tmpAdrValue = tmpDescValue.Remove(tmpDescValue.LastIndexOf(tmpAdrCC)).Replace(tmpNameValue, "").Trim().Trim(',').Trim();
                                            }
                                            else
                                            {
                                                tmpAdrValue = tmpDescValue.Replace(tmpNameValue, "").Trim();
                                            }
                                        }
                                    }
                                    if (tmpNameValue != null && tmpAdrValue != null)
                                    {
                                        currentElement.I71A = tmpAdrValue.Replace("\n", " ").Trim();
                                        currentElement.I71N = tmpNameValue;
                                        if (tmpCountryCode != null)
                                        {
                                            currentElement.I71C = tmpCountryCode;
                                        }
                                    }
                                }
                                /*72*/
                                if (inidCode.StartsWith(I72) || inidCode.StartsWith(I72A))
                                {
                                    var (ownerName, ownerAdr) = OwnerSplit(inidCode.Replace(I72, "").Replace(I72A, "").Replace("\n", " ").Trim());
                                    currentElement.I72N = ownerName;
                                    currentElement.I72A = ownerAdr;
                                }
                                /*Agent info*/
                                if (inidCode.StartsWith(I74) || inidCode.StartsWith(I74A))
                                {
                                    var tmpAgentValue = inidCode.Replace(I74, "").Replace(I74A, "").Trim();
                                    if (tmpAgentValue.Contains("\n"))
                                    {
                                        currentElement.I74N = tmpAgentValue.Remove(tmpAgentValue.IndexOf("\n")).Trim();

                                        if (Regex.IsMatch(tmpAgentValue, @"\[[A-Z]{2}\]"))
                                        {
                                            currentElement.I74C = Regex.Match(tmpAgentValue, @"\[[A-Z]{2}\]").Value.Replace("[", "").Replace("]", "").Trim();
                                        }
                                        else { currentElement.I74C = null; }
                                        if (currentElement.I74C != null)
                                        {
                                            currentElement.I74A = tmpAgentValue.Substring(tmpAgentValue.IndexOf("\n")).Replace("\n", "").Replace(currentElement.I74C, "").Trim();
                                        }
                                        else
                                        {
                                            currentElement.I74A = tmpAgentValue.Substring(tmpAgentValue.IndexOf("\n")).Replace("\n", "").Trim();
                                        }
                                    }
                                    else
                                    {
                                        currentElement.I74N = tmpAgentValue.Replace("\n", "").Trim();
                                    }
                                }
                                /*96*/
                                if (inidCode.StartsWith(I96) || inidCode.StartsWith(I96A))
                                {
                                    string tmpDateValue = null;
                                    var tmpAgentValue = inidCode.Replace(I96, "").Replace(I96A, "").Replace("\n", "").Trim();
                                    if (tmpAgentValue.Contains("/"))
                                    {
                                        tmpDateValue = Regex.Match(tmpAgentValue, @"\d{2}\/\d{2}\/\d{4}").Value;
                                        if (tmpDateValue != null)
                                        {
                                            currentElement.I96D = DateNormalize(tmpDateValue.Trim());
                                            currentElement.I96N = tmpAgentValue.Replace(tmpDateValue, "").Replace("/", "").Trim();
                                        }
                                    }
                                    else
                                    {
                                        currentElement.I96N = tmpAgentValue.Trim();
                                        currentElement.I96D = "";
                                    }
                                }
                                /*97*/
                                if (inidCode.StartsWith(I97) || inidCode.StartsWith(I97A))
                                {
                                    string tmpDateValue = null;
                                    var tmpAgentValue = inidCode.Replace(I97, "").Replace(I97A, "").Replace("\n", "").Trim();
                                    if (tmpAgentValue.Contains("/"))
                                    {
                                        tmpDateValue = Regex.Match(tmpAgentValue, @"\d{2}\/\d{2}\/\d{4}").Value;
                                        if (tmpDateValue != null)
                                        {
                                            currentElement.I97D = DateNormalize(tmpDateValue.Trim());
                                            currentElement.I97N = tmpAgentValue.Replace(tmpDateValue, "").Replace("/", "").Trim();
                                        }
                                    }
                                    else
                                    {
                                        currentElement.I97N = tmpAgentValue.Trim();
                                        currentElement.I97D = "";
                                    }
                                }
                            }
                        }
                    }
                }
                /*Output*/
                if (ElementsOut != null)
                {
                    var leCounter = 1;
                    /*list of record for whole gazette chapter*/
                    var fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
                    /*Create a new event to fill*/
                    foreach (var record in ElementsOut)
                    {
                        var legalEvent = new Diamond.Core.Models.LegalStatusEvent();

                        legalEvent.GazetteName = Path.GetFileName(tetFile.Replace(".tetml", ".pdf"));
                        /*Setting subcode*/
                        legalEvent.SubCode = "3";
                        /*Setting Section Code*/
                        legalEvent.SectionCode = "FG";
                        /*Setting Country Code*/
                        legalEvent.CountryCode = "AL";
                        /*Setting File Name*/
                        legalEvent.Id = leCounter++; // creating uniq identifier
                        var biblioData = new Biblio();
                        /*Elements output*/
                        biblioData.Publication.Number = record.I11;
                        biblioData.Application.Number = record.I21;
                        biblioData.Application.Date = record.I22;
                        /*30*/
                        if (record.I30C != null && record.I30D != null && record.I30N != null)
                        {
                            biblioData.Priorities = new List<Priority>();
                            for (var i = 0; i < record.I30N.Count(); i++)
                            {
                                var priority = new Priority();
                                priority.Country = record.I30C[i];
                                priority.Date = record.I30D[i];
                                priority.Number = record.I30N[i];
                                priority.Sequence = i;
                                biblioData.Priorities.Add(priority);
                            }
                        }
                        /*45 date of publication*/
                        biblioData.DOfPublication = new DOfPublication()
                        {
                            date_45 = record.I45
                        };
                        /*---------------------*/
                        /*54 Title*/
                        var title = new Title()
                        {
                            Language = "SQ",
                            Text = record.I54
                        };
                        biblioData.Titles.Add(title);
                        /*--------*/
                        /*57 description*/
                        biblioData.Abstracts = new List<Abstract>();
                        var description = new Abstract()
                        {
                            Language = "SQ",
                            Text = record.I57
                        };
                        biblioData.Abstracts.Add(description);
                        /*--------------*/
                        /*71 name, address, country code*/
                        biblioData.Applicants = new List<PartyMember>();
                        var applicants = new PartyMember()
                        {
                            Name = record.I71N,
                            Address1 = record.I71A,
                            Country = record.I71C
                        };
                        biblioData.Applicants.Add(applicants);
                        /*--------------*/
                        /*72 name, country code*/
                        if (record.I72A != null && record.I72N != null)
                        {
                            biblioData.Inventors = new List<PartyMember>();
                            for (var i = 0; i < record.I72N.Count(); i++)
                            {
                                var inventor = new PartyMember()
                                {
                                    Name = record.I72N[i],
                                    Address1 = record.I72A[i]
                                };
                                biblioData.Inventors.Add(inventor);
                            }
                        }
                        /*---------------------*/
                        /*74 name, address, cc*/
                        biblioData.Agents = new List<PartyMember>();
                        var agent = new PartyMember()
                        {
                            Name = record.I74N,
                            Address1 = record.I74A,
                            Country = record.I74C
                        };
                        biblioData.Agents.Add(agent);
                        /*--------------------*/
                        /*96/97 number and date*/
                        biblioData.EuropeanPatents = new List<EuropeanPatent>();
                        var euPatent = new EuropeanPatent()
                        {
                            AppNumber = record.I96N,
                            AppDate = record.I96D,
                            PubNumber = record.I97N,
                            PubDate = record.I97D
                        };
                        biblioData.EuropeanPatents.Add(euPatent);
                        legalEvent.Biblio = biblioData;
                        fullGazetteInfo.Add(legalEvent);
                    }

                    foreach (var rec in fullGazetteInfo)
                    {
                        var tmpValue = JsonConvert.SerializeObject(rec);
                        var url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                        var httpClient = new HttpClient();
                        httpClient.BaseAddress = new Uri(url);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var content = new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json");
                        var result = httpClient.PostAsync("", content).Result;
                        var answer = result.Content.ReadAsStringAsync().Result;
                    }


                    //foreach (var record in ElementsOut)
                    //{
                    //        /*------------------*/
                    //    sf.WriteLine("****");
                    //    sf.WriteLine("11:\t" + record.I11);
                    //    sf.WriteLine("21:\t" + record.I21);
                    //    sf.WriteLine("22:\t" + record.I22);
                    //    if (record.I30C != null && record.I30D != null && record.I30N != null)
                    //    {
                    //        for (int i = 0; i < record.I30N.Count(); i++)
                    //        {
                    //            sf.WriteLine("30N:\t" + record.I30N[i]);
                    //            sf.WriteLine("30D:\t" + record.I30D[i]);
                    //            sf.WriteLine("30C:\t" + record.I30C[i]);
                    //        }
                    //    }
                    //    sf.WriteLine("45:\t" + record.I45);
                    //    sf.WriteLine("54:\t" + record.I54);
                    //    sf.WriteLine("57:\t" + record.I57);
                    //    sf.WriteLine("71N:\t" + record.I71N);
                    //    sf.WriteLine("71A:\t" + record.I71A);
                    //    sf.WriteLine("71C:\t" + record.I71C);
                    //    if (record.I72A != null && record.I72N != null)
                    //    {
                    //        for (int i = 0; i < record.I72N.Count(); i++)
                    //        {
                    //            sf.WriteLine("72N:\t" + record.I72N[i]);
                    //            sf.WriteLine("72D:\t" + record.I72A[i]);
                    //        }
                    //    }
                    //    sf.WriteLine("74N:\t" + record.I74N);
                    //    sf.WriteLine("74A:\t" + record.I74A);
                    //    if (record.I74C != null) { sf.WriteLine("74C:\t" + record.I74C); }
                    //    sf.WriteLine("96N:\t" + record.I96N);
                    //    sf.WriteLine("96D:\t" + record.I96D);
                    //    sf.WriteLine("97N:\t" + record.I97N);
                    //    sf.WriteLine("97D:\t" + record.I97D);
                    //}
                }
                //sf.Flush();
                //sf.Close();
            }
        }
    }
}
