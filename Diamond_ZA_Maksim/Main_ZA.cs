using System;

namespace Diamond_ZA_Maksim
{
    class Main_ZA
    {
        private const string Path = @"D:\LENS\TET\ZA\ZA_20230726_07(2)";
        private const string SubCode = "1";
        private const bool SendToProd = false; //true - send to prod; false - send to stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var convertedPatents = SubCode switch
            {
                "1" => methods.Start(Path, SubCode),
                "3" => methods.Start(Path, SubCode),
                "5" => methods.Start(Path, SubCode),
                "6" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(convertedPatents, SendToProd);
        }
    }
}
