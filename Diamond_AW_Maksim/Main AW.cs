namespace Diamond_AW_Maksim
{
    internal class Main_AW
    {
        private static readonly string Path = @"D:\_work\TET\AW";
        private static readonly string SubCode = "1";
        private static readonly bool SendToProd = false;   // true - send to Prod ; false - send to Stag
        static void Main(string[] args)
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> patents = SubCode switch
            {
                "1" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong subcode");
        }
    }
}