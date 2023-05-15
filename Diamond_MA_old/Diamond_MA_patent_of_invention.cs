using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_MA_old
{
    class Diamond_MA_patent_of_invention
    {
        private static readonly string I11 = "(11) N° de publication :";
        private static readonly string I21 = "(21) N° Dépôt :";
        private static readonly string I22 = "(22) Date de Dépôt :";
        private static readonly string I30 = "(30) Données de Priorité :";
        private static readonly string I43 = "(43) Date de publication :";
        private static readonly string I51 = "(51) Cl. internationale :";
        private static readonly string I54 = "(54) Titre :";
        private static readonly string I57 = "(57)";
        private static readonly string I71 = "(71) Demandeur(s) :";
        private static readonly string I72 = "(72) Inventeur(s) :";
        private static readonly string I74 = "(74) Mandataire :";
        private static readonly string I86 = "(86) Données relatives à la demande internationale selon le PCT:";

        /*Date*/
        static string DateNormalize(string tmpDate)
        {
            var swapDate = tmpDate;
            string[] splitDate = null;
            if (tmpDate.Contains("."))
            {
                splitDate = tmpDate.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                if (splitDate.Count() == 3)
                {
                    return swapDate = splitDate[2] + "-" + splitDate[1] + "-" + splitDate[0];
                }
            }
            return tmpDate;
        }

        static string[] RecSplit(string recString)
        {
            string[] splittedRecord = null;
            var tempStrC = recString.Replace("•", "").Trim();
            string tempDesc = null;
            if (tempStrC.Contains(I57))
            {
                tempDesc = tempStrC.Substring(tempStrC.IndexOf(I57)).Trim();
                tempStrC = tempStrC.Remove(tempStrC.IndexOf(I57)).Trim();
            }
            if (tempStrC != "")
            {
                if (tempStrC.Contains("\n"))
                {
                    tempStrC = tempStrC.Replace("\n", " ");
                }
                var regexPatOne = new Regex(@"\(\d{2}\)\s", RegexOptions.IgnoreCase);
                var matchesClass = regexPatOne.Matches(tempStrC);
                if (matchesClass.Count > 0)
                {
                    foreach (Match matchC in matchesClass)
                    {
                        tempStrC = tempStrC.Replace(matchC.Value, "***" + matchC.Value);
                    }
                }
                /*Splitting record*/
                splittedRecord = tempStrC.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempDesc != null)
                {
                    splittedRecord = (splittedRecord ?? Enumerable.Empty<string>()).Concat(new string[] { tempDesc.Replace("\n", " ") }).ToArray();
                }
            }
            return splittedRecord;
        }

        //static List<ElementOut> ElementsOut = new List<ElementOut>();

        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\MA\Gaz\");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (var file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            string fileName; //имя файла tetml
            string folderPath; //путь к папке с файлом tetml
            string path; //путь к файлу tetml
            XElement tet;
            DirectoryInfo root;
            StreamWriter sf;
            ElementOut currentElement;
            foreach (var tetFile in files)
            {
                ElementOut.ElementsOut.Clear();
                //ElementsOut.Clear();
                fileName = tetFile;
                root = Directory.GetParent(fileName);
                folderPath = Path.Combine(root.FullName);
                path = Path.Combine(folderPath, fileName.Substring(0, fileName.IndexOf(".")) + ".txt"); //Output Filename
                tet = XElement.Load(fileName);
                Directory.CreateDirectory(folderPath);
                sf = new StreamWriter(path);
                /*TETML elements*/
                var elements = tet.Descendants().Where(d => d.Name.LocalName == "Text").ToList();
                for (var i = 0; i < elements.Count; ++i)
                {
                    var element = elements[i];
                    var value = element.Value;
                    string tmpRecordValue = null;
                    string[] splittedRecord = null;
                    int tmpInc;
                    string tmpRecValue;
                    if (value.StartsWith(I11))
                    {
                        currentElement = new ElementOut();
                        ElementOut.ElementsOut.Add(currentElement);
                        tmpRecordValue = "";
                        tmpInc = i;
                        do
                        {
                            tmpRecordValue += elements[tmpInc].Value + "\n";
                            ++tmpInc;
                        } while (tmpInc < elements.Count()
                        && !elements[tmpInc].Value.StartsWith("ROYAUME DU MAROC")
                        && !elements[tmpInc].Value.StartsWith(I11));
                        if (tmpRecordValue != null)
                        {
                            splittedRecord = RecSplit(tmpRecordValue);
                        }
                        foreach (var inidCode in splittedRecord)
                        {
                            tmpRecValue = null;
                            /*Resolution value*/
                            if (inidCode.StartsWith(I11))
                            {
                                tmpRecValue = inidCode.Replace(I11, "").Replace("\n", "").Trim();
                                if (tmpRecValue.Contains(" "))
                                {
                                    currentElement.I11 = tmpRecValue.Remove(tmpRecValue.LastIndexOf(" ")).Trim();
                                    currentElement.I13 = tmpRecValue.Substring(tmpRecValue.LastIndexOf(" ")).Trim();
                                }
                                else
                                {
                                    currentElement.I11 = tmpRecValue;
                                }
                            }
                            /*43*/
                            if (inidCode.StartsWith(I43))
                            {
                                tmpRecValue = inidCode.Replace(I43, "").Trim();
                                currentElement.I43 = DateNormalize(tmpRecValue);
                            }
                            /*21*/
                            if (inidCode.StartsWith(I21))
                            {
                                tmpRecValue = inidCode.Replace(I21, "").Trim();
                                currentElement.I21 = tmpRecValue;
                            }
                            /*22*/
                            if (inidCode.StartsWith(I22))
                            {
                                tmpRecValue = inidCode.Replace(I22, "").Trim();
                                currentElement.I22 = DateNormalize(tmpRecValue);
                            }
                            ///*57*/
                            //if (value.StartsWith(I57))
                            //{
                            //    tmpRecValue = inidCode.Replace(I57, "").Replace("(57) Abrégé", "").Trim();
                            //    currentElement.I57 = tmpRecValue;
                            //}
                            /*31,32,33*/
                            if (inidCode.StartsWith(I30))
                            {
                                string tmpCC = null;
                                string tmpDate = null;
                                string tmpNumber = null;
                                tmpRecValue = inidCode.Replace(I30, "").Trim();
                                if (Regex.IsMatch(tmpRecValue, @"\d{2}\.\d{2}\.\d{4}"))
                                {
                                    tmpDate = Regex.Match(tmpRecValue, @"\d{2}\.\d{2}\.\d{4}").Value;
                                    tmpRecValue = tmpRecValue.Replace(tmpDate, "").Trim();
                                }
                                if (Regex.IsMatch(tmpRecValue, @"\s*[A-Z]{2}\s"))
                                {
                                    tmpCC = Regex.Match(tmpRecValue, @"\s*[A-Z]{2}\s").Value;
                                    tmpRecValue = tmpRecValue.Replace(tmpCC, "").Trim();
                                }
                                if (tmpDate != null && tmpCC != null)
                                {
                                    tmpNumber = tmpRecValue.Trim();
                                }
                                if (tmpCC != null && tmpDate != null && tmpNumber != null)
                                {
                                    currentElement.I31 = tmpNumber;
                                    currentElement.I32 = DateNormalize(tmpDate);
                                    currentElement.I33 = tmpCC.Trim();
                                }
                            }
                            /*86*/
                            if (inidCode.StartsWith(I86))
                            {
                                string tmpDate = null;
                                string tmpNumber = null;
                                tmpRecValue = inidCode.Replace(I86, "").Trim();
                                if (Regex.IsMatch(tmpRecValue, @"\d{2}\.\d{2}\.\d{4}"))
                                {
                                    tmpDate = Regex.Match(tmpRecValue, @"\d{2}\.\d{2}\.\d{4}").Value;
                                    tmpNumber = tmpRecValue.Replace(tmpDate, "").Trim();
                                }
                                if (tmpDate != null && tmpNumber != null)
                                {
                                    currentElement.I86N = tmpNumber.Trim();
                                    currentElement.I86D = DateNormalize(tmpDate);
                                }
                            }
                            /*51 international class*/
                            if (inidCode.StartsWith(I51))
                            {
                                string[] tmpSplittedValue = null;
                                string[] tmpIntClass = null;
                                tmpRecValue = inidCode.Replace(I51, "");
                                if (tmpRecValue.Contains(";"))
                                {
                                    tmpSplittedValue = tmpRecValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    if (tmpSplittedValue != null)
                                    {
                                        foreach (var record in tmpSplittedValue)
                                        {
                                            tmpIntClass = (tmpIntClass ?? Enumerable.Empty<string>()).Concat(new string[] { record.Trim() }).ToArray();
                                        }
                                    }
                                }
                                else
                                {
                                    tmpIntClass = (tmpIntClass ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Trim() }).ToArray();
                                }
                                if (tmpIntClass != null)
                                {
                                    currentElement.I51N = tmpIntClass;
                                }
                            }
                            /*71*/
                            if (inidCode.StartsWith(I71))
                            {
                                tmpRecValue = inidCode.Replace(I71, "").Trim();
                                try
                                {
                                    if (Regex.IsMatch(tmpRecValue, @".*\([A-Z]{2}\)"))
                                    {
                                        var pat = new Regex(@".*?\([A-Z]{2}\)", RegexOptions.Singleline);
                                        var matchCollection = pat.Matches(tmpRecValue);
                                        foreach (Match item in matchCollection)
                                        {
                                            var tmpSplValue = item.Value;
                                            string tmpSplName = null;
                                            string tmpSplCC = null;
                                            string tmpSplAddr = null;
                                            if (Regex.IsMatch(tmpSplValue, @"\([A-Z]{2}\)"))
                                            {
                                                tmpSplCC = Regex.Match(tmpSplValue, @"\([A-Z]{2}\)").Value.Replace("(", "").Replace(")", "").Trim();
                                            }
                                            /*Test*/
                                            if (tmpSplValue.Contains(","))
                                            {
                                                if (Regex.IsMatch(tmpSplValue.ToLower(), @"(\,.*\sllc\,)|(\,.*\sgmbh\,)|(\,.*\sinc(\.|\,)*\,)|(\,.*\sltd(\.|\,)*\,)|(\,.*\ss\.a\.\,)|(\,.*\ss\.l\.\,)|(\,.*\ss\.a\.u\.\,)"))
                                                {
                                                    if (tmpSplValue.Count(f => f == ',') >= 2)
                                                    {
                                                        var firstIndex = tmpSplValue.IndexOf(",");
                                                        var secondIndex = tmpSplValue.IndexOf(",", firstIndex + 1);
                                                        tmpSplName = tmpSplValue.Remove(secondIndex).Trim();
                                                        tmpSplAddr = tmpSplValue.Substring(secondIndex).Trim(',').Trim();
                                                    }
                                                }
                                                else
                                                {
                                                    tmpSplName = tmpSplValue.Remove(tmpSplValue.IndexOf(",")).Trim();
                                                    tmpSplAddr = tmpSplValue.Substring(tmpSplValue.IndexOf(",")).Trim(',').Trim();
                                                    /*Deleting extra phrases fron address*/
                                                    //if (Regex.IsMatch(tmpSplAddr.ToLower(), @"^llc\,")
                                                    //    || Regex.IsMatch(tmpSplAddr.ToLower(), @"^gmbh\,")
                                                    //    || Regex.IsMatch(tmpSplAddr.ToLower(), @"^inc(\.|\,)*\,")
                                                    //    || Regex.IsMatch(tmpSplAddr.ToLower(), @"^ltd(\.|\,)*\,")
                                                    //    || Regex.IsMatch(tmpSplAddr.ToLower(), @"^s\.a\.\,")
                                                    //    || Regex.IsMatch(tmpSplAddr.ToLower(), @"^s\.l\.\,")
                                                    //    || Regex.IsMatch(tmpSplAddr.ToLower(), @"^s\.a\.u\.\,"))
                                                    //{
                                                    //    tmpSplAddr = tmpSplAddr.Substring(tmpSplAddr.IndexOf(",")).Trim().Trim(',').Trim();
                                                    //}
                                                }
                                            }
                                            if (tmpSplName != null && tmpSplAddr != null && tmpSplCC != null)
                                            {
                                                currentElement.I71N = (currentElement.I71N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpSplName }).ToArray();
                                                currentElement.I71A = (currentElement.I71A ?? Enumerable.Empty<string>()).Concat(new string[] { Regex.Replace(tmpSplAddr.Trim(), @"\([A-Z]{2}\)$", "").Trim().Trim('.').Trim(',').Trim() }).ToArray();
                                                currentElement.I71C = (currentElement.I71C ?? Enumerable.Empty<string>()).Concat(new string[] { tmpSplCC }).ToArray();
                                            }
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("72 inid identifications error");
                                }
                            }
                            /*72*/
                            if (inidCode.StartsWith(I72))
                            {
                                string[] tmpSplittedValue = null;
                                tmpRecValue = inidCode.Replace(I72, "").Trim();
                                if (tmpRecValue.Contains(";"))
                                {
                                    tmpSplittedValue = tmpRecValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    if (tmpSplittedValue != null)
                                    {
                                        foreach (var item in tmpSplittedValue)
                                        {
                                            if (item.Length > 2)
                                            {
                                                currentElement.I72 = (currentElement.I72 ?? Enumerable.Empty<string>()).Concat(new string[] { item.Trim() }).ToArray();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    currentElement.I72 = (currentElement.I72 ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Trim() }).ToArray();
                                }
                            }
                            /*74*/
                            if (inidCode.StartsWith(I74))
                            {
                                tmpRecValue = inidCode.Replace(I74, "").Replace("\n", " ");
                                currentElement.I74 = tmpRecValue.Trim();
                            }
                            /*54 title*/
                            if (inidCode.StartsWith(I54))
                            {
                                tmpRecValue = inidCode.Replace(I54, "").Trim();
                                currentElement.I54 = tmpRecValue;
                            }
                            /*57*/
                            if (inidCode.StartsWith(I57))
                            {
                                tmpRecValue = inidCode.Replace("(57) ABSTRACT", "").Replace("(57) Abrégé :", "").Replace("(57) Abrégé", "").Replace(I57, "").Trim();
                                currentElement.I57 = tmpRecValue;
                            }
                        }
                    }
                }
                /*Output*/
                if (ElementOut.ElementsOut != null)
                {
                    foreach (var field in ElementOut.ElementsOut)
                    {
                        sf.WriteLine("****");
                        sf.WriteLine("11:\t{0}", field.I11);
                        if (field.I13 != null)
                        {
                            sf.WriteLine("13:\t{0}", field.I13);
                        }
                        sf.WriteLine("21:\t{0}", field.I21);
                        if (field.I31 != null && field.I32 != null && field.I33 != null)
                        {
                            sf.WriteLine("30:\t{0}\t{1}\t{2}", field.I31, field.I32, field.I33);
                        }
                        if (field.I43 != null)
                        {
                            sf.WriteLine("43:\t{0}", field.I43);
                        }
                        if (field.I51N != null)
                        {
                            for (var i = 0; i < field.I51N.Count(); i++)
                            {
                                sf.WriteLine("51:\t{0}", field.I51N[i]);
                            }
                        }
                        if (field.I71N != null && field.I71A != null && field.I71C != null)
                        {
                            if (field.I71N.Count() == field.I71A.Count() && field.I71N.Count() == field.I71C.Count())
                            {
                                for (var i = 0; i < field.I71N.Count(); i++)
                                {
                                    sf.WriteLine("71N:\t{0}", field.I71N[i]);
                                    sf.WriteLine("71A:\t{0}", field.I71A[i]);
                                    sf.WriteLine("71C:\t{0}", field.I71C[i]);
                                }
                            }
                        }
                        if (field.I72 != null)
                        {
                            foreach (var item in field.I72)
                            {
                                sf.WriteLine("72:\t{0}", item);
                            }
                        }
                        if (field.I74 != null)
                        {
                            sf.WriteLine("74:\t{0}", field.I74);
                        }
                        if (field.I86D != null && field.I86N != null)
                        {
                            sf.WriteLine("86:\t{0}\t{1}", field.I86N, field.I86D);
                        }
                        sf.WriteLine("54:\t{0}", field.I54);
                        sf.WriteLine("57:\t{0}", field.I57);
                    }
                }
                sf.Flush();
                sf.Close();
            }
        }
    }
}
