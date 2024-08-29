namespace Diamond_LV_Maksim
{
    internal class Main_LV
    {
        private const string Path = @"D:\LENS\TET\LV\LV_20230320_03";
        private const string SubCode = "4";
        private const bool SendToProd = false; //true - send to prod; false - send to stag

        static void Main(string[] args)
        {
            var methods = new Methods();

            var convertedPatents = SubCode switch
            {
                "4" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(convertedPatents, SendToProd);
        }
    }
}