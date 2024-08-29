using System;

namespace Diamond_SI_Maksim
{
    class Main_SI
    {
        private const string Path = @"C:\Work\SI\SI_20190830_08";
        private const string SubCode = "20";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var convertedPatents = SubCode switch
            {
                "20" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(convertedPatents, SendToProd);
        }
    }
}
