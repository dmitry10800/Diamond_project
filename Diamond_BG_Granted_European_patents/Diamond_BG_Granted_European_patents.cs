using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_BG_Granted_European_patents
{
    class Diamond_BG_Granted_European_patents
    {
        private static readonly string I11 = "(11)";
        private static readonly string I24 = "(24)";
        private static readonly string I31 = "(31)";
        private static readonly string I32 = "(32)";
        private static readonly string I33 = "(33)";
        private static readonly string I51 = "(51) Int. Cl.";
        private static readonly string I54 = "(54)";
        private static readonly string I57 = "(57)";
        private static readonly string I62 = "(62)";
        private static readonly string I72 = "(72)";
        private static readonly string I73 = "(73)";
        private static readonly string I74 = "(74)";
        private static readonly string I86 = "(86)";
        private static readonly string I87 = "(87)";
        private static readonly string I96 = "(96)";
        private static readonly string I97 = "(97)";

        /*Date*/
        static string DateNormalize(string tmpDate)
        {
            string swapDate = tmpDate;
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
            string tempStrC = recString.Trim();
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
                        if (!matchC.Value.StartsWith(I32) && !matchC.Value.StartsWith(I33))
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

        //static List<ElementOut> ElementsOut = new List<ElementOut>();

        static void Main(string[] args)
        {

            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\BG\Grant\");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
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
                var elements = tet.Descendants().Where(d => d.Name.LocalName == "Text" /*&& d.Value != "РАЗДЕЛ А"*/)
                    .SkipWhile(e => !e.Value.StartsWith("РАЗДЕЛ А"))
                    .ToList();
                for (int i = 0; i < elements.Count; ++i)
                {
                    var element = elements[i];
                    string value = element.Value;
                    string tmpRecordValue = null;
                    string[] splittedRecord = null;
                    int tmpInc;
                    string tmpRecValue;
                    if (value.StartsWith(I11) && Regex.IsMatch(value, @"\(11\)\s"))
                    {
                        currentElement = new ElementOut();
                        //ElementsOut.Add(currentElement);
                        ElementOut.ElementsOut.Add(currentElement);
                        tmpRecordValue = "";
                        tmpInc = i;
                        do
                        {
                            tmpRecordValue += elements[tmpInc].Value + "\n";
                            ++tmpInc;
                        } while (tmpInc < elements.Count()
                        //&& !elements[tmpInc].Value.StartsWith(I11)
                        && !Regex.IsMatch(elements[tmpInc].Value, @"\(11\)\s")
                        && !Regex.IsMatch(elements[tmpInc].Value, @"(\d{1}|\d{2})\s*претенц"));
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
                            /*51 international class*/
                            if (inidCode.StartsWith(I51))
                            {
                                string[] tmpSplittedValue = null;
                                string[] tmpIntClass = null;
                                string[] tmpVersion = null;
                                string datePattern = @"\(\d{4}\.\d{2}\)";
                                tmpRecValue = inidCode.Replace(I51, "").Trim('\n');
                                if (tmpRecValue.Contains("\n"))
                                {
                                    tmpSplittedValue = tmpRecValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    if (tmpSplittedValue != null)
                                    {
                                        foreach (var record in tmpSplittedValue)
                                        {
                                            if (Regex.IsMatch(record, datePattern))
                                            {
                                                tmpVersion = (tmpVersion ?? Enumerable.Empty<string>()).Concat(new string[] { Regex.Match(record, datePattern).Value.Replace("(", "").Replace(")", "") }).ToArray();
                                                tmpIntClass = (tmpIntClass ?? Enumerable.Empty<string>()).Concat(new string[] { Regex.Replace(record, datePattern, "").Replace(" ", "").Insert(4, " ").Trim() }).ToArray();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (Regex.IsMatch(tmpRecValue, datePattern))
                                    {
                                        tmpVersion = (tmpVersion ?? Enumerable.Empty<string>()).Concat(new string[] { Regex.Match(tmpRecValue, datePattern).Value.Replace("(", "").Replace(")", "") }).ToArray();
                                        tmpIntClass = (tmpIntClass ?? Enumerable.Empty<string>()).Concat(new string[] { Regex.Replace(tmpRecValue, datePattern, "").Replace(" ", "").Insert(4, " ").Trim() }).ToArray();
                                    }
                                }
                                if (tmpIntClass != null && tmpVersion != null)
                                {
                                    currentElement.I51N = tmpIntClass;
                                    currentElement.I51D = tmpVersion;
                                }
                            }
                            /*97*/
                            if (inidCode.StartsWith(I97))
                            {
                                string datePattern = @"\d{2}\.\d{2}\.\d{4}";
                                tmpRecValue = inidCode.Replace(I97, "").Replace("\n", "").Trim();
                                if (Regex.IsMatch(tmpRecValue, datePattern))
                                {
                                    currentElement.I97D = DateNormalize(Regex.Match(tmpRecValue, datePattern).Value);
                                    currentElement.I97N = Regex.Replace(tmpRecValue, datePattern, "").Replace(",", "").Trim();
                                }

                            }
                            /*96*/
                            if (inidCode.StartsWith(I96))
                            {
                                string datePattern = @"\d{2}\.\d{2}\.\d{4}";
                                tmpRecValue = inidCode.Replace(I96, "").Replace("\n", "").Trim();
                                if (Regex.IsMatch(tmpRecValue, datePattern))
                                {
                                    currentElement.I96D = DateNormalize(Regex.Match(tmpRecValue, datePattern).Value);
                                    currentElement.I96N = Regex.Replace(tmpRecValue, datePattern, "").Replace(",", "").Trim();
                                }
                            }
                            /*24*/
                            if (inidCode.StartsWith(I24))
                            {
                                tmpRecValue = inidCode.Replace(I24, "").Replace("\n", "").Trim();
                                if (Regex.IsMatch(tmpRecValue, @"\d{2}\.\d{2}\.\d{4}"))
                                {
                                    currentElement.I24 = DateNormalize(Regex.Match(tmpRecValue, @"\d{2}\.\d{2}\.\d{4}").Value.Trim());
                                }
                            }
                            /*31,32,33*/
                            if (inidCode.StartsWith(I31))
                            {
                                string[] tmpCC = null;
                                string[] tmpDate = null;
                                string[] tmpNumber = null;
                                string[] tmpSplittedValue = null;
                                string recordPattern = @".*\s*\d{2}\.\d{2}\.\d{4}\s*[A-Z]{2}$";
                                tmpRecValue = inidCode.Replace(I31, "").Replace(I32, "").Replace(I33, "").Trim();
                                if (tmpRecValue.Contains("\n"))
                                {
                                    tmpSplittedValue = tmpRecValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    foreach (var record in tmpSplittedValue)
                                    {
                                        if (Regex.IsMatch(record, recordPattern))
                                        {
                                            tmpCC = (tmpCC ?? Enumerable.Empty<string>()).Concat(new string[]
                                            {
                                                Regex.Match(record, @"[A-Z]{2}$").Value
                                            }).ToArray();
                                            tmpDate = (tmpDate ?? Enumerable.Empty<string>()).Concat(new string[]
                                            {
                                                DateNormalize(Regex.Match(record, @"\d{2}\.\d{2}\.\d{4}").Value)
                                            }).ToArray();
                                            tmpNumber = (tmpNumber ?? Enumerable.Empty<string>()).Concat(new string[]
                                            {
                                                record.Remove(record.IndexOf(Regex.Match(record, @"\d{2}\.\d{2}\.\d{4}").Value))
                                            }).ToArray();
                                        }
                                    }
                                }
                                else
                                {
                                    if (Regex.IsMatch(tmpRecValue, recordPattern))
                                    {
                                        tmpCC = (tmpCC ?? Enumerable.Empty<string>()).Concat(new string[]
                                        {
                                            Regex.Match(tmpRecValue, @"[A-Z]{2}$").Value
                                        }).ToArray();
                                        tmpDate = (tmpDate ?? Enumerable.Empty<string>()).Concat(new string[]
                                        {
                                            DateNormalize(Regex.Match(tmpRecValue, @"\d{2}\.\d{2}\.\d{4}").Value)
                                        }).ToArray();
                                        tmpNumber = (tmpNumber ?? Enumerable.Empty<string>()).Concat(new string[]
                                        {
                                            tmpRecValue.Remove(tmpRecValue.IndexOf(Regex.Match(tmpRecValue, @"\d{2}\.\d{2}\.\d{4}").Value))
                                        }).ToArray();
                                    }
                                }
                                if (tmpCC != null && tmpDate != null && tmpNumber != null)
                                {
                                    currentElement.I31 = tmpNumber;
                                    currentElement.I32 = tmpDate;
                                    currentElement.I33 = tmpCC;
                                }
                            }
                            /*86*/
                            if (inidCode.StartsWith(I86))
                            {
                                tmpRecValue = inidCode.Replace(I86, "").Trim();
                                currentElement.I86 = tmpRecValue;
                            }
                            /*87*/
                            if (inidCode.StartsWith(I87))
                            {
                                string datePattern = @"\d{2}\.\d{2}\.\d{4}";
                                tmpRecValue = inidCode.Replace(I87, "").Replace("\n", "").Trim();
                                if (Regex.IsMatch(tmpRecValue, datePattern))
                                {
                                    currentElement.I87D = DateNormalize(Regex.Match(tmpRecValue, datePattern).Value);
                                    currentElement.I87N = Regex.Replace(tmpRecValue, datePattern, "").Replace(",", "").Trim();
                                }
                            }
                            /*73*/
                            if (inidCode.StartsWith(I73))
                            {
                                string tmpAddress = null;
                                string tmpCountryCode = null;
                                string[] tmpSplittedValue = null;
                                tmpRecValue = inidCode.Replace(I73, "").Replace("\n", "").Replace(I73, "").Trim();
                                if (tmpRecValue.Contains(";"))
                                {
                                    tmpSplittedValue = tmpRecValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                                    if (tmpSplittedValue != null)
                                    {
                                        Array.Reverse(tmpSplittedValue);
                                        foreach (var record in tmpSplittedValue)
                                        {
                                            if (record.Contains(","))
                                            {
                                                string tmpSplName = null;
                                                string tmpSplAddr = null;
                                                if (Regex.IsMatch(record.ToLower(), @"(\,.*\sllc\,)|(\,.*\sgmbh\,)|(\,.*\sinc(\.|\,)*\,)|(\,.*\sltd(\.|\,)*\,)|(\,.*\ss\.a\.\,)|(\,.*\ss\.l\.\,)|(\,.*\ss\.a\.u\.\,)"))
                                                {
                                                    if (record.Count(f => f == ',') >= 2)
                                                    {
                                                        int firstIndex = record.IndexOf(",");
                                                        int secondIndex = record.IndexOf(",", firstIndex + 1);
                                                        tmpSplName = record.Remove(secondIndex).Trim();
                                                        tmpSplAddr = record.Substring(secondIndex).Trim(',').Trim();
                                                    }
                                                }
                                                else
                                                {
                                                    tmpSplName = record.Remove(record.IndexOf(",")).Trim();
                                                    tmpSplAddr = record.Substring(record.IndexOf(",")).Trim(',').Trim();
                                                }
                                                currentElement.I73N = (currentElement.I73N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpSplName }).ToArray();
                                                if (Regex.IsMatch(tmpSplAddr, @"\([A-Z]{2}\)$"))
                                                {
                                                    try
                                                    {
                                                        tmpCountryCode = Regex.Match(tmpSplAddr, @"\([A-Z]{2}\)$").Value.Replace("(", "").Replace(")", "").Trim();
                                                        tmpAddress = Regex.Replace(tmpSplAddr, @"\([A-Z]{2}\)$", "").Trim(',').Trim();
                                                    }
                                                    catch (Exception)
                                                    {
                                                        Console.WriteLine("73 field address or CC error");
                                                    }
                                                }
                                                else if (!Regex.IsMatch(record, @"\([A-Z]{2}\)$") && tmpCountryCode != null)
                                                {
                                                    tmpAddress = Regex.Replace(record.Substring(record.IndexOf(",")), @"\([A-Z]{2}\)$", "").Trim(',').Trim();
                                                }
                                                if (tmpAddress != null && tmpCountryCode != null)
                                                {
                                                    currentElement.I73A = (currentElement.I73A ?? Enumerable.Empty<string>()).Concat(new string[] { tmpAddress }).ToArray();
                                                    currentElement.I73C = (currentElement.I73C ?? Enumerable.Empty<string>()).Concat(new string[] { tmpCountryCode }).ToArray();
                                                }
                                            }
                                            else
                                            {
                                                currentElement.I73N = (currentElement.I73N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue }).ToArray(); ;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (tmpRecValue.Contains(","))
                                    {
                                        string tmpSplName = null;
                                        string tmpSplAddr = null;
                                        if (Regex.IsMatch(tmpRecValue.ToLower(), @"(\,.*\sllc\,)|(\,.*\sgmbh\,)|(\,.*\sinc(\.|\,)*\,)|(\,.*\sltd(\.|\,)*\,)|(\,.*\ss\.a\.\,)|(\,.*\ss\.l\.\,)|(\,.*\ss\.a\.u\.\,)"))
                                        {
                                            if (tmpRecValue.Count(f => f == ',') >= 2)
                                            {
                                                int firstIndex = tmpRecValue.IndexOf(",");
                                                int secondIndex = tmpRecValue.IndexOf(",", firstIndex + 1);
                                                tmpSplName = tmpRecValue.Remove(secondIndex).Trim();
                                                tmpSplAddr = tmpRecValue.Substring(secondIndex).Trim(',').Trim();
                                            }
                                        }
                                        else
                                        {
                                            tmpSplName = tmpRecValue.Remove(tmpRecValue.IndexOf(",")).Trim();
                                            tmpSplAddr = tmpRecValue.Substring(tmpRecValue.IndexOf(",")).Trim(',').Trim();
                                        }
                                        currentElement.I73N = (currentElement.I73N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpSplName }).ToArray();
                                        if (Regex.IsMatch(tmpSplAddr, @"\([A-Z]{2}\)$"))
                                        {
                                            try
                                            {
                                                tmpCountryCode = Regex.Match(tmpSplAddr, @"\([A-Z]{2}\)$").Value.Replace("(", "").Replace(")", "").Trim();
                                                tmpAddress = Regex.Replace(tmpSplAddr, @"\([A-Z]{2}\)$", "").Trim(',').Trim();
                                            }
                                            catch (Exception)
                                            {
                                                Console.WriteLine("73 field address or CC error");
                                            }
                                        }
                                        if (tmpAddress != null && tmpCountryCode != null)
                                        {
                                            currentElement.I73A = (currentElement.I73A ?? Enumerable.Empty<string>()).Concat(new string[] { tmpAddress }).ToArray();
                                            currentElement.I73C = (currentElement.I73C ?? Enumerable.Empty<string>()).Concat(new string[] { tmpCountryCode }).ToArray();
                                        }
                                    }
                                    else
                                    {
                                        currentElement.I73N = (currentElement.I73N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue }).ToArray(); ;
                                    }
                                }
                            }
                            /*54 title*/
                            if (inidCode.StartsWith(I54))
                            {
                                tmpRecValue = inidCode.Replace(I54, "").Replace("-\n", " ").Replace("\n", " ").Trim();
                                currentElement.I54 = tmpRecValue;
                            }
                            /*72 Inventor*/
                            if (inidCode.StartsWith(I72))
                            {
                                tmpRecValue = inidCode.Replace(I72, "").Replace("\n", " ").Replace(",", " ").Replace("  ", " ").Trim();
                                if (tmpRecValue.Contains(";"))
                                {
                                    string[] tmpSplValue = tmpRecValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    if (tmpSplValue.Count() > 0)
                                    {
                                        foreach (var inventor in tmpSplValue)
                                        {
                                            currentElement.I72 = (currentElement.I72 ?? Enumerable.Empty<string>()).Concat(new string[] { inventor.Trim() }).ToArray();
                                        }
                                    }
                                }
                                else
                                {
                                    currentElement.I72 = (currentElement.I72 ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue }).ToArray();
                                }
                            }
                            /*74*/
                            if (inidCode.StartsWith(I74))
                            {
                                string tmpAddr = null;
                                string[] splittedValue = null;
                                tmpRecValue = inidCode.Replace(I74, "").Replace("\n", " ");
                                if (tmpRecValue.Contains(";"))
                                {
                                    splittedValue = tmpRecValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    Array.Reverse(splittedValue);
                                    foreach (var item in splittedValue)
                                    {
                                        if (item.Contains(","))
                                        {
                                            currentElement.I74N = (currentElement.I74N ?? Enumerable.Empty<string>()).Concat(new string[] { item.Remove(item.IndexOf(",")).Trim() }).ToArray();
                                            currentElement.I74A = (currentElement.I74A ?? Enumerable.Empty<string>()).Concat(new string[] { item.Substring(item.IndexOf(",")).Trim().Trim(',').Trim() }).ToArray();
                                            currentElement.I74C = (currentElement.I74C ?? Enumerable.Empty<string>()).Concat(new string[] { "BG" }).ToArray();
                                            tmpAddr = item.Substring(item.IndexOf(",")).Trim().Trim(',').Trim();
                                        }
                                        else if (tmpAddr != null)
                                        {
                                            currentElement.I74N = (currentElement.I74N ?? Enumerable.Empty<string>()).Concat(new string[] { item.Trim() }).ToArray();
                                            currentElement.I74A = (currentElement.I74A ?? Enumerable.Empty<string>()).Concat(new string[] { tmpAddr.Trim() }).ToArray();
                                            currentElement.I74C = (currentElement.I74C ?? Enumerable.Empty<string>()).Concat(new string[] { "BG" }).ToArray();
                                        }
                                    }
                                }
                                else if (tmpRecValue.Contains(","))
                                {
                                    currentElement.I74N = (currentElement.I74N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Remove(tmpRecValue.IndexOf(",")).Trim() }).ToArray();
                                    currentElement.I74A = (currentElement.I74A ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Substring(tmpRecValue.IndexOf(",")).Trim().Trim(',').Trim() }).ToArray();
                                    currentElement.I74C = (currentElement.I74C ?? Enumerable.Empty<string>()).Concat(new string[] { "BG" }).ToArray();

                                }
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
                        sf.WriteLine("11:\t" + field.I11);
                        if (field.I13 != null)
                        {
                            sf.WriteLine("13:\t" + field.I13);
                        }
                        sf.WriteLine("24:\t" + field.I24);
                        if (field.I31 != null && field.I32 != null && field.I33 != null)
                        {
                            for (int i = 0; i < field.I31.Count(); i++)
                            {
                                sf.WriteLine("31:\t{0}\t{1}\t{2}", field.I31[i].Trim(), field.I32[i].Trim(), field.I33[i].Trim());
                            }
                        }
                        if (field.I51D != null && field.I51N != null)
                        {
                            for (int i = 0; i < field.I51D.Count(); i++)
                            {
                                sf.WriteLine("51:\t{0}\t{1}", field.I51N[i], field.I51D[i]);
                            }
                        }
                        sf.WriteLine("54:\t" + field.I54);
                        if (field.I72 != null)
                        {
                            foreach (var item in field.I72)
                            {
                                sf.WriteLine("72:\t" + item);
                            }
                        }
                        if (field.I73A != null && field.I73N != null && field.I73C != null)
                        {
                            if (field.I73N.Count() == field.I73A.Count() && field.I73N.Count() == field.I73C.Count())
                            {
                                for (int i = 0; i < field.I73N.Count(); i++)
                                {
                                    sf.WriteLine("73N:\t" + field.I73N[i]);
                                    sf.WriteLine("73A:\t" + field.I73A[i]);
                                    sf.WriteLine("73C:\t" + field.I73C[i]);
                                }
                            }
                        }
                        if (field.I74N != null && field.I74A != null)
                        {
                            for (int i = 0; i < field.I74N.Count(); i++)
                            {
                                sf.WriteLine("74N:\t" + field.I74N[i]);
                                sf.WriteLine("74A:\t" + field.I74A[i]);
                                sf.WriteLine("74C:\t" + field.I74C[i]);
                            }
                        }
                        if (field.I86 != null)
                        {
                            sf.WriteLine("86:\t" + field.I86);
                        }
                        if (field.I87D != null && field.I87N != null)
                        {
                            sf.WriteLine("87:\t{0}\t{1}", field.I87N, field.I87D);
                        }
                        if (field.I96D != null && field.I96N != null)
                        {
                            sf.WriteLine("96:\t{0}\t{1}", field.I96N, field.I96D);
                        }
                        if (field.I97D != null && field.I97N != null)
                        {
                            sf.WriteLine("97:\t{0}\t{1}", field.I97N, field.I97D);
                        }
                    }
                }
                sf.Flush();
                sf.Close();
            }
        }
    }
}
