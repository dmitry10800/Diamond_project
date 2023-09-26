namespace Diamond_VE_Maksim_Excel
{
    internal class Program
    {
        private const string _path = @"D:\LENS\TET\VE\VE_20230822_623\26";
        private const string _subCode = "26";
        private const bool _sendToProd = false; // true - send to Prod ; false - send to Stag
        static void Main(string[] args)
        {
            var methods = new Methods();

            var convertedPatents = _subCode switch
            {
                "26" => methods.Start(_path, _subCode),
                "64" => methods.Start(_path, _subCode),
                _ => null
            };

            Console.WriteLine();

            if (convertedPatents != null) methods.SendToDiamond(convertedPatents, _sendToProd);
        }
    }
}