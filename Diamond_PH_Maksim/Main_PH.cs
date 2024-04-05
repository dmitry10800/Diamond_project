using System;

namespace Diamond_PH_Maksim
{
    class Main_PH
    {
        private const string Path = @"D:\LENS\PH\PH_20230120_08";
        private const string SubCode = "35";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        static void Main(string[] args)
        {
            Methods methods = new();

            var patents = SubCode switch
            {
                "5" => methods.Start(Path,SubCode),
                "7" => methods.Start(Path,SubCode),
                "12" => methods.Start(Path, SubCode),
                "22" => methods.Start(Path, SubCode),               
                "29" => methods.Start(Path, SubCode),               
                "35" => methods.Start(Path, SubCode),                
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong subcode");
        }
    }
}
