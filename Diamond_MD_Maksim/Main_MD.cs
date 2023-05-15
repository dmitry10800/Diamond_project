using System;
using System.Collections.Generic;

namespace Diamond_MD_Maksim
{
    class Main_MD
    {
        private readonly static string path = @"C:\Work\MD\MD_20200331_03";
        private readonly static string subCode = "2";

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
