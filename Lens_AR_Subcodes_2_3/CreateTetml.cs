﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lens_AR_Subcodes_2_3
{
    public class CreateTetml
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
