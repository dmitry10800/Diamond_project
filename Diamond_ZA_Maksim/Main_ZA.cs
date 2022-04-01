using System;
using System.Collections.Generic;

namespace Diamond_ZA_Maksim
{
    class Main_ZA
    {
        private static readonly string path = @"C:\!Work\ZA\ZA_20220330_03(2)";
        private static readonly string subCode = "1";
        private static readonly bool SendToProd = false; //true - send to prod; false - send to stag

        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = subCode switch
            {
                "1" => methods.Start(path, subCode),
                "3" => methods.Start(path, subCode),
                "5" => methods.Start(path, subCode),
                "6" => methods.Start(path, subCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents, SendToProd);
            else Console.WriteLine("Wrong sub code");
        }
    }
}
