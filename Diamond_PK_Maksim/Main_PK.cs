namespace Diamond_PK_Maksim
{
    class Main_PK
    {
        private const string Path = @"C:\Work\PK\PK_20211231";
        private const string SubCode = "13";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {
            Methods methods = new();

            var convertedPatents = SubCode switch
            {
                "13" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(convertedPatents, SendToProd);
        }


    }
}