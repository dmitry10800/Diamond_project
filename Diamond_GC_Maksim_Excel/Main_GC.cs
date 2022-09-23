namespace Diamond_GC_Maksim_Excel
{
    internal class Main_GC
    {
        private const string Path = @"D:\LENS\GC\GC_20220801_65\26";
        private const string SubCode = "26";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main()
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> patents = SubCode switch
            {
                "14" => methods.Start(Path, SubCode),
                "26" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong subcode");
        }
    }
}