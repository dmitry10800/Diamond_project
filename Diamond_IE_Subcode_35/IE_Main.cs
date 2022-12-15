using System.Collections.Generic;

namespace Diamond_IE_Subcode_35
{
    class IE_Main
    {

        static void Main(string[] args)
        {
            string path = @"C:\Work\IE\IE_20200401_2408";

            Process process = new Process();

            List<Record> records = process.Start(path);

            List<Diamond.Core.Models.LegalStatusEvent> convertedRecords = DiamondConverter.Sub35Convertor(records);

            process.SendToDiamond(convertedRecords);

        }
    }
}
