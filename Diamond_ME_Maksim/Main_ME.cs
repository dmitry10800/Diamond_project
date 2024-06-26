﻿using System;

namespace Diamond_ME_Maksim
{
    class Main_ME
    {
        private const string Path = @"D:\LENS\TET\ME";
        private const string SubCode = "2";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        static void Main(string[] args)
        {

            Methods methods = new();

            var patents = SubCode switch
            {
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
