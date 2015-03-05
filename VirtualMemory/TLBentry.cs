using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualMemory
{
    public class TLBentry
    {
        public int priority { get; set; }
        public int sp { get; set; }
        public int f { get; set; }

        public TLBentry(int priority, int sp, int f)
        {
            this.priority = priority;
            this.sp = sp;
            this.f = f;
        }

        public TLBentry(int priority)
        {
            this.priority = priority;
        }
    }
}
