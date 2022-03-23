using System;
using System.Collections.Generic;

namespace Diamond_AP_Maksim
{
    class Main_AP
    {
        private static readonly string path = @"C:\!Work\AP\AP_20220228_02";
        private static readonly string subCode = "7";
        private static readonly bool SendToProd = false; //true - send to prod; false - send to stag

        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = subCode switch
            {
                "1" => methods.Start(path, subCode),
                "2" => methods.Start(path, subCode),
                "3" => methods.Start(path, subCode),
                "7" => methods.Start(path, subCode),
                "10" => methods.Start(path, subCode),
                "20" => methods.Start(path, subCode),
                _ => null
            };

                Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents, SendToProd);
            else Console.WriteLine("Wrong sub code");
        }
    }
}
