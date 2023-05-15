using System;
using System.Collections.Generic;

namespace BD_Diamond_Maksim
{
    class Main_BD
    {
        private static readonly string path = @"C:\Work\BD\BD_20210218_07";
        private static readonly string subCode ="2";

        static void Main(string[] args)
        {
            Methods methods = new();

            var convertedPatents = subCode switch
            {
                "2" => methods.Start(path, subCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents);
            else Console.WriteLine("Wrong sub code");
        }
    }
}
