using System;

namespace Diamond_EC_Maksim
{
    class Main_EC
    {
        private const string Path = @"C:\Work\EC\EC_20210531_692";
        private const string SubCode = "3";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var patents = SubCode switch
            {
                "3" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(patents, SendToProd);
        }
    }
}
