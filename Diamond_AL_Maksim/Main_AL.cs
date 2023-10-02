using System;
using System.Collections.Generic;

namespace Diamond_AL_Maksim
{
    class Main_AL
    {
        private const string Path = @"D:\LENS\TET\AL";
        private const string SubCode = "3";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        static void Main(string[] args)
        {
            Methods methods = new();

            var convertedPatents = SubCode switch
            {
                "3" => methods.Start(Path, SubCode),
                "19" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents, SendToProd);
            else Console.WriteLine("Wrong sub code");

        }
    }
}
