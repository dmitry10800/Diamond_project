namespace Diamond_PK_Maksim
{
    class Main_PK
    {
        private static readonly string Path = @"C:\Work\PK\PK_20211231";
        private static readonly string SubCode = "13";
        private static readonly bool SendToProd = false;   // true - send to Prod ; false - send to Stag

        static void Main(string[] args)
        {
            Methods methods = new();

            var convertedPatents = SubCode switch
            {
                "13" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents, SendToProd);
            else Console.WriteLine("wrong subcode");
        }


    }
}