using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Integration;

namespace Diamond_BA_publication_of_europen_patents
{
    class Diamond_BA_publication_of_europen_patents
    {
        /*string keys*/
        private static readonly string I11 = "(11)";
        private static readonly string I21 = "(21)";
        private static readonly string I22 = "(22)";
        private static readonly string I26 = "(26)";
        private static readonly string I31 = "(31)";
        private static readonly string I32 = "(32)";
        private static readonly string I33 = "(33)";
        private static readonly string I96 = "(96)";
        private static readonly string I97 = "(97)";
        private static readonly string I51 = "(51)";
        private static readonly string I72 = "(72)";
        private static readonly string I73 = "(73)";
        private static readonly string I74 = "(74)";
        private static readonly string I54 = "(54)";
        private static readonly string I57 = "(57)";

        /*Date swapping*/
        static string dateSwap(string tmpDate)
        {
            try
            {
                string swapDate = "";
                string[] splitDate = tmpDate.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                if (splitDate.Count() == 3)
                {
                    swapDate = splitDate[2] + splitDate[1] + splitDate[0];
                }
                return swapDate;
            }
            catch (Exception)
            {
                return tmpDate;
            }

        }
        /*Splitting record by INIDs numbers*/
        static string[] RecSplit(string recString)
        {
            string[] splittedRecord = null;
            string tempStrC = recString.Replace(I32, "").Replace(I33, "");
            if (tempStrC != "")
            {
                if (tempStrC.Contains("("))
                {
                    Regex regexPatOne = new Regex(@"\(\d{2}\)", RegexOptions.IgnoreCase);
                    MatchCollection matchesClass = regexPatOne.Matches(tempStrC);
                    if (matchesClass.Count > 0)
                    {
                        foreach (Match matchC in matchesClass)
                        {
                            tempStrC = tempStrC.Replace(matchC.Value, "***" + matchC.Value);
                        }
                    }
                }
                /*Splitting record*/
                splittedRecord = tempStrC.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries);
            }
            return splittedRecord;
        }

        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\BA\Reg\");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            /*Tetml files processing*/
            foreach (var tetFile in files)
            {
                ElementOut.ElementsOut.Clear();
                string FileName = tetFile;
                XElement tet = XElement.Load(FileName);
                /*TETML elements*/
                var elements = tet.Descendants().Where(d => d.Name.LocalName == "Text" || d.Name.LocalName == "PlacedImage" || d.Name.LocalName == "Page").ToList();

                /*Root settings*/
                var root = Directory.GetParent(FileName);
                string folderPath = Path.Combine(root.FullName);
                Directory.CreateDirectory(folderPath);
                var processed = Directory.CreateDirectory(Path.Combine(folderPath, FileName.Remove(FileName.IndexOf("."))));
                /*TXT file for output information*/
                string path = Path.Combine(folderPath, FileName.Substring(0, FileName.IndexOf(".")) + ".txt"); //Output Filename
                StreamWriter sf = new StreamWriter(path);
                ElementOut currentElement = null;
                for (int i = 0; i < elements.Count; ++i)
                {
                    var element = elements[i];
                    string value = element.Value;
                    /*Main part except description*/
                    if (value.StartsWith(I11))
                    {
                        currentElement = new ElementOut();
                        ElementOut.ElementsOut.Add(currentElement);
                        /*Reading value of Record*/
                        int tmpCounter = i;
                        string tmpRecValue = "";
                        string[] splittedRev = null;
                        do
                        {
                            if (!elements[tmpCounter].Value.Contains("tetml"))
                            {
                                tmpRecValue += elements[tmpCounter].Value + "\n";
                            }
                            ++tmpCounter;
                        } while (tmpCounter < elements.Count() && !elements[tmpCounter].Value.StartsWith(I57));
                        /*Splitting value of Record by INIDs*/
                        splittedRev = RecSplit(tmpRecValue);
                        /*Elements identification*/
                        if (splittedRev != null)
                        {
                            for (int k = 0; k < splittedRev.Length; k++)
                            {
                                string iValue = splittedRev[k];
                                /*11*/
                                if (iValue.StartsWith(I11))
                                {
                                    currentElement.I11 = iValue.Replace(I11, "").Replace("\n", "").Trim();
                                }
                                /*21*/
                                if (iValue.StartsWith(I21))
                                {
                                    currentElement.I21 = iValue.Replace(I21, "").Replace("\n", "").Trim();
                                }
                                /*22*/
                                if (iValue.StartsWith(I22))
                                {
                                    currentElement.I22 = iValue.Replace(I22, "").Replace("\n", "").Trim();
                                }
                                /*26*/
                                if (iValue.StartsWith(I26))
                                {
                                    currentElement.I26 = iValue.Replace(I26, "").Replace("\n", "").Trim();
                                }
                                /*96*/
                                if (iValue.StartsWith(I96))
                                {
                                    string tmpValue = iValue.Replace(I96, "").Replace("\n", "").Trim();
                                    string tmpDate = null;
                                    if (Regex.IsMatch(tmpValue, @"\d{4}\-\d{2}\-\d{2}"))
                                    {
                                        tmpDate = Regex.Match(tmpValue, @"\d{4}\-\d{2}\-\d{2}").Value;
                                        currentElement.I96D = tmpDate;
                                        currentElement.I96N = tmpValue.Replace(tmpDate, "").Trim();
                                    }
                                }
                                /*97*/
                                if (iValue.StartsWith(I97))
                                {
                                    string tmpValue = iValue.Replace(I97, "").Replace("\n", "").Trim();
                                    if (Regex.IsMatch(tmpValue, @"\d{4}\-\d{2}\-\d{2}"))
                                    {
                                        currentElement.I97D = Regex.Match(tmpValue, @"\d{4}\-\d{2}\-\d{2}").Value;
                                    }
                                }
                                /*54*/
                                if (iValue.StartsWith(I54))
                                {
                                    currentElement.I54 = iValue.Replace(I54, "").Replace("\n", " ").Trim();
                                }
                                /*31*/
                                if (iValue.StartsWith(I31))
                                {
                                    string tmpDate = null;
                                    string tmpNumber = null;
                                    string tmpCC = null;
                                    string[] splittedValue = null;
                                    string tmpValue = iValue.Replace(I31, "");
                                    if (tmpValue.Contains("\n"))
                                    {
                                        splittedValue = tmpValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                        foreach (var record in splittedValue)
                                        {
                                            if (Regex.IsMatch(record, @"\s[A-Z]{2}$"))
                                            {
                                                tmpCC = Regex.Match(record, @"\s[A-Z]{2}$").Value.Trim();
                                            }
                                            if (Regex.IsMatch(record, @"\d{4}\-\d{2}\-\d{2}"))
                                            {
                                                tmpDate = Regex.Match(record, @"\d{4}\-\d{2}\-\d{2}").Value;
                                            }
                                            if (tmpDate != null)
                                            {
                                                tmpNumber = record.Remove(record.IndexOf(tmpDate)).Trim();
                                            }
                                            if (tmpCC != null && tmpNumber != null && tmpDate != null)
                                            {
                                                currentElement.I31 = (currentElement.I31 ?? Enumerable.Empty<string>()).Concat(new string[] { tmpNumber.Trim() }).ToArray();
                                                currentElement.I32 = (currentElement.I32 ?? Enumerable.Empty<string>()).Concat(new string[] { tmpDate.Trim() }).ToArray();
                                                currentElement.I33 = (currentElement.I33 ?? Enumerable.Empty<string>()).Concat(new string[] { tmpCC.Trim() }).ToArray();
                                            }
                                        }
                                    }
                                }
                                /*51*/
                                if (iValue.StartsWith(I51))
                                {
                                    string[] splittedValue = null;
                                    string[] tmpDateNum = null; // whole record with number and (if present) date 
                                    string[] tmpNumValue = null; // only number after splitting
                                    string[] tmpDateValue = null; //only date after splitting
                                    string tmpValue = iValue.Replace(I51, "").Replace("\n", " ").Trim();
                                    if (tmpValue.Contains(","))
                                    {
                                        splittedValue = tmpValue.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                        foreach (var record in splittedValue)
                                        {
                                            tmpDateNum = (tmpDateNum ?? Enumerable.Empty<string>()).Concat(new string[] { record.Trim() }).ToArray();
                                        }
                                    }
                                    else
                                    {
                                        tmpDateNum = (tmpDateNum ?? Enumerable.Empty<string>()).Concat(new string[] { tmpValue }).ToArray();
                                    }
                                    foreach (var rec in tmpDateNum)
                                    {
                                        if (Regex.IsMatch(rec, @"\(\d{4}\.\d{2}\)"))
                                        {
                                            tmpDateValue = (tmpDateValue ?? Enumerable.Empty<string>()).Concat(new string[]
                                            {
                                                Regex.Match(rec, @"\(\d{4}\.\d{2}\)").Value.Replace("(","").Replace(")","")
                                            }).ToArray();
                                            tmpNumValue = (tmpNumValue ?? Enumerable.Empty<string>()).Concat(new string[]
                                            {
                                                rec.Replace(Regex.Match(rec, @"\(\d{4}\.\d{2}\)").Value, "").Trim()
                                            }).ToArray();
                                        }
                                    }
                                    if (tmpDateValue != null)
                                    {
                                        currentElement.I51D = tmpDateValue;
                                        currentElement.I51N = tmpNumValue;
                                    }
                                    else if (tmpDateNum != null)
                                    {
                                        currentElement.I51N = tmpDateNum;
                                    }
                                }
                                /*72*/
                                if (iValue.StartsWith(I72))
                                {
                                    string[] tmpName = null;
                                    string[] tmpAddr = null;
                                    string[] tmpCC = null;
                                    string tmpValue = iValue.Replace(I72, "").Trim();
                                    if (Regex.IsMatch(tmpValue, @"\n[A-Z]{2}"))
                                    {
                                        string[] tmpSplValue = Regex.Split(tmpValue, @"(?<=(\n[A-Z]{2}))").Where(d => d.Length > 3).Select(d => d.Trim()).ToArray(); //(?<=\([A - Z]{ 2}\))
                                        foreach (var rec in tmpSplValue)
                                        {
                                            string tmpRec = rec;
                                            if (Regex.IsMatch(tmpRec, @"\n[A-Z]{2}$"))
                                            {
                                                tmpCC = (tmpCC ?? Enumerable.Empty<string>()).Concat(new string[] { Regex.Match(tmpRec, @"\n[A-Z]{2}$").Value.Trim() }).ToArray();
                                                tmpRec = tmpRec.Replace(Regex.Match(tmpRec, @"\n[A-Z]{2}$").Value, "").Trim();
                                            }
                                            if (tmpRec.Contains("\n"))
                                            {
                                                tmpName = (tmpName ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRec.Remove(tmpRec.IndexOf("\n")).Trim() }).ToArray();
                                                tmpAddr = (tmpAddr ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRec.Substring(tmpRec.IndexOf("\n")).Replace("\n", " ").Trim().Trim(',').Trim() }).ToArray();
                                            }
                                        }
                                    }
                                    if (tmpName != null && tmpAddr != null && tmpCC != null)
                                    {
                                        currentElement.I72N = tmpName;
                                        currentElement.I72A = tmpAddr;
                                        currentElement.I72C = tmpCC;
                                    }
                                }
                                /*73*/
                                if (iValue.StartsWith(I73))
                                {
                                    string[] tmpName = null;
                                    string[] tmpAddr = null;
                                    string[] tmpCC = null;
                                    string tmpValue = iValue.Replace(I73, "").Trim();
                                    if (Regex.IsMatch(tmpValue, @"\n[A-Z]{2}"))
                                    {
                                        string[] tmpSplValue = Regex.Split(tmpValue, @"(?<=(\n[A-Z]{2}))").Where(d => d.Length > 3).Select(d => d.Trim()).ToArray(); //(?<=\([A - Z]{ 2}\))
                                        foreach (var rec in tmpSplValue)
                                        {
                                            string tmpRec = rec;
                                            if (Regex.IsMatch(tmpRec, @"\n[A-Z]{2}$"))
                                            {
                                                tmpCC = (tmpCC ?? Enumerable.Empty<string>()).Concat(new string[] { Regex.Match(tmpRec, @"\n[A-Z]{2}$").Value.Trim() }).ToArray();
                                                tmpRec = tmpRec.Replace(Regex.Match(tmpRec, @"\n[A-Z]{2}$").Value, "").Trim();
                                            }
                                            if (tmpRec.Contains("\n"))
                                            {
                                                tmpName = (tmpName ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRec.Remove(tmpRec.IndexOf("\n")).Trim() }).ToArray();
                                                tmpAddr = (tmpAddr ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRec.Substring(tmpRec.IndexOf("\n")).Replace("\n", " ").Trim().Trim(',').Trim() }).ToArray();
                                            }
                                        }
                                    }
                                    if (tmpName != null && tmpAddr != null && tmpCC != null)
                                    {
                                        currentElement.I73N = tmpName;
                                        currentElement.I73A = tmpAddr;
                                        currentElement.I73C = tmpCC;
                                    }
                                }
                                /*74*/
                                if (iValue.StartsWith(I74))
                                {
                                    string tmpName = null;
                                    string tmpAddr = null;
                                    string tmpCC = null;
                                    string tmpValue = iValue.Replace(I74, "").Trim();
                                    if (Regex.IsMatch(tmpValue, @"\n[A-Z]{2}$"))
                                    {
                                        tmpCC = Regex.Match(tmpValue, @"\n[A-Z]{2}$").Value.Trim();
                                        tmpValue = tmpValue.Replace(Regex.Match(tmpValue, @"\n[A-Z]{2}$").Value, "").Trim();
                                    }
                                    if (tmpValue.Contains("\n"))
                                    {
                                        tmpName = tmpValue.Remove(tmpValue.IndexOf("\n")).Trim();
                                        tmpAddr = tmpValue.Substring(tmpValue.IndexOf("\n")).Replace("\n", " ").Trim().Trim(',').Trim();
                                    }
                                    if (tmpName != null && tmpAddr != null && tmpCC != null)
                                    {
                                        currentElement.I74N = tmpName;
                                        currentElement.I74A = tmpAddr;
                                        currentElement.I74C = tmpCC;
                                    }
                                }
                            }
                        }
                    }
                    /*Desc 57*/
                    if (value.StartsWith(I57) && currentElement != null)
                    {
                        int tmpCounter = i;
                        string tmpRecValue = "";
                        do
                        {
                            if (!elements[tmpCounter].Value.Contains("tetml"))
                            {
                                tmpRecValue += elements[tmpCounter].Value + " ";
                            }
                            ++tmpCounter;
                        } while (tmpCounter < elements.Count() && !elements[tmpCounter].Value.StartsWith(I11));
                        if (tmpRecValue != null && tmpRecValue != "")
                        {
                            currentElement.I57 = tmpRecValue.Replace(I57, "").Trim();
                        }
                    }
                }
                /*Output*/
                if (ElementOut.ElementsOut != null)
                {
                    int leCounter = 1;
                    List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
                    foreach (var record in ElementOut.ElementsOut)
                    {
                        Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                        legalEvent.GazetteName = Path.GetFileName(tetFile.Replace(".tetml", ".pdf"));
                        legalEvent.SubCode = "4";
                        legalEvent.SectionCode = "BA";
                        legalEvent.CountryCode = "BS";
                        legalEvent.Id = leCounter++; // creating uniq identifier
                        Biblio biblioData = new Biblio();
                        /*Elements output*/
                        biblioData.Publication.Number = record.I11;
                        biblioData.Publication.Language = "BS";
                        biblioData.Application.Number = record.I21;
                        biblioData.Application.Date = record.I22;
                        biblioData.Application.Language = "BS";
                        /*30*/
                        if (record.I31 != null && record.I32 != null && record.I33 != null)
                        {
                            biblioData.Priorities = new List<Priority>();
                            for (int i = 0; i < record.I31.Count(); i++)
                            {
                                Priority priority = new Priority();
                                priority.Country = record.I31[i];
                                priority.Date = record.I32[i];
                                priority.Number = record.I33[i];
                                priority.Sequence = i;
                                biblioData.Priorities.Add(priority);
                            }
                        }
                        /*51*/
                        if (record.I51N != null && record.I51D != null)
                        {
                            biblioData.Ipcs = new List<Ipc>();
                            for (int i = 0; i < record.I51N.Count(); i++)
                            {
                                Ipc ipc = new Ipc()
                                {
                                    Class = record.I51N[i],
                                    Date = record.I51D[i]
                                };
                                biblioData.Ipcs.Add(ipc);
                            }
                        }
                        else if (record.I51N != null && record.I51D == null)
                        {
                            biblioData.Ipcs = new List<Ipc>();
                            for (int i = 0; i < record.I51N.Count(); i++)
                            {
                                Ipc ipc = new Ipc()
                                {
                                    Class = record.I51N[i],
                                };
                                biblioData.Ipcs.Add(ipc);
                            }
                        }
                        /*54 Title*/
                        Title title = new Title()
                        {
                            Language = "BS",
                            Text = record.I54
                        };
                        biblioData.Titles.Add(title);
                        /*57 description*/
                        biblioData.Abstracts = new List<Abstract>();
                        Abstract description = new Abstract()
                        {
                            Language = "BS",
                            Text = record.I57
                        };
                        biblioData.Abstracts.Add(description);
                        /*72 name, country code*/
                        if (record.I72N != null)
                        {
                            biblioData.Inventors = new List<PartyMember>();
                            for (int i = 0; i < record.I72N.Count(); i++)
                            {
                                PartyMember inventor = new PartyMember()
                                {
                                    Name = record.I72N[i],
                                    Address1 = record.I72A[i],
                                    Country = record.I72C[i]
                                };
                                biblioData.Inventors.Add(inventor);
                            }
                        }
                        /*73 name, country code*/
                        if (record.I73N != null)
                        {
                            biblioData.Assignees = new List<PartyMember>();
                            for (int i = 0; i < record.I73N.Count(); i++)
                            {
                                PartyMember Assignees = new PartyMember()
                                {
                                    Name = record.I73N[i],
                                    Address1 = record.I73A[i],
                                    Country = record.I73C[i]
                                };
                                biblioData.Assignees.Add(Assignees);
                            }
                        }
                        /*74 name, address, cc*/
                        biblioData.Agents = new List<PartyMember>();
                        PartyMember agent = new PartyMember()
                        {
                            Name = record.I74N,
                            Address1 = record.I74A,
                            Country = record.I74C
                        };
                        biblioData.Agents.Add(agent);
                        /*--------------------*/
                        /*96/97 number and date*/
                        biblioData.EuropeanPatents = new List<EuropeanPatent>();
                        EuropeanPatent euPatent = new EuropeanPatent()
                        {
                            AppNumber = record.I96N,
                            AppDate = record.I96D,
                            PubDate = record.I97D
                        };
                        biblioData.EuropeanPatents.Add(euPatent);
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
            }
        }
    }
}
