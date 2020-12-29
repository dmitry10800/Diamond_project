using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Lens_AR_Subcodes_2_3
{
    public class ProcessSubcode2
    {
        public static (List<Subcodes2_3>, List<Subcodes2_3>, List<string>) ProcessingSubCodes2_3(List<string> elements, string nameGazette)
        {
            List<Subcodes2_3> outSubcode2 = new List<Subcodes2_3>();
            List<Subcodes2_3> outSubcode3 = new List<Subcodes2_3>();
            List<string> ErrorsList = new List<string>();
            Subcodes2_3 currentElement;

            string fullStrWithPatents = "";
            if (elements.Count > 0)
            {
                int tempInk;
                for (int i = 0; i < elements.Count; i++)
                {
                    string recordValues = "";
                    var value = elements[i];
                    tempInk = i;

                    if (Regex.IsMatch(value, @"<Primera>"))
                    {
                        do
                        {
                            recordValues += elements[tempInk] + "\n";
                            tempInk++;

                        } while (tempInk < elements.Count && !elements[tempInk].StartsWith("<Primera>"));
                    }

                    if (!string.IsNullOrEmpty(recordValues))
                    {
                        var splittedRecord = Methods.RecSplit(recordValues);

                        if (splittedRecord.Length > 0)
                        {
                            currentElement = new Subcodes2_3();
                            currentElement.LegalStatusEvents = new List<LegalStatusEvents>();
                            foreach (var record in splittedRecord)
                            {
                                if (record.StartsWith(@"(10)"))
                                {
                                    currentElement.PlainLanguageDesignation = record.Replace(@"(10)", "").Trim();
                                }

                                if (record.StartsWith(@"(11)"))
                                {
                                    var applicInfo = Regex.Match(record.Replace(@"(11)", "".Trim()), @"Resolución Administrativa Nº\s*(?<number>[A-Z]{2}\d+)\s*(?<kind>[A-Z]\d*)");
                                    currentElement.PublicationNumber = applicInfo.Groups["number"].Value;
                                    currentElement.PublicationKind = applicInfo.Groups["kind"].Value;
                                }

                                if (record.StartsWith(@"(21)"))
                                {
                                    currentElement.ApplicationNumber = record.Replace(@"(21) Acta Nº", "").Trim();
                                }

                                if (record.StartsWith(@"(22)"))
                                {
                                    currentElement.ApplicationDate = Methods.DateNormalize(record.Replace(@"(22) Fecha de Presentación", "").Trim());
                                }

                                if (record.StartsWith(@"(24)"))
                                {
                                    currentElement.EffectiveDate = Methods.DateNormalize(record.Replace(@"(24) Fecha de Resolución", "").Trim());
                                }

                                if (record.StartsWith(@"(--)"))
                                {
                                    var date = Methods.DateNormalize(record.Replace("(--) Fecha de Vencimiento", "").Trim());
                                    currentElement.LegalStatusEvents.Add(new LegalStatusEvents
                                    {
                                        Language = "ES",
                                        Note = $"|| Fecha de Vencimiento | {date}",
                                        NextLanguageField = new NextLanguageField
                                        {
                                            Language = "EN",
                                            Note = $"|| Expiration date | {date}"
                                        }
                                    });
                                }

                                if (record.StartsWith(@"(30)"))
                                {
                                    currentElement.PriorityInformation = Methods.GetNormalizedPriorities(record.Replace(@"(30) Prioridad convenio de Paris", "").Trim());
                                }

                                if (record.StartsWith(@"(47)"))
                                {
                                    currentElement.Date_Field47 = Methods.DateNormalize(record.Replace("(47) Fecha de Puesta a Disposición", "").Trim());
                                }

                                if (record.StartsWith(@"(45)"))
                                {
                                    currentElement.Date_Field45 = Methods.DateNormalize(record.Replace("(45) Fecha de Publicación", "").Trim());
                                }

                                if (record.StartsWith(@"(51)"))
                                {
                                    var results = Methods.NormalizedClassificationField(record.Replace(@"(51)", ""), currentElement?.PublicationNumber);
                                    currentElement.ClassificationInformation = results.Item1;
                                    ErrorsList = results.Item2;
                                }

                                if (record.StartsWith(@"(54)"))
                                {
                                    currentElement.Title = new TitleAbstaractInformation()
                                    {
                                        Language = "ES",
                                        Text = record.Replace("(54) Titulo -", "").Trim()
                                    };
                                }

                                if (record.StartsWith(@"(57)"))
                                {
                                    currentElement.Abstaract = new TitleAbstaractInformation
                                    {
                                        Language = "ES",
                                        Text = record.Replace("(57) REIVINDICACIÓN", "").Trim()
                                    };
                                }

                                if (record.StartsWith(@"(72)"))
                                {
                                    currentElement.InventorInformation = Methods.GetNormalizedInventorsInformation(record.Replace(@"(72) Inventor -", "").Trim());
                                }

                                if (record.StartsWith(@"(74)"))
                                {
                                    var list = Methods.GetNormolizedAgentInformation(record.Replace("(74) Agente/s", "").Trim());
                                    currentElement.AgentInformation = list;
                                    foreach (var personInformation in list)
                                    {
                                        currentElement.LegalStatusEvents.Add(new LegalStatusEvents
                                        {
                                            Language = "ES", 
                                            Note = $"(74) || Agente/s Nro. | {personInformation.Name}",
                                            NextLanguageField = new NextLanguageField
                                            {
                                                Language = "EN",
                                                Note = $"(74) || Agent/s number | {personInformation.Name}"
                                            }
                                        });
                                    }
                                }

                                if (Regex.IsMatch(record, @"Siguen \d+ Reivindicaciones", RegexOptions.IgnoreCase))
                                {
                                    var number = Regex.Match(record, @"\d+");
                                    currentElement.LegalStatusEvents.Add(new LegalStatusEvents
                                    {
                                        Language = "ES",
                                        Note = $"|| REIVINDICACIONES | Siguen {number} Reivindicaciones",
                                        NextLanguageField = new NextLanguageField
                                        {
                                            Language = "EN",
                                            Note = $"|| Claims | {number} Claims follow"
                                        }
                                    });
                                }
                            }

                            if (Regex.IsMatch(currentElement.PlainLanguageDesignation, @"Patente de Invención", RegexOptions.IgnoreCase))
                                outSubcode2.Add(currentElement);
                            if (Regex.IsMatch(currentElement.PlainLanguageDesignation, @"Modelo de Utilidad", RegexOptions.IgnoreCase))
                                outSubcode3.Add(currentElement);
                            Console.WriteLine($"Processed patent {currentElement.PublicationNumber}");
                        }
                    }
                }
            }

            return (outSubcode2, outSubcode3,ErrorsList);
        }
    }
}
