namespace Diamond_IN
{
    class IN_main
    {
        public static string CurrentFileName;

        private const string FolderPath = @"C:\Work\IN\IN_20210101_01(1)";
        private const bool SendToProd = false;

        private static void Main(string[] args)
        {
            var FerFilesGet = new List<string>(Directory.GetFiles(FolderPath, "*_TableFER.txt")); //Copy/Paste text from original pdf to txt file that ends with _TableFER.txt

            if (FerFilesGet.Count > 0)
            {
                foreach (var file in FerFilesGet)
                {
                    CurrentFileName = file.Remove(file.IndexOf("_TableFER.txt")) + ".pdf";
                    var tableFebData = new ProcessFebTableData();
                    var el = tableFebData.OutputValue(file);
                    var legalStatusEvents = ConvertToDiamond.FerTableConvertation(el);
                    try
                    {
                        DiamondUtilities.DiamondSender.SendToDiamond(legalStatusEvents, SendToProd);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Sending error");
                        throw;
                    }
                }
            }
        }
    }
}
