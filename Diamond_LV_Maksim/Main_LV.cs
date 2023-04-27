namespace Diamond_LV_Maksim
{
    internal class Main_LV
    {
        private const string path = @"D:\LENS\TET\LV\LV_20230320_03";
        private const string subCode = "4";
        private const bool SendToProd = false; //true - send to prod; false - send to stag

        static void Main(string[] args)
        {
            var methods = new Methods();

            var convertedPatents = subCode switch
            {
                "4" => methods.Start(path, subCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents, SendToProd);
            else Console.WriteLine("Wrong sub code");
        }
    }
}