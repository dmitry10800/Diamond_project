using System;

namespace Diamond_IN_Maksim
{
    class Main_IN
    {
        private const string Path = @"D:\LENS\TET\IN\IN_20140606_23(1)";
        private const string SubCode = "26";
        private const bool SendToProd = false; //true - send to prod; false - send to stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var convertedPatents = SubCode switch
            {
                "1" => methods.Start(Path, SubCode),
                "10" => methods.Start(Path, SubCode),
                "26" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(convertedPatents, SendToProd);
        }
    }
}
