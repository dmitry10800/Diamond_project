namespace Diamond_MY_Maksim
{
    class Main_PK
    {
        private const string Path = @"D:\LENS\TET\MY\MY_20230419_05";
        private const string SubCode = "10";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        static void Main(string[] args)
        {
            Methods methods = new();

            var convertedPatents = SubCode switch
            {
                "1" => methods.Start(Path, SubCode),
                "9" => methods.Start(Path, SubCode),
                "10" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents, SendToProd);
            else Console.WriteLine("wrong subcode");
        }
    }
}