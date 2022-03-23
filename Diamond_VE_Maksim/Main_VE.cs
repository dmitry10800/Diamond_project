using System;
using System.Collections.Generic;

namespace Diamond_VE_Maksim
{
    class Main_VE
    {
        private static readonly string Path = @"C:\!Work\VE\VE_20220217_614";
        private static readonly string SubCode = "12";
        private static readonly bool SendToProd = false;   // true - send to Prod ; false - send to Stag
        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> patents = SubCode switch
            {
                "12" => methods.Start(Path, SubCode),
                "19" => methods.Start(Path, SubCode),
                "24" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong subcode");
        }
    }
}
