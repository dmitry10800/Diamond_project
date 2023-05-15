using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_ES
{
    public class SubCode12Processing
    {
        private static readonly string PUBL_NUMBER = "11 ";
        private static readonly string APPL_NUMBER = "21 ";
        private static readonly string APPL_DATE = "22 ";
        private static readonly string TITLE = "54 ";
        private static readonly string GRANTEE_ASSIG_OWNER = "73 ";
        private static readonly string AGENT_INF = "74 ";
        private static readonly string EVENT_DATE = "Fecha de incorporación al dominio público:";
        private static readonly string EVENT_NOTE = "Motivo de caducidad:";

        public static List<SubCode12> ProcessSubCode12(List<XElement> elements)
        {
            var ElementsOut = new List<SubCode12>();
            var finalListSubCode12Elements = new List<XElement>();

            if (elements != null && elements.Count > 0)
            {
                string[] splittedRecord = null;
                var recordValues = "";
                int tmpInc;
                var statusSearchingStart = false;

                int startSubCode12 = -1, endSubCode12 = -1;

                for (var i = 0; i < elements.Count; i++)
                {
                    var value = elements[i].Value;

                    if (value.StartsWith(@"CADUCIDAD"))
                        startSubCode12 = i;

                    if (startSubCode12 > -1)
                    {
                        endSubCode12 = elements.Count - 1;
                        break;
                    }
                }

                if (startSubCode12 > -1 && endSubCode12 > -1)
                {
                    for (var i = startSubCode12; i <= endSubCode12; i++)
                    {
                        finalListSubCode12Elements.Add(elements[i]);
                    }
                }

                int tempInc;

                for (var i = 0; i < finalListSubCode12Elements.Count; i++)
                {
                    var value = finalListSubCode12Elements[i].Value;

                    if (value.StartsWith(PUBL_NUMBER))
                    {
                        var currentElement = new SubCode12();
                        ElementsOut.Add(currentElement);
                        recordValues = "";
                        tempInc = i;

                        do
                        {
                            recordValues += finalListSubCode12Elements[tempInc].Value + "\n";
                            ++tempInc;

                        } while (tempInc < finalListSubCode12Elements.Count() &&
                                 !finalListSubCode12Elements[tempInc].Value.StartsWith(PUBL_NUMBER));

                        if (recordValues != null)
                        {
                            splittedRecord = Methods.RecSplit(recordValues,
                                new string[]
                                {
                                    PUBL_NUMBER, APPL_NUMBER, APPL_DATE, TITLE, GRANTEE_ASSIG_OWNER, AGENT_INF,
                                    EVENT_DATE, EVENT_NOTE
                                });
                        }

                        foreach (var record in splittedRecord)
                        {
                            if (record.StartsWith(PUBL_NUMBER))
                            {
                                var result = Methods.PublicationNumberNormalize(record.Replace(PUBL_NUMBER, "").Trim());
                                currentElement.PublicationNumber = result.PublicationNumber;
                                currentElement.PublicationKind = result.PublicationKind;
                            }

                            if (record.StartsWith(APPL_NUMBER))
                            {
                                currentElement.ApplicationNumber = Methods
                                    .ApplicationNumberNormalize(record.Replace(APPL_NUMBER, "")).Trim();
                            }

                            if (record.StartsWith(APPL_DATE))
                            {
                                currentElement.ApplicationDate =
                                    Methods.DateNormalize(record.Replace(APPL_DATE, "").Trim());
                            }

                            if (record.StartsWith(TITLE))
                            {
                                currentElement.TitleText = record.Replace(TITLE, "").Trim();
                            }

                            if (record.StartsWith(GRANTEE_ASSIG_OWNER))
                            {
                                currentElement.GranteeAssigneeOwnerInformation =
                                    Methods.GranteeAssigneeOwnerInformationNornalize(
                                        record.Replace(GRANTEE_ASSIG_OWNER, "").Trim());
                            }

                            if (record.StartsWith(AGENT_INF))
                            {
                                currentElement.AgentName = Methods.AgentNameNormalize(record.Replace(AGENT_INF, "").Trim());
                            }

                            if (record.StartsWith(EVENT_DATE))
                            {
                                currentElement.LegalStatusEvents_EventDate =
                                    Methods.DateNormalize(record.Replace(EVENT_DATE, "").Trim());
                            }

                            if (record.StartsWith(EVENT_NOTE))
                            {
                                currentElement.LegalStatusEvents_Note = Methods.NormalizeLegalNote(record);
                            }
                        }
                    }

                }
            }
            return ElementsOut;
        }
    }
}
