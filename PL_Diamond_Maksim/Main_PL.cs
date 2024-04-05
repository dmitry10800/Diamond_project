using System;

namespace PL_Diamond_Maksim
{
    class Main_PL
    {
        private const string Path = @"D:\LENS\TET\PL\PL_20230522_21W";
        private const string SubCode = "25";
        private const string NewOrOld = "new"; // new / old
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main()
        {
            Methods methods = new();

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
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents, SendToProd);
            else Console.WriteLine("Wrong subCode");
        }
    }
}
