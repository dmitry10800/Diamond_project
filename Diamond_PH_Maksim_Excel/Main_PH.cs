﻿namespace Diamond_PH_Maksim_Excel
{
    internal class Main_PH
    {
        private const string Path = @"D:\LENS\PH\PH_20220831_99";
        private const string SubCode = "7";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> patents = SubCode switch
            {
                "7" => methods.Start(Path, SubCode),
                "12" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong subcode");
        }
    }
}