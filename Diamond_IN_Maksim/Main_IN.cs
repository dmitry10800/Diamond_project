using System;
using System.Collections.Generic;

namespace Diamond_IN_Maksim
{
    class Main_IN
    {

        private static readonly string path = @"D:\LENS\TET\IN\IN_20230324_12";
        private static readonly string subCode = "1";
        private static readonly bool SendToProd = false; //true - send to prod; false - send to stag

        static void Main(string[] args)
        {
            Methods methods = new();

            var convertedPatents = subCode switch
            {
                "1" => methods.Start(path, subCode),
                "10" => methods.Start(path, subCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents, SendToProd);
            else Console.WriteLine("Wrong sub code");
        }
    }
}
