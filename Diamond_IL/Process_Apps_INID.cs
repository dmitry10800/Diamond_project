using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Diamond_IL
{
    class Process_Apps_INID
    {
        /*string keys*/
        private static readonly string I21 = "[21]";
        private static readonly string I22 = "[22]";
        private static readonly string I31 = "[31]";
        private static readonly string I32 = "[32]";
        private static readonly string I33 = "[33]";
        private static readonly string I51 = "[51]";
        private static readonly string I54 = "[54]";
        private static readonly string I62 = "[62]";
        private static readonly string I71 = "[71]";
        private static readonly string I72 = "[72]";
        private static readonly string I74 = "[74]";
        private static readonly string I87 = "[87]";

        public class ElementOut
        {
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string[] I31 { get; set; }
            public string[] I32 { get; set; }
            public string[] I33 { get; set; }
            public string I51Version { get; set; }
            public string[] I51Number { get; set; }
            public string I54Eng { get; set; }
            public string I54Hebrew { get; set; }
            public string I62 { get; set; }
            public string[] I71NameENG { get; set; }
            public string[] I71CountryENG { get; set; }
            public string[] I71NameIL { get; set; }
            public string[] I71AddrIL { get; set; }
            public string[] I71CountryIL { get; set; }
            public string[] I72Eng { get; set; }
            public string[] I72IL { get; set; }
            public string I74EngName { get; set; }
            public string I74EngAddr { get; set; }
            public string I74HebName { get; set; }
            public string I74HebAddr { get; set; }
            public string I87 { get; set; }
            public string INoteHeb { get; set; }
            public string INoteEng { get; set; }
        }

        public List<ElementOut> OutputValue(string elemList)
        {
            List<ElementOut> ElementsOut = new List<ElementOut>();
            ElementOut currentElement = null;
            if (elemList != null)
            {
                ElementsOut.Clear();
                var pattern = @"(\d{6}\r*\n*\[21\]\[11\])";
                var repText = Regex.Replace(elemList, pattern, m => "ISePaRaToR" + m.Groups[1].Value);
                var splittedRecords = Regex.Split(repText.Replace("\r\n", "\n").Replace(I31, "").Replace(I32, "").Replace(I33, ""), @"ISePaRaToR").Where(x => x != "" && x != "  " && x != "\n" && x != " ").Select(x => x.Trim());
                foreach (var record in splittedRecords)
                {
                    var splPattern = @"(?<=.*\[\d{2}\])"; /*works*/
                    var nSpl = Regex.Split(record, splPattern)/*.Where(x => !x.Contains(@"__________"))*/.Select(x => x.Trim().Trim('\t'));
                    string tmpPrioValue = null;
                    foreach (var splittedRec in nSpl)
                    {
                        string splittedRecClear = splittedRec.Replace("\n", " ").Trim().Trim('\t');
                        if (splittedRec.EndsWith(I21))
                        {
                            currentElement = new ElementOut();
                            ElementsOut.Add(currentElement);
                            currentElement.I21 = splittedRec.Replace(I21, "").Trim();
                        }
                        if (splittedRecClear.EndsWith(I22)) { currentElement.I22 = Methods.NormalizeDate(splittedRec); }
                        /*51*/
                        if (splittedRecClear.EndsWith(I51))
                        {
                            string tmpIntClassValue = null; //51 values
                            if (splittedRec.Contains("Int. Cl."))
                            {
                                tmpIntClassValue = splittedRecClear.Substring(splittedRec.IndexOf("Int. Cl.")).Trim();
                                tmpPrioValue = splittedRec.Remove(splittedRec.IndexOf("Int. Cl.")).Trim(); //30x values
                            }
                            /*51 inid processing*/
                            if (tmpIntClassValue != null)
                            {
                                var intClassProcess = Methods.IntClassSplit(tmpIntClassValue.Replace(I51, "").Trim());
                                currentElement.I51Number = intClassProcess.INumber;
                                currentElement.I51Version = intClassProcess.IVersion;
                            }
                        }
                        /*Division*/
                        if (splittedRecClear.EndsWith(I62))
                        {
                            string tmpString = null;
                            if (splittedRec.Contains("DIVISION FROM"))
                            {
                                tmpString = splittedRec.Replace(I62, "").Substring(splittedRec.IndexOf("DIVISION FROM")).Replace("DIVISION FROM", "").Trim();
                                //tmpPrioValue = splittedRec.Remove(splittedRec.IndexOf("DIVISION FROM")).Trim();
                                if (tmpPrioValue.Contains("DIVISION FROM"))
                                {
                                    tmpPrioValue = tmpPrioValue.Remove(splittedRec.IndexOf("DIVISION FROM")).Trim();
                                }
                            }
                            if (tmpString != null)
                            {
                                currentElement.I62 = tmpString;
                            }
                        }
                        /*Title*/
                        if (splittedRecClear.EndsWith(I54))
                        {
                            string tmpValue = splittedRec.Replace(I54, "").Trim();
                            if (tmpValue.Contains("\t"))
                            {
                                string[] tmpSpl = tmpValue.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                                if (tmpSpl.Count() == 2)
                                {
                                    currentElement.I54Eng = tmpSpl[1].Trim();
                                    currentElement.I54Hebrew = tmpSpl[0].Trim();
                                }
                                else
                                {
                                    currentElement.I54Eng = tmpValue;
                                }
                            }
                            else
                            {
                                currentElement.I54Eng = tmpValue;
                            }
                        }
                        if (splittedRec.EndsWith(I71))
                        {
                            string tmpValue = splittedRec.Replace(I71, "").Trim();
                            string[] twoLangRecords = null;
                            /*delete excess comma*/
                            tmpValue = Regex.Replace(tmpValue, @", llc", " llc", RegexOptions.IgnoreCase);
                            tmpValue = Regex.Replace(tmpValue, @", limited", " limited", RegexOptions.IgnoreCase);
                            tmpValue = Regex.Replace(tmpValue, @", inc", " inc", RegexOptions.IgnoreCase);
                            tmpValue = Regex.Replace(tmpValue, @", ltd", " ltd", RegexOptions.IgnoreCase);
                            /*Case first - two lang separated with \t*/
                            /*If the two languages are present*/
                            if (tmpValue.Contains("\t"))
                            {
                                twoLangRecords = tmpValue.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                                if (twoLangRecords.Count() == 2)
                                {
                                    currentElement.I71NameENG = (currentElement.I71NameENG ?? Enumerable.Empty<string>()).Concat(new string[] { twoLangRecords[1].Trim() }).ToArray();
                                    currentElement.I71CountryENG = (currentElement.I71CountryENG ?? Enumerable.Empty<string>()).Concat(new string[] { "IL" }).ToArray();
                                    if (twoLangRecords[0].Trim().Contains(","))
                                    {
                                        currentElement.I71NameIL = (currentElement.I71NameIL ?? Enumerable.Empty<string>())
                                            .Concat(new string[]
                                            {
                                                twoLangRecords[0].Remove(twoLangRecords[0].IndexOf(",")).Trim()
                                            }).ToArray();
                                        currentElement.I71AddrIL = (currentElement.I71AddrIL ?? Enumerable.Empty<string>())
                                            .Concat(new string[]
                                            {
                                                twoLangRecords[0].Substring(twoLangRecords[0].IndexOf(",")).Replace(",","").Trim()
                                            }).ToArray();
                                    }
                                    else
                                    {
                                        currentElement.I71NameIL = (currentElement.I71NameIL ?? Enumerable.Empty<string>())
                                            .Concat(new string[] { twoLangRecords[0].Trim() }).ToArray();
                                        currentElement.I71AddrIL = (currentElement.I71AddrIL ?? Enumerable.Empty<string>())
                                            .Concat(new string[] { "" }).ToArray();
                                    }
                                    currentElement.I71CountryIL = (currentElement.I71CountryIL ?? Enumerable.Empty<string>()).Concat(new string[] { "IL" }).ToArray();
                                }
                            }
                            /*Case second*/
                            /*If string has no comma at all, has no \t - this means that the country code is IL*/
                            if (!tmpValue.Contains(",") && !tmpValue.Contains("\t"))
                            {
                                currentElement.I71NameENG = (currentElement.I71NameENG ?? Enumerable.Empty<string>()).Concat(new string[] { tmpValue.Trim() }).ToArray();
                                currentElement.I71CountryENG = (currentElement.I71CountryENG ?? Enumerable.Empty<string>()).Concat(new string[] { "IL" }).ToArray();
                            }
                            /*Case third - has comma, has no \t*/
                            /*if there're two or more records separated with comma + country name*/
                            if (tmpValue.Contains(",") && !tmpValue.Contains("\t"))
                            {
                                //string[] tstSpl = Regex.Split(tmpValue, @"(\,\s*[a-zA-Z]+=?)");
                                string[] splittedOwners = Regex.Split(tmpValue, @"(?<=\,\s*[a-zA-Z]+(\n|\$))").Where(x => x != "\n").Select(x => x.Trim()).ToArray();
                                if (splittedOwners != null && splittedOwners.Count() > 0)
                                {
                                    foreach (var owner in splittedOwners)
                                    {
                                        string tmpOwner = null;
                                        string tmpCountry = null;
                                        if (owner.Contains(","))
                                        {
                                            tmpOwner = owner.Remove(owner.IndexOf(","));
                                            tmpCountry = Methods.CountryCodeIdentification(owner.Substring(owner.IndexOf(",")).Trim(','));
                                        }
                                        if (tmpOwner != null)
                                        {
                                            currentElement.I71NameENG = (currentElement.I71NameENG ?? Enumerable.Empty<string>()).Concat(new string[] { tmpOwner.Trim() }).ToArray();
                                        }
                                        if (tmpCountry != null)
                                        {
                                            currentElement.I71CountryENG = (currentElement.I71CountryENG ?? Enumerable.Empty<string>()).Concat(new string[] { tmpCountry }).ToArray();
                                        }
                                        else
                                        {
                                            Console.WriteLine("Country code in the field 71 missed in\t" + currentElement.I21);
                                        }
                                    }
                                }
                            }
                        }
                        if (splittedRec.EndsWith(I72))
                        {
                            string tmpValue = splittedRec.Replace(I72, "").Trim();
                            List<string> listOfNamesEng = new List<string>();
                            List<string> listOfNamesIL = new List<string>();
                            /*First case - only eng names without \t*/
                            if (!tmpValue.Contains("\t"))
                            {
                                if (tmpValue.Contains(","))
                                {
                                    listOfNamesEng = tmpValue.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
                                }
                                else
                                {
                                    listOfNamesEng.Add(tmpValue);
                                }
                                /*Case if eng and hebrew lang separated with \t*/
                            }
                            else if (tmpValue.Contains("\t"))
                            {
                                string namesHebrew = null;
                                string namesEng = null;
                                string[] splittedNames = tmpValue.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                                if (splittedNames.Count() == 2)
                                {
                                    namesHebrew = splittedNames[0].Trim();
                                    namesEng = splittedNames[1].Trim();
                                }
                                if (namesEng != null && namesHebrew != null)
                                {
                                    if (namesEng.Contains(","))
                                    {
                                        listOfNamesEng = namesEng.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
                                    }
                                    else listOfNamesEng.Add(namesEng);
                                    if (namesHebrew.Contains(","))
                                    {
                                        listOfNamesIL = namesHebrew.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
                                    }
                                    else listOfNamesIL.Add(namesHebrew);
                                }
                            }
                            /*Case - if only eng names*/
                            if (listOfNamesEng.Count > 0 && listOfNamesIL.Count == 0)
                            {
                                currentElement.I72Eng = listOfNamesEng.ToArray();
                            }
                            /*Case - if eng and hebrew names has same count*/
                            if (listOfNamesEng.Count > 0 && listOfNamesIL.Count > 0 && listOfNamesEng.Count == listOfNamesIL.Count)
                            {
                                currentElement.I72Eng = listOfNamesEng.ToArray();
                                currentElement.I72IL = listOfNamesIL.ToArray();
                            }
                            /*Case - if eng and hebrew names count are not the same*/
                            if (listOfNamesEng.Count > 0 && listOfNamesIL.Count > 0 && listOfNamesEng.Count > listOfNamesIL.Count)
                            {
                                Console.WriteLine("Possible error in 72 field with name identification:\t" + currentElement.I21);
                                currentElement.I72IL = listOfNamesIL.ToArray();
                                currentElement.I72Eng = listOfNamesEng.ToArray();
                            }
                        }
                        if (splittedRec.EndsWith(I74))
                        {
                            string tmpValue = splittedRec.Replace(I74, "").Trim();
                            string recordEng = null;
                            string recordHeb = null;
                            string recEngName = null;
                            string recEngAddr = null;
                            string recHebName = null;
                            string recHebAddr = null;

                            if (tmpValue.Contains("\t"))
                            {
                                var splValue = tmpValue.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                                if (splValue.Count() == 2)
                                {
                                    recordHeb = splValue[0].Trim();
                                    recordEng = splValue[1].Trim();
                                }
                            }
                            else
                            {
                                recordHeb = tmpValue;
                            }
                            if (recordHeb != null)
                            {
                                /*Hebrew record*/
                                recordHeb = recordHeb.Replace("\n", " ").Trim();
                                if (recordHeb.Contains(","))
                                {
                                    recHebName = recordHeb.Remove(recordHeb.IndexOf(",")).Trim();
                                    recHebAddr = recordHeb.Substring(recordHeb.IndexOf(",")).Trim().Trim(',').Trim();
                                }
                                else
                                {
                                    recHebName = recordHeb;
                                }
                            }
                            if (recordEng != null)
                            {
                                /*Eng record*/
                                recordEng = recordEng.Replace("\n", " ").Trim();
                                if (recordEng.Contains(","))
                                {
                                    recEngName = recordEng.Remove(recordEng.IndexOf(",")).Trim();
                                    recEngAddr = recordEng.Substring(recordEng.IndexOf(",")).Trim().Trim(',').Trim();
                                }
                                else
                                {
                                    recEngName = recordEng;
                                }
                            }
                            if (recHebName != null && recHebAddr != null)
                            {
                                currentElement.I74HebName = recHebName;
                                currentElement.I74HebAddr = recHebAddr;
                            }
                            if (recEngName != null && recEngAddr != null)
                            {
                                currentElement.I74EngName = recEngName;
                                currentElement.I74EngAddr = recEngAddr;
                            }
                        }
                        if (splittedRec.EndsWith(I87))
                        {
                            string tmpValue = splittedRec.Replace(I87, "").Trim();
                            currentElement.I87 = tmpValue;
                        }
                        /*Notes info*/
                        if (splittedRec.Contains("The parent application from which") ||
                            splittedRec.Contains("The applications for division") ||
                            splittedRec.Contains("This specification was examined in"))
                        {
                            string tmpInfo = splittedRec;
                            string hebNotes = null;
                            string engNotes = null;
                            string numbersNotes = null;
                            string patternNotes = @"(?<hebrewPart>^.*)(\t(?<numbers>.*))*\t(?<engPart>.*$)";
                            if (splittedRec.Contains("______")) tmpInfo = splittedRec.Remove(splittedRec.IndexOf("______")).Trim();
                            if (Regex.IsMatch(tmpInfo, patternNotes))
                            {
                                hebNotes = Regex.Match(tmpInfo, patternNotes).Groups["hebrewPart"].Value.Replace("\t", " ").Replace("\n", " ").Trim();
                                numbersNotes = Regex.Match(tmpInfo, patternNotes).Groups["numbers"].Value.Replace("\t", " ").Replace("\n", " ").Trim();
                                engNotes = Regex.Match(tmpInfo, patternNotes).Groups["engPart"].Value.Replace("\t", " ").Replace("\n", " ").Trim();
                            }
                            if (hebNotes != null && engNotes != null)
                            {
                                currentElement.INoteHeb = hebNotes.Trim(',').Trim();
                                currentElement.INoteEng = engNotes;
                            }
                        }
                    }
                    /*Priority processing (3x inids)*/
                    if (tmpPrioValue != null && tmpPrioValue.Length > 10)
                    {
                        List<string> prioLine = new List<string>();
                        if (tmpPrioValue.Contains("\n"))
                        {
                            prioLine = tmpPrioValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
                        }
                        else
                        {
                            prioLine.Add(tmpPrioValue.Trim());
                        }
                        if (prioLine != null && prioLine.Count() > 0)
                        {
                            foreach (var line in prioLine)
                            {
                                string tmpLineValue = line.Trim();
                                string country = null;
                                string date = null;
                                string number = null;
                                if (Regex.IsMatch(tmpLineValue, @"^[A-Z]{2}"))
                                    country = Regex.Match(tmpLineValue, @"^[A-Z]{2}").Value;
                                if (Regex.IsMatch(tmpLineValue, @"\d{2}\.\d{2}\.\d{4}"))
                                    date = Regex.Match(tmpLineValue, @"\d{2}\.\d{2}\.\d{4}").Value;
                                if (country != null && date != null)
                                {
                                    number = tmpLineValue.Replace(country, "").Replace(date, "").Trim();
                                }
                                if (country != null && number != null && date != null)
                                {
                                    /*Number*/
                                    currentElement.I31 = (currentElement.I31 ?? Enumerable.Empty<string>()).Concat(new string[] { number }).ToArray();
                                    /*Date*/
                                    currentElement.I32 = (currentElement.I32 ?? Enumerable.Empty<string>()).Concat(new string[] { Methods.NormalizeDate(date) }).ToArray();
                                    /*Country*/
                                    currentElement.I33 = (currentElement.I33 ?? Enumerable.Empty<string>()).Concat(new string[] { country }).ToArray();
                                }
                            }
                        }
                    }
                }
            }
            return ElementsOut;
        }
    }
}
