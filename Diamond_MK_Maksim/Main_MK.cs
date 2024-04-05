using System;

namespace Diamond_MK_Maksim
{
    class Main_MK
    {
        static void Main(string[] args)
        {
            var path = @"C:\Work\MK\MK_20210531_05";
            var subCode = "3";

            var methods = new Methods();

            var convertedPatents = subCode switch
            {
                "3" => methods.Start(path,subCode),
                _=> null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents);
            else Console.WriteLine("Wrong subCode");
        }
    }
}
