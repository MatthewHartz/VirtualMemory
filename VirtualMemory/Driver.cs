using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualMemory;

namespace ProcessAndResourceManager
{
    class Driver
    {
        static void Main(string[] args)
        {
            var sb = new StringBuilder(); // Will contain file output
            var stream1 = new StreamReader("C:\\Users\\Matthew\\Desktop\\input1.txt");
            var stream2 = new StreamReader("C:\\Users\\Matthew\\Desktop\\input2.txt");
            var pairs = new List<SegmentFramePair>();
            var triplets = new List<PageSegmentFrameTriplet>();
            var isTlbEnabled = true;


            String line;

            line = stream1.ReadLine();

            var tokens = line.Split(' ');

            // Gets the "segment frame" pairs out of input file 1
            for (var i = 0; i < tokens.Length; i += 2)
            {
                pairs.Add(new SegmentFramePair(Int32.Parse(tokens[i]), Int32.Parse(tokens[i + 1])));
            }

            // Gets the "page segement frame" triplets out of input file 1
            line = stream1.ReadLine();

            tokens = line.Split(' ');
            for (var i = 0; i < tokens.Length; i += 3)
            {
                triplets.Add(new PageSegmentFrameTriplet(Int32.Parse(tokens[i]), Int32.Parse(tokens[i + 1]), Int32.Parse(tokens[i + 2])));
            }

            stream1.Close();

            // Initialize the virtual memory
            var handler = new VirtualMemoryHandler(pairs, triplets, isTlbEnabled);

            line = stream2.ReadLine();

            tokens = line.Split(' ');

            for (var i = 0; i < tokens.Length; i += 2)
            {
                string value = "";

                switch (tokens[i])
                {
                    case "0":
                        value = handler.Read(Int32.Parse(tokens[i + 1]));
                        break;
                    case "1":
                        value = handler.Write(Int32.Parse(tokens[i + 1]));
                        break;
                    default:
                        //sb.Append("err \n");
                        break;
                }

                switch (value)
                {
                    case "0":
                        sb.Append("err ");
                        break;
                    case "-1":
                        sb.Append("pf ");
                        break;
                    default:
                        sb.Append(value + " ");
                        break;
                }
            }

            stream2.Close();
            File.WriteAllText("C:\\Users\\Matthew\\Desktop\\87401675.txt", sb.ToString());
        }
    }
}
