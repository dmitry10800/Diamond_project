using System;
using System.Collections.Generic;

namespace Diamond_ID_Maksim
{
    class Main_ID
    {
        private const string Path = @"D:\LENS\TET\ID\ID_20230428_798";
        private const string SubCode = "1";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        static void Main(string[] args)
        {
            Methods methods = new();

            var patents = SubCode switch
            {
                "1" => methods.Start(Path, SubCode),
                "2" => methods.Start(Path, SubCode),
                "3" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong subcode");
        }
    }
}
