using System;
using System.Collections.Generic;

namespace PL_Diamond_Maksim
{
    class Main_PL
    {
        private static string path = @"C:\Work\PL\PL_20200331_03W";
        private static string subCode = "10";
        private static string newOrOld = "new";  // new / old
        static void Main(string[] args)
        {
            Methods methods = new Methods();

            List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = subCode switch
            {
                "10" => methods.Start(path, subCode, newOrOld),
                "25" => methods.Start(path, subCode, newOrOld),
                "32" => methods.Start(path, subCode, newOrOld),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents);
            else Console.WriteLine("Wrong subCode");
        }
    }
}
