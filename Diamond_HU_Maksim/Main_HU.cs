namespace Diamond_HU_Maksim
{
    class Main_HU
    {
        private const string Path = @"C:\Work\HU";
        private const string SubCode = "3";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var patents = SubCode switch
            {
            "3" => methods.Start(Path, SubCode),
            _ => null
            };

            DiamondUtilities.DiamondSender.SendToDiamond(patents, SendToProd);
        }
    }
}
