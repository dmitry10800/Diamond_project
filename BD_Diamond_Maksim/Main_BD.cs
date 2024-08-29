using System;

namespace BD_Diamond_Maksim
{
    class Main_BD
    {
        private const string Path = @"C:\Work\BD\BD_20210218_07";
        private const string SubCode = "2";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var convertedPatents = SubCode switch
            {
                "2" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(convertedPatents, SendToProd);

        }
    }
}
