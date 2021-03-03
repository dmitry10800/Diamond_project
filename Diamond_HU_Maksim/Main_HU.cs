using System;
using System.Collections.Generic;

namespace Diamond_HU_Maksim
{
    class Main_HU
    {
        static void Main(string[] args)
        {
            string path = @"C:\Work\HU\HU_20200928_18";
            string subCode = @"3";

            Methods methods = new Methods();

            List<Diamond.Core.Models.LegalStatusEvent> patents = subCode switch
            {
            "3" => methods.Start(path, subCode),
            _ => null
            };

            if (patents != null) methods.SendToDiamond(patents);
            else Console.WriteLine("Wrong sub code");
        }
    }
}
