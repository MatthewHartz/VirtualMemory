using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualMemory
{
    public class SegmentFramePair
    {
        public int segment { get; set; }
        public int frame { get; set; }

        public SegmentFramePair(int s, int f)
        {
            segment = s;
            frame = f;
        }
    }
}
