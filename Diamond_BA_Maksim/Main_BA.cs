namespace Diamond_BA_Maksim
{
    internal class Main_BA
    {
        private static readonly string Path = @"C:\!Work\BA\BA_20220630_02";
        private static readonly string SubCode = "4";
        private static readonly bool SendToProd = false;   // true - send to Prod ; false - send to Stag
        static void Main(string[] args)
        {
            Methods methods = new();

            var patents = SubCode switch
            {
                "4" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong subcode");
        }
    }
}