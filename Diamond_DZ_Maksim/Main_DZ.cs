using System;
using System.Collections.Generic;

namespace Diamond_DZ_Maksim
{
    class Main_DZ
    {
        private const string Path = @"C:\!Work\DZ\DZ_20211231_02(1)";
        private const string SubCode = "1";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        static void Main(string[] args)
        {

            Methods methods = new();

            var patents = SubCode switch
            {
                "1" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong subcode");
        }
    }
}
