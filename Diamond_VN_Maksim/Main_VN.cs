using System;
using System.Collections.Generic;
using System.Text;

namespace Diamond_VN_Maksim
{
    class Main_VN
    {
        private const string Path = @"D:\_DFA_main\_JiraTasks\_Lantern\LAN-3316\";
        private const string SubCode = "12";
        private const bool SendToProd = false; //true - send to prod; false - send to stag

        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> patents = SubCode switch
            {
                "3" => methods.Start(Path, SubCode),
                "4" => methods.Start(Path, SubCode),
                "6" => methods.Start(Path, SubCode),
                "7" => methods.Start(Path, SubCode),
                "12" => methods.Start(Path, SubCode), // Abstract screenshots has to be added
                "13" => methods.Start(Path, SubCode), // Abstract screenshots has to be added
                "14" => methods.Start(Path, SubCode), // Abstract screenshots has to be added
                "15" => methods.Start(Path, SubCode), // Abstract screenshots has to be added
                "16" => methods.Start(Path, SubCode),
                "17" => methods.Start(Path, SubCode),
                "18" => methods.Start(Path, SubCode),
                "19" => methods.Start(Path, SubCode),
                "20" => methods.Start(Path, SubCode),
                "22" => methods.Start(Path, SubCode),
                "23" => methods.Start(Path, SubCode),
                "24" => methods.Start(Path, SubCode),
                "29" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong SubCode");
        }
    }
}
