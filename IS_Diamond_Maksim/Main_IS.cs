using System;

namespace IS_Diamond_Maksim
{
    class Main_IS
    {
        private const string Path = @"C:\!Work\IS\IS_20220315_03";
        private const string SubCode = "9";
        private const bool SendToProd = false; //true - send to prod; false - send to stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var convertedPatents = SubCode switch
            {
                "3" => methods.Start(Path, SubCode),
                "9" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(convertedPatents, SendToProd);
        }
    }
}
