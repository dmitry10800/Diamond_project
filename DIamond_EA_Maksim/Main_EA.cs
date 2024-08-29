using System;

namespace DIamond_EA_Maksim
{
    class Main_EA
    {
        private const string Path = @"D:\_work\TET\EA\EA_20220531_05";
        private const string SubCode = "31";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        static void Main(string[] args)
        {

            var methods = new Methods();

            var patents = SubCode switch
            {
                "5" => methods.Start(Path, SubCode),
                "9" => methods.Start(Path, SubCode),
                "11" => methods.Start(Path, SubCode),
                "12" => methods.Start(Path, SubCode),
                "14" => methods.Start(Path, SubCode),
                "31" => methods.Start(Path,SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(patents, SendToProd);
        }
    }
}
