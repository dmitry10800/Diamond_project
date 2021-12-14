using System;
using System.Collections.Generic;

namespace Diamond_VN_Maksim
{
    class Main_VN
    {

        private static readonly string Path = @"C:\Work\VN\VN_20210825_401B";
        private static readonly string SubCode = "16";
        private static readonly bool SendToProd = false; //true - send to prod; false - send to stag

        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> patents = SubCode switch
            {
                "6" => methods.Start(Path, SubCode),
                "16" => methods.Start(Path, SubCode),
                "23" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong SubCode");
        }
    }
}
