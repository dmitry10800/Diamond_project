using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diamond_EA
{
    class EA_main
    {
        public static string CurrentFileName;
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\EA\20200122");
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
                else
                if (Path.GetFileNameWithoutExtension(textFile).EndsWith("_sub5"))
                {
                    Process.Sub5(textFile);
                    Console.WriteLine("Subcode 5 sended!");
                }
            }
        }
    }
}
