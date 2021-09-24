﻿using System;
using System.Collections.Generic;

namespace Diamond_BG_Maksim
{
    class Main_BG
    {

        private static readonly string Path = @"C:\Work\BG\BG_20210915_09(1)";
        private static readonly string SubCode = "4";
        private static readonly bool SendToProd = false; //true - send to prod; false - send to stag

        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> patents = SubCode switch
            {
                "1" => methods.Start(Path,SubCode),
                "3" => methods.Start(Path, SubCode),
                "4" => methods.Start(Path, SubCode),
                "5" => methods.Start(Path, SubCode),
                "7" => methods.Start(Path, SubCode),
                "21" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong SubCode");
        }
    }
}
