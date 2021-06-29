using System;
using System.Collections.Generic;

namespace Diamond_VE_Maksim
{
    class Main_VE
    {
        private static readonly string Path = @"C:\Work\VE\VE_20190702_594";
        private static readonly string SubCode = "24";
        private static readonly bool SendToProd = false;   // true - send to Prod ; false - send to Stag
        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> patents = SubCode switch
            {
                "24" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong subcode");
        }
    }
}
