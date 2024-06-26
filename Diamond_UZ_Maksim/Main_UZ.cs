﻿using System;

namespace Diamond_UZ_Maksim
{
    class Main_UZ
    {
        private static readonly string Path = @"C:\!Work\UZ\UZ_20220228_02";
        private static readonly string SubCode = "4";
        private static readonly bool SendToProd = false;   // true - send to Prod ; false - send to Stag
        static void Main(string[] args)
        {
            Methods methods = new();

            var patents = SubCode switch
            {
                "1" => methods.Start(Path, SubCode),
                "3" => methods.Start(Path, SubCode),
                "4" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong subcode");
        }
    }
}
