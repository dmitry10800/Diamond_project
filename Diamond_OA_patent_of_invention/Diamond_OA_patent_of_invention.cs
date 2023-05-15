using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_OA_patent_of_invention
{
    class Diamond_OA_patent_of_invention
    {
        private static readonly string I11 = "(11)";
        private static readonly string I13 = "(13)";
        private static readonly string I21 = "(21)";
        private static readonly string I22 = "(22)";
        private static readonly string I30 = "(30)";
        private static readonly string I45 = "(45)";
        private static readonly string I51 = "(51)";
        private static readonly string I54 = "(54)";
        private static readonly string I57 = "(57)";
        private static readonly string I73 = "(73)";
        private static readonly string I72 = "(72)";
        private static readonly string I74 = "(74)";
        private static readonly string I96 = "(96)";
        private static readonly string I97 = "(97)";

        class ElementOut
        {
            public string I11 { get; set; }
            public string I13 { get; set; }
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string[] I30C { get; set; }
            public string[] I30N { get; set; }
            public string[] I30D { get; set; }
            public string I45 { get; set; }
            public string I54 { get; set; }
            public string I57 { get; set; }
            public string I71N { get; set; }
            public string I71A { get; set; }
            public string I71C { get; set; }
            public string[] I72N { get; set; }
            public string[] I72C { get; set; }
            public string[] I73C { get; set; }
            public string[] I73A { get; set; }
            public string[] I73N { get; set; }
            public string I74N { get; set; }
            public string I74A { get; set; }
            public string I74C { get; set; }
            public string I86 { get; set; }
            public string[] I51D { get; set; }
            public string[] I51C { get; set; }
        }

        /*Date*/
        static string DateNormalize(string tmpDate)
        {
            var swapDate = tmpDate;
            string[] splitDate = null;
            try
            {
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
            catch (Exception)
            {
                return swapDate;
            }
        }

        static List<ElementOut> ElementsOut = new List<ElementOut>();
        private static Dictionary<string, string> Images = new Dictionary<string, string>();

        static string[] RecSplit(string recString)
        {
            string[] splittedRecord = null;
            var tempStrC = recString.Trim();
            string tmpDescValue = null;
            if (recString != "")
            {
                if (recString.Contains(I57))
                {
                    tmpDescValue = tempStrC.Substring(tempStrC.IndexOf(I57)).Trim();
                    tempStrC = tempStrC.Remove(recString.IndexOf(I57)).Trim();
                }
                var regexPatOne = new Regex(@"\(\d{2}\)", RegexOptions.IgnoreCase);
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
            if (tmpDescValue != null)
            {
                splittedRecord = (splittedRecord ?? Enumerable.Empty<string>()).Concat(new string[] { tmpDescValue }).ToArray();
            }
            return splittedRecord;
        }
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\OA\Reg\");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (var file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            string fileName; //имя файла tetml
            string folderPath; //путь к папке с файлом tetml
            string path; //путь к файлу tetml
            XElement tet;
            DirectoryInfo root, processed;
            StreamWriter sf;
            ElementOut currentElement;
            foreach (var tetFile in files)
            {
                ElementsOut.Clear();
                fileName = tetFile;
                Images.Clear();
                root = Directory.GetParent(fileName);
                folderPath = Path.Combine(root.FullName);
                path = Path.Combine(folderPath, fileName.Substring(0, fileName.IndexOf(".")) + ".txt"); //Output Filename
                processed = Directory.CreateDirectory(Path.Combine(folderPath, fileName.Remove(fileName.IndexOf(".")) + "_IMG"));
                tet = XElement.Load(fileName);
                Directory.CreateDirectory(folderPath);
                sf = new StreamWriter(path);
                /*TETML elements*/
                var elements = tet.Descendants().Where(d => (d.Name.LocalName == "Text" || d.Name.LocalName == "PlacedImage")
                && d.Value != "________________________________________"
                && !d.Value.StartsWith("Consulter le mémoire"))
                    .SkipWhile(e => !e.Value.StartsWith("A\nREPERTOIRE UMERIQUE"))
                    .TakeWhile(e => !e.Value.Contains("B\nREPERTOIRE SUIVANT") && !e.Value.Contains("B\nREPERTOIRE SU"))
                    .ToList();
                /*Saving images*/
                var imageElements = tet.Descendants().Where(d => d.Name.LocalName == "Image").ToList();
                foreach (var item in imageElements)
                {
                    var value = item.Attribute("id")?.Value;
                    if (value != null)
                        Images.Add(value, item.Attribute("filename")?.Value);
                }
                for (var i = 0; i < elements.Count; ++i)
                {
                    var element = elements[i];
                    var value = element.Value;
                    string tmpRecordValue = null;
                    string[] splittedRecord = null;
                    int tmpInc;
                    string[] imageNames;
                    string tmpRecValue;
                    if (value.StartsWith(I11))
                    {
                        currentElement = new ElementOut();
                        ElementsOut.Add(currentElement);
                        imageNames = null;
                        tmpRecordValue = "";
                        tmpInc = i;
                        do
                        {
                            /*image search*/
                            if (elements[tmpInc].Name.LocalName == "PlacedImage")
                            {
                                imageNames = (imageNames ?? Enumerable.Empty<string>()).Concat(new string[] { elements[tmpInc].Attribute("image").Value }).ToArray();
                            }
                            tmpRecordValue += elements[tmpInc].Value + "\n";
                            ++tmpInc;
                        } while (tmpInc < elements.Count()
                        && !elements[tmpInc].Value.StartsWith(I11));
                        if (tmpRecordValue != null)
                        {
                            splittedRecord = RecSplit(tmpRecordValue);
                        }
                        foreach (var inidCode in splittedRecord)
                        {
                            tmpRecValue = null;
                            /*Act number*/
                            if (inidCode.StartsWith(I11))
                            {
                                tmpRecValue = inidCode.Replace(I11, "").Replace("\n", "").Trim();
                                currentElement.I11 = tmpRecValue;
                                /*Saving images*/
                                if (imageNames != null)
                                {
                                    var imgCount = 1;
                                    foreach (var image in imageNames)
                                    {
                                        var ext = Path.GetExtension(Images[image]);
                                        var imageFileName = Path.Combine(root.FullName, Images[image]);
                                        if (File.Exists(imageFileName))
                                            try
                                            {
                                                File.Copy(imageFileName, Path.Combine(processed.FullName, currentElement.I11 + "_" + imgCount) + ext);
                                            }
                                            catch (Exception)
                                            {
                                                Console.WriteLine("Image already exist:\t" + fileName);
                                            }
                                        else
                                        {
                                            Console.WriteLine("Cannot locate file " + fileName);
                                        }
                                        imgCount++;
                                    }
                                }
                            }
                            /*Priority value*/
                            if (inidCode.StartsWith(I51))
                            {
                                string[] splPrio = null;
                                string[] splCode = null;
                                string[] splDate = null;
                                tmpRecValue = inidCode.Replace(I51, "").Trim();
                                if (tmpRecValue.Contains("\n"))
                                {
                                    splPrio = tmpRecValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                }
                                else
                                {
                                    splPrio = (splPrio ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Trim() }).ToArray();
                                }
                                if (splPrio != null)
                                {
                                    foreach (var record in splPrio)
                                    {
                                        if (Regex.IsMatch(record, @"\(\d{4}\.\d{2}\)"))
                                        {
                                            splDate = (splDate ?? Enumerable.Empty<string>()).Concat(new string[] { Regex.Match(record, @"\(\d{4}\.\d{2}\)").Value.Replace("(", "").Replace(")", "").Trim() }).ToArray();
                                            splCode = (splCode ?? Enumerable.Empty<string>()).Concat(new string[] { Regex.Replace(record, @"\(\d{4}\.\d{2}\)", "").Trim() }).ToArray();
                                        }
                                    }
                                }
                                if (splCode != null && splDate != null)
                                {
                                    currentElement.I51D = splDate;
                                    currentElement.I51C = splCode;
                                }
                            }
                            /*21 and 86*/
                            if (inidCode.StartsWith(I21))
                            {
                                tmpRecValue = inidCode.Replace(I21, "").Replace("\n", "").Trim();
                                if (tmpRecValue.Contains("–"))
                                {
                                    currentElement.I21 = tmpRecValue.Remove(tmpRecValue.IndexOf("–")).Trim();
                                    try
                                    {
                                        currentElement.I86 = tmpRecValue.Substring(tmpRecValue.IndexOf("–") + 1).Trim();
                                    }
                                    catch (Exception)
                                    {
                                        currentElement.I86 = tmpRecValue.Substring(tmpRecValue.IndexOf("–")).Trim();
                                    }

                                }
                                else
                                {
                                    currentElement.I21 = tmpRecValue;
                                }
                            }
                            /*22 Date*/
                            if (inidCode.StartsWith(I22))
                            {
                                tmpRecValue = DateNormalize(inidCode.Replace(I22, "").Replace("\n", "").Trim());
                                currentElement.I22 = tmpRecValue;
                            }
                            /*30 Priority*/
                            if (inidCode.StartsWith(I30))
                            {
                                string[] splValue = null;
                                tmpRecValue = inidCode.Replace(I30, "").Trim();
                                if (tmpRecValue.Contains("\n"))
                                {
                                    splValue = tmpRecValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    if (splValue != null)
                                    {
                                        foreach (var recordPrio in splValue)
                                        {
                                            if (recordPrio.Contains(" "))
                                            {
                                                var tmpRecordPrio = recordPrio.Replace("n°", "").Replace("du", "").Trim();
                                                var splTmpRecordPrio = tmpRecordPrio.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                                if (splTmpRecordPrio.Count() == 3)
                                                {
                                                    currentElement.I30N = (currentElement.I30N ?? Enumerable.Empty<string>()).Concat(new string[] { splTmpRecordPrio[1].Trim() }).ToArray();
                                                    currentElement.I30D = (currentElement.I30D ?? Enumerable.Empty<string>()).Concat(new string[] { DateNormalize(splTmpRecordPrio[2].Trim()) }).ToArray();
                                                    currentElement.I30C = (currentElement.I30C ?? Enumerable.Empty<string>()).Concat(new string[] { splTmpRecordPrio[0].Trim() }).ToArray();
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (tmpRecValue.Contains(" ") && !tmpRecValue.Contains("\n"))
                                {
                                    var tmpRecordPrio = tmpRecValue.Replace("n°", "").Replace("du", "").Trim();
                                    var splTmpRecordPrio = tmpRecordPrio.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    if (splTmpRecordPrio.Count() == 3)
                                    {
                                        currentElement.I30N = (currentElement.I30N ?? Enumerable.Empty<string>()).Concat(new string[] { splTmpRecordPrio[1].Trim() }).ToArray();
                                        currentElement.I30D = (currentElement.I30D ?? Enumerable.Empty<string>()).Concat(new string[] { DateNormalize(splTmpRecordPrio[2].Trim()) }).ToArray();
                                        currentElement.I30C = (currentElement.I30C ?? Enumerable.Empty<string>()).Concat(new string[] { splTmpRecordPrio[0].Trim() }).ToArray();
                                    }
                                }
                            }
                            /*54 title*/
                            if (inidCode.StartsWith(I54))
                            {
                                tmpRecValue = inidCode.Replace(I54, "").Replace("-", " ").Replace("\n", " ").Trim();
                                currentElement.I54 = tmpRecValue;
                            }
                            /*57 description*/
                            if (inidCode.StartsWith(I57))
                            {
                                tmpRecValue = inidCode.Replace(I57, "").Replace("\n", " ").Trim();
                                currentElement.I57 = tmpRecValue;
                            }
                            /*72 Inventor*/
                            if (inidCode.StartsWith(I72))
                            {
                                try
                                {
                                    tmpRecValue = inidCode.Replace(I72, "");
                                    if (Regex.IsMatch(tmpRecValue, @".*\([A-Z]{2}\)"))
                                    {
                                        var pat = new Regex(@".*?\([A-Z]{2}\)", RegexOptions.Singleline);
                                        var matchCollection = pat.Matches(tmpRecValue);
                                        foreach (Match item in matchCollection)
                                        {
                                            var tmpSplValue = item.Value.Replace("\n", " ");
                                            tmpSplValue = Regex.Replace(tmpSplValue, @"^\s*et(\s|\n)", "");
                                            string tmpSplName = null;
                                            string tmpSplCountry = null;
                                            if (tmpSplValue.Contains(")"))
                                            {
                                                tmpSplValue = tmpSplValue.Remove(tmpSplValue.IndexOf(")")).Trim();
                                            }
                                            if (tmpSplValue.Contains("("))
                                            {
                                                tmpSplName = tmpSplValue.Remove(tmpSplValue.IndexOf("(")).Trim();
                                                tmpSplCountry = tmpSplValue.Substring(tmpSplValue.IndexOf("(")).Replace("(", "").Replace(")", "").Trim();
                                            }
                                            if (tmpSplName != null && tmpSplCountry != null)
                                            {
                                                currentElement.I72N = (currentElement.I72N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpSplName.Trim(';').Trim() }).ToArray();
                                                currentElement.I72C = (currentElement.I72C ?? Enumerable.Empty<string>()).Concat(new string[] { tmpSplCountry.Replace(".", "") }).ToArray();
                                            }
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("72 inid identifications error");
                                }
                            }
                            /*73 Agent*/
                            if (inidCode.StartsWith(I73))
                            {
                                tmpRecValue = inidCode.Replace(I73, "").Trim();
                                string[] splValue = null;
                                string coutryCode = null;
                                string name = null;
                                string address = null;
                                if (Regex.IsMatch(tmpRecValue, @"\([A-Z]{2}\)"))
                                {
                                    splValue = Regex.Split(tmpRecValue, @"(?<=\([A-Z]{2}\))", RegexOptions.None).Where(d => d.Length > 2).ToArray();
                                    foreach (var record in splValue)
                                    {
                                        if (record.Contains(","))
                                        {
                                            var tmpValue = Regex.Replace(record, @"^\s*et(\s|\n)", "");
                                            //try
                                            //{
                                            //    coutryCode = Regex.Match(tmpValue, @"\([A-Z]{2}\)$").Value.Replace("(","").Replace(")","");
                                            //    address = tmpValue.Remove(tmpValue.Length - 4).Substring(tmpValue.IndexOf(",")).Trim().TrimStart(',').Trim();
                                            //    name = tmpValue.Remove(tmpValue.IndexOf(",")).Trim();
                                            //}
                                            //catch (Exception) { coutryCode = null; name = null; address = null; }
                                            string tmpSplName = null;
                                            string tmpSplAddr = null;
                                            if (Regex.IsMatch(tmpValue.ToLower(), @"(\,.*\sllc\,)|(\,.*\sgmbh\,)|(\,.*\sinc(\.|\,)*\,)|(\,.*\sltd(\.|\,)*\,)|(\,.*\ss\.a\.\,)|(\,.*\ss\.l\.\,)|(\,.*\ss\.a\.u\.\,)"))
                                            {
                                                if (tmpValue.Count(f => f == ',') >= 2)
                                                {
                                                    var firstIndex = tmpValue.IndexOf(",");
                                                    var secondIndex = tmpValue.IndexOf(",", firstIndex + 1);
                                                    tmpSplName = tmpValue.Remove(secondIndex).Trim();
                                                    tmpSplAddr = tmpValue.Substring(secondIndex).Trim(',').Trim();
                                                }
                                            }
                                            else
                                            {
                                                tmpSplName = tmpValue.Remove(tmpValue.IndexOf(",")).Trim();
                                                tmpSplAddr = tmpValue.Substring(tmpValue.IndexOf(",")).Trim(',').Trim();
                                            }
                                            name = tmpSplName;
                                            if (Regex.IsMatch(tmpSplAddr, @"\([A-Z]{2}\)$"))
                                            {
                                                try
                                                {
                                                    coutryCode = Regex.Match(tmpSplAddr, @"\([A-Z]{2}\)$").Value.Replace("(", "").Replace(")", "").Trim();
                                                    address = Regex.Replace(tmpSplAddr, @"\([A-Z]{2}\)$", "").Trim(',').Trim();
                                                }
                                                catch (Exception)
                                                {
                                                    Console.WriteLine("73 field address or CC error");
                                                }
                                            }
                                            else if (!Regex.IsMatch(record, @"\([A-Z]{2}\)$") && coutryCode != null)
                                            {
                                                address = Regex.Replace(record.Substring(record.IndexOf(",")), @"\([A-Z]{2}\)$", "").Trim(',').Trim();
                                            }
                                            if (address != null && coutryCode != null && name != null)
                                            {
                                                currentElement.I73N = (currentElement.I73N ?? Enumerable.Empty<string>()).Concat(new string[] { name.Replace("\n", " ").Trim() }).ToArray();
                                                currentElement.I73A = (currentElement.I73A ?? Enumerable.Empty<string>()).Concat(new string[] { address.Replace("\n", " ").Trim() }).ToArray();
                                                currentElement.I73C = (currentElement.I73C ?? Enumerable.Empty<string>()).Concat(new string[] { coutryCode.Replace("\n", " ").Trim() }).ToArray();
                                            }
                                        }
                                        else
                                        {
                                            coutryCode = null;
                                            name = null;
                                            address = null;
                                        }
                                    }
                                }
                                else
                                {
                                    currentElement.I73N = (currentElement.I73N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue }).ToArray();
                                }
                            }
                            /*74 Agent*/
                            if (inidCode.StartsWith(I74))
                            {
                                string coutryCode = null;
                                string name = null;
                                string address = null;
                                tmpRecValue = inidCode.Replace(I74, "").Replace("\n", " ").Trim();
                                if (tmpRecValue.Contains(","))
                                {
                                    var tmpValue = Regex.Replace(tmpRecValue, @"\.$", "").Trim();
                                    try
                                    {
                                        coutryCode = Regex.Match(tmpValue, @"\([A-Z]{2}\)$").Value.Replace("(", "").Replace(")", "");
                                        address = tmpValue.Remove(tmpValue.Length - 4).Substring(tmpValue.IndexOf(",")).Trim().TrimStart(',').Trim();
                                        name = tmpValue.Remove(tmpValue.IndexOf(",")).Trim();
                                    }
                                    catch (Exception) { coutryCode = null; name = null; address = null; }
                                }
                                if (coutryCode != null && name != null && address != null)
                                {
                                    currentElement.I74C = coutryCode;
                                    currentElement.I74A = address;
                                    currentElement.I74N = name;
                                }
                            }
                        }
                    }
                }
                /*Output*/
                if (ElementsOut != null)
                {
                    foreach (var field in ElementsOut)
                    {
                        sf.WriteLine("****");
                        sf.WriteLine("11:\t" + field.I11);
                        sf.WriteLine("21:\t" + field.I21);
                        sf.WriteLine("22:\t" + field.I22);
                        if (field.I30C != null && field.I30C.Count() == field.I30N.Count() && field.I30C.Count() == field.I30D.Count())
                        {
                            for (var i = 0; i < field.I30C.Count(); i++)
                            {
                                sf.WriteLine("30C:\t" + field.I30C[i]);
                                sf.WriteLine("30N:\t" + field.I30N[i]);
                                sf.WriteLine("30D:\t" + field.I30D[i]);
                            }
                        }
                        if (field.I51C != null && field.I51D != null)
                        {
                            for (var i = 0; i < field.I51D.Count(); i++)
                            {
                                sf.WriteLine("51N:\t" + field.I51D[i].Trim());
                                sf.WriteLine("51C:\t" + field.I51C[i].Trim());
                            }
                        }
                        sf.WriteLine("54:\t" + field.I54);
                        if (field.I57 != null && field.I57 != "")
                        {
                            sf.WriteLine("57:\t" + field.I57);
                        }
                        if (field.I72N != null && field.I72C != null)
                        {
                            for (var i = 0; i < field.I72N.Count(); i++)
                            {
                                sf.WriteLine("72N:\t" + field.I72N[i]);
                                sf.WriteLine("72C:\t" + field.I72C[i]);
                            }
                        }
                        if (field.I73N != null && field.I73C != null && field.I73N != null)
                        {
                            for (var i = 0; i < field.I73N.Count(); i++)
                            {
                                sf.WriteLine("73N:\t" + field.I73N[i].Trim());
                                sf.WriteLine("73A:\t" + field.I73A[i].Trim());
                                sf.WriteLine("73C:\t" + field.I73C[i].Trim());
                            }
                        }
                        if (field.I74N != null)
                        {
                            sf.WriteLine("74N:\t" + field.I74N);
                            sf.WriteLine("74A:\t" + field.I74A);
                            sf.WriteLine("74C:\t" + field.I74C);
                        }
                        if (field.I86 != null)
                        {
                            sf.WriteLine("86:\t" + field.I86);
                        }
                    }
                }
                sf.Flush();
                sf.Close();
            }
        }
    }
}
