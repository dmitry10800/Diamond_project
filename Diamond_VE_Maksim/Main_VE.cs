using System;
using System.Collections.Generic;

namespace Diamond_VE_Maksim
{
    class Main_VE
    {
        private static readonly string Path = @"D:\Develop\Country\VE\VE_20220329_615";
        private static readonly string SubCode = "19";
        private static readonly bool SendToProd = false;   // true - send to Prod ; false - send to Stag
        /*
         * if 12 sub - you must find in the tet-file the beginning of the section "LA PROPIEDAD INTELECTUAL - REGISTRO DE LA PROPIEDAD INDUSTRIAL" and insert "12_"
         * at the beginning of the phrase,
         * which would be "12_LA PROPIEDAD INTELECTUAL - REGISTRO DE LA PROPIEDAD INDUSTRIAL" - then correctly effective date will be displayed
         *
         *
         * if 19 sub - "LA PROPIEDAD INTELECTUAL - REGISTRO DE LA PROPIEDAD INDUSTRIAL"  => "19_LA PROPIEDAD INTELECTUAL - REGISTRO DE LA PROPIEDAD INDUSTRIAL"
         */


        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> patents = SubCode switch
            {
                "12" => methods.Start(Path, SubCode),
                "19" => methods.Start(Path, SubCode),
                "24" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong subcode");
        }
    }
}
