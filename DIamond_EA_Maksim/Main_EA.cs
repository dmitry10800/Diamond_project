using System;
using System.Collections.Generic;

namespace DIamond_EA_Maksim
{
    class Main_EA
    {

        private static readonly string Path = @"C:\Work\EA\EA_20210930_09";
        private static readonly string SubCode = "12";
        private static readonly bool SendToProd = false;   // true - send to Prod ; false - send to Stag
        static void Main(string[] args)
        {

            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> patents = SubCode switch
            {
                "5" => methods.Start(Path, SubCode),
                "9" => methods.Start(Path, SubCode),
                "11" => methods.Start(Path, SubCode),
                "12" => methods.Start(Path, SubCode),
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
