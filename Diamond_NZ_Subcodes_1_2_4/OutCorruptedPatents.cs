using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diamond_NZ_Subcodes_1_2_4
{
    public class OutCorruptedPatents
    {
        public static void ErrorsToFile(List<string> output, DirectoryInfo pathProcessed)
        {
            var path = Path.Combine(pathProcessed.FullName, "NZ_CorruptedPatents_SubCode1.txt");
            var sf = new StreamWriter(path);
            if (output != null)
            {
                foreach (var record in output)
                {
                    try
                    {
                        sf.WriteLine("Проблемный патент:\t" + record);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Что-то пошло не так....");
                    }
                }
            }
            sf.Flush();
            sf.Close();
        }
    }
}
