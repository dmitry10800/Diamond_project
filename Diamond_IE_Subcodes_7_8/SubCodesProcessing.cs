using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Diamond_IE_Subcodes_7_8
{
    class SubCodesProcessing
    {
        public static List<SubCode7_8> SubCodes7_8_Processing(List<string> elements, string nameGazette)
        {
            List<SubCode7_8> elementsOut = new List<SubCode7_8>();

            if (elements != null && elements.Count > 0)
            {
                string recordValues = "";
                foreach (var element in elements)
                {
                    var currentElement = new SubCode7_8();
                    elementsOut.Add(currentElement);

                    recordValues = element.Replace("\n", " ").Trim();

                    var tempRecord = Regex.Match(recordValues,
                        @"(?<PublNumber>^S?\d{5}\b)\s*Int\.\s*Cl\.\s*\((?<IPCVersion>\d{4}\.\d{2})\)\s*(?<IPCClass>[^\.]+)\.\s*(?<Title>[^\.]+)\.\s*(?<NameGranAssigOwner>.*)");

                    string PubNumber = "", IPCVersion = "", IPCClassification = "", Title = "", NameAssignOwnGrant = "", EventDate = "";

                    if (tempRecord.Success)
                    {
                        PubNumber = tempRecord.Groups["PublNumber"].Value;
                        IPCVersion = tempRecord.Groups["IPCVersion"].Value;
                        IPCClassification = tempRecord.Groups["IPCClass"].Value;
                        Title = tempRecord.Groups["Title"].Value;
                        NameAssignOwnGrant = tempRecord.Groups["NameGranAssigOwner"].Value;
                    }

                    currentElement.LegalEventDate = Methods.GetDateFromNameGazette(nameGazette);
                    currentElement.PublicationNumber = PubNumber;
                    currentElement.IpcClassifications =
                        Methods.IPCClassificationsNormalize(IPCVersion, IPCClassification);
                    currentElement.Title = Title;
                    currentElement.Grantee_Assignee_OwnerInformation = Methods.AgentNameNormalize(NameAssignOwnGrant.TrimEnd('.').Trim());
                }
            }

            return elementsOut;
        }
    }
}
