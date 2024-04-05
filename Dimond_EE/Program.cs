using Dimond_EE.Subs;
using System;


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
                    var sub_5 = new Sub5();
                    
                    var listPatents = sub_5.Start(path);

                    var convertedPatents = DiamondConverter.Sub5Convertor(listPatents);

                    var methods = new Methods();

                    methods.SendToDiamond(convertedPatents);

                    break;

                default:

                    Console.WriteLine("Такого саба нет");

                    break;

            }
        }
    }
}
