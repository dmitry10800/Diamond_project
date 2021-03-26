using System;
using System.Collections.Generic;

namespace Diamond_AP_Maksim
{
    class Main_AP
    {
        private static readonly string path = @"C:\Work\AP\AP_20191231_12_(Supplement)";
        private static readonly string subCode = "20";

        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = subCode switch
            {
                "20" => methods.Start(path, subCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents);
            else Console.WriteLine("Wrong sub code");
        }
    }
}
