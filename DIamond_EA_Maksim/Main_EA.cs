using System;
using System.Collections.Generic;

namespace DIamond_EA_Maksim
{
    class Main_EA
    {

        private static readonly string Path = @"C:\Work\EA\EA_20210531_05";
        private static readonly string SubCode = "5";
        private static readonly bool SendToProd = false;   // true - send to Prod ; false - send to Stag
        static void Main(string[] args)
        {

            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> patents = SubCode switch
            {
                "5" => methods.Start(Path, SubCode),
                "9" => methods.Start(Path, SubCode),
                "11" => methods.Start(Path, SubCode),
                "14" => methods.Start(Path, SubCode),
                "31" => methods.Start(Path,SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong subcode");

        }
    }
}
