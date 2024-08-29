using System;

namespace Diamond_ID_Maksim
{
    class Main_ID
    {
        private const string Path = @"D:\LENS\TET\ID\ID_20240223_01(1)";
        private const string SubCode = "5";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var patents = SubCode switch
            {
                "1" => methods.Start(Path, SubCode),
                "2" => methods.Start(Path, SubCode),
                "3" => methods.Start(Path, SubCode),
                "4" => methods.Start(Path, SubCode),
                "5" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(patents, SendToProd);
        }
    }
}
