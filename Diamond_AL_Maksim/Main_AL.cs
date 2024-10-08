﻿using System;

namespace Diamond_AL_Maksim
{
    class Main_AL
    {
        private const string Path = @"D:\LENS\TET\AL\AL_20240531_15";
        private const string SubCode = "17";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var convertedPatents = SubCode switch
            {
                "3" => methods.Start(Path, SubCode),
                "17" => methods.Start(Path, SubCode),
                "19" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) 
                DiamondUtilities.DiamondSender.SendToDiamond(convertedPatents,SendToProd);
            else Console.WriteLine("Wrong sub code");

        }
    }
}
