using System;

namespace Diamond_UA_Maksim
{
    class Main_UA
    {
        private const string Path = @"D:\LENS\TET\UA\UA_20230802_31(1)";
        private const string SubCode = "13";
        private const bool SendToProd = false;
        static void Main(string[] args)
        {
            var methods = new Methods();

            var patents = SubCode switch
            {
                "2" => methods.Start(Path, SubCode),
                "7" => methods.Start(Path, SubCode),
                "8" => methods.Start(Path, SubCode),
                "13" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong sub code");
        }
    }
}
