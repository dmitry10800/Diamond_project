﻿using System;

namespace Diamond_PA_Maksim
{
    class Main_PA
    {
        private const string Path = @"D:\LENS\TET\PA\PA_20240426_405";
        private const string SubCode = "2";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        static void Main(string[] args)
        {
            Methods methods = new();

            var patents = SubCode switch
            {
                "1" => methods.Start(Path, SubCode),
                "2" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) 
                methods.SendToDiamond(patents, SendToProd);
        }
    }
}
