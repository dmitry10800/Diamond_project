using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Diamond_ZA_assignments_in_terms
{
    class Diamond_ZA_assignments_in_terms
    {
        class ElementOut
        {
            public string APNR { get; set; }
            public string Assignee { get; set; }
            public string Assignor { get; set; }
        }
        static List<ElementOut> ElementsOut = new List<ElementOut>();
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\ZA\LegalAssignments\");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.txt", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            foreach (var txtFile in files)
            {
                var root = Directory.GetParent(txtFile);
                string folderPath = Path.Combine(root.FullName);
                Directory.CreateDirectory(folderPath);
                string path = Path.Combine(folderPath, txtFile.Substring(0, txtFile.IndexOf(".")) + ".text"); //Output Filename
                StreamWriter sf = new StreamWriter(path);
                ElementOut currentElement = null;
                List<string> allLinesText = File.ReadAllLines(txtFile).ToList();
                allLinesText.RemoveAll(x => x.Contains("ASSIGNMENTS IN TERMS OF SECTION 60-REGULATIONS 58-60 AND 64 (1)")
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
                                currentElement.Assignee = allLinesText[i + 1].Trim();
                                currentElement.Assignor = allLinesText[i + 2].Trim();
                            }
                            else
                            {
                                currentElement.Assignee = allLinesText[i + 1].Trim() + " " + allLinesText[i + 2];
                                currentElement.Assignor = allLinesText[i + 3].Trim();
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
                            sf.WriteLine("Assignee:\t" + elemOut.Assignee);
                            sf.WriteLine("Assignor:\t" + elemOut.Assignor);
                        }
                    }

                }
                sf.Flush();
                sf.Close();
            }
        }
    }
}
