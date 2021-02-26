using System;
using System.Collections.Generic;

namespace Diamond_RS_Maksim
{
    class Main_RS
    {

        // Прежде чем запускать необходимо извлечь из общего файла 3 саб в отдельный PDF файл

        static void Main(string[] args)
        {
            string path = @"C:\Work\RS\RS_20200731_07";
            string subCode = "3";

            Methods methods = new Methods();

            List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = subCode switch
            {
                "3" => methods.Start(path, subCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents);
            else Console.WriteLine("Wrong SubCode");
        }
    }
}
