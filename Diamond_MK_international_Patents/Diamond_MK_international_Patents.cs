using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_MK_international_Patents
{
    class Diamond_MK_international_Patents
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
            public string[] I51 { get; set; }
            public string I54 { get; set; }
            public string I57 { get; set; }
            public string I71N { get; set; }
            public string I71A { get; set; }
            public string I71C { get; set; }
            public string[] I72 { get; set; }
            public string I73 { get; set; }
            public string I74N { get; set; }
            public string I74A { get; set; }
            public string I96D { get; set; }
            public string I96N { get; set; }
            public string I97D { get; set; }
            public string I97N { get; set; }
        }

        /*Date*/
        static string DateNormalize(string tmpDate)
        {
            string swapDate = tmpDate;
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
        private static Dictionary<string, string> Images = new Dictionary<string, string>();

        static string[] RecSplit(string recString)
        {
            string[] splittedRecord = null;
            string tempStrC = recString.Trim();
            string tmpDescValue = null;
            if (recString != "")
            {
                if (recString.Contains(I57))
                {
                    tmpDescValue = tempStrC.Substring(tempStrC.IndexOf(I57)).Trim();
                    tempStrC = tempStrC.Remove(recString.IndexOf(I57)).Trim();
                }
                Regex regexPatOne = new Regex(@"\(\d{2}\)", RegexOptions.IgnoreCase);
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
            if (tmpDescValue != null)
            {
                splittedRecord = (splittedRecord ?? Enumerable.Empty<string>()).Concat(new string[] { tmpDescValue }).ToArray();
            }
            return splittedRecord;
        }
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\MK\Reg\");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
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
                var elements = tet.Descendants().Where(d => d.Name.LocalName == "Text" || d.Name.LocalName == "PlacedImage")
                    .SkipWhile(e => !e.Value.StartsWith("* Internationals agreed Numbers"))
                    .TakeWhile(e => !e.Value.Contains("ПРЕГЛЕДИ"))
                    .ToList();
                /*Saving images*/
                var imageElements = tet.Descendants().Where(d => d.Name.LocalName == "Image").ToList();
                foreach (var item in imageElements)
                {
                    var value = item.Attribute("id")?.Value;
                    if (value != null)
                        Images.Add(value, item.Attribute("filename")?.Value);
                }
                for (int i = 0; i < elements.Count; ++i)
                {
                    var element = elements[i];
                    string value = element.Value;
                    string tmpRecordValue = null;
                    string[] splittedRecord = null;
                    int tmpInc;
                    string[] imageNames;
                    string tmpRecValue;
                    if (value.StartsWith(I51))
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
                            tmpRecordValue += elements[tmpInc].Value.Replace("\n", " ") + "\n";
                            ++tmpInc;
                        } while (tmpInc < elements.Count()
                        && !elements[tmpInc].Value.StartsWith(I51));
                        if (tmpRecordValue != null)
                        {
                            splittedRecord = RecSplit(tmpRecordValue);
                        }
                        foreach (var inidCode in splittedRecord)
                        {
                            tmpRecValue = null;
                            /*Priority value*/
                            if (inidCode.StartsWith(I51))
                            {
                                string[] splPrio = null;
                                tmpRecValue = inidCode.Replace(I51, "").Replace("\n", " ").Replace(" ", "").Trim();
                                if (tmpRecValue.Contains(","))
                                {
                                    splPrio = tmpRecValue.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                }
                                try
                                {
                                    currentElement.I51 = splPrio.Select(x => x.Insert(4, " ")).ToArray();
                                }
                                catch (Exception)
                                {
                                    currentElement.I51 = splPrio;
                                }
                            }
                            /*Act number*/
                            if (inidCode.StartsWith(I11))
                            {
                                tmpRecValue = inidCode.Replace(I11, "").Replace("\n", "").Trim();
                                currentElement.I11 = tmpRecValue;
                                /*Saving images*/
                                if (imageNames != null)
                                {
                                    int imgCount = 1;
                                    foreach (var image in imageNames)
                                    {
                                        string ext = Path.GetExtension(Images[image]);
                                        string imageFileName = Path.Combine(root.FullName, Images[image]);
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
                            /**/
                            if (inidCode.StartsWith(I13))
                            {
                                tmpRecValue = inidCode.Replace(I13, "").Replace("\n", "").Trim();
                                currentElement.I13 = tmpRecValue;
                            }
                            /**/
                            if (inidCode.StartsWith(I21))
                            {
                                tmpRecValue = inidCode.Replace(I21, "").Replace("\n", "").Trim();
                                currentElement.I21 = tmpRecValue;
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
                                //string[] splPriorityRecord = null;
                                List<string> prioInstance = new List<string>();
                                tmpRecValue = inidCode.Replace(I30, "").Trim();
                                if (tmpRecValue.Contains("\n") && (tmpRecValue.Contains(" and ") || tmpRecValue.Contains(" and")))
                                {
                                    string[] tmpSplValue = tmpRecValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    if (tmpSplValue.Count() > 0)
                                    {
                                        foreach (var priority in tmpSplValue)
                                        {
                                            if (priority.Contains(" and"))
                                            {
                                                string[] tmpSlpValueA = priority.Split(new string[] { " and" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                                foreach (var item in tmpSlpValueA)
                                                {
                                                    prioInstance.Add(item.Trim());
                                                }
                                            }
                                            else
                                            {
                                                prioInstance.Add(priority.Trim());
                                            }
                                        }
                                    }
                                }
                                if (tmpRecValue.Contains(" and") && !tmpRecValue.Contains("\n"))
                                {
                                    string[] tmpSlpValueA = tmpRecValue.Split(new string[] { " and" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    foreach (var item in tmpSlpValueA)
                                    {
                                        prioInstance.Add(item.Trim());
                                    }
                                }
                                if (!tmpRecValue.Contains(" and") && !tmpRecValue.Contains("\n") && tmpRecValue.Contains(" "))
                                {
                                    try
                                    {
                                        currentElement.I30N = (currentElement.I30N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Remove(tmpRecValue.IndexOf(" ")).Trim() }).ToArray();
                                        currentElement.I30D = (currentElement.I30D ?? Enumerable.Empty<string>()).Concat(new string[] { DateNormalize(tmpRecValue.Remove(tmpRecValue.LastIndexOf(" ")).Substring(tmpRecValue.IndexOf(" ")).Trim()) }).ToArray();
                                        currentElement.I30C = (currentElement.I30C ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Substring(tmpRecValue.LastIndexOf(" ")).Trim() }).ToArray();

                                    }
                                    catch (Exception A)
                                    {
                                        Console.WriteLine("Priority identification error in:\t" + currentElement.I11);
                                    }
                                }
                                if (prioInstance != null)
                                {
                                    foreach (var priorityRecords in prioInstance)
                                    {
                                        if (priorityRecords.Contains(" "))
                                        {
                                            try
                                            {
                                                currentElement.I30N = (currentElement.I30N ?? Enumerable.Empty<string>()).Concat(new string[] { priorityRecords.Remove(priorityRecords.IndexOf(" ")).Trim() }).ToArray();
                                                currentElement.I30D = (currentElement.I30D ?? Enumerable.Empty<string>()).Concat(new string[] { DateNormalize(priorityRecords.Remove(priorityRecords.LastIndexOf(" ")).Substring(priorityRecords.IndexOf(" ")).Trim()) }).ToArray();
                                                currentElement.I30C = (currentElement.I30C ?? Enumerable.Empty<string>()).Concat(new string[] { priorityRecords.Substring(priorityRecords.LastIndexOf(" ")).Trim() }).ToArray();
                                            }
                                            catch (Exception E)
                                            {
                                                Console.WriteLine("Priority identification error in:\t" + currentElement.I11);
                                            }
                                        }
                                    }
                                }

                                //if (tmpRecValue.Contains("\n") &&  tmpRecValue != "" && !tmpRecValue.Contains(" and "))
                                //{
                                //    splPriorityRecord = tmpRecValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                //}
                                //if (splPriorityRecord != null && splPriorityRecord.Count() > 1)
                                //{
                                //    foreach (var prioRecord in splPriorityRecord)
                                //    {
                                //        if (prioRecord.Contains(" "))
                                //        {
                                //            try
                                //            {
                                //                currentElement.I30N = (currentElement.I30N ?? Enumerable.Empty<string>()).Concat(new string[] { prioRecord.Remove(prioRecord.IndexOf(" ")).Trim() }).ToArray();
                                //                currentElement.I30D = (currentElement.I30D ?? Enumerable.Empty<string>()).Concat(new string[] { DateNormalize(tmpRecValue.Remove(prioRecord.LastIndexOf(" ")).Substring(prioRecord.IndexOf(" ")).Trim()) }).ToArray();
                                //                currentElement.I30C = (currentElement.I30C ?? Enumerable.Empty<string>()).Concat(new string[] { prioRecord.Substring(prioRecord.LastIndexOf(" ")).Trim() }).ToArray();
                                //            }
                                //            catch (Exception)
                                //            {
                                //                Console.WriteLine("Priority identification error in:\t" + currentElement.I11);
                                //            }
                                //        }
                                //    }
                                //}
                                //else if (tmpRecValue.Contains(" ") && !tmpRecValue.Contains(" and "))
                                //{
                                //    try
                                //    {
                                //        currentElement.I30N = (currentElement.I30N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Remove(tmpRecValue.IndexOf(" ")).Trim() }).ToArray();
                                //        currentElement.I30D = (currentElement.I30D ?? Enumerable.Empty<string>()).Concat(new string[] { DateNormalize(tmpRecValue.Remove(tmpRecValue.LastIndexOf(" ")).Substring(tmpRecValue.IndexOf(" ")).Trim()) }).ToArray();
                                //        currentElement.I30C = (currentElement.I30C ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Substring(tmpRecValue.LastIndexOf(" ")).Trim() }).ToArray();
                                //    }
                                //    catch (Exception)
                                //    {
                                //        Console.WriteLine("Priority identification error in:\t" + currentElement.I11);
                                //    }
                                //}
                            }
                            /*96*/
                            if (inidCode.StartsWith(I96))
                            {
                                string tmpDateValue = null;
                                string tmpAgentValue = inidCode.Replace(I96, "").Replace("\n", "").Trim();
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
                            if (inidCode.StartsWith(I97))
                            {
                                string tmpDateValue = null;
                                string tmpAgentValue = inidCode.Replace(I97, "").Replace("\n", "").Trim();
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
                            /*45 date*/
                            if (inidCode.StartsWith(I45))
                            {
                                tmpRecValue = inidCode.Replace(I45, "").Replace("\n", " ").Trim();
                                if (tmpRecValue.Length > 12)
                                {
                                    string tmpValue = Regex.Match(tmpRecValue, @"\d{2}\/\d{2}\/\d{4}").Value;
                                    currentElement.I45 = DateNormalize(tmpValue);
                                    currentElement.I74A = tmpRecValue.Replace(tmpValue, "").Trim();
                                }
                                else
                                {
                                    currentElement.I45 = DateNormalize(tmpRecValue);
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
                                List<string> nameInstance = new List<string>();
                                tmpRecValue = inidCode.Replace(I72, "").Replace("\n", " ");
                                if (tmpRecValue.Contains(";") && tmpRecValue.Contains(" and "))
                                {
                                    string[] tmpSplValue = tmpRecValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    if (tmpSplValue.Count() > 0)
                                    {
                                        foreach (var inventor in tmpSplValue)
                                        {
                                            if (inventor.Contains(" and "))
                                            {
                                                string[] tmpSlpValueA = inventor.Split(new string[] { " and " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                                foreach (var item in tmpSlpValueA)
                                                {
                                                    nameInstance.Add(item.Trim());
                                                }
                                            }
                                            else
                                            {
                                                nameInstance.Add(inventor.Trim());
                                            }
                                        }

                                    }
                                }
                                if (tmpRecValue.Contains(" and ") && !tmpRecValue.Contains(";"))
                                {
                                    string[] tmpSlpValueA = tmpRecValue.Split(new string[] { " and " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    foreach (var item in tmpSlpValueA)
                                    {
                                        nameInstance.Add(item.Trim());
                                    }
                                }
                                if (nameInstance != null)
                                {
                                    currentElement.I72 = nameInstance.ToArray();
                                }
                            }
                            /*73 Agent*/
                            if (inidCode.StartsWith(I73))
                            {
                                tmpRecValue = inidCode.Replace(I73, "").Replace("\n", " ").Trim();
                                currentElement.I73 = tmpRecValue;
                            }
                            /*74 Agent*/
                            if (inidCode.StartsWith(I74))
                            {
                                tmpRecValue = inidCode.Replace(I74, "").Trim();
                                if (tmpRecValue.Contains("\n"))
                                {
                                    currentElement.I74N = tmpRecValue.Remove(tmpRecValue.IndexOf("\n"));
                                    currentElement.I74A = tmpRecValue.Substring(tmpRecValue.IndexOf("\n")).Replace("\n", " ").Trim();
                                }
                                else
                                {
                                    currentElement.I74N = tmpRecValue.Trim();
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
                        if (field.I13 != null)
                        {
                            sf.WriteLine("13:\t" + field.I13);
                        }
                        sf.WriteLine("21:\t" + field.I21);
                        sf.WriteLine("22:\t" + field.I22);
                        if (field.I30C != null && field.I30C.Count() == field.I30N.Count() && field.I30C.Count() == field.I30D.Count())
                        {
                            for (int i = 0; i < field.I30C.Count(); i++)
                            {
                                sf.WriteLine("30C:\t" + field.I30C[i]);
                                sf.WriteLine("30N:\t" + field.I30N[i]);
                                sf.WriteLine("30D:\t" + field.I30D[i]);
                            }
                        }
                        sf.WriteLine("45:\t" + field.I45);
                        if (field.I51 != null)
                        {
                            foreach (var item in field.I51)
                            {
                                sf.WriteLine("51:\t" + item.Trim());
                            }
                        }
                        sf.WriteLine("54:\t" + field.I54);
                        if (field.I57 != null && field.I57 != "")
                        {
                            sf.WriteLine("57:\t" + field.I57);
                        }
                        if (field.I72 != null)
                        {
                            foreach (var item in field.I72)
                            {
                                sf.WriteLine("72:\t" + item);
                            }
                        }
                        if (field.I73 != null)
                        {
                            sf.WriteLine("73:\t" + field.I73);
                        }
                        sf.WriteLine("74N:\t" + field.I74N);
                        sf.WriteLine("74A:\t" + field.I74A);
                        if (field.I96D != null && field.I96N != null)
                        {
                            sf.WriteLine("96N:\t" + field.I96N);
                            sf.WriteLine("96D:\t" + field.I96D);
                        }
                        if (field.I97D != null && field.I97N != null)
                        {
                            sf.WriteLine("97N:\t" + field.I97N);
                            sf.WriteLine("97D:\t" + field.I97D);
                        }
                    }
                }
                sf.Flush();
                sf.Close();
            }
        }
    }
}
