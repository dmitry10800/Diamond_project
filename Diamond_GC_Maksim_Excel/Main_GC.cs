namespace Diamond_GC_Maksim_Excel
{
    internal class Main_GC
    {
        private const string Path = @"D:\LENS\GC\GC_20220801_65\26";
        private const string SubCode = "26";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main()
        {
            var methods = new Methods();

            var patents = SubCode switch
            {
                "14" => methods.Start(Path, SubCode),
                "26" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(patents, SendToProd);
        }
    }
}