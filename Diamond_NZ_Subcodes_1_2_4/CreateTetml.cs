using System;
using System.Diagnostics;
using System.IO;

namespace Diamond_NZ_Subcodes_1_2_4
{
    public class CreateTetml
    {
        public static void CreateTetmlDocument(string dirForPDF)
        {
            try
            {
                var cmdChoosenValue = File.ReadAllText(@".\CMD_COM.txt");

                var cmdInfoCommands = new ProcessStartInfo();
                cmdInfoCommands.FileName = "cmd.exe";
                cmdInfoCommands.WorkingDirectory = Directory.GetCurrentDirectory();
                cmdInfoCommands.Arguments = "/c " + cmdChoosenValue + dirForPDF + " " + dirForPDF + @"\*.pdf";
                cmdInfoCommands.WindowStyle = ProcessWindowStyle.Hidden;

                using (var exeProcess = Process.Start(cmdInfoCommands))
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
