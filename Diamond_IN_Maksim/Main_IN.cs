using System;
using System.Collections.Generic;

namespace Diamond_IN_Maksim
{
    class Main_IN
    {
        private const string Path = @"D:\LENS\TET\IN\IN_20230324_12";
        private const string SubCode = "1";
        private const bool SendToProd = false; //true - send to prod; false - send to stag

        static void Main(string[] args)
        {
            Methods methods = new();

            var convertedPatents = SubCode switch
            {
                "1" => methods.Start(Path, SubCode),
                "10" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents, SendToProd);
            else Console.WriteLine("Wrong sub code");
        }
    }
}
