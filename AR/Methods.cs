using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AR
{
    class Methods
    {
        public static List<FileInfo> GetTetmlFiles(string path)
        {
            var pathInfo = new DirectoryInfo(path);
            if (pathInfo.Exists)
            {
                return pathInfo.GetFiles("*.tetml", SearchOption.TopDirectoryOnly).ToList();
            }
            else
            {
                Console.WriteLine($"Folder \"{path}\" doesn't exists");
                return null;
            }
        }
        public static List<string> RecSplit(string s)
        {
            var splittedRecord = new List<string>();

            var tmp = s.Replace("\r", "").Replace("\n", " ").Trim();

            var field57 = "";


            if (tmp.Contains("(62)"))
            {
                field57 = tmp.Substring(tmp.IndexOf("(57)"), tmp.IndexOf("(62)") - tmp.IndexOf("(57)")).Trim();

                var regex = new Regex(@"(?<f57>.+)\s(?<note>Sigue.+)");

                var match = regex.Match(field57);

                var note = "";
                if (match.Success)
                {
                    field57 = match.Groups["f57"].Value.Trim();
                    note = match.Groups["note"].Value.Trim();
                }
                else
                {
                    note = "<>";
                }

                var textWithOut57field = tmp.Substring(0, tmp.IndexOf("(57)")).Trim() + " " + tmp.Substring(tmp.IndexOf("(62)")).Trim();
                if (s != "")
                {

                    var regexSplit = new Regex(@"(?=\(..\)\s)");
                    splittedRecord = regexSplit.Split(textWithOut57field).ToList();
                    splittedRecord.Add(field57);
                    splittedRecord.Add(note);

                }
                return splittedRecord;

            }
            else
            if (tmp.Contains("(71)"))
            {
                field57 = tmp.Substring(tmp.IndexOf("(57)"), tmp.IndexOf("(71)") - tmp.IndexOf("(57)")).Trim();

                var regex = new Regex(@"(?<f57>.+)\s(?<note>Sigue.+)");

                var match = regex.Match(field57);

                var note = "";
                if (match.Success)
                {
                    field57 = match.Groups["f57"].Value.Trim();
                    note = match.Groups["note"].Value.Trim();
                }
                else
                {
                    note = "<>";
                }

                var textWithOut57field = tmp.Substring(0, tmp.IndexOf("(57)")).Trim() + " " + tmp.Substring(tmp.IndexOf("(71)")).Trim();
                if (s != "")
                {

                    var regexSplit = new Regex(@"(?=\(..\)\s)");
                    splittedRecord = regexSplit.Split(textWithOut57field).ToList();
                    splittedRecord.Add(field57);
                    splittedRecord.Add(note);

                }
                return splittedRecord;

            }
            else
            {
                field57 = tmp.Substring(tmp.IndexOf("(57)"), tmp.IndexOf("(72)") - tmp.IndexOf("(57)")).Trim();


                var regex = new Regex(@"(?<f57>.+)\s(?<note>Sigue.+)");

                var match = regex.Match(field57);

                var note = "";
                if (match.Success)
                {
                    field57 = match.Groups["f57"].Value.Trim();
                    note = match.Groups["note"].Value.Trim();
                }
                else
                {
                    note = "<>";
                }

                var textWithOut57field = tmp.Substring(0, tmp.IndexOf("(57)")).Trim() + " " + tmp.Substring(tmp.IndexOf("(72)")).Trim();
                if (s != "")
                {

                    var regexSplit = new Regex(@"(?=\(..\)\s)");
                    splittedRecord = regexSplit.Split(textWithOut57field).ToList();
                    splittedRecord.Add(field57);
                    splittedRecord.Add(note);

                }
                return splittedRecord;
            }


            //Regex regexPatOne = new Regex(@"\(..\)\s(Patente de Invención
            //    |Resolución Nº
            //    |Acta Nº
            //    |Fecha de Presentación
            //    |Fecha de Resolución
            //    |Fecha de Vencimiento
            //    |Prioridad convenio de Paris
            //    |Fecha de Puesta a Disposición
            //    |Int. Cl.
            //    |Titulo
            //    |REIVINDICACIÓN
            //    |Titular
            //    |Inventor
            //    |Agente/s
            //    |Fecha de Publicación)", RegexOptions.IgnoreCase);
            //MatchCollection matches = regexPatOne.Matches(s);
            //if (matches.Count > 0)
            //{
            //    foreach (Match match in matches)
            //    {
            //        tmp = tmp.Replace(match.Value, "***" + match.Value);
            //    }
            //}
            ///*Splitting record*/
            //splittedRecord = tmp.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries).ToList();

        }
    }
}
