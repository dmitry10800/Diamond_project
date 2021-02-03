using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Diamond_RO
{
    class RO_main
    {
        public static DirectoryInfo PathToTeml = new DirectoryInfo(@"C:\Работа\RO\ttml");
        public static FileInfo currentFile = null;
        public static string currentFileName = null;

        static void Main(string[] args)
        {
            var files = new List<string>();
            foreach (FileInfo file in PathToTeml.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);

                XElement elem = 
            }
        }
    }
}