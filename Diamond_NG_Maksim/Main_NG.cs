using System;

namespace Diamond_NG_Maksim
{
    class Main_NG
    {
        private const string Path = @"D:\LENS\TET\NG\NG_20250630_02";
        private const string SubCode = "4";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var patents = SubCode switch
            {
                "1" => methods.Start(Path, SubCode),
                "4" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(patents, SendToProd);
        }
    }
}
