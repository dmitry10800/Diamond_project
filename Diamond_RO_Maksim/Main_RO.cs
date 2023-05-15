using System;

namespace Diamond_RO_Maksim
{
    class Main_RO
    {
        private const string Path = @"D:\LENS\TET\RO\RO_20230228_02_E";
        private const string SubCode = "17";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        static void Main()
        {
            Methods methods = new();

            var convertedPatents = SubCode switch
            {
                "5" => methods.Start(Path, SubCode),
                "11" => methods.Start(Path, SubCode),
                "12" => methods.Start(Path, SubCode),
                "13" => methods.Start(Path, SubCode),
                "14" => methods.Start(Path, SubCode),
                "16" => methods.Start(Path, SubCode),
                "17" => methods.Start(Path, SubCode),
                "20" => methods.Start(Path, SubCode),
                "22" => methods.Start(Path, SubCode),
                "23" => methods.Start(Path, SubCode),
                "24" => methods.Start(Path, SubCode),
                "27" => methods.Start(Path, SubCode),
                "29" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents, SendToProd);
            else Console.WriteLine("Wrong sub");
        }
    }
}
