namespace Diamond_PK_Maksim_Excel
{
    internal class Program
    {
        private const string Path = @"D:\LENS\TET\PK\PK_20241231_expired";
        private const string SubCode = "13";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

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