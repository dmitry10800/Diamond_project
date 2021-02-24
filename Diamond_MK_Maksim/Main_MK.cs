using System;
using System.Collections.Generic;

namespace Diamond_MK_Maksim
{
    class Main_MK
    {
        static void Main(string[] args)
        {
            string path = @"C:\Work\MK\MK_20200131_01";
            string subCode = "3";

            Methods methods = new Methods();

            List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = subCode switch
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
