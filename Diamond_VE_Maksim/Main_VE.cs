using System;
using System.Collections.Generic;

namespace Diamond_VE_Maksim
{
    class Main_VE
    {
        private const string Path = @"D:\LENS\VE\VE_20220921_618";
        private const string SubCode = "12";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
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
