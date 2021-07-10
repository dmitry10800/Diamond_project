using System;
using System.Collections.Generic;

namespace Diamond_AR_Maksim
{
    class Main_AR
    {

        private static readonly string Path = @"C:\Work\AR\AR_20210707_1159";
        private static readonly string SubCode = "5";
        private static readonly bool SendToProd = false;   // true - send to Prod ; false - send to Stag


        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> patents = SubCode switch
            {
                "1" => methods.Start(Path, SubCode),
                "2" => methods.Start(Path, SubCode),
                "3" => methods.Start(Path, SubCode),
                "5" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong subcode");
        }
    }
}
