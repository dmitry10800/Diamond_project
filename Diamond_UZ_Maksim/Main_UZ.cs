using System;

namespace Diamond_UZ_Maksim
{
    class Main_UZ
    {
        private const string Path = @"D:\LENS\TET\UZ\UZ_20241231_12";
        private const string SubCode = "20";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var patents = SubCode switch
            {
                "1" => methods.Start(Path, SubCode),
                "3" => methods.Start(Path, SubCode),
                "4" => methods.Start(Path, SubCode),
                "13" => methods.Start(Path, SubCode),
                "16" => methods.Start(Path, SubCode),
                "17" => methods.Start(Path, SubCode),
                "20" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(patents, SendToProd);
        }
    }
}
