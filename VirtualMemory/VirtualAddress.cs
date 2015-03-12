using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualMemory
{
    public class VirtualAddress
    {
        public int segment { get; private set; }
        public int page { get; private set; }
        public int word { get; private set; }
        public int sp { get; private set; } // this is just used for TLB stuff

        private int _sMask = 0x0FF80000;
        private int _pMask = 0x0007FE00;
        private int _wMask = 0x000001FF;

        public VirtualAddress(int address)
        {
            segment = (address & _sMask) >> 19;
            page = (address & _pMask) >> 9;
            word = address & _wMask;
            sp = address >> 9;
        }
    }
}
