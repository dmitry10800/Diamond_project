using System;
using System.Collections.Generic;

namespace Diamond_IN_Maksim
{
    class Main_IN
    {

        private static readonly string path = @"C:\Work\IN\IN_20210820_34(1)";
        private static readonly string subCode = "10";
        private static readonly bool SendToProd = false; //true - send to prod; false - send to stag

        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = subCode switch
            {
                "10" => methods.Start(path, subCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents, SendToProd);
            else Console.WriteLine("Wrong sub code");
        }
    }
}
