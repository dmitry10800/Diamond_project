using System;
using System.Collections.Generic;

namespace Diamond_IL_Maksim
{
    class Main_IL
    {

        private readonly static string Path = @"C:\Work\IL\IL_20210325_03";
        private readonly static string SubCode = "19";
        private readonly static bool SendToProduction = false;    // если false то отправляет на стейджинг - если true то отправляет на продакшен

        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = SubCode switch
            {
                "19" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents, SendToProduction);
            else Console.WriteLine("Wrong sub code");
        }
    }
}
