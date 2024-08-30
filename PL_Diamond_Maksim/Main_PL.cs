using System;

namespace PL_Diamond_Maksim
{
    class Main_PL
    {
        private const string Path = @"D:\LENS\TET\PL\PL_20240708_28W";
        private const string SubCode = "58";
        private const bool NewOrOld = true; // new - true / old - false
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main()
        {
            var methods = new Methods();

            var convertedPatents = SubCode switch
            {
                "10" => methods.Start(Path, SubCode, NewOrOld),
                "25" => methods.Start(Path, SubCode, NewOrOld),
                "31" => methods.Start(Path, SubCode, NewOrOld),
                "32" => methods.Start(Path, SubCode, NewOrOld),
                "33" => methods.Start(Path, SubCode, NewOrOld),
                "46" => methods.Start(Path, SubCode, NewOrOld),
                "47" => methods.Start(Path, SubCode, NewOrOld),
                "51" => methods.Start(Path, SubCode, NewOrOld),
                "58" => methods.Start(Path, SubCode, NewOrOld),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(convertedPatents, SendToProd);
        }
    }
}
