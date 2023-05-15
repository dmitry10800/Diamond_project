using System;
using System.Collections.Generic;

namespace Diamond_PA_Maksim
{
    class Main_PA
    {

        private static readonly string Path = @"C:\!Work\PA\PA_20220125_390";
        private static readonly string SubCode = "1";
        private static readonly bool SendToProd = false;   // true - send to Prod ; false - send to Stag

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

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong subcode");
        }
    }
}
