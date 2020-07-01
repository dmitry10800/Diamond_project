using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_AR_Subcodes_2_3
{
    class Processing
    {
        public const string PubNumber = "(11)";
        public const string AppNumber = "(21)";
        public const string AppDate = "(22)";
        public const string Priority = "(30)";
        public const string Ipc = "(51)";
        public const string Title = "(54)";
        public const string Abstract = "(57)";
        public const string Applicant = "(71)";
        public const string Inventor = "(72)";
        public const string Agent = "(74)";
        public const string Date41 = "(41)";
        public const string Date45 = "(45)";
        public const string Date47 = "(47)";
        public const string EffectiveDate = "(24)";
        public const string ExpirationDate = "(--)";
        public const string Start = "<Primera>";
        public static Regex Field10 = new Regex(@"\(10\)\s*(Modelo de Utilidad|Patente de Invención)");
        public static int id;

        public static List<Elements> Sub2(List<XElement> elements)
        {
            List<Elements> elementsOut = new List<Elements>();

            for (int i = 0; i < elements.Count; i++)
            {
                int inc = i;
                string val = null;

                if (i == 148)
                {

                }

                var value = elements[i].Value;
                if (Field10.Match(value).Success)
                {
                    var curElem = new Elements();
                    do
                    {
                        val += elements[inc].Value + "\n";
                        ++inc;
                    } while (inc < elements.Count && !Field10.Match(elements[inc].Value).Success);

                    if (val.Contains("AR088825"))
                    {

                    }

                    if (val.Contains("AR054612B1"))
                    {

                    }

                    if (i == 3)
                    {

                    }

                    var indexOfAbs = val.IndexOf("(57)");
                    var indexOfApp = val.IndexOf("(71)");
                    var @abstract = val.Substring(indexOfAbs, indexOfApp - indexOfAbs);
                    var agents = "";
                    val = val.Replace(@abstract, "");
                    var indexOfAgents = val.IndexOf("(74)");
                    if (indexOfAgents > -1)
                    {
                        agents = val.Substring(indexOfAgents);
                        agents = agents.Substring(0, agents.IndexOf('\n'));
                        val = val.Replace(agents, "");
                    }



                    var splittedRecords = Methods.RecSplit(val, new string[] { Field10.Match(val).Value, PubNumber, AppNumber, AppDate, Priority, Ipc, Title, Applicant, Inventor, Agent, Date41, Date45, Date47, EffectiveDate, ExpirationDate });

                    splittedRecords.Add(@abstract);

                    if (indexOfAgents > -1)
                    {
                        splittedRecords.Add(agents);
                    }


                    foreach (var record in splittedRecords)
                    {
                        if (Field10.Match(record).Success && record.Contains("Patente de Invención"))
                            curElem.Is2 = true;

                        if (record.StartsWith(PubNumber))
                        {
                            var number = record.Split(' ').Last();
                            var kind = number.Substring(number.Trim().Length - 2).Trim();
                            curElem.PubNumber = number.Replace(kind, "").Trim();
                            curElem.Kind = kind;
                        }

                        if (record.StartsWith(AppNumber))
                            curElem.AppNumber = record.Replace(AppNumber, "").Replace("Acta Nº", "").Replace("Acta N°", "").Trim();

                        if (record.StartsWith(AppDate))
                            curElem.AppDate = Methods.DateNormalize(record.Replace(AppDate, "").Replace("Fecha de Presentación", "").Trim());

                        if (record.StartsWith(Abstract))
                        {
                            var matchNote = Regex.Match(record, @"Siguen\s*\d+\s*Reivindicaci(o|ó)n(es)?");
                            var abstractText = record.Replace(Abstract, "").Replace("REIVINDICACIÓN", "").Trim();
                            var reivindicacion = "";
                            var claim = "";
                            if (matchNote.Success)
                            {
                                abstractText = abstractText.Replace(matchNote.Value, "").Trim();
                                if (matchNote.Value.Contains("Reivindicaciónes") || matchNote.Value.Contains("Reivindicaciones"))
                                {
                                    reivindicacion = "REIVINDICACIONES";
                                    claim = "Claims";
                                }
                                else
                                {
                                    reivindicacion = "REIVINDICACION";
                                    claim = "Claim";
                                }

                                var number = Regex.Match(matchNote.Value, @"\d+");

                                curElem.Note = curElem.Note + $" || {reivindicacion} | {matchNote.Value}";
                                curElem.Tranlation = curElem.Tranlation + $" || {claim} | {number.Value} {claim} follow";
                            }
                            curElem.Abstract = abstractText;
                        }

                        if (record.StartsWith(EffectiveDate))
                            curElem.EffectiveDate = Methods.DateNormalize(record.Replace(EffectiveDate, "").Replace("Fecha de Resolución", "").Trim());

                        if (record.StartsWith(ExpirationDate))
                        {
                            var date = Methods.DateNormalize(record.Replace(ExpirationDate, "").Replace("Fecha de Vencimiento", "").Trim());
                            curElem.Note = $"|| Fecha de Vencimiento | {date}";
                            curElem.Tranlation = $"|| Expiration date | {date}";
                        }

                        if (record.StartsWith(Date41))
                            curElem.Date41 = Methods.DateNormalize(record.Replace(Date41, "").Trim());

                        if (record.StartsWith(Date45))
                            curElem.Date45 = Methods.DateNormalize(record.Replace(Date45, "").Replace("Fecha de Publicación", "").Trim());

                        if (record.StartsWith(Date47))
                            curElem.Date47 = Methods.DateNormalize(record.Replace(Date47, "").Replace("Fecha de Puesta a Disposición", "").Trim());

                        if (record.StartsWith(Title))
                            curElem.Title = record.Replace(Title, "").Replace("Titulo -", "").Trim();

                        if (record.StartsWith(Priority))
                        {
                            var text = record.Replace(Priority, "").Replace("Prioridad convenio de Paris", "").Replace("\n", " ").Trim();
                            var tmp = text.Split(';', StringSplitOptions.RemoveEmptyEntries);
                            curElem.Priorities = new List<Priority>();

                            foreach (var priority in tmp)
                            {
                                var pattern = Regex.Match(priority, @"(?<country>[A-Z]{2})\s*(?<number>.+)\s*(?<date>\d{2}\/\d{2}\/\d{4});?", RegexOptions.Singleline);

                                if (string.IsNullOrEmpty(pattern.Value))
                                {

                                }

                                curElem.Priorities.Add(
                                    new Priority
                                    {
                                        Country = pattern.Groups["country"].Value,
                                        Number = pattern.Groups["number"].Value,
                                        Date = Methods.DateNormalize(pattern.Groups["date"].Value.Replace(";", "").Trim())
                                    });
                            }
                        }

                        if (record.StartsWith(Ipc))
                        {
                            var text = record.Replace(Ipc, "").Replace("Int. Cl.", "").Replace("\n", " ").Trim();
                            var ipcs = text.Split(' ');
                            var matchDate = Regex.Match(text, @"\(.{0,5}\d{4}\.\d{2}.{0,5}\)");

                            if (matchDate.Success)
                                curElem.IpcVersion = (matchDate.Value + ".01").Replace(".", "-");

                            curElem.Ipcs = new List<Ipc>();
                            var LNNL = "";
                            var NNNNNN = "";
                            foreach (var ipc in ipcs)
                            {
                                var part = ipc.Replace(";", "").Replace(",", "").Replace("-", "");
                                var matchIpcWithSpace = Regex.Match(ipc, @"[A-Z]{1}\d{2}[A-Z]{1} \d{1,4}\/\d+");
                                var matchIpcWoSpace = Regex.Match(ipc, @"[A-Z]{1}\d{2}[A-Z]{1}\d{1,4}\/\d+");
                                var versionMatch = Regex.Match(ipc, @"\d{4}\.\d{2}");
                                var matchIpcFirstPart = Regex.Match(ipc, @"\p{Lu}{1}\d{2}\p{Lu}");
                                var matchIpcSecondPart = Regex.Match(ipc, @"\d{1,3}\/\d{1,3}");
                                var @class = "";
                                if (matchIpcWoSpace.Success)
                                    @class = matchIpcWoSpace.Value.Substring(0, 4) + " " + matchIpcWoSpace.Value.Substring(4);
                                else if (matchIpcWithSpace.Success)
                                    @class = matchIpcWithSpace.Value;
                                else if (matchIpcFirstPart.Success)
                                {
                                    LNNL = matchIpcFirstPart.Value;
                                }
                                else if (matchIpcSecondPart.Success)
                                {
                                    NNNNNN = matchIpcSecondPart.Value;
                                }

                                if (versionMatch.Success)
                                    curElem.IpcVersion = (versionMatch.Value + ".01").Replace(".", "-");

                                if (!string.IsNullOrEmpty(LNNL) && !string.IsNullOrEmpty(NNNNNN))
                                {
                                    @class = LNNL + " " + NNNNNN;
                                }

                                if (!string.IsNullOrEmpty(@class))
                                {
                                    curElem.Ipcs.Add(new Ipc
                                    {
                                        Classification = @class,
                                        Version = curElem.IpcVersion
                                    });
                                }
                            }
                        }

                        //if (record.StartsWith(Applicant))
                        //{
                        //    var text = record.Replace(Applicant, "").Replace("Titular -", "");
                        //    var indexLLC = text.IndexOf("LLC");
                        //    var indexINC = text.IndexOf("INC.");
                        //    curElem.Applicant = new List<Applicant>();

                        //    foreach(var country in curElem.Countries())
                        //    {
                        //        text = text.Replace($"{country}\n", $"{country}***");
                        //    }

                        //    var applicants = text.Split("***", StringSplitOptions.RemoveEmptyEntries);

                        //    foreach(var applicant in applicants)
                        //    {
                        //        var pattern = Regex.Match(applicant, @"(?<name>(\p{Lu}+\s*\/?)+(\n|\.))\s*(?<address>.+)", RegexOptions.Singleline);

                        //        var country = pattern.Value.Split(',').Last();
                        //        curElem.Applicant.Add(new AR.Applicant 
                        //        {
                        //            Name = pattern.Groups["name"].Value
                        //            .Replace("\n", " "),
                        //            Address = pattern.Groups["address"].Value
                        //            .Replace($",{country}", "")
                        //            .Replace("\n", " "),
                        //            Country = country.Trim()
                        //        });
                        //    }

                        //}

                        if (record.StartsWith(Inventor))
                        {
                            var text = record.Replace(Inventor, "").Replace(" Inventor -", "").Replace("\n", " ").Trim();
                            var inventors = text.Split('-');
                            curElem.Inventors = new List<Inventor>();
                            foreach (var inventor in inventors)
                            {
                                curElem.Inventors.Add(new Inventor
                                {
                                    Name = inventor.Trim()
                                });
                            }
                        }

                        if (record.StartsWith(Agent))
                        {
                            var splittedAgents = record.Replace(Agent, "").Replace("Agente/s", "").Trim().Split(',');
                            var numbers = "";
                            curElem.Agents = new List<Agent>();
                            for (int j = 0; j < splittedAgents.Count(); j++)
                            {
                                curElem.Agents.Add(new Agent
                                {
                                    Name = splittedAgents[j].Trim()
                                });

                                if (j + 1 == splittedAgents.Count())
                                    numbers = numbers + splittedAgents[0];
                                else
                                    numbers = numbers + splittedAgents[0] + ", ";
                            }

                            curElem.Note = curElem.Note + $" || (74) Agente/s Nro. | {numbers}";
                            curElem.Tranlation = curElem.Tranlation + $" || (74) Agent(s) number | {numbers}";
                        }
                    }

                    if (curElem.Is2 == true)
                        curElem.PlainLang = "Patente de Invención";
                    else
                        curElem.PlainLang = "Modelo de Utilidad";

                    if (string.IsNullOrEmpty(curElem.Date47))
                    {
                        id++;
                    }

                    elementsOut.Add(curElem);
                }
            }

            return elementsOut;
        }
    }
}
