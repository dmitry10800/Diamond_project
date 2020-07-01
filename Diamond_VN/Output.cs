using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Diamond_VN
{
    class Output
    {
        public static void Test(List<Applications> output)
        {
            var path = Path.Combine(@"C:\Users\Razrab\Desktop\Test\" + "_Reg.txt");
            var sf = new StreamWriter(path);
            if (output != null)
            {
                foreach (var record in output)
                {
                    try
                    {
                        sf.WriteLine("****");
                        if (!string.IsNullOrEmpty(record.PubNumber))
                        {
                            sf.WriteLine("PubNumber:\t" + record.PubNumber);
                        }
                        if (!string.IsNullOrEmpty(record.PubDate))
                        {
                            sf.WriteLine("PubDate:\t" + record.PubDate);
                        }
                        if (!string.IsNullOrEmpty(record.AppNumber))
                        {
                            sf.WriteLine("AppNumber:\t" + record.AppNumber);
                        }
                        if (!string.IsNullOrEmpty(record.AppDate))
                        {
                            sf.WriteLine("AppDate:\t" + record.AppDate);
                        }
                        if (record.Pct84 != null)
                        {
                            sf.WriteLine("PCT84:\t" + record.Pct84.PubNumber);
                            sf.WriteLine("PCT84:\t" + record.Pct84.PubDate);
                        }
                        if (record.Pct85 != null)
                        {
                            sf.WriteLine("PCT85:\t" + record.Pct85.PubNumber);
                            sf.WriteLine("PCT85:\t" + record.Pct85.PubDate);
                        }
                        if (record.Pct86 != null)
                        {
                            sf.WriteLine("PCT86:\t" + record.Pct86.AppDate);
                            sf.WriteLine("PCT86:\t" + record.Pct86.AppNumber);
                        }
                        if (record.PubNumber != null)
                        {
                            sf.WriteLine("PCT87:\t" + record.Pct87.PubDate);
                            sf.WriteLine("PCT87:\t" + record.Pct87.PubNumber);
                        }
                        if (record.Abstract != null)
                        {
                            sf.WriteLine("AText:\t" + record.Abstract.Text);
                            sf.WriteLine("ALang:\t" + record.Abstract.Language);
                        }
                        if (record.EventDate != null)
                        {
                            sf.WriteLine("EventDate:\t" + record.EventDate);
                        }
                        if (record.ClassificationIpcs != null)
                        {
                            for (int i = 0; i < record.ClassificationIpcs.Count; i++)
                            {
                                sf.WriteLine("Class:\t" + record.ClassificationIpcs[i].Classification);
                                sf.WriteLine("Edition:\t" + record.ClassificationIpcs[i].Edition);
                            }
                        }
                        if (record.Inventors != null)
                        {
                            for (int i = 0; i < record.Inventors.Count; i++)
                            {
                                sf.WriteLine("IAddress:\t" + record.Inventors[i].Address);
                                sf.WriteLine("ICountry:\t" + record.Inventors[i].Country);
                                sf.WriteLine("IName:\t" + record.Inventors[i].Name);
                            }
                        }
                        if (record.Agents != null)
                        {
                            for (int i = 0; i < record.Agents.Count; i++)
                            {
                                sf.WriteLine("AName:\t" + record.Agents[i].Name);
                            }
                        }
                        if (record.Applicants != null)
                        {
                            for (int i = 0; i < record.Applicants.Count; i++)
                            {
                                sf.WriteLine("AAddress:\t" + record.Applicants[i].Address);
                                sf.WriteLine("ACountry:\t" + record.Applicants[i].Country);
                                sf.WriteLine("AName:\t" + record.Applicants[i].Name);
                            }
                        }
                        if (record.Priorities != null)
                        {
                            for (int i = 0; i < record.Priorities.Count; i++)
                            {
                                sf.WriteLine("PNumber:\t" + record.Priorities[i].Number);
                                sf.WriteLine("PCountry:\t" + record.Priorities[i].Country);
                                sf.WriteLine("PDate:\t" + record.Priorities[i].Date);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //Console.WriteLine("Error:\t" + "PDF:\t" + Directory.GetParent(pathProcessed.FullName).Name + "\tAPNR:\t" + record.RGNR);
                    }
                }
            }
            sf.Flush();
            sf.Close();
        }
    }
}
