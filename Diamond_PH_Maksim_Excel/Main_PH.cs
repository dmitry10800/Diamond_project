namespace Diamond_PH_Maksim_Excel
{
    internal class Main_PH
    {
        private const string Path = @"D:\LENS\TET\PH\PH_20260128_11";
        private const string SubCode = "5";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var patents = SubCode switch
            {
                "5" => methods.Start(Path, SubCode),
                "7" => methods.Start(Path, SubCode),
                "12" => methods.Start(Path, SubCode),
                "22" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(patents, SendToProd);
        }
    }
}