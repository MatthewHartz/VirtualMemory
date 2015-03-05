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
        private const int FrameSize = 512;
        private int[] physicalMemory = new int[524288];
        private Bitmap bitmap = new Bitmap();
        private TLB tlb;


        public VirtualMemoryHandler(List<SegmentFramePair> pairs, List<PageSegmentFrameTriplet> triplets, bool tlbEnabled)
        {
            // Determines if TLB is enabled for this run
            if (tlbEnabled)
            {
                tlb = new TLB();
            }

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
                var frame = GetFreeFrame();
                EmptyFrame(frame);
                pageTable = frame * FrameSize;
                SetSegmentToPageTable(virtualAddress.segment, pageTable);
            }

            var page = GetPage(pageTable + virtualAddress.page);

            if (page == -1) return -1;

            // Create page
            if (page == 0)
            {
                var frame = GetFreeFrame();
                EmptyFrame(frame);
                page = frame * FrameSize;
                SetPageToPageTable(pageTable + virtualAddress.word, page);
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

            bitmap.SetBit(pageTable/FrameSize);
        }

        /// <summary>
        /// Sets the page table to the corresponding page.
        /// </summary>
        /// <param name="pageTable">The page table.</param>
        /// <param name="page">The page.</param>
        private void SetPageToPageTable(int pageTable, int page)
        {
            physicalMemory[pageTable] = page;

            bitmap.SetBit(page / FrameSize);
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
            for (var i = 0; i < 1024; i++)
            {
                if (bitmap.GetBit(i) == 0) return i;
            }

            return -1;
        }

        /// <summary>
        /// Empties the frame for new allocation
        /// </summary>
        /// <param name="frame">The frame.</param>
        private void EmptyFrame(int frame)
        {
            for (var i = 0; i < FrameSize; i++)
            {
                physicalMemory[(frame * FrameSize) + i] = 0;
            }
        }
    }
}
