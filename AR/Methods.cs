using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            List<string> splittedRecord = new List<string>();
            string tmp = s.Trim();
            if (s != "")
            {
                Regex regexPatOne = new Regex(@"\(\d{2}\)\s(Patente de Invención
                    |Resolución Nº
                    |Acta Nº
                    |Fecha de Presentación
                    |Fecha de Resolución
                    |Fecha de Vencimiento
                    |Prioridad convenio de Paris
                    |Fecha de Puesta a Disposición
                    |Int. Cl.
                    |Titulo
                    |REIVINDICACIÓN
                    |Titular
                    |Inventor
                    |Agente/s
                    |Fecha de Publicación)", RegexOptions.IgnoreCase);
                MatchCollection matches = regexPatOne.Matches(s);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        tmp = tmp.Replace(match.Value, "***" + match.Value);
                    }
                }
                /*Splitting record*/
                splittedRecord = tmp.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            return splittedRecord;
        }
    }
}
