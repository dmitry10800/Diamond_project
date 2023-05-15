using System;
using System.Collections.Generic;

namespace Diamond_UY_Maksim
{
    class Main_UY
    {

        private static readonly string Path = @"C:\Work\UY\UY_20210831_263";
        private static readonly string SubCode = "8";
        private static readonly bool SendToProd = false;   // true - send to Prod ; false - send to Stag

        static void Main(string[] args)
        {
            Methods methods = new();

            var patents = SubCode switch
            {
                "8" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong subcode");
        }
    }
}
