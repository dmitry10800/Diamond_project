using System;
using System.Collections.Generic;

namespace Diamond_ME_Maksim
{
    class Main_ME
    {

        private static readonly string Path = @"C:\Work\ME\ME_20210420_38";
        private static readonly string SubCode = "3";
        private static readonly bool SendToProd = false;   // true - send to Prod ; false - send to Stag

        static void Main(string[] args)
        {

            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> patents = SubCode switch
            {
                "2" => methods.Start(Path, SubCode),
                "3" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong subcode");
        }
    }
}
