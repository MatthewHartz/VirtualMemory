using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAndResourceManager
{
    class Driver
    {
        static void Main(string[] args)
        {
            //var manager = ProcessAndResourceManager.Instance;
            var sb = new StringBuilder(); // Will contain file output
            var stream = new StreamReader("C:\\Users\\Matthew\\Desktop\\input.txt");
            String line;

            while ((line = stream.ReadLine()) != null)
            {
                var tokens = line.Split(' ');

                try
                {
                    

                }
                catch (Exception e)
                {
                    sb.Append(String.Format(" error({0})", e.Message));
                }
            }

            stream.Close();
            File.WriteAllText("C:\\Users\\Matthew\\Desktop\\87401675.txt", sb.ToString());
        }
    }
}
