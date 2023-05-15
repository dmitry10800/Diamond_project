using System.Collections.Generic;
using System.Linq;

namespace Diamond_ES
{
    public class SubCode11Processing
    {
        private static readonly string PUBL_NUMBER = "11 ";
        private static readonly string APPL_NUMBER = "21 ";
        private static readonly string APPL_DATE = "22 ";
        private static readonly string TITLE = "54 ";
        private static readonly string GRANTEE_ASSIG_OWNER = "73 ";
        private static readonly string AGENT_INF = "74 ";
        private static readonly string EVENT_DATE = "Fecha de incorporación al dominio público:";
        private static readonly string EVENT_NOTE = "Motivo de caducidad:";

        public static List<SubCode11> ProcessSubCode11(List<XElement> elements)
        {
            var ElementsOut = new List<SubCode11>();
            List<XElement> finalListSubCode11Elements = new List<XElement>();

            if (elements != null && elements.Count > 0)
            {
                string[] splittedRecord = null;
                var recordValues = "";
                int tmpInc;
                var statusSearchingStart = false;

                int startSubCode11 = -1, endSubCode11 = -1;

                for (var i = 0; i < elements.Count; i++)
                {
                    var value = elements[i].Value;

                    if (value.StartsWith(@"CADUCIDADES"))
                        startSubCode11 = i;

                    if (startSubCode11 > -1)
                    {
                        endSubCode11 = elements.Count - 1;
                        break;
                    }
                }

                if (startSubCode11 > -1 && endSubCode11 > -1)
                {
                    for (var i = startSubCode11; i < endSubCode11; i++)
                    {
                        finalListSubCode11Elements.Add(elements[i]);
                    }
                }

                int tempInc;

                for (var i = 0; i < finalListSubCode11Elements.Count; i++)
                {
                    var value = finalListSubCode11Elements[i].Value;

                    if (value.StartsWith(PUBL_NUMBER))
                    {
                        var currentElement = new SubCode11();
                        ElementsOut.Add(currentElement);
                        recordValues = "";
                        tempInc = i;

                        do
                        {
                            recordValues += finalListSubCode11Elements[tempInc].Value + "\n";
                            ++tempInc;

                        } while (tempInc < finalListSubCode11Elements.Count() && !finalListSubCode11Elements[tempInc].Value.StartsWith(PUBL_NUMBER));

                        if (recordValues != null)
                        {
                            splittedRecord = Methods.RecSplitSubCode11(recordValues, new string[] { PUBL_NUMBER, APPL_NUMBER, APPL_DATE, TITLE, GRANTEE_ASSIG_OWNER, AGENT_INF, EVENT_DATE, EVENT_NOTE });
                        }

                        foreach (var record in splittedRecord)
                        {
                            if (record.StartsWith(PUBL_NUMBER))
                            {
                                var result = Methods.PublicationNumberNormalize(record.Replace(PUBL_NUMBER, "").Trim());
                                if (result != null)
                                {
                                    currentElement.PublicationNumber = result.PublicationNumber;
                                    currentElement.PublicationKind = result.PublicationKind;
                                }
                                else
                                {

                                }

                            }

                            if (record.StartsWith(APPL_NUMBER))
                            {
                                currentElement.ApplicationNumber = Methods.ApplicationNumberNormalize(record.Replace(APPL_NUMBER, "")).Trim();
                            }

                            if (record.StartsWith(APPL_DATE))
                            {
                                currentElement.ApplicationDate = Methods.DateNormalize(record.Replace(APPL_DATE, "").Trim());
                            }

                            if (record.StartsWith(TITLE))
                            {
                                currentElement.TitleText = record.Replace(TITLE, "").Trim();
                            }

                            if (record.StartsWith(GRANTEE_ASSIG_OWNER))
                            {
                                currentElement.GranteeAssigneeOwnerInformation =
                                    Methods.GranteeAssigneeOwnerInformationNornalize(record.Replace(GRANTEE_ASSIG_OWNER, "").Trim());
                            }

                            if (record.StartsWith(EVENT_DATE))
                            {
                                currentElement.LegalStatusEvents_EventDate = Methods.DateNormalize(record.Replace(EVENT_DATE, "").Trim());
                            }

                            if (record.StartsWith(EVENT_NOTE))
                            {
                                currentElement.LegalStatusEvents_Note = Methods.NormalizeLegalNote(record);
                            }

                            if (record.StartsWith(AGENT_INF))
                            {
                                currentElement.AgentName =
                                    Methods.AgentNameNormalize(record.Replace(AGENT_INF, "").Trim());
                            }
                        }
                    }
                }
            }


            return ElementsOut;
        }
    }
}
