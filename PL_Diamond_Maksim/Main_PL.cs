﻿using System;
using System.Collections.Generic;

namespace PL_Diamond_Maksim
{
    class Main_PL
    {
        private static readonly string path = @"C:\Work\PL\PL_20210628_13W";
        private static readonly string subCode = "31";
        private static readonly string newOrOld = "new";  // new / old
        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = subCode switch
            {
                "10" => methods.Start(path, subCode, newOrOld),
                "25" => methods.Start(path, subCode, newOrOld),
                "31" => methods.Start(path, subCode, newOrOld),
                "32" => methods.Start(path, subCode, newOrOld),
                "46" => methods.Start(path, subCode, newOrOld),
                "47" => methods.Start(path, subCode, newOrOld),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents);
            else Console.WriteLine("Wrong subCode");
        }
    }
}
