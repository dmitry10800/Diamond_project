using System;

namespace Diamond_MK_Maksim
{
    internal class Main_MK
    {
        private const string Path = @"D:\LENS\TET\MK\MK_20241231_12";
        private const string SubCode = "7";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var convertedPatents = SubCode switch
            {
                "3" => methods.Start(Path,SubCode),
                "7" => methods.Start(Path,SubCode),
                _=> null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(convertedPatents, SendToProd);
        }
    }
}
