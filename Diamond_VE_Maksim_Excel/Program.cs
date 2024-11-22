namespace Diamond_VE_Maksim_Excel
{
    internal class Program
    {
        private const string Path = @"D:\LENS\TET\VE\VE_20240305_628";
        private const string SubCode = "72";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var convertedPatents = SubCode switch
            {
                "26" => methods.Start(Path, SubCode),
                "34" => methods.Start(Path, SubCode),
                "55" => methods.Start(Path, SubCode),
                "56" => methods.Start(Path, SubCode),
                "64" => methods.Start(Path, SubCode),
                "65" => methods.Start(Path, SubCode),
                "72" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(convertedPatents, SendToProd);
        }
    }
}