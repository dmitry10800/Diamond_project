using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_ES
{
    class SubCode3Processing
    {
        private static readonly string PUBL_NUMBER = "11 ";
        private static readonly string APPL_NUMBER = "21 ";
        private static readonly string APPL_DATE = "22 ";
        private static readonly string TITLE = "54 ";
        private static readonly string RELATED_PUB_INF = "61 ";
        private static readonly string GRANTEE_ASSIG_OWNER = "73 ";
        private static readonly string AGENT_INF = "74 ";
        private static readonly string EVENT_DATE = "Fecha de incorporación al dominio público:";
        private static readonly string EVENT_NOTE = "Motivo de caducidad:";


        public static List<SubCode3> ProcessSubCode3(List<XElement> elements)
        {
            var ElementsOut = new List<SubCode3>();
            var finalListSubCode3Elements = new List<XElement>();

            if (elements != null && elements.Count > 0)
            {
                string[] splittedRecord = null;
                var recordValues = "";
                int tmpInc;
                var statusSearchingStart = false;

                int startSubCode3 = -1, endSubCode3 = -1;
                var endSubCodeIndexes = new List<int>();
                var flag1 = false;
                var flag2 = false;
                var flag3 = false;
                var flag4 = false;
                var flag5 = false;
                for (var i = 0; i < elements.Count; i++)
                {
                    var value = elements[i].Value;
                    if (value.StartsWith(@"CADUCIDAD (ART. 116 LP)"))
                        startSubCode3 = i;

                    if (value.StartsWith("CADUCIDAD") && startSubCode3 == -1)
                        startSubCode3 = i;



                    if (value.StartsWith("CAMBIO DE MODALIDAD"))
                    {
                        if (!flag1)
                        {
                            endSubCodeIndexes.Add(i);
                            flag1 = true;

                        }
                    }

                    if (value.StartsWith("CONCESIÓN"))
                    {
                        if (!flag2)
                        {
                            endSubCodeIndexes.Add(i);
                            flag2 = true;
                        }

                    }

                    if (value.StartsWith("RETIRADA"))
                    {
                        if (!flag3)
                        {
                            endSubCodeIndexes.Add(i);
                            flag3 = true;
                        }
                    }

                    if (value.StartsWith("TRAMITACIÓN"))
                    {
                        if (!flag4)
                        {
                            endSubCodeIndexes.Add(i);
                            flag4 = true;
                        }
                    }

                    /*if (value.StartsWith("MODELOS DE"))
                    {
                        if (!flag5)
                        {
                            endSubCodeIndexes.Add(i);
                            flag5 = true;
                        }
                    }*/


                }

                var sortedList = SortIndexes(endSubCodeIndexes);

                foreach (var item in sortedList)
                {
                    if (item > startSubCode3)
                    {
                        endSubCode3 = item;
                        break;
                    }

                }

                if (startSubCode3 > -1 && endSubCode3 == -1)
                {
                    Console.WriteLine("Берем 3 Subcode до конца 1 главы, проверить наличие других подглав для этой газеты...");
                    endSubCode3 = elements.Count;
                }

                if (startSubCode3 > -1 && endSubCode3 > -1)
                {
                    for (var i = startSubCode3; i < endSubCode3; i++)
                    {
                        finalListSubCode3Elements.Add(elements[i]);
                    }
                }

                int tempInc;

                for (var i = 0; i < finalListSubCode3Elements.Count; i++)
                {
                    var value = finalListSubCode3Elements[i].Value;

                    if (value.StartsWith("11 "))
                    {
                        var currentElement = new SubCode3();
                        ElementsOut.Add(currentElement);
                        recordValues = "";
                        tempInc = i;

                        do
                        {
                            recordValues += finalListSubCode3Elements[tempInc].Value + "\n";
                            ++tempInc;

                        } while (tempInc < finalListSubCode3Elements.Count() && !finalListSubCode3Elements[tempInc].Value.StartsWith("11 "));

                        if (recordValues != null)
                        {
                            splittedRecord = Methods.RecSplit(recordValues, new string[] { "11 ES", "21 ", "22 ", "54 ", "61 ", "73 ", "74 ", "Fecha de incorporación al dominio público:", "Motivo de caducidad:" });
                        }

                        foreach (var record in splittedRecord)
                        {
                            if (record.StartsWith(PUBL_NUMBER))
                            {
                                var result = Methods.PublicationNumberNormalize(record.Replace(PUBL_NUMBER, "").Trim());
                                currentElement.PublicationNumber = result.PublicationNumber.Replace("\n", " ").Trim();
                                currentElement.PublicationKind = result.PublicationKind.Replace("\n", " ").Trim();
                            }

                            if (record.StartsWith(APPL_NUMBER))
                            {
                                currentElement.ApplicationNumber = Methods.ApplicationNumberNormalize(record.Replace(APPL_NUMBER, "")).Replace("\n", " ").Trim();
                            }

                            if (record.StartsWith(APPL_DATE))
                            {
                                currentElement.ApplicationDate = Methods.DateNormalize(record.Replace(APPL_DATE, "").Replace("\n", " ").Trim());
                            }

                            if (record.StartsWith(TITLE))
                            {
                                currentElement.TitleText = record.Replace(TITLE, "").Replace("\n", " ").Trim();
                            }

                            if (record.StartsWith(GRANTEE_ASSIG_OWNER))
                            {
                                currentElement.GranteeAssigneeOwnerInformation =
                                    Methods.GranteeAssigneeOwnerInformationNornalize(record.Replace(GRANTEE_ASSIG_OWNER, "").Replace("\n", " ").Trim());
                            }

                            if (record.StartsWith(AGENT_INF))
                            {
                                currentElement.AgentName = Methods.AgentNameNormalize(record.Replace(AGENT_INF, "").Replace("\n", " ").Trim());
                            }

                            if (record.StartsWith(EVENT_DATE))
                            {
                                currentElement.LegalStatusEvents_EventDate = Methods.DateNormalize(record.Replace(EVENT_DATE, "").Replace("\n", " ").Trim());
                            }

                            if (record.StartsWith(EVENT_NOTE))
                            {
                                currentElement.LegalStatusEvents_Note = Methods.NormalizeLegalNote(record);
                            }

                            if (record.StartsWith(RELATED_PUB_INF))
                            {
                                var outList = new List<RelatedPublicationInformation>();
                                var related = new RelatedPublicationInformation();
                                var result = Methods.RelatedPublicationInformationNormalize(record.Trim());

                                related.Number = result[0].Number;
                                related.InidNumber = result[0].InidNumber;
                                related.Date = Methods.DateNormalize(result[0].Date);
                                outList.Add(related);
                                currentElement.RelatedPublicationInformation = outList;
                            }
                        }

                    }
                }

            }

            return ElementsOut;
        }

        static List<int> SortIndexes(List<int> list)
        {
            var sortedList = list.OrderBy(i => i).ToList();
            return sortedList;
        }
    }
}
