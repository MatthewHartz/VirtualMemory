using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualMemory
{
    public class PageSegmentFrameTriplet
    {
        public int page { get; set; }
        public int segment { get; set; }
        public int frame { get; set; }

        public PageSegmentFrameTriplet(int p, int s, int f)
        {
            page = p;
            segment = s;
            frame = f;
        }
    }
}
