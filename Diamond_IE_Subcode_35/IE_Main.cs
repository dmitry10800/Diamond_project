namespace Diamond_IE_Subcode_35
{
    class IE_Main
    {

        static void Main(string[] args)
        {
            var path = @"C:\Work\IE\IE_20200401_2408";

            var process = new Process();

            var records = process.Start(path);

            var convertedRecords = DiamondConverter.Sub35Convertor(records);

            process.SendToDiamond(convertedRecords);

        }
    }
}
