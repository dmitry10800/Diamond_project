using System;

namespace Diamond_UY_Maksim
{
    class Main_UY
    {
        private const string Path = @"C:\Work\UY\UY_20210831_263";
        private const string SubCode = "8";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var patents = SubCode switch
            {
                "8" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(patents, SendToProd);
        }
    }
}
