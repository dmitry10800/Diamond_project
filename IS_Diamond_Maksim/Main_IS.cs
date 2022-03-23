using System;
using System.Collections.Generic;

namespace IS_Diamond_Maksim
{
    class Main_IS
    {

        private static string path = @"C:\!Work\IS\IS_20220315_03";
        private static string subCode = "9";
        private static readonly bool SendToProd = false; //true - send to prod; false - send to stag
        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = subCode switch
            {
                "3" => methods.Start(path, subCode),
                "9" => methods.Start(path, subCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents, SendToProd);
            else Console.WriteLine("Wrong sub code");
        }
    }
}
