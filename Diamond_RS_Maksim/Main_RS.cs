using System;
using System.Collections.Generic;

namespace Diamond_RS_Maksim
{
    class Main_RS
    {

        // Прежде чем запускать необходимо извлечь из общего файла 3 саб в отдельный PDF файл

        static void Main(string[] args)
        {
            var path = @"C:\Work\RS\RS_20200731_07";
            var subCode = "3";

            var methods = new Methods();

            var convertedPatents = subCode switch
            {
                "3" => methods.Start(path, subCode),
                _ => null
            };

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents);
            else Console.WriteLine("Wrong SubCode");
        }
    }
}
