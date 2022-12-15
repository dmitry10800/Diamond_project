using System;
using System.Diagnostics;
using System.IO;

namespace Diamond_ES
{
    public static class CreateTetml
    {
        public static void CreateTetmlDocument(string dirForPDF)
        {
            try
            {
                string cmdChoosenValue = File.ReadAllText(@".\CMD_COM.txt");

                ProcessStartInfo cmdInfoCommands = new ProcessStartInfo();
                cmdInfoCommands.FileName = "cmd.exe";
                cmdInfoCommands.WorkingDirectory = Directory.GetCurrentDirectory();
                cmdInfoCommands.Arguments = "/c " + cmdChoosenValue + dirForPDF + " " + dirForPDF + @"\*.pdf";
                cmdInfoCommands.WindowStyle = ProcessWindowStyle.Hidden;

                using (Process exeProcess = Process.Start(cmdInfoCommands))
                {
                    exeProcess.WaitForExit();
                    Console.WriteLine("TETml file extracted");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("CMD error");
            }
        }
    }
}
