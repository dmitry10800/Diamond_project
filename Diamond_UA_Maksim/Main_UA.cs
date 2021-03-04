using System;
using System.Collections.Generic;

namespace Diamond_UA_Maksim
{
    class Main_UA
    {
        static void Main(string[] args)
        {
            string path = @"C:\Work\UA\UA_20210303_9(1)";
            string subCode = "8";

            Methods methods = new Methods();

            List<Diamond.Core.Models.LegalStatusEvent> patents = subCode switch
            {
                "2" => methods.Start(path, subCode),
                "8" => methods.Start(path, subCode),
                _ => null           
            };

            if (patents != null) methods.SendToDiamond(patents);
            else Console.WriteLine("Wrong sub code");
        }
    }
}
