﻿using System;

namespace Diamond_EC_Maksim
{
    class Main_EC
    {
        private static readonly string Path = @"C:\Work\EC\EC_20210531_692";
        private static readonly string SubCode = "3";
        private static readonly bool SendToProd = false;   // true - send to Prod ; false - send to Stag

        static void Main(string[] args)
        {
            Methods methods = new();

            var patents = SubCode switch
            {
                "3" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong subcode");
        }
    }
}
