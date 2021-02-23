using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diamond_TR_Maksim
{
    class Main_TR
    {
        static void Main(string[] args)
        {
            string path = @"C:\Work\TR\TR_20210121_01";
            string sub = "17";

            Methods methods = new Methods();

            List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = sub switch
            {
                "10" => methods.Start(path,sub),
                "13" => methods.Start(path, sub),
                "16" => methods.Start(path, sub),
                "17" => methods.Start(path, sub),
                "30" => methods.Start(path, sub),
                "31" => methods.Start(path, sub),
                "39" => methods.Start(path, sub),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null)
            {
                methods.SendToDiamond(convertedPatents);
            }

            else Console.WriteLine("SubCode must be 10, 13, 16, 17, 30, 31, 39");

        }
    }
}
