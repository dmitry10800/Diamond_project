using System;
using System.Collections.Generic;

namespace Diamond_MD_Maksim
{
    class Main_MD
    {
        private const string Path = @"D:\LENS\TET\MD\MD_20230731_07";
        private const string SubCode = "2";
        private const bool SendToProd = false;

        static void Main(string[] args)
        {
            var methods = new Methods();

            var convertedPatents = SubCode switch
            {
                "2" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) 
                methods.SendToDiamond(convertedPatents, SendToProd);
            else 
                Console.WriteLine("Wrong sub code");
        }
    }
}
