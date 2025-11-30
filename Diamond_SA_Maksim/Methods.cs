using Diamond.Core.Models;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_SA_Maksim;

public class Methods
{
    private string _currentFileName;
    private int _id = 1;

    internal List<LegalStatusEvent> Start(string path, string subCode)
    {
        var statusEvents = new List<LegalStatusEvent>();
        var directory = new DirectoryInfo(path);
        var files = directory.GetFiles("*.tetml", SearchOption.AllDirectories).Select(file => file.FullName).ToList();

        XElement tet;
        var xElements = new List<XElement>();

        foreach (var tetml in files)
        {
            _currentFileName = tetml;
            tet = XElement.Load(tetml);

            if (subCode == "1")
            {
                xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                    .SkipWhile(val => !val.Value.StartsWith(""))
                    .TakeWhile(val => !val.Value.StartsWith(""))
                    .ToList();

                //var text = MakeText(xElements, subCode);

                //var notes = Regex.Split(text, @"(?=\(11\).+)", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(11)")).ToList();

                //foreach (var note in notes)
                //{
                //    statusEvents.Add();
                //}
            }
        }
        return statusEvents;
    }
}