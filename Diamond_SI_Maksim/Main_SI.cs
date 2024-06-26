﻿using System;

namespace Diamond_SI_Maksim
{
    class Main_SI
    {
        private readonly static string path = @"C:\Work\SI\SI_20190830_08";
        private readonly static string subcode = "20";
        static void Main(string[] args)
        {
            Methods methods = new();

            var convertedPatents = subcode switch
            {
                "20" => methods.Start(path, subcode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents);
            else Console.WriteLine("Wrong sub code");
           
        }
    }
}
