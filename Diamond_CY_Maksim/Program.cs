namespace Diamond_CY_Maksim
{
    internal class Program
    {
        private const string Path = @"D:\LENS\TET\CY\CY_20240920_4616B";
        private const string SubCode = "11";
        private const bool SendToProd = false; //true - send to prod; false - send to stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var convertedPatents = SubCode switch
            {
                "3" => methods.Start(Path, SubCode),
                "6" => methods.Start(Path, SubCode),
                "11" => methods.Start(Path, SubCode),
                "39" => methods.Start(Path, SubCode),
                "57" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(convertedPatents, SendToProd);
        }
    }
}
