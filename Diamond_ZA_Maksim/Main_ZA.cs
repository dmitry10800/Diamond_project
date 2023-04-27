using System;

namespace Diamond_ZA_Maksim
{
    class Main_ZA
    {
        private const string path = @"D:\LENS\TET\ZA\ZA_20230329_03(2)";
        private const string subCode = "5";
        private const bool SendToProd = false; //true - send to prod; false - send to stag

        static void Main(string[] args)
        {
            Methods methods = new();

            var convertedPatents = subCode switch
            {
                "1" => methods.Start(path, subCode),
                "3" => methods.Start(path, subCode),
                "5" => methods.Start(path, subCode),
                "6" => methods.Start(path, subCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents, SendToProd);
            else Console.WriteLine("Wrong sub code");
        }
    }
}
