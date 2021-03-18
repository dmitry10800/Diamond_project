using System;
using System.Collections.Generic;

namespace Diamond_SI_Maksim
{
    class Main_SI
    {
        private readonly static string path = @"C:\Work\SI\SI_20210129_01";
        private readonly static string subcode = "20";
        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = subcode switch
            {
                "20" => methods.Start(path, subcode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents);
            else Console.WriteLine("Wrong sub code");
           
        }
    }
}
