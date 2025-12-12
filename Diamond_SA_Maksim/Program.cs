namespace Diamond_SA_Maksim
{
    internal class Program
    {
        private const string Path = @"D:\LENS\TET\SA\SA_20220729_65";
        private const string SubCode = "1";
        private const bool SendToProd = false; //true - send to prod; false - send to stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var convertedPatents = SubCode switch
            {
                "1" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(convertedPatents, SendToProd);
        }
    }
}