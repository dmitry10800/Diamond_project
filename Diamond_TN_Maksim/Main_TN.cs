namespace Diamond_TN_Maksim
{
    internal class Main_TN
    {
        private const string Path = @"D:\LENS\TET\TN\TN_20230404_448";
        private const string SubCode = "2";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var patents = SubCode switch
            {
                "2" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(patents, SendToProd);
        }
    }
}