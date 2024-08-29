using System;

namespace Diamond_AP_Maksim
{
    class Main_AP
    {
        private const string Path = @"D:\_work\TET\AP\AP_20220531_05\2";
        private const string SubCode = "3";
        private const bool SendToProd = false; //true - send to prod; false - send to stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var convertedPatents = SubCode switch
            {
                "1" => methods.Start(Path, SubCode),
                "2" => methods.Start(Path, SubCode),
                "3" => methods.Start(Path, SubCode),
                "7" => methods.Start(Path, SubCode),
                "10" => methods.Start(Path, SubCode),
                "20" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null)
                DiamondUtilities.DiamondSender.SendToDiamond(convertedPatents, SendToProd);
        }
    }
}
