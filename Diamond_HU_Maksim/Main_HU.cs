using System;
using System.Collections.Generic;

namespace Diamond_HU_Maksim
{
    class Main_HU
    {
        static void Main(string[] args)
        {
            var path = @"C:\Work\HU";
            var subCode = @"3";

            var methods = new Methods();

            var patents = subCode switch
            {
            "3" => methods.Start(path, subCode),
            _ => null
            };

            if (patents != null) methods.SendToDiamond(patents);
            else Console.WriteLine("Wrong sub code");
        }
    }
}
