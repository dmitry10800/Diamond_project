﻿using System;
using System.Collections.Generic;

namespace Diamond_VN_Maksim
{
    class Main_VN
    {
        private const string Path = @"D:\LENS\VN\VN_20221125_416B";
        private const string SubCode = "15";
        private const bool SendToProd = true; //true - send to prod; false - send to stag

        static void Main(string[] args)
        {
            Methods methods = new();

            var patents = SubCode switch
            {
                "3" => methods.Start(Path, SubCode),
                "4" => methods.Start(Path, SubCode),
                "6" => methods.Start(Path, SubCode),
                "7" => methods.Start(Path, SubCode),
                "12" => methods.Start(Path, SubCode), // Abstract screenshots has to be added
                "13" => methods.Start(Path, SubCode), // Abstract screenshots has to be added
                "14" => methods.Start(Path, SubCode), // Abstract screenshots has to be added
                "15" => methods.Start(Path, SubCode), // Abstract screenshots has to be added
                "16" => methods.Start(Path, SubCode),
                "17" => methods.Start(Path, SubCode),
                "18" => methods.Start(Path, SubCode),
                "19" => methods.Start(Path, SubCode),
                "20" => methods.Start(Path, SubCode),
                "22" => methods.Start(Path, SubCode),
                "23" => methods.Start(Path, SubCode),
                "24" => methods.Start(Path, SubCode),
                "29" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong SubCode");
        }
    }
}
