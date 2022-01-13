using System;
using System.Collections.Generic;

namespace Diamond_MA_Maksim
{
    class Main_MA
    {
        private static readonly string path = @"C:\!Work\MA\MA_20211231_12";
        private static readonly string subCode = "2";
        private static readonly bool SendToProd = false; //true - send to prod; false - send to stag

        static void Main(string[] args)
        {
            Methods methods = new();

            List <Diamond.Core.Models.LegalStatusEvent> convertedPatents = subCode switch
            {
                "1" => methods.Start(path, subCode),
                "2" => methods.Start(path, subCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents, SendToProd);
            else Console.WriteLine("Wrong sub code");
        }
    }
}