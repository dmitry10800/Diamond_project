using System;

namespace Diamond_MK_Maksim
{
    class Main_MK
    {
        private const string Path = @"C:\Work\MK\MK_20210531_05";
        private const string SubCode = "3";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var convertedPatents = SubCode switch
            {
                "3" => methods.Start(Path,SubCode),
                _=> null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(convertedPatents, SendToProd);
        }
    }
}
