namespace Diamond_CL_Maksim_Excel
{
    internal class Program
    {
        private const string Path = @"D:\LENS\CL\CL_20221231_01";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag
        private static void Main()
        {
            var methods = new Methods();

            var patents = methods.Start(Path);

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(patents, SendToProd);

        }
    }
}