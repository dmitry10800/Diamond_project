using System;
using System.Collections.Generic;

namespace Diamond_JE_Maksim
{
    class Main_JE
    {
        private static readonly string Path = @"C:\!Work\JE\JE_20211231_(11)";
        private static readonly string SubCode = "1";
        private static readonly bool SendToProd = false; //true - send to prod; false - send to stag

        private static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = SubCode switch
            {
                "1" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents, SendToProd);
            else Console.WriteLine("Wrong sub code");
        }
    }
}