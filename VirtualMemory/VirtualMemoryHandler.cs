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
        private List<TLBentry> tlb;


        public VirtualMemoryHandler(List<SegmentFramePair> pairs, List<PageSegmentFrameTriplet> triplets, bool tlbEnabled)
        {
            // Determines if TLB is enabled for this run
            if (tlbEnabled)
            {
                tlb = new List<TLBentry>
                {
                    new TLBentry(0, -1, -1),
                    new TLBentry(1, -1, -1),
                    new TLBentry(2, -1, -1),
                    new TLBentry(3, -1, -1)
                };
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

                bitmap.SetBit(triplet.frame/512);
            }
        }

        /// <summary>
        /// Converts the virtual address into a physical address, then returns the address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns>Returns the physical address</returns>
        public string Read(int address)
        {
            var virtualAddress = new VirtualAddress(address);

            if (tlb != null)
            {
                var pa = DoesEntryExist(virtualAddress);
                if (pa != -1) return "h " + pa;
            }

            var pageTable = GetPageTable(virtualAddress.segment);

            if (pageTable == -1 || pageTable == 0) return pageTable.ToString();

            var page = GetPage(pageTable + virtualAddress.page);

            if (page == -1 || page == 0) return page.ToString();

            if (tlb != null) InsertEntry(virtualAddress);

            return tlb != null ? "m " + (page + virtualAddress.word) : (page + virtualAddress.word).ToString();
        }

        /// <summary>
        /// Converts the virtual address into a phsyical address. If the segment table's value is 
        /// 0, then create a new page table. Consequently this will then have 0 for the page table,
        /// and a new page needs to be created.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns>Returns the physical address</returns>
        public string Write(int address)
        {
            var virtualAddress = new VirtualAddress(address);

            if (tlb != null)
            {
                var pa = DoesEntryExist(virtualAddress);
                if (pa != -1) return "h " + pa;
            }

            var pageTable = GetPageTable(virtualAddress.segment);

            if (pageTable == -1) return pageTable.ToString();
            
            // Create page table
            if (pageTable == 0)
            {
                var frame = GetFreePageTable();
                EmptyFrame(frame);
                pageTable = frame * FrameSize;
                SetSegmentToPageTable(virtualAddress.segment, pageTable);
            }

            var page = GetPage(pageTable + virtualAddress.page);

            if (page == -1) return page.ToString();

            // Create page
            if (page == 0)
            {
                var frame = GetFreeFrame();
                EmptyFrame(frame);
                page = frame * FrameSize;
                SetPageToPageTable(pageTable + virtualAddress.page, page);
            }

            if (tlb != null) InsertEntry(virtualAddress);

            return tlb != null ? "m " + (page + virtualAddress.word) : (page + virtualAddress.word).ToString();
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

            if (pageTable > 0)
            {
                bitmap.SetBit(pageTable / FrameSize);
                bitmap.SetBit((pageTable / FrameSize) + 1);
            }
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
        /// Using the bitmap, returns the initial bit of 2 consecutive free bits.
        /// Returns null if both bits are not found.
        /// </summary>
        /// <returns></returns>
        private int GetFreePageTable()
        {
            for (var i = 0; i < 1024; i++)
            {
                var bit = bitmap.GetBit(i);
                if (bit == 0)
                {
                    if (bitmap.GetBit(i + 1) == 0) return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Using the bitmap, gets 1st open frame.
        /// Returns null if open bit is not found.
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

        /// <summary>
        /// Inserts the tlb entry if it does not exist. If the entry exists, return
        /// the frame number of the entry. Otherwise, return -1;
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="frame">The frame.</param>
        /// <returns></returns>
        private void InsertEntry(VirtualAddress va)
        {
            // Entry was not found, so replace the least recently used entry
            foreach (var entry in tlb)
            {
                if (entry.priority == 0)
                {
                    entry.priority = 3;
                    entry.sp = va.sp;
                    entry.f = physicalMemory[physicalMemory[va.segment] + va.page];
                }
                else
                {
                    entry.priority -= 1;
                }

            }
        }

        private int DoesEntryExist(VirtualAddress va)
        {
            // search TLB for entry, then update it's priority and return PA
            for (var i = 0; i < tlb.Count; i++)
            {
                if (tlb[i].sp == va.sp)
                {
                    foreach (var entry in tlb)
                    {
                        if (entry.priority > tlb[i].priority) entry.priority -= 1;
                    }

                    tlb[i].priority = 3;
                    return tlb[i].f + va.word;
                }
            }

            return -1;
        }
    }
}
