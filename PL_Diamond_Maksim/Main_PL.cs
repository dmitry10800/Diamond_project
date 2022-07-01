using System;
using System.Collections.Generic;

namespace PL_Diamond_Maksim
{
    class Main_PL
    {
        private static readonly string Path = @"D:\_work\TET\PL\PL_20220606_23W";
        private static readonly string SubCode = "51";
        private static readonly string NewOrOld = "new";  // new / old
        private static readonly bool SendToProd = true;   // true - send to Prod ; false - send to Stag
        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = SubCode switch
            {
                "10" => methods.Start(Path, SubCode, NewOrOld),
                "25" => methods.Start(Path, SubCode, NewOrOld),
                "31" => methods.Start(Path, SubCode, NewOrOld),
                "32" => methods.Start(Path, SubCode, NewOrOld),
                "46" => methods.Start(Path, SubCode, NewOrOld),
                "47" => methods.Start(Path, SubCode, NewOrOld),
                "51" => methods.Start(Path, SubCode, NewOrOld),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents, SendToProd);
            else Console.WriteLine("Wrong subCode");
        }
    }
}
