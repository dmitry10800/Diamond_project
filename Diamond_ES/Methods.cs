using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Diamond_ES
{
    public class Methods
    {
        internal static string[] RecSplit(string recString, string[] parametrs)
        {
            string[] splittedRecord = null;
            var tempStrC = recString;

            if (tempStrC != "")
            {
                if (tempStrC.StartsWith("11 "))
                {
                    Regex reg;
                    MatchCollection matches;

                    foreach (var parametr in parametrs)
                    {
                        switch (parametr)
                        {
                            case "11 ":
                                reg = new Regex(@"11 ES \d{7} [A-Z]{1}\d+");
                                matches = reg.Matches(tempStrC);
                                if (matches.Count > 0)
                                    tempStrC = tempStrC.Replace(matches[0].Value, "***" + matches[0].Value);
                                break;
                            case "21 ":
                                reg = new Regex(@"21 [A-Z]{1} \d+ \(\s*.{2,}\s*");
                                matches = reg.Matches(tempStrC);
                                if (matches.Count > 0)
                                    tempStrC = tempStrC.Replace(matches[0].Value, "***" + matches[0].Value);
                                break;
                            case "22 ":
                                reg = new Regex(@"22 \d{2}\/\d{2}\/\d{4}");
                                matches = reg.Matches(tempStrC);
                                if (matches.Count > 0)
                                    tempStrC = tempStrC.Replace(matches[0].Value, "***" + matches[0].Value);
                                break;
                            case "54 ":
                                reg = new Regex(@"54 \p{L}{1}\s*[^0-9]{1}");
                                matches = reg.Matches(tempStrC);
                                if (matches.Count > 0)
                                    tempStrC = tempStrC.Replace(matches[0].Value, "***" + matches[0].Value);
                                break;
                            case "73 ":
                                reg = new Regex(@"73 \p{L}{1}\s*[^0-9]{1}");
                                matches = reg.Matches(tempStrC);
                                if (matches.Count > 0)
                                    tempStrC = tempStrC.Replace(matches[0].Value, "***" + matches[0].Value);
                                break;
                            case "74 ":
                                reg = new Regex(@"74 \p{L}{1}\s*[^0-9]{1}");
                                matches = reg.Matches(tempStrC);
                                if (matches.Count > 0)
                                    tempStrC = tempStrC.Replace(matches[0].Value, "***" + matches[0].Value);
                                break;
                            case "Fecha de incorporación al dominio público:":
                                reg = new Regex(@"Fecha de incorporación al dominio público:.*");
                                matches = reg.Matches(tempStrC);
                                if (matches.Count > 0)
                                    tempStrC = tempStrC.Replace(matches[0].Value, "***" + matches[0].Value);
                                break;
                            case "Motivo de caducidad:":
                                reg = new Regex(@"Motivo de caducidad:.*");
                                matches = reg.Matches(tempStrC);
                                if (matches.Count > 0)
                                    tempStrC = tempStrC.Replace(matches[0].Value, "***" + matches[0].Value);
                                break;
                            case "61 ":
                                reg = new Regex(@"61 .*");
                                matches = reg.Matches(tempStrC);
                                if (matches.Count > 0)
                                    tempStrC = tempStrC.Replace(matches[0].Value, "***" + matches[0].Value);
                                break;
                        }
                    }
                }

                /*Splitting record*/
                splittedRecord = tempStrC.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            }
            return splittedRecord;
        }

        internal static string[] RecSplitSubCode11(string s, string[] parametrs)
        {
            string[] outArr = null;
            var tempOutArr = new List<string>();
            var tempStrC = s;

            var tempArray = tempStrC.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();

            for (var i = 0; i < tempArray.Length; i++)
            {
                tempArray[i] = "**" + tempArray[i];

                foreach (var parametr in parametrs)
                {
                    if (tempArray[i].Contains(parametr))
                    {
                        tempArray[i] = tempArray[i].Replace("**", "").Trim();
                        tempArray[i] = tempArray[i].Replace(tempArray[i], "***" + tempArray[i]);
                        break;
                    }
                }
            }

            var tempStr = "";

            foreach (var s1 in tempArray)
            {
                tempStr += s1;
            }

            outArr = tempStr.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            foreach (var s1 in outArr)
            {
                if (s1.Contains("**"))
                {
                    tempOutArr.Add(s1.Replace("**", "\n"));
                }
                else
                {
                    tempOutArr.Add(s1);
                }

            }
            return tempOutArr.ToArray();
        }

        public class PubllicationInformation
        {
            public string PublicationNumber { get; set; }
            public string PublicationKind { get; set; }
        }

        internal static PubllicationInformation PublicationNumberNormalize(string s)
        {
            var publlicationInformation = new PubllicationInformation();

            var numberKind = Regex.Match(s, @"(?<Number>[A-Z]{2}\s*\d+)\s*(?<Kind>[A-Z]{1}\d{1,2}|Y)");
            if (numberKind.Success)
            {
                publlicationInformation.PublicationNumber = numberKind.Groups["Number"].Value;
                publlicationInformation.PublicationKind = numberKind.Groups["Kind"].Value;
                return publlicationInformation;
            }
            return null;
        }

        internal static string ApplicationNumberNormalize(string s)
        {
            return Regex.Replace(s, @"\(\s*.{1,}\s*\)", "").Trim();
        }

        internal static string DateNormalize(string s)
        {
            var dateNormalized = s;
            if (Regex.IsMatch(s, @"\d{2}\/*\-*\.*\d{2}\/*\-*\.*\d{4}"))
            {
                var date = Regex.Match(s, @"(?<day>\d{2})\/*\-*\.*(?<month>\d{2})\/*\-*\.*(?<year>\d{4})");
                dateNormalized = date.Groups["year"].Value + "/" + date.Groups["month"].Value + "/" + date.Groups["day"].Value;
            }
            return dateNormalized.Trim();
        }

        internal static (string, string) LegalEventNoteNormalize(string s)
        {
            var note = Regex.Match(s, @"(?<nameField>.*):(?<text>.*)");
            return ("|| " + note.Groups["nameField"].Value + " | " + note.Groups["text"].Value.Trim(), note.Groups["text"].Value);
        }

        internal static List<string> GranteeAssigneeOwnerInformationNornalize(string s)
        {
            var outList = new List<string>();
            var splittedRecord = s.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();

            foreach (var record in splittedRecord)
            {
                if (!string.IsNullOrEmpty(Regex.Replace(record, @"\(.*\)", "").Trim()))
                    outList.Add(Regex.Replace(record, @"\(.*\)", "").Trim());
            }
            return outList;
        }

        internal static List<string> AgentNameNormalize(string s)
        {
            var outList = new List<string>();
            var tempS = s;
            tempS = Regex.Replace(tempS, @"LEY \d+/\d+", "").Trim();

            var splittedRecord = tempS.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();

            foreach (var record in splittedRecord)
            {
                outList.Add(Regex.Replace(record, @"\(.*\)", "").Trim());
            }
            return outList;
        }

        internal static List<RelatedPublicationInformation> RelatedPublicationInformationNormalize(string s)
        {
            var outList = new List<RelatedPublicationInformation>();
            var outObject = new RelatedPublicationInformation();
            var relatedPubInf = Regex.Match(s, @"(?<InidNumber>\d{2})\s*(?<Number>[A-Z]{1}\d+)\s*(?<Date>\d{2}\/\d{2}\/\d{4})");
            outObject.InidNumber = relatedPubInf.Groups["InidNumber"].Value;
            outObject.Date = relatedPubInf.Groups["Date"].Value;
            outObject.Number = relatedPubInf.Groups["Number"].Value;
            outList.Add(outObject);
            return outList;
        }

        internal static List<LegalNote> NormalizeLegalNote(string s)
        {
            var outList = new List<LegalNote>();
            LegalNote note;
            var tempESValue = "";

            for (var i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    note = new LegalNote();
                    note.Language = "ES";
                    var result = LegalEventNoteNormalize(s.Replace("\n", ""));
                    note.Note = result.Item1.Trim();
                    tempESValue = result.Item2;
                    outList.Add(note);
                }

                if (i == 1)
                {
                    note = new LegalNote();
                    var number = "";

                    if (Regex.IsMatch(tempESValue, @"\d{1,2}"))
                    {
                        //есть цифры которые нужно извлечь
                        var reg = new Regex(@"\d{1,2}");
                        var matches = reg.Matches(tempESValue);

                        if (matches.Count > 0)
                        {
                            number = matches[0].Value;
                        }

                        var searchResult = GetEnglishTranslatingForESLegalEvent(tempESValue);
                        searchResult = searchResult.Replace("****", number);
                        note.Language = "EN";
                        note.Note = "|| Reason for expiration | " + searchResult.Trim();
                        outList.Add(note);
                    }
                    else
                    {
                        //нету цифр, нужно сделать только перевод предложения для Legal Note
                        var ressultStrNote = "|| Reason for expiration | " +
                                             GetEnglishTranslatingForESLegalEvent(tempESValue);
                        note.Language = "EN";
                        note.Note = ressultStrNote;
                        outList.Add(note);
                    }
                }
            }
            return outList;
        }

        internal static string GetEnglishTranslatingForESLegalEvent(string s)
        {
            switch (s)
            {
                case var phrase when new Regex(@"Por expiración de vida legal", RegexOptions.IgnoreCase).IsMatch(phrase): return "By expiration of legal life";
                case var phrase when new Regex(@"Por renuncia del titular", RegexOptions.IgnoreCase).IsMatch(phrase): return "By resignation of the holder";
                case var phrase when new Regex(@"Por expiración de vida legal de la patente principal", RegexOptions.IgnoreCase).IsMatch(phrase): return "Due to the legal life of the main patent";
                //case var phrase when new Regex(@"Falta de pago de sexta anualidad", RegexOptions.IgnoreCase).IsMatch(phrase): return "";
                case var phrase when new Regex(@"Por impago de la \d{1,2} anualidad", RegexOptions.IgnoreCase).IsMatch(phrase): return "For non-payment of **** annual fee";
                default: return "00";
            }
        }
    }
}
