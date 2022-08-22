namespace Diamond_SG_Maksim
{
    internal class Main_SG
    {
        private static readonly string Path = @"C:\!Work\SG\SG_20220715_202207A_5";
        private static readonly string SubCode = "5";
        private static readonly bool SendToProd = false; //true - send to prod; false - send to stag

        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = SubCode switch
            {
                "5" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents, SendToProd);
            else Console.WriteLine("Wrong sub code");
        }
    }
}