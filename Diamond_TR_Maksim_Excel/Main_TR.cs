namespace Diamond_TR_Maksim_Excel
{
    class Main_TR
    {
        private static readonly string Path = @"C:\Work\TR\TR_20211122_11\47";
        private static readonly string SubCode = "47";
        private static readonly bool SendToProd = false;   // true - send to Prod ; false - send to Stag

        static void Main(string[] args)
        {           
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> ? convertedPatents = SubCode switch
            {
                "10" => methods.Start(Path, SubCode),
                "13" => methods.Start(Path, SubCode),
                "16" => methods.Start(Path, SubCode),
                "17" => methods.Start(Path, SubCode),
                "19" => methods.Start(Path, SubCode),
                "30" => methods.Start(Path, SubCode),
                "31" => methods.Start(Path, SubCode),
                "39" => methods.Start(Path, SubCode),
                "41" => methods.Start(Path, SubCode),
                "47" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null)
            {
                methods.SendToDiamond(convertedPatents, SendToProd);
            }

            else Console.WriteLine("SubCode must be 10, 13, 16, 17, 19, 27, 30, 31, 39");

        }
    }
}
