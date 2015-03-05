using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileSystem;

namespace VirtualMemory
{
    class VirtualMemoryHandler
    {
        private int[] physicalMemory = new int[524288];
        private Bitmap bitmap = new Bitmap();

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

        /// <summary>
        /// Converts the virtual address into a physical address, then returns the address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns>Returns the physical address</returns>
        public int Read(int address)
        {
            var virtualAddress = new VirtualAddress(address);

            var pageTable = GetPageTable(virtualAddress.segment);

            if (pageTable == -1 || pageTable == 0) return pageTable;

            var page = GetPage(pageTable);

            if (page == -1 || page == 0) return page;

            return page + virtualAddress.word;
        }

        /// <summary>
        /// Converts the virtual address into a phsyical address. If the segment table's value is 
        /// 0, then create a new page table. Consequently this will then have 0 for the page table,
        /// and a new page needs to be created.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns>Returns the physical address</returns>
        public int Write(int address)
        {
            var virtualAddress = new VirtualAddress(address);

            var pageTable = GetPageTable(virtualAddress.segment);

            if (pageTable == -1) return -1;
            
            // Create page table
            if (pageTable == 0)
            {
                
            }

            var page = GetPage(pageTable);

            if (page == -1) return -1;

            // Create page
            if (page == 0)
            {
                
            }

            return page + virtualAddress.word;
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

            bitmap.SetBit(segment);
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

        /// <summary>
        /// Using the bitmap, gets an open frame.
        /// </summary>
        /// <returns></returns>
        private int GetFreeFrame()
        {


            return -1;
        }
    }
}
