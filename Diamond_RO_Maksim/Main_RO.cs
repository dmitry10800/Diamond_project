using System;

namespace Diamond_RO_Maksim
{
    class Main_RO
    {
        private const string path = @"D:\LENS\RO\RO_20221028_10_E";
        private const string subCode = "12";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        static void Main(string[] args)
        {
            Methods methods = new();

            var convertedPatents = subCode switch
            {
                "11" => methods.Start(path, subCode),
                "12" => methods.Start(path, subCode),
                "13" => methods.Start(path, subCode),
                "14" => methods.Start(path, subCode),
                "16" => methods.Start(path, subCode),
                "17" => methods.Start(path, subCode),
                "20" => methods.Start(path, subCode),
                "22" => methods.Start(path, subCode),
                "23" => methods.Start(path, subCode),
                "24" => methods.Start(path, subCode),
                "27" => methods.Start(path, subCode),
                "29" => methods.Start(path, subCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents, SendToProd);
            else Console.WriteLine("Wrong sub");
        }
    }
}
