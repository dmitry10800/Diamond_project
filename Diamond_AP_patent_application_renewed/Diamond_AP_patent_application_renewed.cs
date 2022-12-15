using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_AP_patent_application_renewed
{
    class Diamond_AP_patent_application_renewed
    {
        public static string CurrentFileName;
        public class ElementOut
        {
            public string AppNumber { get; set; }
            public string DateFeePaid { get; set; }
            public string ValidUntil { get; set; }
            public string Anniversary { get; set; }
        }
        /*Date*/
        static string DateNormalize(string tmpDate)
        {
            string swapDate;
            string[] splitDate = tmpDate.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            return swapDate = splitDate[2] + "-" + splitDate[1] + "-" + splitDate[0];
        }
        static List<ElementOut> ElementsOut = new List<ElementOut>();
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\AP\App_renew\");
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
                ElementsOut.Clear();
                fileName = tetFile;
                CurrentFileName = fileName;
                root = Directory.GetParent(fileName);
                folderPath = Path.Combine(root.FullName);
                path = Path.Combine(folderPath, fileName.Substring(0, fileName.IndexOf(".")) + ".txt"); //Output Filename
                tet = XElement.Load(fileName);
                Directory.CreateDirectory(folderPath);
                sf = new StreamWriter(path);
                //currentElement = null;

                /*TETML elements*/
                var elements = tet.Descendants().Where(d => d.Name.LocalName == "Text" && d.Value != "PATENTS" && d.Value != "Patent\nApplications\nFiled(Contd.)")
                    .ToList();
                List<string> splittedRecords = new List<string>();
                foreach (var item in elements)
                {
                    string value = item.Value;
                    string[] tmpSplValue = null;
                    if (value.Contains("\n"))
                    {
                        tmpSplValue = value.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var rec in tmpSplValue)
                        {
                            splittedRecords.Add(rec.Trim());
                        }
                    }
                    else
                    {
                        splittedRecords.Add(value.Trim());
                    }
                }
                if (splittedRecords != null)
                {
                    foreach (var record in splittedRecords)
                    {
                        string pattern = @"[A-Z]{2}\/[A-Z]{1}\/\d{4}\/\d{6}\s+\d{2}\.\d{2}\.\d{4}\s+\d{2}\.\d{2}\.\d{4}";
                        if (Regex.IsMatch(record, pattern))
                        {
                            currentElement = new ElementOut();
                            ElementsOut.Add(currentElement);
                            string tmpMatchedValue = Regex.Match(record, pattern).Value.Trim();
                            string[] splittedMatchedValue = tmpMatchedValue.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            if (splittedMatchedValue.Count() == 3)
                            {
                                currentElement.AppNumber = splittedMatchedValue[0];
                                currentElement.DateFeePaid = DateNormalize(splittedMatchedValue[1]);
                                currentElement.ValidUntil = DateNormalize(splittedMatchedValue[2]);
                                currentElement.Anniversary = record.Replace(tmpMatchedValue, "").Trim();
                            }
                        }
                    }
                }
                /*Output*/
                if (ElementsOut != null)
                {
                    foreach (var elemOut in ElementsOut)
                    {
                        sf.WriteLine("***");
                        sf.WriteLine("ApplicationNo:\t" + elemOut.AppNumber);
                        sf.WriteLine("DateFeePaid:\t" + elemOut.DateFeePaid);
                        sf.WriteLine("ValidUntil:\t" + elemOut.ValidUntil);
                        sf.WriteLine("Anniversary:\t" + elemOut.Anniversary);
                    }
                }
                sf.Flush();
                sf.Close();
            }
        }
    }
}
