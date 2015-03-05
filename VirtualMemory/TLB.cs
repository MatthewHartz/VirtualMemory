using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualMemory
{
    class TLB
    {
        List<TLBentry> entries { get; set; }

        public void InsertEntry()
        {
            
        }

        private void UpdateTable()
        {
            
        }
    }

    class TLBentry
    {
        int priority { get; set; }
        int segmentAndPage { get; set; }
        int frame { get; set; }
    }
}
