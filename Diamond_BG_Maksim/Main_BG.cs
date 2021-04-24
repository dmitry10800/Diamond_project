using System;
using System.Collections.Generic;

namespace Diamond_BG_Maksim
{
    class Main_BG
    {

        private static readonly string Path = @"C:\Work\BG\BG_20210331_03(2)_full";
        private static readonly string SubCode = "21";
        private static readonly bool SendToProd = false; //true - send to prod; false - send to stag

        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> patents = SubCode switch
            {
                "1" => methods.Start(Path,SubCode),
                "3" => methods.Start(Path, SubCode),
                "5" => methods.Start(Path, SubCode),
                "7" => methods.Start(Path, SubCode),
                "21" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong SubCode");
        }
    }
}
