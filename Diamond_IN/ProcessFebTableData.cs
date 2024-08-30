using System.Text.RegularExpressions;

namespace Diamond_IN
{
    internal class ProcessFebTableData
    {
        public class ElementsForOutput
        {
            public class AgentStruct
            {
                public string Name { get; set; }
                public string Address { get; set; }
                public string Country { get; set; }
            }
            public string PageNumber { get; set; }
            public string LocationName { get; set; }
            public string AppNumber { get; set; }
            public string FerDate { get; set; }

            public AgentStruct agent = new AgentStruct();
            public string Email { get; set; }
        }
        public List<ElementsForOutput> OutputValue(string file)
        {
            var ListOfElements = new List<ElementsForOutput>();
            var fileData = Methods.ClearString(File.ReadAllLines(file));
            ElementsForOutput output = null;
            if (fileData.Count() > 0)
            {
                for (var i = 0; i < fileData.Count(); i++)
                {
                    if (Regex.IsMatch(fileData[i], @"^(\d{3}|\d{2}|\d{1})$"))
                    {
                        output = new ElementsForOutput();
                        output.PageNumber = fileData[i];
                        output.LocationName = fileData[i + 1];
                        output.AppNumber = fileData[i + 2];
                        output.FerDate = Methods.DateProcess(fileData[i + 3]);
                        var agentInfo = Methods.AgentInfoSplit(fileData[i + 4]);
                        if (agentInfo != null)
                        {
                            output.agent = agentInfo;
                        }
                        output.Email = fileData[i + 5];
                        if (i + 6 < fileData.Count() && !Regex.IsMatch(fileData[i + 6], @"^(\d{3}|\d{2}|\d{1})$"))
                        {
                            output.Email = output.Email + " " + fileData[i + 6];
                            i += 6;
                        }
                        else i += 5;
                    }
                    if (output != null) ListOfElements.Add(output);
                }
            }
            return ListOfElements;
        }
    }
}
