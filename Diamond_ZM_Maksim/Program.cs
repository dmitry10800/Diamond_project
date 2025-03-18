namespace Diamond_ZM_Maksim
{
    internal class Program
    {
        private const string Path = @"D:\LENS\TET\ZM\ZM_20250228_02";
        private const string SubCode = "5";
        private const bool SendToProd = false; //true - send to prod; false - send to stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var convertedPatents = SubCode switch
            {
                "5" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(convertedPatents, SendToProd);
        }
    }
}