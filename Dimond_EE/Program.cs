using Dimond_EE.Subs;
using System;
using System.Collections.Generic;


namespace Dimond_EE
{
    class Program
    {
        static string path = @"C:\Work\EE\5sub\";
        static string sub = "5";

        static void Main(string[] args)
        {
            switch (sub)
            {
                case "5":
                    Sub5 sub_5 = new Sub5();
                    
                    List<Patent> listPatents = sub_5.Start(path);

                    List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = DiamondConverter.Sub5Convertor(listPatents);

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
