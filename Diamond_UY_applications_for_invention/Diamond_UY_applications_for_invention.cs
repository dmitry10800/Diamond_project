using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_UY_applications_for_invention
{
    class Diamond_UY_applications_for_invention
    {
        private static readonly string I11 = "(11)";
        private static readonly string I19 = "(19)";
        private static readonly string I12 = "(12) Invención";
        private static readonly string I22 = "(22) Fecha de presentación:";
        private static readonly string I30 = "(30) Datos de prioridad:";
        private static readonly string I51 = "(51) Clasificación Internacional:";
        private static readonly string I54 = "(54) Título:";
        private static readonly string I57 = "(57) Resumen:";
        private static readonly string I71 = "(71) Solicitante:";
        private static readonly string I72 = "(72) Inventor:";
        private static readonly string I74 = "(74) Agente/apoderado:";

        class ElementOut
        {
            public string I11 { get; set; }
            public string I22 { get; set; }
            public string[] I30C { get; set; }
            public string[] I30N { get; set; }
            public string[] I30D { get; set; }
            public string I54 { get; set; }
            public string I57 { get; set; }
            public string[] I71N { get; set; }
            public string[] I71C { get; set; }
            public string[] I72N { get; set; }
            public string[] I72C { get; set; }
            public string I74N { get; set; }
            public string[] I51 { get; set; }
            //public string[] I51D { get; set; }
            //public string[] I51C { get; set; }
        }

        /*Date*/
        static string DateNormalize(string tmpDate)
        {
            string swapDate = tmpDate;
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
            string tempStrC = recString.Trim();
            string tmpDescValue = null;
            if (recString != "")
            {
                if (recString.Contains(I57))
                {
                    tmpDescValue = tempStrC.Substring(tempStrC.IndexOf(I57)).Trim();
                    tempStrC = tempStrC.Remove(recString.IndexOf(I57)).Trim();
                }
                Regex regexPatOne = new Regex(@"\(\d{2}\)\s*(Clasificación\sInternacional\:|Fecha\sde\spresentación\:|Datos\sde\sprioridad\:|Solicitante\:|Inventor\:|Agente\/apoderado\:|Título\:|Invención)", RegexOptions.IgnoreCase);
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
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\UY\Reg\");
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
                var elements = tet.Descendants().Where(d => (d.Name.LocalName == "Text" || d.Name.LocalName == "PlacedImage")
                /*&& !d.Value.StartsWith("(19) Dirección Nacional")*/)
                    .SkipWhile(e => e.Value != "SOLICITUDES DE PATENTES DE INVENCIÓN")
                    .TakeWhile(e => !e.Value.Contains("RECTIFICACIONES DE PATENTES")
                    && !e.Value.Contains("ÍNDICE POR TITULARES DE MARCAS")
                    && !e.Value.Contains("ÍNDICE POR TITULARES DE FRASES PUBLICITARIAS")
                    && !e.Value.Contains("SOLICITUDES DE DISEÑOS INDUSTRIALES"))
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
                    if (value.StartsWith(I19))
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
                        && !elements[tmpInc].Value.StartsWith(I19));
                        if (tmpRecordValue != null)
                        {
                            splittedRecord = RecSplit(tmpRecordValue);
                        }
                        foreach (var inidCode in splittedRecord)
                        {
                            tmpRecValue = null;
                            /*Act number*/
                            if (inidCode.StartsWith(I12))
                            {
                                tmpRecValue = inidCode.Replace(I12, "").Replace(I11, "").Replace("\n", "").Trim();
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
                            /*Priority value*/
                            if (inidCode.StartsWith(I51))
                            {
                                string[] splIPC = null;
                                string tmpIpcClassValue = null;
                                tmpRecValue = inidCode.Replace(I51, "").Replace("\n", " ").Replace("//", ",").Replace(" ", "").Trim();
                                //string ipcPattern = @"([A-Z]{1}\s*\d{2}[A-Z]{1}.*?\,|$)";
                                if (tmpRecValue.Contains(","))
                                {
                                    splIPC = tmpRecValue.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    if (splIPC != null)
                                    {
                                        foreach (var record in splIPC)
                                        {
                                            if (Regex.IsMatch(record, @"[A-Z]{1}\s*\d{2}[A-Z]{1}"))
                                            {
                                                tmpIpcClassValue = Regex.Match(record, @"[A-Z]{1}\s*\d{2}[A-Z]{1}").Value.Insert(4, " ");
                                                currentElement.I51 = (currentElement.I51 ?? Enumerable.Empty<string>()).Concat(new string[] { record.Insert(4, " ").Trim() }).ToArray();
                                            }
                                            else if (tmpIpcClassValue != null)
                                            {
                                                currentElement.I51 = (currentElement.I51 ?? Enumerable.Empty<string>()).Concat(new string[] { tmpIpcClassValue + record.Trim() }).ToArray();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    currentElement.I51 = (currentElement.I51 ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Insert(4, " ") }).ToArray();
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
                                tmpRecValue = inidCode.Replace(I30, "").Replace("\n", " ").Trim();
                                if (tmpRecValue.Contains(";") && !tmpRecValue.Contains(" y "))
                                {
                                    splValue = tmpRecValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    if (splValue != null)
                                    {
                                        foreach (var record in splValue)
                                        {
                                            if (record.Contains(" y "))
                                            {
                                                string[] tmpSpl = null;
                                                tmpSpl = record.Split(new string[] { " y " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                                if (tmpSpl != null)
                                                {
                                                    foreach (var item in tmpSpl)
                                                    {
                                                        if (item.Contains(" "))
                                                        {
                                                            string[] tmpSplValue = item.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                                            try
                                                            {
                                                                if (tmpSplValue.Count() == 3)
                                                                {
                                                                    currentElement.I30N = (currentElement.I30N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpSplValue[0].Trim() }).ToArray();
                                                                    currentElement.I30D = (currentElement.I30D ?? Enumerable.Empty<string>()).Concat(new string[] { DateNormalize(tmpSplValue[1].Trim()) }).ToArray();
                                                                    currentElement.I30C = (currentElement.I30C ?? Enumerable.Empty<string>()).Concat(new string[] { tmpSplValue[2].Trim() }).ToArray();
                                                                }
                                                            }
                                                            catch (Exception)
                                                            {
                                                                Console.WriteLine("Priority error occured in " + currentElement.I11);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else if (record.Contains(" "))
                                            {
                                                string[] tmpSplValue = record.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                                try
                                                {
                                                    if (tmpSplValue.Count() == 3)
                                                    {
                                                        currentElement.I30N = (currentElement.I30N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpSplValue[0].Trim() }).ToArray();
                                                        currentElement.I30D = (currentElement.I30D ?? Enumerable.Empty<string>()).Concat(new string[] { DateNormalize(tmpSplValue[1].Trim()) }).ToArray();
                                                        currentElement.I30C = (currentElement.I30C ?? Enumerable.Empty<string>()).Concat(new string[] { tmpSplValue[2].Trim() }).ToArray();
                                                    }
                                                }
                                                catch (Exception)
                                                {
                                                    Console.WriteLine("Priority error occured in " + currentElement.I11);
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (tmpRecValue.Contains(" y ") && !tmpRecValue.Contains(";"))
                                {
                                    string[] tmpSpl = null;
                                    tmpSpl = tmpRecValue.Split(new string[] { " y " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    if (tmpSpl != null)
                                    {
                                        foreach (var item in tmpSpl)
                                        {
                                            if (item.Contains(" "))
                                            {
                                                string[] tmpSplValue = item.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                                try
                                                {
                                                    if (tmpSplValue.Count() == 3)
                                                    {
                                                        currentElement.I30N = (currentElement.I30N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpSplValue[0].Trim() }).ToArray();
                                                        currentElement.I30D = (currentElement.I30D ?? Enumerable.Empty<string>()).Concat(new string[] { DateNormalize(tmpSplValue[1].Trim()) }).ToArray();
                                                        currentElement.I30C = (currentElement.I30C ?? Enumerable.Empty<string>()).Concat(new string[] { tmpSplValue[2].Trim() }).ToArray();
                                                    }
                                                }
                                                catch (Exception)
                                                {
                                                    Console.WriteLine("Priority error occured in " + currentElement.I11);
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (!tmpRecValue.Contains(";") && !tmpRecValue.Contains(" y "))
                                {
                                    if (tmpRecValue.Contains(" "))
                                    {
                                        string[] tmpSplValue = tmpRecValue.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                        try
                                        {
                                            if (tmpSplValue.Count() == 3)
                                            {
                                                currentElement.I30N = (currentElement.I30N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpSplValue[0].Trim() }).ToArray();
                                                currentElement.I30D = (currentElement.I30D ?? Enumerable.Empty<string>()).Concat(new string[] { DateNormalize(tmpSplValue[1].Trim()) }).ToArray();
                                                currentElement.I30C = (currentElement.I30C ?? Enumerable.Empty<string>()).Concat(new string[] { tmpSplValue[2].Trim() }).ToArray();
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            Console.WriteLine("Priority error occured in " + currentElement.I11);
                                        }
                                    }
                                }
                            }
                            /*54 title*/
                            if (inidCode.StartsWith(I54))
                            {
                                tmpRecValue = inidCode.Replace(I54, "").Replace("\n", " ").Trim();
                                currentElement.I54 = tmpRecValue;
                            }
                            /*57 description*/
                            if (inidCode.StartsWith(I57))
                            {
                                tmpRecValue = inidCode.Replace(I57, "").Replace("\n", " ").Trim();
                                currentElement.I57 = tmpRecValue;
                            }
                            /*71*/
                            if (inidCode.StartsWith(I71))
                            {
                                tmpRecValue = inidCode.Replace(I71, "").Replace("\n", "").Replace(" y ", " ");
                                string[] splValue = null;
                                if (Regex.IsMatch(tmpRecValue, @"\[[A-Z]{2}\]"))
                                {
                                    splValue = Regex.Split(tmpRecValue, @"(?<=\[[A-Z]{2}\])", RegexOptions.None).Where(d => d.Length > 2).ToArray();
                                    if (splValue != null)
                                    {
                                        foreach (var item in splValue)
                                        {
                                            try
                                            {
                                                string countryCode = Regex.Match(item, @"\[[A-Z]{2}\]").Value;
                                                currentElement.I71N = (currentElement.I71N ?? Enumerable.Empty<string>()).Concat(new string[] { item.Replace(countryCode, "").Trim() }).ToArray();
                                                currentElement.I71C = (currentElement.I71C ?? Enumerable.Empty<string>()).Concat(new string[] { countryCode.Replace("[", "").Replace("]", "").Trim() }).ToArray();
                                            }
                                            catch (Exception)
                                            {
                                                currentElement.I71N = (currentElement.I71N ?? Enumerable.Empty<string>()).Concat(new string[] { item }).ToArray();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    currentElement.I71N = (currentElement.I71N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue }).ToArray();
                                }
                            }
                            /*72 Inventor*/
                            if (inidCode.StartsWith(I72))
                            {
                                tmpRecValue = inidCode.Replace(I72, "").Replace("\n", " ").Trim();
                                if (tmpRecValue.Contains(";"))
                                {
                                    string[] tmpSplValue = null;
                                    tmpSplValue = tmpRecValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    if (tmpSplValue != null)
                                    {
                                        foreach (var item in tmpSplValue)
                                        {
                                            try
                                            {
                                                string countryCode = Regex.Match(item, @"\[[A-Z]{2}\]").Value;
                                                currentElement.I72N = (currentElement.I72N ?? Enumerable.Empty<string>()).Concat(new string[] { item.Replace(countryCode, "").Trim() }).ToArray();
                                                currentElement.I72C = (currentElement.I72C ?? Enumerable.Empty<string>()).Concat(new string[] { countryCode.Replace("[", "").Replace("]", "").Trim() }).ToArray();

                                            }
                                            catch (Exception)
                                            {
                                                currentElement.I72N = (currentElement.I72N ?? Enumerable.Empty<string>()).Concat(new string[] { item }).ToArray();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        string countryCode = Regex.Match(tmpRecValue, @"\[[A-Z]{2}\]").Value;
                                        currentElement.I72N = (currentElement.I72N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Replace(countryCode, "").Trim() }).ToArray();
                                        currentElement.I72C = (currentElement.I72C ?? Enumerable.Empty<string>()).Concat(new string[] { countryCode.Replace("[", "").Replace("]", "").Trim() }).ToArray();

                                    }
                                    catch (Exception)
                                    {
                                        currentElement.I72N = (currentElement.I72N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue }).ToArray();
                                    }
                                }
                            }
                            /*74 Agent*/
                            if (inidCode.StartsWith(I74))
                            {
                                tmpRecValue = inidCode.Replace(I74, "").Replace("\n", " ").Trim();
                                tmpRecValue = Regex.Replace(tmpRecValue, @"\((\d{1}|\d{2}|\d{3})\)", "").Trim();
                                currentElement.I74N = tmpRecValue;
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
                        if (field.I71N != null && field.I71C != null)
                        {
                            for (int i = 0; i < field.I71N.Count(); i++)
                            {
                                sf.WriteLine("71N:\t" + field.I71N[i]);
                                sf.WriteLine("71C:\t" + field.I71C[i]);
                            }
                        }
                        if (field.I72N != null && field.I72C != null)
                        {
                            for (int i = 0; i < field.I72N.Count(); i++)
                            {
                                sf.WriteLine("72N:\t" + field.I72N[i]);
                                sf.WriteLine("72C:\t" + field.I72C[i]);
                            }
                        }
                        if (field.I74N != null)
                        {
                            sf.WriteLine("74N:\t" + field.I74N);
                        }
                    }
                }
                sf.Flush();
                sf.Close();
            }
        }
    }
}
