using System;

namespace Diamond_IE_Maksim
{
    class Main_IE
    {
        private const string Path = @"D:\LENS\TET\IE\IE_20231220_2505";
        private const string SubCode = "1";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var patents = SubCode switch
            {
                "1" => methods.Start(Path, SubCode),
                "6" => methods.Start(Path, SubCode),
                "52" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(patents, SendToProd);
        }
    }
}
