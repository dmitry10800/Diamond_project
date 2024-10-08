﻿using System;

namespace Diamond_VN_Maksim
{
    class Main_VN
    {
        private const string Path = @"D:\LENS\TET\VN\VN_20240527_434A";
        private const string SubCode = "12";
        private const bool SendToProd = false; //true - send to prod; false - send to stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var patents = SubCode switch
            {
                "3" => methods.Start(Path, SubCode),
                "4" => methods.Start(Path, SubCode),
                "6" => methods.Start(Path, SubCode),
                "7" => methods.Start(Path, SubCode),
                "12" => methods.Start(Path, SubCode), // Abstract screenshots has to be added
                "13" => methods.Start(Path, SubCode), // Abstract screenshots has to be added
                "14" => methods.Start(Path, SubCode), // Abstract screenshots has to be added
                "15" => methods.Start(Path, SubCode), // Abstract screenshots has to be added
                "16" => methods.Start(Path, SubCode),
                "17" => methods.Start(Path, SubCode),
                "18" => methods.Start(Path, SubCode),
                "19" => methods.Start(Path, SubCode),
                "20" => methods.Start(Path, SubCode),
                "22" => methods.Start(Path, SubCode),
                "23" => methods.Start(Path, SubCode),
                "24" => methods.Start(Path, SubCode),
                "29" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(patents, SendToProd);
        }
    }
}
