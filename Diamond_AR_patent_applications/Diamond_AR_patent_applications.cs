using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_AR_patent_applications
{
    class Diamond_AR_patent_applications
    {
        private static readonly string I10 = "(10)";
        private static readonly string I21 = "(21)";
        private static readonly string I22 = "(22)";
        private static readonly string I30 = "(30)";
        private static readonly string I41 = "(41) Fecha:";
        private static readonly string I51 = "(51)";
        private static readonly string I54 = "(54)";
        private static readonly string I57 = "(57)";
        private static readonly string I62 = "(62)";
        private static readonly string I71 = "(71)";
        private static readonly string I72 = "(72)";
        private static readonly string I74 = "(74)";
        private static readonly string I83 = "(83)";

        class ElementOut
        {
            public string I10 { get; set; }
            public string I13 { get; set; }
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string[] I30C { get; set; }
            public string[] I30N { get; set; }
            public string[] I30D { get; set; }
            public string I41 { get; set; }
            public string I41B { get; set; }
            public string[] I51 { get; set; }
            public string I54 { get; set; }
            public string I57 { get; set; }
            public string I62 { get; set; }
            public string I71N { get; set; }
            public string I71A { get; set; }
            public string I71C { get; set; }
            public string[] I72 { get; set; }
            public string I74 { get; set; }
            public string I83 { get; set; }
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
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\AR\App\");
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
                    .SkipWhile(e => !e.Value.StartsWith("PUBLICACIONES DE TRAMITE NORMAL"))
                    .TakeWhile(e => !e.Value.Contains("Boletín de Marcas y/o Patentes por ejemp") && !e.Value.Contains("República Argentina - Poder Ejecutivo Nacional"))
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
                    if (value.StartsWith(I10) && Regex.IsMatch(value, @"\(10\)\s"))
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
                        //&& !elements[tmpInc].Value.StartsWith(I10)
                        && !Regex.IsMatch(elements[tmpInc].Value, @"\(10\)\s"));
                        if (tmpRecordValue != null)
                        {
                            splittedRecord = RecSplit(tmpRecordValue);
                        }
                        foreach (var inidCode in splittedRecord)
                        {
                            tmpRecValue = null;
                            /*Resolution value*/
                            if (inidCode.StartsWith(I10))
                            {
                                tmpRecValue = inidCode.Replace(I10, "").Replace("\n", "").Trim();
                                if (tmpRecValue.Contains(" "))
                                {
                                    currentElement.I10 = tmpRecValue.Remove(tmpRecValue.LastIndexOf(" ")).Trim();
                                    currentElement.I13 = tmpRecValue.Substring(tmpRecValue.LastIndexOf(" ")).Trim();
                                }
                                else
                                {
                                    currentElement.I10 = tmpRecValue;
                                }
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
                                                File.Copy(imageFileName, Path.Combine(processed.FullName, currentElement.I10 + "_" + imgCount) + ext);
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
                            /*Act number*/
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
                                string[] splPriorityRecord = null;
                                tmpRecValue = inidCode.Replace(I30, "").Trim();
                                if (tmpRecValue.Contains("\n"))
                                {
                                    splPriorityRecord = tmpRecValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                }
                                if (splPriorityRecord != null && splPriorityRecord.Count() > 1)
                                {
                                    foreach (var prioRecord in splPriorityRecord)
                                    {
                                        if (prioRecord.Contains(" "))
                                        {
                                            try
                                            {
                                                currentElement.I30C = (currentElement.I30C ?? Enumerable.Empty<string>()).Concat(new string[] { prioRecord.Remove(prioRecord.IndexOf(" ")).Trim() }).ToArray();
                                                currentElement.I30N = (currentElement.I30N ?? Enumerable.Empty<string>()).Concat(new string[] { prioRecord.Remove(prioRecord.LastIndexOf(" ")).Substring(prioRecord.IndexOf(" ")).Trim() }).ToArray();
                                                currentElement.I30D = (currentElement.I30D ?? Enumerable.Empty<string>()).Concat(new string[] { DateNormalize(prioRecord.Substring(prioRecord.LastIndexOf(" ")).Trim()) }).ToArray();
                                            }
                                            catch (Exception)
                                            {
                                                Console.WriteLine("Priority identification error in:\t" + currentElement.I10);
                                            }
                                        }
                                    }
                                }
                                else if (tmpRecValue.Contains(" "))
                                {
                                    try
                                    {
                                        currentElement.I30C = (currentElement.I30C ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Remove(tmpRecValue.IndexOf(" ")).Trim() }).ToArray();
                                        currentElement.I30N = (currentElement.I30N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Remove(tmpRecValue.LastIndexOf(" ")).Substring(tmpRecValue.IndexOf(" ")).Trim() }).ToArray();
                                        currentElement.I30D = (currentElement.I30D ?? Enumerable.Empty<string>()).Concat(new string[] { DateNormalize(tmpRecValue.Substring(tmpRecValue.LastIndexOf(" ")).Trim()) }).ToArray();
                                    }
                                    catch (Exception)
                                    {
                                        Console.WriteLine("Priority identification error in:\t" + currentElement.I10);
                                    }
                                }
                                //currentElement.I30 = tmpRecValue;
                            }
                            /*41*/
                            if (inidCode.StartsWith(I41))
                            {
                                tmpRecValue = inidCode.Replace(I41, "").Replace("\n", " ").Trim();
                                if (tmpRecValue.Contains("Bol. Nro.:"))
                                {
                                    string tRec = tmpRecValue.Substring(tmpRecValue.IndexOf("Bol. Nro.:")).Replace("Bol. Nro.:", "").Replace("SOLICITUDES E PATENTE", "").Trim();
                                    currentElement.I41B = tRec.Trim();
                                    currentElement.I41 = DateNormalize(tmpRecValue.Remove(tmpRecValue.IndexOf("Bol. Nro.:")).Trim());
                                }
                                else
                                {
                                    currentElement.I41 = tmpRecValue;
                                }
                            }
                            /*51 international class*/
                            if (inidCode.StartsWith(I51))
                            {
                                string[] tmpIntClass = null;
                                string tmpCodeValue = null;
                                tmpRecValue = inidCode.Replace(I51, "").Replace("\n", " ").Trim();
                                string ipcPattern = @"([A-Z]{1}\d{2}[A-Z]{1}).*?";
                                string[] splIPC = Regex.Split(tmpRecValue, ipcPattern).Where(s => s != String.Empty).ToArray();
                                if (splIPC != null && splIPC.Count() % 2 == 0)
                                {
                                    for (int k = 0; k < splIPC.Count(); k++)
                                    {
                                        if (Regex.IsMatch(splIPC[k].Trim(), @"[A-Z]{1}\d{2}[A-Z]{1}"))
                                        {
                                            tmpCodeValue = splIPC[k];
                                        }
                                        if (splIPC[k].Contains(","))
                                        {
                                            tmpIntClass = splIPC[k].Trim().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                            foreach (var item in tmpIntClass)
                                            {
                                                if (tmpCodeValue != null)
                                                {
                                                    currentElement.I51 = (currentElement.I51 ?? Enumerable.Empty<string>()).Concat(new string[] { tmpCodeValue + " " + item.Trim() }).ToArray();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (tmpCodeValue != null && !Regex.IsMatch(splIPC[k].Trim(), @"[A-Z]{1}\d{2}[A-Z]{1}"))
                                            {
                                                currentElement.I51 = (currentElement.I51 ?? Enumerable.Empty<string>()).Concat(new string[] { tmpCodeValue + " " + splIPC[k].Trim() }).ToArray();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    currentElement.I51 = (currentElement.I51 ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue }).ToArray();
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
                            /*62 division*/
                            if (inidCode.StartsWith(I62))
                            {
                                tmpRecValue = inidCode.Replace(I62, "").Replace("\n", " ").Trim();
                                currentElement.I62 = tmpRecValue;
                            }
                            /*71 titular*/
                            if (inidCode.StartsWith(I71))
                            {
                                tmpRecValue = inidCode.Replace(I71, "").Trim();
                                if (tmpRecValue.Contains("\n"))
                                {
                                    string[] tmpSplValue = tmpRecValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    if (tmpSplValue.Count() > 1)
                                    {
                                        string tmpSplValueTitular = "";
                                        currentElement.I71N = tmpSplValue[0].Replace("-", " ").Trim();
                                        for (int s = 1; s < tmpSplValue.Count(); s++)
                                        {
                                            tmpSplValueTitular += " " + tmpSplValue[s];
                                        }
                                        if (tmpSplValueTitular != "")
                                        {
                                            if (Regex.IsMatch(tmpSplValueTitular.Trim(), @"\s[A-Z]{2}$"))
                                            {
                                                currentElement.I71A = Regex.Replace(tmpSplValueTitular.Trim(), @"\s[A-Z]{2}$", "").Trim().Trim(',').Trim();
                                                currentElement.I71C = Regex.Match(tmpSplValueTitular.Trim(), @"\s[A-Z]{2}$").Value.Trim();
                                            }
                                            else
                                            {
                                                currentElement.I71A = tmpSplValueTitular.Trim();
                                                currentElement.I71C = "";
                                            }
                                        }
                                    }
                                }
                            }
                            /*72 Inventor*/
                            if (inidCode.StartsWith(I72))
                            {
                                tmpRecValue = inidCode.Replace(I72, "").Replace("\n", " ");
                                if (tmpRecValue.Contains(" - "))
                                {
                                    string[] tmpSplValue = tmpRecValue.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    if (tmpSplValue.Count() > 0)
                                    {
                                        foreach (var inventor in tmpSplValue)
                                        {
                                            currentElement.I72 = (currentElement.I72 ?? Enumerable.Empty<string>()).Concat(new string[] { inventor.Trim() }).ToArray();
                                        }
                                    }
                                }
                            }
                            /*74 Agent*/
                            if (inidCode.StartsWith(I74))
                            {
                                tmpRecValue = inidCode.Replace(I74, "").Replace("\n", " ").Trim();
                                currentElement.I74 = tmpRecValue;
                            }
                            /*83 Agent*/
                            if (inidCode.StartsWith(I83))
                            {
                                tmpRecValue = inidCode.Replace(I83, "").Replace("\n", " ").Trim();
                                currentElement.I83 = tmpRecValue;
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
                        sf.WriteLine("10:\t" + field.I10);
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
                        //sf.WriteLine("30:\t" + field.I30);
                        sf.WriteLine("41:\t" + field.I41);
                        sf.WriteLine("41B:\t" + field.I41B);
                        if (field.I51 != null)
                        {
                            foreach (var item in field.I51)
                            {
                                sf.WriteLine("51:\t" + item.Trim());
                            }
                        }
                        sf.WriteLine("54:\t" + field.I54);
                        sf.WriteLine("57:\t" + field.I57);
                        sf.WriteLine("71N:\t" + field.I71N);
                        sf.WriteLine("71A:\t" + field.I71A);
                        sf.WriteLine("71C:\t" + field.I71C);
                        if (field.I72 != null)
                        {
                            foreach (var item in field.I72)
                            {
                                sf.WriteLine("72:\t" + item);
                            }
                        }
                        sf.WriteLine("74:\t" + field.I74);
                        if (field.I62 != null)
                        {
                            sf.WriteLine("62:\t" + field.I62);
                        }
                        if (field.I83 != null)
                        {
                            sf.WriteLine("83:\t" + field.I83);
                        }
                    }
                }
                sf.Flush();
                sf.Close();
            }
        }
    }
}
