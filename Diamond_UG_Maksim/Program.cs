namespace Diamond_UG_Maksim
{
    internal class Program
    {
        private const string Path = @"D:\lens\ug\ug_20221231_01um";
        private const string SubCode = "4";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag
        static void Main(string[] args)
        {
            Methods methods = new();

            var patents = SubCode switch
            {
                "3" => methods.Start(Path, SubCode),
                "4" => methods.Start(Path, SubCode),
                _ => null
            };

            Console.WriteLine(); //this line is a stop and check point before sending to staging or production

            if (patents != null) methods.SendToDiamond(patents, SendToProd);
            else Console.WriteLine("Wrong subcode");
        }
    }
}