using System;
using System.Collections.Generic;

namespace Diamond_RO_Maksim
{
    class Main_RO
    {
        private static readonly string path = @"C:\Work\RO\RO_20211029_10";
        private static readonly string subCode = "22";
        private static readonly bool SendToProd = false;   // true - send to Prod ; false - send to Stag

        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = subCode switch
            {
                "13" => methods.Start(path, subCode),
                "14" => methods.Start(path, subCode),
                "16" => methods.Start(path, subCode),
                "17" => methods.Start(path, subCode),
                "20" => methods.Start(path, subCode),
                "22" => methods.Start(path, subCode),
                "23" => methods.Start(path, subCode),
                "24" => methods.Start(path, subCode),
                "27" => methods.Start(path, subCode),
                "29" => methods.Start(path, subCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents, SendToProd);
            else Console.WriteLine("Wrong sub");
        }
    }
}
