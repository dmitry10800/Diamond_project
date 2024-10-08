﻿using System;

namespace Diamond_DZ_Maksim
{
    class Main_DZ
    {
        private const string Path = @"C:\!Work\DZ\DZ_20211231_02(1)";
        private const string SubCode = "1";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {

            var methods = new Methods();

            var patents = SubCode switch
            {
                "1" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(patents, SendToProd);
        }
    }
}
