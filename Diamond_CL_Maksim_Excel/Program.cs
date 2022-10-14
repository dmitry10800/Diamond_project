namespace Diamond_CL_Maksim_Excel
{
    internal class Program
    {
        private const string Path = @"D:\LENS\CL\CL_20211231_01";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag
        private static void Main()
        {
            Methods methods = new();

            List<Diamond.Core.Models.LegalStatusEvent> patents = methods.Start(Path);

            Console.WriteLine();

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong subcode");
        }
    }
}