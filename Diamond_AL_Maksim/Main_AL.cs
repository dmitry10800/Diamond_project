using System;
using System.Collections.Generic;

namespace Diamond_AL_Maksim
{
    class Main_AL
    {

        private static readonly string Path = @"C:\Work\AL\AL_20211122_36";
        private static readonly string SubCode = "3";
        private static readonly bool SendToProd = false;   // true - send to Prod ; false - send to Stag

        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = SubCode switch
            {
                "3" => methods.Start(Path, SubCode),
                "19" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents, SendToProd);
            else Console.WriteLine("Wrong sub code");

        }
    }
}
