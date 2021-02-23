using System;

namespace Diamond_MK_Maksim
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World     !");

            string sub = "3";

            string hello = sub switch
            {
                "3" => "Max"
            };
        }
    }
}
