namespace Diamond_TR_Maksim_Excel
{
    class Main_TR
    {
        private const string Path = @"D:\LENS\TR\TR_20220921_09\15";
        private const string SubCode = "15";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {           
            var methods = new Methods();

            var convertedPatents = SubCode switch
            {
                "10" => methods.Start(Path, SubCode),
                "13" => methods.Start(Path, SubCode),
                "15" => methods.Start(Path, SubCode),
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

            DiamondUtilities.DiamondSender.SendToDiamond(convertedPatents, SendToProd);
        }
    }
}
