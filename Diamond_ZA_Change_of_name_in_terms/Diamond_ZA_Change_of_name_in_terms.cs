using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Diamond_ZA_Change_of_name_in_terms
{
    class Diamond_ZA_Change_of_name_in_terms
    {
        class ElementOut
        {
            public string APNR { get; set; }
            public string OwnerOld { get; set; }
            public string OwnerNew { get; set; }
        }
        static List<ElementOut> ElementsOut = new List<ElementOut>();
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\ZA\LegalOwner\");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.txt", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            foreach (var txtFile in files)
            {
                //ElementsOut.Clear();
                //ElementOut currentElement = null;
                //string FileName = tetFile;
                //var root = Directory.GetParent(FileName);
                //string folderPath = Path.Combine(root.FullName);
                //Directory.CreateDirectory(folderPath);
                //string path = Path.Combine(folderPath, FileName.Substring(0, FileName.IndexOf(".")) + ".txt"); //Output Filename
                //StreamWriter sf = new StreamWriter(path);
                //XElement tet = XElement.Load(FileName);
                //var elements = tet.Descendants().Where(d => d.Name.LocalName == "Text")
                //    .SkipWhile(x => x.Value != "CHANGE OF NAME IN TERMS OF REGULATION 39")
                //    //.SkipWhile(x => Regex.IsMatch(x.Value, @"\d{4}\/\d{5}"))
                //    .TakeWhile(x => x.Value != "PATENT LICENSES IN TERMS OF SECTION 53 (7)-REGULATIONS 62 AND 63")
                //    .ToList();
                ///*Removing excess phrases*/
                //elements.RemoveAll(x => x.Value == "CHANGE OF NAME IN TERMS OF REGULATION 39" 
                //|| x.Value == "Application Number"
                //|| x.Value == "In the name of"
                //|| x.Value == "New name"
                //);

                var root = Directory.GetParent(txtFile);
                string folderPath = Path.Combine(root.FullName);
                Directory.CreateDirectory(folderPath);
                string path = Path.Combine(folderPath, txtFile.Substring(0, txtFile.IndexOf(".")) + ".text"); //Output Filename
                StreamWriter sf = new StreamWriter(path);
                ElementOut currentElement = null;
                List<string> allLinesText = File.ReadAllLines(txtFile).ToList();
                allLinesText.RemoveAll(x => x.Contains("CHANGE OF NAME IN TERMS OF REGULATION 39")
                || x.Contains("Application Number")
                || x.Contains("PATENT JOURNAL")
                || x.Contains("Page |"));
                for (int i = 0; i < allLinesText.Count(); ++i)
                {
                    if (Regex.IsMatch(allLinesText[i], @"\d{4}\/\d{5}"))
                    {
                        currentElement = new ElementOut();
                        ElementsOut.Add(currentElement);
                        currentElement.APNR = allLinesText[i].Trim();
                        try
                        {
                            if (i + 3 == allLinesText.Count || Regex.IsMatch(allLinesText[i + 3], @"\d{4}\/\d{5}"))
                            {
                                currentElement.OwnerOld = allLinesText[i + 1].Trim();
                                currentElement.OwnerNew = allLinesText[i + 2].Trim();
                            }
                            else
                            {
                                currentElement.OwnerOld = allLinesText[i + 1].Trim() + " " + allLinesText[i + 2];
                                currentElement.OwnerNew = allLinesText[i + 3].Trim();
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                if (ElementsOut != null)
                {
                    foreach (var elemOut in ElementsOut)
                    {
                        if (elemOut.APNR != null)
                        {
                            sf.WriteLine("****");
                            sf.WriteLine("21:\t" + elemOut.APNR);
                            sf.WriteLine("73_Old:\t" + elemOut.OwnerOld);
                            sf.WriteLine("73_New:\t" + elemOut.OwnerNew);
                        }
                    }
                }
                sf.Flush();
                sf.Close();
            }
        }
    }
}
