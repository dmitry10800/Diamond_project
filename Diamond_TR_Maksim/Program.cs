namespace Diamond_TR_Maksim
{
    internal class Program
    {
        private const string Path = @"D:\LENS\TET\TR\TR_20230424_04";
        private const string Sub = "27";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var convertedPatents = Sub switch
            {
                "10" => methods.Start(Path, Sub),
                "13" => methods.Start(Path, Sub),
                "16" => methods.Start(Path, Sub),
                "17" => methods.Start(Path, Sub),
                "19" => methods.Start(Path, Sub),
                "27" => methods.Start(Path, Sub),
                "30" => methods.Start(Path, Sub),
                "31" => methods.Start(Path, Sub),
                "39" => methods.Start(Path, Sub),
                _ => null
            };

            Console.WriteLine();

            DiamondUtilities.DiamondSender.SendToDiamond(convertedPatents, SendToProd);
        }
    }
}
