using System;
using System.Collections.Generic;

namespace Diamond_AL_Maksim
{
    class Main_AL
    {

        static readonly string path = @"C:\Work\AL\AL_20111101_39";
        static readonly string subCode = "19";

        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = subCode switch
            {
                "19" => methods.Start(path, subCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents);
            else Console.WriteLine("Wrong sub code");

        }
    }
}
