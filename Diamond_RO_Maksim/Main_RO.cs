using System;
using System.Collections.Generic;

namespace Diamond_RO_Maksim
{
    class Main_RO
    {
        private static readonly string path = @"C:\Work\RO\RO_20191129_11_E";

        private static readonly string subCode = "17";

        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = subCode switch
            {
                "13" => methods.Start(path, subCode),
                "14" => methods.Start(path, subCode),
                "16" => methods.Start(path, subCode),
                "17" => methods.Start(path, subCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents);
            else Console.WriteLine("Wrong sub");
        }
    }
}
