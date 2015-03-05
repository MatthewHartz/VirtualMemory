using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualMemory
{
    class VirtualMemoryHandler
    {
        private int[] physicalMemory = new int[524288];

        public VirtualMemoryHandler(List<SegmentFramePair> pairs, List<PageSegmentFrameTriplet> triplets)
        {
            // Initialze the segment table
            foreach (var pair in pairs)
            {
                SetSegmentToPageTable(pair.segment, pair.frame);
            }

            // Initialize the page tables with frames
            foreach (var triplet in triplets)
            {
                var pageTable = GetPageTable(triplet.segment);

                physicalMemory[pageTable + triplet.page] = triplet.frame;
            }
        }

        public int Read(int address)
        {
            var virtualAddress = new VirtualAddress(address);

            var pageTable = GetPageTable(virtualAddress.segment);

            if (pageTable == -1) return -1;

            var page = GetPage(pageTable);

            if (page == -1) return -1;

            return page + virtualAddress.word;
        }

        public void Write(int address)
        {
            
        }

        /// <summary>
        /// Gets the page table.
        /// </summary>
        /// <param name="segment">The segment.</param>
        /// <returns></returns>
        private int GetPageTable(int segment)
        {
            if (segment < 0 || segment > 511)
            {
                return -1;
            }

            return physicalMemory[segment];
        }

        /// <summary>
        /// Sets the segment number to the corresponding page table.
        /// </summary>
        /// <param name="segment">The segment.</param>
        /// <param name="pageTable">The page table.</param>
        private void SetSegmentToPageTable(int segment, int pageTable)
        {
            physicalMemory[segment] = pageTable;
        }

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <param name="pageTable">The page table.</param>
        /// <returns></returns>
        private int GetPage(int pageTable)
        {
            return physicalMemory[pageTable];
        }
    }
}
