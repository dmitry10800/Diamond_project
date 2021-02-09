using Diamond_LV.Sub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diamond_LV
{
    class Program
    {

        static string path = @"C:\Work\LV\LV_20210120_01";

        static string sub = "4";
        static void Main(string[] args)
        {
            switch (sub)
            {
                case "4":

                    Sub4 sub_4 = new Sub4();

                    List<Patent> listPatents = sub_4.Start(path);

                    List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = DiamondConverter.Sub4Convertor(listPatents);

                    Methods methods = new Methods();

                    methods.SendToDiamond(convertedPatents);

                    break;

                default:

                    Console.WriteLine("Такого саба нет");

                    break;

            }
        }
    }
}
