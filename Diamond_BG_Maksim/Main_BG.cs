using System;
using System.Collections.Generic;

namespace Diamond_BG_Maksim
{
    class Main_BG
    {
        private const string Path = @"D:\LENS\BG\BG_20220930_09(2)";
        private const string SubCode = "1";
        private const bool SendToProd = false; //true - send to prod; false - send to stag

        private static void Main()
        {
            Methods methods = new();

            var patents = SubCode switch
            {
                "1" => methods.Start(Path,SubCode),
                "3" => methods.Start(Path, SubCode),
                "4" => methods.Start(Path, SubCode),
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
