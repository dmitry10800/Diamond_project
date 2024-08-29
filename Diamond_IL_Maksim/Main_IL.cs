using System;

namespace Diamond_IL_Maksim
{
    class Main_IL
    {
        private const string Path = @"C:\Work\IL\IL_20210325_03";
        private const string SubCode = "19";
        private const bool SendToProduction = false; // если false то отправляет на стейджинг - если true то отправляет на продакшен

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var convertedPatents = SubCode switch
            {
                "19" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(convertedPatents, SendToProduction);
        }
    }
}
