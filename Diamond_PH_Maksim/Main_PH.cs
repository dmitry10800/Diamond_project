using System;
using System.Collections.Generic;

namespace Diamond_PH_Maksim
{
    class Main_PH
    {

        private static readonly string Path = @"C:\Work\PH\PH_20210507_50";
        private static readonly string SubCode = "5";
        private static readonly bool SendToProd = false;   // true - send to Prod ; false - send to Stag

        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> patents = SubCode switch
            {
                "5" => methods.Start(Path,SubCode),
                "22" => methods.Start(Path, SubCode),                
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong subcode");
        }
    }
}
