namespace Diamond_BN_Maksim
{
    internal class Program
    {
        private const string Path = @"D:\LENS\TET\BN\BN_20231130_202311";
        private const string SubCode = "2";
        private const bool SendToProd = false; //true - send to prod; false - send to stag
        static void Main(string[] args)
        {
            var methods = new Methods();

            var patents = SubCode switch
            {
                "2" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) DiamondUtilities.DiamondSender.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong SubCode");
        }
    }
}
