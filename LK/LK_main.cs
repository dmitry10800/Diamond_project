using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LK
{
    class LK_main
    {
        public static string XlsPath { get; set; } = @"D:\_DFA_main\_Patents\LK\Test";
        public static string CurrentFileName { get; set; }
        public static List<string> GetXlsFiles(string path)
        {
            return Directory.GetFiles(path, "*.xlsx").ToList();
        }
        static void Main(string[] args)
        {
            foreach (var file in GetXlsFiles(XlsPath))
            {
                CurrentFileName = Path.GetFileName(file);
                ProcessXls.Read(file);
            }
        }
    }
}
