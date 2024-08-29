namespace Diamond_SM_Maksim
{
    internal class Main_SM
    {
        private const string Path = @"D:\LENS\TET\SM\SM_20230512_02";
        private const string SubCode = "3";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var convertedPatents = SubCode switch
            {
                "3" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(convertedPatents, SendToProd);
        }
    }
}