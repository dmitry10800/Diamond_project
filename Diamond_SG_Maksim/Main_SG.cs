namespace Diamond_SG_Maksim
{
    internal class Main_SG
    {
        private static readonly string Path = @"D:\_work\TET\SG\SG_20220830_202209A";
        private static readonly string SubCode = "6";
        private static readonly bool SendToProd = false; //true - send to prod; false - send to stag

        static void Main(string[] args)
        {
            Methods methods = new();

            var convertedPatents = SubCode switch
            {
                "5" => methods.Start(Path, SubCode),
                "6" => methods.Start(Path, SubCode),
                "7" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents, SendToProd);
            else Console.WriteLine("Wrong sub code");
        }
    }
}