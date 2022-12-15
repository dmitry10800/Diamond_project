﻿using System;

namespace Diamond_VE_Maksim
{
    class Main_VE
    {
        private const string Path = @"D:\LENS\VE\VE_20221125_619";
        private const string SubCode = "19";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {
            Methods methods = new();

            var patents = SubCode switch
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
