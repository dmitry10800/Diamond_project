namespace Diamond_FI_Maksim
{
    internal class Program
    {
        private const string Path = @"D:\LENS\TET\FI\FI_20250317_11";
        private const string SubCode = "23";
        private const bool SendToProd = false; //true - send to prod; false - send to stag

        static void Main(string[] args)
        {
            var methods = new Methods();

            var convertedPatents = SubCode switch
            {
                "23" => methods.Start(Path, SubCode),
                "24" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(convertedPatents, SendToProd);
        }
    }
}
