using System;
using System.Collections.Generic;

namespace IS_Diamond_Maksim
{
    class Main_IS
    {

        private static string path = @"D:\DIAMOND\IS\20210420";
        private static string subCode = "3";
        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = subCode switch
            {
                "3" => methods.Start(path, subCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents);
            else Console.WriteLine("Wrong sub code");
        }
    }
}
