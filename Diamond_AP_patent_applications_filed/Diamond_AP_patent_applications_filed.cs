using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_AP_patent_applications_filed
{
    class Diamond_AP_patent_applications_filed
    {
        private static readonly string I21 = "(21)";
        private static readonly string I22 = "(22)";
        private static readonly string I23 = "(23)";
        private static readonly string I31 = "(31)";
        private static readonly string I32 = "(32)";
        private static readonly string I33 = "(33)";
        private static readonly string I51 = "(51)";
        private static readonly string I54 = "(54)";
        private static readonly string I71 = "(71)";
        private static readonly string I72 = "(72)";
        private static readonly string I74 = "(74)";
        private static readonly string I75 = "(75)";
        private static readonly string I84 = "(84)";
        private static readonly string I86 = "(86)";
        private static readonly string I96 = "(96)";

        class ElementOut
        {
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string I23 { get; set; }
            public string[] I51V { get; set; }
            public string[] I51Y { get; set; }
            public string I54 { get; set; }
            public string I71 { get; set; }
            public string I75 { get; set; }
            public string[] I72 { get; set; }
            public string I74 { get; set; }
            public string[] I31 { get; set; }
            public string[] I32 { get; set; }
            public string[] I33 { get; set; }
            public string[] I84 { get; set; }
            public string I86N { get; set; }
            public string I86D { get; set; }
            public string I96N { get; set; }
            public string I96D { get; set; }
        }

        static string[] RecSplit(string recString)
        {
            string[] splittedRecord = null;
            var tempStrC = recString.Replace("\n", " ").Replace("●●", "").Trim();
            if (recString != "")
            {
                //if (recString.Contains("\n"))
                //{
                //    recString = recString.Replace("\n", " ");
                //}
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
            return splittedRecord;
        }
        /*IPC split*/
        static string[] IPCSplit(string recString, out string[] ipcYear)
        {
            string[] splittedRecord = null;
            ipcYear = null;
            var tempStrC = recString.Replace("\n", " ").Replace("●●", "").Trim();
            if (recString != "")
            {
                var regexPatOne = new Regex(@"\(\d{4}\.\d{2}\)?", RegexOptions.IgnoreCase);
                var matchesClass = regexPatOne.Matches(recString);
                if (matchesClass.Count > 0)
                {
                    foreach (Match matchC in matchesClass)
                    {
                        tempStrC = tempStrC.Replace(matchC.Value, "***");
                        ipcYear = (ipcYear ?? Enumerable.Empty<string>()).Concat(new string[] { matchC.Value }).ToArray();
                    }
                }
                /*Splitting record*/
                splittedRecord = tempStrC.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries);
            }
            return splittedRecord;
        }
        /*71 and 84 split*/
        static string[] OwnerSplit(string tmpStr)
        {
            string[] splittedOwner = null;
            string[] splittedOwnerTrimed = null;
            tmpStr = tmpStr.Replace(", et al", "").Trim();
            if (tmpStr.Contains(","))
            {
                splittedOwner = tmpStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            else if (tmpStr.Contains(" and ") && !tmpStr.Contains(","))
            {
                splittedOwner = tmpStr.Split(new string[] { " and " }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                splittedOwner = (splittedOwner ?? Enumerable.Empty<string>()).Concat(new string[] { tmpStr }).ToArray();
            }
            foreach (var item in splittedOwner)
            {
                splittedOwnerTrimed = (splittedOwnerTrimed ?? Enumerable.Empty<string>()).Concat(new string[] { item.Trim() }).ToArray();
            }
            return splittedOwnerTrimed;
        }
        static string PctSplit(string tmpPct, out string pctNumber)
        {
            pctNumber = null;
            string pctDate = null;
            var pattern = new Regex(@"\d{2}\.\d{2}\.\d{4}", RegexOptions.IgnoreCase);
            var matchesClass = pattern.Matches(tmpPct);
            if (matchesClass.Count == 1)
            {
                foreach (Match matchC in matchesClass)
                {
                    pctDate = DateNormalize(matchC.Value.Trim());
                    pctNumber = tmpPct.Replace(matchC.Value, "").Trim();
                }
            }
            return pctDate;
        }
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
        static List<ElementOut> ElementsOut = new List<ElementOut>();

        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\AP\App\");
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
                ElementsOut.Clear();
                fileName = tetFile;
                root = Directory.GetParent(fileName);
                folderPath = Path.Combine(root.FullName);
                path = Path.Combine(folderPath, fileName.Substring(0, fileName.IndexOf(".")) + ".txt"); //Output Filename
                tet = XElement.Load(fileName);
                Directory.CreateDirectory(folderPath);
                sf = new StreamWriter(path);
                //currentElement = null;

                /*TETML elements*/
                var elements = tet.Descendants().Where(d => d.Name.LocalName == "Text" && d.Value != "PATENTS" && d.Value != "Patent\nApplications\nFiled(Contd.)")
                    .SkipWhile(e => !e.Value.StartsWith(I21))
                    .ToList();

                for (var i = 0; i < elements.Count; ++i)
                {
                    var element = elements[i];
                    var value = element.Value;
                    string tmpRecordValue = null;
                    string[] splittedRecord = null;
                    if (value.StartsWith(I21))
                    {
                        currentElement = new ElementOut();
                        ElementsOut.Add(currentElement);
                        var tmpInc = i;
                        tmpRecordValue = "";
                        do
                        {
                            tmpRecordValue += elements[tmpInc].Value + " ";
                            ++tmpInc;
                        } while (tmpInc < elements.Count()
                        && !elements[tmpInc].Value.StartsWith(I21));
                        if (tmpRecordValue != null)
                        {
                            splittedRecord = RecSplit(tmpRecordValue);
                            foreach (var inidCode in splittedRecord)
                            {
                                if (inidCode.StartsWith(I21))
                                {
                                    currentElement.I21 = inidCode.Replace(I21, "").Trim();
                                }
                                if (inidCode.StartsWith(I22))
                                {
                                    currentElement.I22 = DateNormalize(inidCode.Replace(I22, "").Trim());
                                }
                                if (inidCode.StartsWith(I23))
                                {
                                    currentElement.I23 = DateNormalize(inidCode.Replace(I23, "").Trim());
                                }
                                if (inidCode.StartsWith(I51))
                                {
                                    var tmpValue = inidCode.Replace(I51, "").Trim();
                                    var splittedIPC = IPCSplit(tmpValue, out var ipcYear);
                                    currentElement.I51Y = ipcYear;
                                    foreach (var item in splittedIPC)
                                    {
                                        currentElement.I51V = (currentElement.I51V ?? Enumerable.Empty<string>()).Concat(new string[] { item.Trim() }).ToArray();
                                    }
                                }
                                if (inidCode.StartsWith(I54))
                                {
                                    currentElement.I54 = DateNormalize(inidCode.Replace(I54, "").Trim());
                                }
                                if (inidCode.StartsWith(I31))
                                {
                                    currentElement.I31 = (currentElement.I31 ?? Enumerable.Empty<string>()).Concat(new string[] { inidCode.Replace(I31, "").Trim() }).ToArray();
                                }
                                if (inidCode.StartsWith(I32))
                                {
                                    currentElement.I32 = (currentElement.I32 ?? Enumerable.Empty<string>()).Concat(new string[] { DateNormalize(inidCode.Replace(I32, "").Trim()) }).ToArray();
                                }
                                if (inidCode.StartsWith(I33))
                                {
                                    currentElement.I33 = (currentElement.I33 ?? Enumerable.Empty<string>()).Concat(new string[] { inidCode.Replace(I33, "").Trim() }).ToArray();
                                }
                                if (inidCode.StartsWith(I71))
                                {
                                    currentElement.I71 = inidCode.Replace(I71, "").Trim();
                                }
                                if (inidCode.StartsWith(I72))
                                {
                                    currentElement.I72 = OwnerSplit(inidCode.Replace(I72, "").Trim());
                                }
                                if (inidCode.StartsWith(I74))
                                {
                                    currentElement.I74 = inidCode.Replace(I74, "").Trim();
                                }
                                if (inidCode.StartsWith(I75))
                                {
                                    currentElement.I75 = inidCode.Replace(I75, "").Trim();
                                }
                                if (inidCode.StartsWith(I84))
                                {
                                    var tmpValue = inidCode.Replace(I84, "").Trim();
                                    currentElement.I84 = OwnerSplit(tmpValue);
                                }
                                if (inidCode.StartsWith(I86))
                                {
                                    string pctNumber = null;
                                    var tmpValue = inidCode.Replace(I86, "").Trim();
                                    currentElement.I86D = PctSplit(tmpValue, out pctNumber);
                                    currentElement.I86N = pctNumber;

                                }
                                if (inidCode.StartsWith(I96))
                                {
                                    var tmpValue = inidCode.Replace(I96, "").Trim();
                                    string pctNumber = null;
                                    currentElement.I96D = PctSplit(tmpValue, out pctNumber);
                                    currentElement.I96N = pctNumber;
                                }
                            }
                        }
                    }
                }
                /*Output*/
                if (ElementsOut != null)
                {
                    foreach (var elemOut in ElementsOut)
                    {
                        if (elemOut.I21 != null)
                        {
                            sf.WriteLine("****");
                            sf.WriteLine("21:\t" + elemOut.I21);
                            sf.WriteLine("22:\t" + elemOut.I22);
                            sf.WriteLine("23:\t" + elemOut.I23);
                            /*31,32,33 Priority*/
                            if (elemOut.I31 != null && elemOut.I31.Count() == elemOut.I32.Count() && elemOut.I31.Count() == elemOut.I33.Count())
                            {
                                for (var i = 0; i < elemOut.I31.Count(); i++)
                                {
                                    sf.WriteLine("31:\t" + elemOut.I31[i]);
                                    sf.WriteLine("32:\t" + elemOut.I32[i]);
                                    sf.WriteLine("33:\t" + elemOut.I33[i]);
                                }
                            }
                            if (elemOut.I51V.Count() == elemOut.I51Y.Count())
                            {
                                for (var i = 0; i < elemOut.I51V.Count(); i++)
                                {
                                    sf.WriteLine("51:\t" + elemOut.I51V[i] + "\t" + elemOut.I51Y[i]);
                                }
                            }
                            sf.WriteLine("54:\t" + elemOut.I54);
                            sf.WriteLine("71:\t" + elemOut.I71);
                            if (elemOut.I72 != null)
                            {
                                for (var i = 0; i < elemOut.I72.Count(); i++)
                                {
                                    sf.WriteLine("72:\t" + elemOut.I72[i]);
                                }
                            }
                            sf.WriteLine("74:\t" + elemOut.I74);
                            if (elemOut.I75 != null)
                            {
                                sf.WriteLine("75:\t" + elemOut.I75);
                            }
                            for (var i = 0; i < elemOut.I84.Count(); i++)
                            {
                                sf.WriteLine("84:\t" + elemOut.I84[i]);
                            }
                            sf.WriteLine("86:\t" + elemOut.I86N + "\t" + elemOut.I86D);
                            sf.WriteLine("96:\t" + elemOut.I96N + "\t" + elemOut.I96D);
                        }
                    }
                }
                sf.Flush();
                sf.Close();
            }
        }
    }
}
