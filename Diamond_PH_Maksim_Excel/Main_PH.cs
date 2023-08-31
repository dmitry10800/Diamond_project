namespace Diamond_PH_Maksim_Excel
{
    internal class Main_PH
    {
        private const string Path = @"D:\LENS\TET\PH\PH_20230816_89";
        private const string SubCode = "5";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        static void Main(string[] args)
        {
            Methods methods = new();

            var patents = SubCode switch
            {
                "5" => methods.Start(Path, SubCode),
                "7" => methods.Start(Path, SubCode),
                "12" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong subcode");
        }
    }
}