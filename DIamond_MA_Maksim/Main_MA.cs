namespace Diamond_MA_Maksim
{
    class Main_MA
    {
        private const string path = @"C:\!Work\MA\MA_20211231_12";
        private const string subCode = "1";
        private const bool SendToProd = false; //true - send to prod; false - send to stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var convertedPatents = subCode switch
            {
                "1" => methods.Start(path, subCode),
                "2" => methods.Start(path, subCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(convertedPatents, SendToProd);
        }
    }
}