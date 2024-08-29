namespace Diamond_RS_Maksim
{
    class Main_RS
    {
        // Прежде чем запускать необходимо извлечь из общего файла 3 саб в отдельный PDF файл
        private const string Path = @"C:\Work\RS\RS_20200731_07";
        private const string SubCode = "3";
        private const bool SendToProd = false; // true - send to Prod ; false - send to Stag

        private static void Main(string[] args)
        {
            var methods = new Methods();

            var convertedPatents = SubCode switch
            {
                "3" => methods.Start(Path, SubCode),
                _ => null
            };

            DiamondUtilities.DiamondSender.SendToDiamond(convertedPatents, SendToProd);
        }
    }
}
