namespace Diamond_MY_Maksim
{
    class Main_PK
    {
        private static readonly string Path = @"C:\!Work\MY\MY_20220323_01";
        private static readonly string SubCode = "1";
        private static readonly bool SendToProd = false;   // true - send to Prod ; false - send to Stag

        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent>? convertedPatents = SubCode switch
            {
                "1" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents, SendToProd);
            else Console.WriteLine("wrong subcode");
        }


    }
}