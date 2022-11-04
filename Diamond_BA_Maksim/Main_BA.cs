﻿namespace Diamond_BA_Maksim
{
    internal class Main_BA
    {
        private const string Path = @"D:\LENS\BA\BA_20200930_03";
        private const string SubCode = "4";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        static void Main(string[] args)
        {
            Methods methods = new();

            var patents = SubCode switch
            {
                "4" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong subcode");
        }
    }
}