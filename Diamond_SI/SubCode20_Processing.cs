using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_SI
{
    public class SubCode20_Processing
    {
        public static List<Subcode20> Subcode20Process(List<XElement> allElements)
        {
            List<Subcode20> allSubcode20Trademarks = new List<Subcode20>();
            int tempInc = 0;
            for(int i = 0; i < allElements.Count; i++)
            {
                tempInc = i;
                string strValueOnePatent = "";
                string valueStr = allElements[tempInc].Value;

                if (valueStr.StartsWith(@"(51)"))
                {
                    do
                    {
                        strValueOnePatent += allElements[tempInc].Value + " ";
                        tempInc++;
                    } while (tempInc < allElements.Count && !allElements[tempInc].Value.StartsWith(@"(51)"));


                    var splittedRecords = Methods.Subcode20RecordSplit(strValueOnePatent);

                    if(splittedRecords != null && splittedRecords.Length > 0)
                    {
                        Subcode20 currentTrademark = new Subcode20();

                        foreach(var record in splittedRecords)
                        {
                            if (record.StartsWith(@"(51)"))
                            {
                                var classifications = Methods.GetClassificationArray(record.Replace(@"(51)", "").Trim());
                                foreach(var item in classifications)
                                {
                                    currentTrademark.Classification.Add(item);
                                }
                            }

                            if (record.StartsWith(@"(11)"))
                            {
                                currentTrademark.PublicationNumber = record.Replace(@"(11)", "").Trim();
                            }

                            if (record.StartsWith(@"(13)"))
                            {
                                currentTrademark.PublicationKind = record.Replace(@"(13)", "").Trim();
                            }

                            if (record.StartsWith(@"(46)"))
                            {
                                currentTrademark.DateField46 = Methods.DateNormalize(record.Replace(@"(46)", "").Trim());
                            }

                            if (record.StartsWith(@"(21)"))
                            {
                                currentTrademark.ApplicationNumber = record.Replace(@"(21)", "").Trim();
                            }

                            if (record.StartsWith(@"(22)"))
                            {
                                currentTrademark.ApplicationDate = Methods.DateNormalize(record.Replace(@"(22)", "").Trim());
                            }

                            if (record.StartsWith(@"(86)"))
                            {
                                currentTrademark.Field_86 = Methods.GetListField86_87_96_97(record.Replace(@"(86)", "").Trim(), 86);
                            }

                            if (record.StartsWith(@"(96)"))
                            {
                                currentTrademark.Field_96 = Methods.GetListField86_87_96_97(record.Replace(@"(96)", "").Trim(), 96);
                            }

                            if (record.StartsWith(@"(87)"))
                            {
                                currentTrademark.Field_87 = Methods.GetListField86_87_96_97(record.Replace(@"(87)", "").Trim(), 87);
                            }

                            if (record.StartsWith(@"(97)"))
                            {
                                currentTrademark.Field_97 = Methods.GetListField86_87_96_97(record.Replace(@"(97)", "").Trim(), 97);
                            }

                            if (record.StartsWith(@"(30)"))
                            {
                                currentTrademark.PriorityInformation = Methods.GetPriorityList(record.Replace(@"(30)", "").Trim());
                            }
                             
                            if (record.StartsWith(@"(72)"))
                            {
                                currentTrademark.InventorInformation = Methods.GetInventors_Grantee_Assignee_Owner_AgentInformation(record.Replace(@"(72)", "").Trim());
                            }

                            if (record.StartsWith(@"(73)"))
                            {
                                currentTrademark.Grantee_Assignee_OwnerInformation = Methods.GetInventors_Grantee_Assignee_Owner_AgentInformation(record.Replace(@"(73)", "").Trim());
                            }

                            if (record.StartsWith(@"(74)"))
                            {
                                currentTrademark.AgentInformation = Methods.GetInventors_Grantee_Assignee_Owner_AgentInformation(record.Replace(@"(74)", "").Trim());
                            }

                            if (record.StartsWith(@"(54)"))
                            {
                                Title title = new Title();
                                title.Language = "SL";
                                title.Text = record.Replace(@"(54)", "").Replace("\n"," ").Trim();
                                currentTrademark.Title = title;
                            }
                        }
                        allSubcode20Trademarks.Add(currentTrademark);
                    }
                }
            }
            return allSubcode20Trademarks;
        }
    }
}
