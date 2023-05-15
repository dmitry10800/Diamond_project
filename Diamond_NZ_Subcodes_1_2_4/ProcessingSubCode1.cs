using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_NZ_Subcodes_1_2_4
{
    public class ProcessingSubCode1
    {
        private static readonly string AGENT_INF = "Agent changed from:";
        private static readonly string APPLICANT_INF = "Assigned from:";
        private static readonly string SERVICES_CHANGE = "Address for services changed from:";
        private static readonly string PATENT_ASSIGN = "Patent Assignment";
        public static bool corruptedRecord = false;
        public static List<string> CorruptedPatents = new List<string>();

        List<string> allNeedElementsSubCode1 = new List<string>();

        public static List<SubCode1> ProcessSubCode1(List<XElement> elements, DirectoryInfo pathToTetml)
        {
            var outListSubCode1 = new List<SubCode1>();
            if (elements != null && elements.Count > 0)
            {
                string[] splittedValues = null;
                SubCode1 currentElement;
                var tempInk = 0;
                for (var i = 0; i < elements.Count; i++)
                {
                    var recordValues = "";
                    var value = elements[i].Value;
                    tempInk = i;

                    if (value.StartsWith("Patent\nAssignment"))
                    {
                        corruptedRecord = false;
                        currentElement = new SubCode1();
                        do
                        {
                            recordValues += elements[tempInk].Value + "\n";
                            tempInk++;
                        } while (tempInk < elements.Count && !elements[tempInk].Value.StartsWith("Patent\nAssignment"));

                        if (!string.IsNullOrEmpty(recordValues))
                        {
                            splittedValues = SplitSubCode1(recordValues, new[] { AGENT_INF, APPLICANT_INF, SERVICES_CHANGE });
                        }

                        if (splittedValues != null)
                        {

                            foreach (var record in splittedValues)
                            {
                                if (record.StartsWith(AGENT_INF))
                                {
                                    var result = Methods.AgentInformationNormalize(record.Replace(AGENT_INF, "").Trim());
                                    if (result.Item1 != null && result.Item2 != null)
                                    {
                                        currentElement.AgentInformation = result.Item1;
                                        currentElement.AgentInformationNew = result.Item2;
                                    }
                                    else
                                    {
                                        Console.WriteLine(
                                            "Запись разорвана не несколько страниц и предрставлена не полная информация: " +
                                            currentElement.PublicationNumber + " Необходимо обработать ее вручную");
                                        corruptedRecord = true;
                                        if (!CorruptedPatents.Contains(currentElement.PublicationNumber))
                                            CorruptedPatents.Add(currentElement.PublicationNumber);
                                        break;
                                    }
                                }

                                if (record.StartsWith(APPLICANT_INF))
                                {
                                    var result =
                                        Methods.ApplicantInformationNormalize(record.Replace(APPLICANT_INF, "")
                                            .Trim());
                                    if (result.Item1 != null && result.Item2 != null)
                                    {

                                        currentElement.ApplicantInformation = result.Item1;
                                        currentElement.ApplicantInformationNew = result.Item2;
                                    }
                                    else
                                    {
                                        Console.WriteLine(
                                            "Запись разорвана не несколько страниц и предрставлена не полная информация: " +
                                            currentElement.PublicationNumber + " Необходимо обработать ее вручную");
                                        corruptedRecord = true;
                                        if (!CorruptedPatents.Contains(currentElement.PublicationNumber))
                                            CorruptedPatents.Add(currentElement.PublicationNumber);
                                        break;
                                    }
                                }

                                if (Regex.IsMatch(record, @"\d{2}\s+\w+\s+\d{4}"))
                                {
                                    //обработка даты
                                    currentElement.LegalEventDate = Methods.DateNormalize(record);
                                }

                                if (record.StartsWith(PATENT_ASSIGN))
                                {
                                    var temp = record.Replace(PATENT_ASSIGN, "").Trim();
                                    var reg = new Regex(@"^\d{6}\b");
                                    var matches = reg.Matches(temp);

                                    if (matches.Count > 0)
                                    {
                                        currentElement.PublicationNumber = matches[0].Value;
                                    }
                                }
                            }
                        }
                        if (!corruptedRecord)
                            outListSubCode1.Add(currentElement);
                    }
                }
            }
            OutCorruptedPatents.ErrorsToFile(CorruptedPatents, pathToTetml);
            return outListSubCode1;
        }

        private static string[] SplitSubCode1(string s, string[] parametrs)
        {
            string[] splittedRecords;
            var tempStrC = s;

            tempStrC = tempStrC.Replace("\n", " ").Trim();

            foreach (var parametr in parametrs)
            {
                tempStrC = Regex.Replace(tempStrC, parametr, "***" + parametr);
            }

            var regex = new Regex(@"\d{2}\s+[A-Z]{1}[a-z]{1,9}\s+\d{4}");
            var matches = regex.Matches(tempStrC);

            if (matches.Count > 0)
                tempStrC = tempStrC.Replace(matches[0].Value, "***" + matches[0].Value);

            splittedRecords = tempStrC.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            return splittedRecords;
        }
    }
}
