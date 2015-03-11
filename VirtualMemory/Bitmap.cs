using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{
    /// <summary>
    /// This class represents the bitmap for all the open blocks
    /// </summary>
    class Bitmap
    {
        private int[] _map = new int[32]; // 1024 bits
        private int _bitmask = 0x00000001;   

        public Bitmap()
        {
            SetBit(0); // This is for the Segment Table that resides in position 0
        }

        /// <summary>
        /// Gets the bit indicated by index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public int GetBit(int index)
        {
            var group = index / 32;
            var section = index % 32;

            return _map[group] & (_bitmask << section);
        }

        /// <summary>
        /// Sets the bit indicated by index to the value of bit.
        /// </summary>
        /// <param name="bit">The bit.</param>
        /// <param name="index">The index.</param>
        public void SetBit(int index)
        {
            //_map |= (_bitmask << index);
            var group = index/32;
            var section = index%32;

            _map[group] |= (_bitmask << section);
        }

        /// <summary>
        /// Clears the bit indicated by index to the value of bit.
        /// </summary>
        /// <param name="bit">The bit.</param>
        /// <param name="index">The index.</param>
        public void ClearBit(int index)
        {
            var group = index / 32;
            var section = index % 8;

            _map[group] &= ~(_bitmask << section);
            //_map &= ~(_bitmask << index);
        }
    }
}