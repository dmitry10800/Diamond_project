using System;
using System.Collections.Generic;
using System.IO;

namespace EE
{
    class EE_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\EE\Test");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.txt", SearchOption.AllDirectories)) { files.Add(file.FullName); }

            foreach (var textFile in files)
            {
                CurrentFileName = textFile;
                if (Path.GetFileNameWithoutExtension(textFile).EndsWith("_sub12"))
                {
                    //Process.Sub12(textFile);
                    Console.WriteLine("Subcode 12 sended!");
                }
            }
        }
    }
}
