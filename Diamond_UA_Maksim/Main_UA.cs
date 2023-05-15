using System;
using System.Collections.Generic;

namespace Diamond_UA_Maksim
{
    class Main_UA
    {
        static void Main(string[] args)
        {
            var path = @"C:\Work\UA\UA_20211110_45(1)";
            var subCode = "2";

            var methods = new Methods();

            var patents = subCode switch
            {
                "2" => methods.Start(path, subCode),
                "8" => methods.Start(path, subCode),
                _ => null           
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents);
            else Console.WriteLine("Wrong sub code");
        }
    }
}
