using System;
using System.Collections.Generic;

namespace Diamond_IE_Maksim
{
    class Main_IE
    {

        private static readonly string Path = @"C:\Work\IE\IE_20210623_2440";
        private static readonly string SubCode = "52";
        private static readonly bool SendToProd = false;   // true - send to Prod ; false - send to Stag

        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> patents = SubCode switch
            {
                "52" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong subcode");
        }
    }
}
