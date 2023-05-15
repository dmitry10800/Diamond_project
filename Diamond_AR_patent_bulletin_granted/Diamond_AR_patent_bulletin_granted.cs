using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_AR_patent_bulletin_granted
{
    class Diamond_AR_patent_bulletin_granted
    {
        private static readonly string I11 = "(11) Resolución Nº";
        private static readonly string I21 = "(21) Acta Nº";
        private static readonly string I22 = "(22) Fecha de Presentación";
        private static readonly string I24 = "(24) Fecha de Resolución";
        private static readonly string I24A = "(--) Fecha de Vencimiento";
        private static readonly string I30 = "(30) Prioridad convenio de Paris";
        private static readonly string I47 = "(47) Fecha de Puesta a Disposición";
        private static readonly string I51 = "(51) Int. Cl.";
        private static readonly string I54 = "(54) Titulo";
        private static readonly string I57 = "(57) REIVINDICACIÓN";
        private static readonly string I71 = "(71) Titular";
        private static readonly string I72 = "(72) Inventor";
        private static readonly string I74 = "(74) Agente/s";
        private static readonly string I45 = "(45) Fecha de Publicación";

        class ElementOut
        {
            public string I11 { get; set; }
            public string I13 { get; set; }
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string I24 { get; set; }
            public string I24A { get; set; }
            public string[] I31 { get; set; }
            public string[] I32 { get; set; }
            public string[] I33 { get; set; }
            public string I45 { get; set; }
            public string I47 { get; set; }
            public string[] I51 { get; set; }
            public string I51D { get; set; }
            public string I54 { get; set; }
            public string I57 { get; set; }
            public string I71N { get; set; }
            public string I71A { get; set; }
            public string I71C { get; set; }
            public string[] I72 { get; set; }
            public string I74 { get; set; }
        }

        /*Date*/
        static string DateNormalize(string tmpDate)
        {
            var swapDate = tmpDate;
            string[] splitDate = null;
            if (tmpDate.Contains("/"))
            {
                try
                {
                    splitDate = tmpDate.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitDate.Count() == 3)
                    {
                        return swapDate = splitDate[2] + "-" + splitDate[1] + "-" + splitDate[0];
                    }
                }
                catch (Exception)
                {
                    return tmpDate;
                }
            }
            return tmpDate;
        }

        static List<ElementOut> ElementsOut = new List<ElementOut>();
        private static Dictionary<string, string> Images = new Dictionary<string, string>();

        static string[] RecSplit(string recString)
        {
            string[] splittedRecord = null;
            var tempStrC = recString.Trim();
            if (recString != "")
            {
                //if (recString.Contains("\n"))
                //{
                //    recString = recString.Replace("\n", " ");
                //}
                var regexPatOne = new Regex(@"\(\d{2}\)\s(Patente de Invención|Resolución Nº|Acta Nº|Fecha de Presentación|Fecha de Resolución|Fecha de Vencimiento|Prioridad convenio de Paris|Fecha de Puesta a Disposición|Int. Cl.|Titulo|REIVINDICACIÓN|Titular|Inventor|Agente/s|Fecha de Publicación)", RegexOptions.IgnoreCase);
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
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\AR\Reg\");
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
                var elements = tet.Descendants().Where(d => d.Name.LocalName == "Text" || d.Name.LocalName == "PlacedImage")
                    .SkipWhile(e => !e.Value.StartsWith("<Primera>"))
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
                    if (value.StartsWith("<Primera>"))
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
                        && !elements[tmpInc].Value.StartsWith("<Primera>"));
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
                                try
                                {
                                    currentElement.I11 = tmpRecValue.Remove(tmpRecValue.Length - 2);
                                    currentElement.I13 = tmpRecValue.Substring(tmpRecValue.Length - 2);
                                }
                                catch (Exception)
                                {
                                    currentElement.I11 = tmpRecValue;
                                }
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
                            /*24 date*/
                            if (inidCode.StartsWith(I24))
                            {
                                if (inidCode.Contains(I24A))
                                {
                                    currentElement.I24 = DateNormalize(inidCode.Remove(inidCode.IndexOf(I24A)).Replace("\n", " ").Replace(I24, " ").Trim()).Replace(" ", "");
                                    currentElement.I24A = DateNormalize(inidCode.Substring(inidCode.IndexOf(I24A)).Replace("\n", " ").Trim().Replace(I24A, "")).Replace(" ", "");
                                }
                            }
                            /*30 Priority*/
                            if (inidCode.StartsWith(I30))
                            {
                                string[] tmpSplValue = null;
                                tmpRecValue = inidCode.Replace(I30, "").Replace("\n", " ").Trim();
                                if (tmpRecValue.Contains(";"))
                                {
                                    tmpSplValue = tmpRecValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                }
                                else
                                {
                                    tmpSplValue = (tmpSplValue ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Trim() }).ToArray();
                                }
                                if (tmpSplValue != null)
                                {
                                    foreach (var record in tmpSplValue)
                                    {
                                        if (Regex.IsMatch(record, @"^[A-Z]{2}\s") && Regex.IsMatch(record, @"\d{2}\/\d{2}\/\d{4}"))
                                        {
                                            string tmpCC = null;
                                            string tmpDate = null;
                                            string tmpNum = null;
                                            tmpCC = Regex.Match(record, @"^[A-Z]{2}\s").Value;
                                            tmpDate = Regex.Match(record, @"\d{2}\/\d{2}\/\d{4}").Value;
                                            tmpNum = Regex.Replace(record, @"^[A-Z]{2}\s", "").Trim();
                                            tmpNum = Regex.Replace(tmpNum, @"\d{2}\/\d{2}\/\d{4}", "").Trim();
                                            if (tmpCC != null && tmpDate != null && tmpNum != null)
                                            {
                                                currentElement.I31 = (currentElement.I31 ?? Enumerable.Empty<string>()).Concat(new string[] { tmpCC.Trim() }).ToArray();
                                                currentElement.I32 = (currentElement.I32 ?? Enumerable.Empty<string>()).Concat(new string[] { tmpNum }).ToArray();
                                                currentElement.I33 = (currentElement.I33 ?? Enumerable.Empty<string>()).Concat(new string[] { DateNormalize(tmpDate) }).ToArray();
                                            }
                                        }
                                    }
                                }
                            }
                            /*47 date*/
                            if (inidCode.StartsWith(I47))
                            {
                                tmpRecValue = inidCode.Replace(I47, "").Replace("\n", " ").Trim();
                                currentElement.I47 = DateNormalize(tmpRecValue);
                            }
                            /*51 international class*/
                            if (inidCode.StartsWith(I51))
                            {
                                string[] tmpIntClass = null;
                                tmpRecValue = inidCode.Replace(I51, "").Replace("\n", " ").Trim();
                                var standartRedaction = new Regex(@"\(\d{4}.*\)?");
                                if (Regex.IsMatch(tmpRecValue, @"\(\d{4}.*\)?"))
                                {
                                    currentElement.I51D = Regex.Match(tmpRecValue, @"\(\d{4}.*\)").Value;
                                    tmpRecValue = Regex.Replace(tmpRecValue, @"\(\d{4}.*\)", "").Trim();

                                }
                                if (tmpRecValue.Contains(","))
                                {
                                    tmpIntClass = tmpRecValue.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    currentElement.I51 = tmpIntClass;
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
                            /*71 titular*/
                            if (inidCode.StartsWith(I71))
                            {
                                tmpRecValue = inidCode.Replace(I71, "").Trim();
                                if (tmpRecValue.Contains("\n"))
                                {
                                    var tmpSplValue = tmpRecValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    if (tmpSplValue.Count() > 1)
                                    {
                                        var tmpSplValueTitular = "";
                                        currentElement.I71N = tmpSplValue[0].Replace("-", " ").Trim();
                                        for (var s = 1; s < tmpSplValue.Count(); s++)
                                        {
                                            tmpSplValueTitular += " " + tmpSplValue[s];
                                        }
                                        if (tmpSplValueTitular != "")
                                        {
                                            currentElement.I71A = Regex.Replace(tmpSplValueTitular.Trim(), @"\,\s*[A-Z]{2}$", "").Trim();
                                            currentElement.I71C = Regex.Match(tmpSplValueTitular.Trim(), @"[A-Z]{2}$").Value;
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
                                    var tmpSplValue = tmpRecValue.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    if (tmpSplValue.Count() > 0)
                                    {
                                        foreach (var inventor in tmpSplValue)
                                        {
                                            currentElement.I72 = (currentElement.I72 ?? Enumerable.Empty<string>()).Concat(new string[] { inventor.Replace(",", "").Trim() }).ToArray();
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
                            /*45*/
                            if (inidCode.StartsWith(I45))
                            {
                                tmpRecValue = inidCode.Replace(I45, "").Replace("\n", " ").Replace("-", "").Trim();
                                tmpRecValue = DateNormalize(Regex.Match(tmpRecValue, @"\d{2}\/\d{2}\/\d{4}").Value);
                                currentElement.I45 = tmpRecValue;
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
                        sf.WriteLine("13:\t" + field.I13);
                        sf.WriteLine("21:\t" + field.I21);
                        sf.WriteLine("22:\t" + field.I22);
                        sf.WriteLine("24:\t" + field.I24);
                        sf.WriteLine("--:\t" + field.I24A);
                        //sf.WriteLine("30:\t" + field.I30);
                        if (field.I31 != null)
                        {
                            for (var i = 0; i < field.I31.Count(); i++)
                            {
                                sf.WriteLine("31:\t" + field.I31[i]);
                                sf.WriteLine("32:\t" + field.I32[i]);
                                sf.WriteLine("33:\t" + field.I33[i]);
                            }
                        }
                        sf.WriteLine("47:\t" + field.I47);
                        if (field.I51 != null)
                        {
                            if (field.I51D != null) { sf.WriteLine("51D:\t" + field.I51D); }
                            foreach (var item in field.I51)
                            {
                                sf.WriteLine("51:\t" + item.Trim());
                            }
                        }
                        sf.WriteLine("54:\t" + field.I54);
                        sf.WriteLine("57:\t" + field.I57);
                        sf.WriteLine("71N:\t" + field.I71N);
                        sf.WriteLine("71A:\t" + field.I71A);
                        if (field.I71C != null)
                        {
                            sf.WriteLine("71C:\t" + field.I71C);
                        }
                        if (field.I72 != null)
                        {
                            foreach (var item in field.I72)
                            {
                                sf.WriteLine("72:\t" + item);
                            }
                        }
                        sf.WriteLine("74:\t" + field.I74);
                        sf.WriteLine("45:\t" + field.I45);
                    }
                }
                sf.Flush();
                sf.Close();
            }
        }
    }
}
