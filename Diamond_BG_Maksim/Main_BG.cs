using System;

namespace Diamond_BG_Maksim
{
    class Main_BG
    {
        private const string Path = @"D:\LENS\TET\BG\BG_20240415_04(1)";
        private const string SubCode = "7";
        private const bool SendToProd = false; //true - send to prod; false - send to stag

        private static void Main()
        {
            var methods = new Methods();

            var patents = SubCode switch
            {
                "1" => methods.Start(Path,SubCode),
                "3" => methods.Start(Path, SubCode),
                "4" => methods.Start(Path, SubCode),
                "5" => methods.Start(Path, SubCode),
                "7" => methods.Start(Path, SubCode),
                "21" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) DiamondUtilities.DiamondSender.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong SubCode");
        }
    }
}
