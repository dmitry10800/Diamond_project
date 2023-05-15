using System;
using System.Collections.Generic;
using System.IO;

namespace Diamond_EA
{
    class EA_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"C:\Work\EA\EA_20201130_11");
            var subcode = "12";
            /*list of tetml files*/
            var files = new List<string>();
            foreach (var file in dir.GetFiles("*.txt", SearchOption.AllDirectories)) { files.Add(file.FullName); }

            foreach (var textFile in files)
            {
                CurrentFileName = textFile;
                if (subcode == "12")
                {
                    Process.Sub12(textFile);
                    Console.WriteLine("Subcode 12 sended!");
                }
                else
                if (subcode == "5")
                {
                    Process.Sub5(textFile);
                    Console.WriteLine("Subcode 5 sended!");
                }
            }
        }
    }
}
