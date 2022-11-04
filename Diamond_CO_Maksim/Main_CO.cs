using System;
using System.Collections.Generic;

namespace Diamond_CO_Maksim
{
    class Main_CO
    {
        private const string Path = @"D:\LENS\CO\CO_20220809_970";
        private const string SubCode = "8";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        static void Main(string[] args)
        {
            Methods methods = new();

            var patents = SubCode switch
            {
                "6" => methods.Start(Path, SubCode),
                "7" => methods.Start(Path, SubCode),
                "8" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong subcode");
        }
    }
}