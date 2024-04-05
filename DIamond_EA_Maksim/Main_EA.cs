using System;

namespace DIamond_EA_Maksim
{
    class Main_EA
    {

        private static readonly string Path = @"D:\_work\TET\EA\EA_20220531_05";
        private static readonly string SubCode = "31";
        private static readonly bool SendToProd = false;   // true - send to Prod ; false - send to Stag
        static void Main(string[] args)
        {

            Methods methods = new();

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

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong subcode");

        }
    }
}
