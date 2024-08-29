using System;

namespace Diamond_AR_Maksim
{
    class Main_AR
    {
        private const string Path = @"D:\LENS\TET\AR\AR_20230419_1283";
        private const string SubCode = "8";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag
        private static void Main(string[] args)
        {
            var methods = new Methods();

            var patents = SubCode switch
            {
                "1" => methods.Start(Path, SubCode),
                "2" => methods.Start(Path, SubCode),
                "3" => methods.Start(Path, SubCode),
                "5" => methods.Start(Path, SubCode),
                "6" => methods.Start(Path, SubCode),
                "7" => methods.Start(Path, SubCode),
                "8" => methods.Start(Path, SubCode),
                "9" => methods.Start(Path, SubCode),
                "10" => methods.Start(Path, SubCode),
                "11" => methods.Start(Path, SubCode),
                "12" => methods.Start(Path, SubCode),
                "13" => methods.Start(Path, SubCode),
                "14" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) DiamondUtilities.DiamondSender.SendToDiamond(patents,SendToProd);
            else Console.WriteLine("Wrong subcode");
        }
    }
}
