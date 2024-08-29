using System;

namespace Diamond_UZ_Maksim
{
    class Main_UZ
    {
        private const string Path = @"C:\!Work\UZ\UZ_20220228_02";
        private const string SubCode = "4";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var patents = SubCode switch
            {
                "1" => methods.Start(Path, SubCode),
                "3" => methods.Start(Path, SubCode),
                "4" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(patents, SendToProd);
        }
    }
}
