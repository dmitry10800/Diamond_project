namespace Diamond_PK_Maksim_Excel
{
    internal class Program
    {
        private const string Path = @"D:\LENS\TET\PK";
        private const string SubCode = "13";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag
        static void Main(string[] args)
        {
            var methods = new Methods();

            var convertedPatents = SubCode switch
            {
                "13" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) 
                methods.SendToDiamond(convertedPatents, SendToProd);
        }
    }
}