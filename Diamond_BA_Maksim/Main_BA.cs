namespace Diamond_BA_Maksim
{
    internal class Main_BA
    {
        private const string Path = @"D:\LENS\BA\BA_20221231_04";
        private const string SubCode = "4";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var patents = SubCode switch
            {
                "4" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(patents, SendToProd);
        }
    }
}