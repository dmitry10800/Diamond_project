﻿using System;

namespace Diamond_PH_Maksim
{
    class Main_PH
    {
        private const string Path = @"D:\LENS\TET\PH\PH_20241111_125";
        private const string SubCode = "29";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var patents = SubCode switch
            {
                "5" => methods.Start(Path,SubCode),
                "7" => methods.Start(Path,SubCode),
                "12" => methods.Start(Path, SubCode),
                "22" => methods.Start(Path, SubCode),               
                "29" => methods.Start(Path, SubCode),               
                "35" => methods.Start(Path, SubCode),                
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(patents, SendToProd);
        }
    }
}
