using System;

namespace Diamond_VE_Maksim
{
    class Main_VE
    {
        private const string Path = @"D:\LENS\TET\VE\VE_20240912_634";
        private const string SubCode = "71";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var patents = SubCode switch
            {
                "12" => methods.Start(Path, SubCode),
                "19" => methods.Start(Path, SubCode),
                "24" => methods.Start(Path, SubCode),
                "71" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(patents, SendToProd);
        }
    }
}
