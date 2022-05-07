/*
 * HLLib
 * Copyright (C) 2006-2010 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

using System;
using System.Collections.Generic;
using System.Text;
using HLLib.Directory;
using HLLib.Mappings;
using HLLib.Streams;

namespace HLLib.Packages.GCF
{
    /// <summary>
    /// Half-Life Game Cache File
    /// </summary>
    public sealed class GCFFile : Package
    {
        #region Constants

        /// <summary>
        /// The item is a file.
        /// </summary>
        private const int HL_GCF_FLAG_FILE = 0x00004000;

        /// <summary>
        /// The item is encrypted.
        /// </summary>
        private const int HL_GCF_FLAG_ENCRYPTED = 0x00000100;

        /// <summary>
        /// Backup the item before overwriting it.
        /// </summary>
        private const int HL_GCF_FLAG_BACKUP_LOCAL = 0x00000040;

        /// <summary>
        /// The item is to be copied to the disk.
        /// </summary>
        private const int HL_GCF_FLAG_COPY_LOCAL = 0x0000000A;

        /// <summary>
        /// Don't overwrite the item if copying it to the disk and the item already exists.
        /// </summary>
        private const int HL_GCF_FLAG_COPY_LOCAL_NO_OVERWRITE = 0x00000001;

        /// <summary>
        /// The maximum data allowed in a 32 bit checksum.
        /// </summary>
        private const int HL_GCF_CHECKSUM_LENGTH = 0x00008000;

        /// <inheritdoc/>
        public override string[] AttributeNames => new string[] { "Version", "Cache ID", "Allocated Blocks", "Used Blocks", "Block Length", "Last Version Played" };

        /// <inheritdoc/>
        public override string[] ItemAttributeNames => new string[] { "Encrypted", "Copy Locally", "Overwrite Local Copy", "Backup Local Copy", "Flags", "Fragmentation" };

        #endregion

        #region Views

        /// <summary>
        /// View representing header data
        /// </summary>
        private View HeaderView;

        #endregion

        #region Fields

        /// <summary>
        /// Deserialized header data
        /// </summary>
        public GCFHeader Header { get; private set; }

        /// <summary>
        /// Deserialized block entry header data
        /// </summary>
        public GCFBlockEntryHeader BlockEntryHeader { get; private set; }

        /// <summary>
        /// Deserialized block entries data
        /// </summary>
        public GCFBlockEntry[] BlockEntries { get; private set; }

        /// <summary>
        /// Deserialized fragmentation map header data
        /// </summary>
        public GCFFragmentationMapHeader FragmentationMapHeader { get; private set; }

        /// <summary>
        /// Deserialized fragmentation map data
        /// </summary>
        public GCFFragmentationMap[] FragmentationMaps { get; private set; }

        /// <summary>
        /// Deserialized block entry map header data
        /// </summary>
        /// <remarks>
        /// Part of version 5 but not version 6.
        /// </remarks>
        public GCFBlockEntryMapHeader BlockEntryMapHeader { get; private set; }

        /// <summary>
        /// Deserialized block entry map data
        /// </summary>
        /// <remarks>
        /// Part of version 5 but not version 6.
        /// </remarks>
        public GCFBlockEntryMap[] BlockEntryMaps { get; private set; }

        /// <summary>
        /// Deserialized directory header data
        /// </summary>
        public GCFDirectoryHeader DirectoryHeader { get; private set; }

        /// <summary>
        /// Deserialized directory entries data
        /// </summary>
        public GCFDirectoryEntry[] DirectoryEntries { get; private set; }

        /// <summary>
        /// Deserialized directory names data
        /// </summary>
        public string DirectoryNames { get; private set; }

        /// <summary>
        /// Deserialized directory info 1 entries data
        /// </summary>
        public GCFDirectoryInfo1Entry[] DirectoryInfo1Entries { get; private set; }

        /// <summary>
        /// Deserialized directory info 2 entries data
        /// </summary>
        public GCFDirectoryInfo2Entry[] DirectoryInfo2Entries { get; private set; }

        /// <summary>
        /// Deserialized directory copy entries data
        /// </summary>
        public GCFDirectoryCopyEntry[] DirectoryCopyEntries { get; private set; }

        /// <summary>
        /// Deserialized directory local entries data
        /// </summary>
        public GCFDirectoryLocalEntry[] DirectoryLocalEntries { get; private set; }

        /// <summary>
        /// Deserialized directory map header data
        /// </summary>
        public GCFDirectoryMapHeader DirectoryMapHeader { get; private set; }

        /// <summary>
        /// Deserialized directory map entries data
        /// </summary>
        public GCFDirectoryMapEntry[] DirectoryMapEntries { get; private set; }

        /// <summary>
        /// Deserialized checksum header data
        /// </summary>
        public GCFChecksumHeader ChecksumHeader { get; private set; }

        /// <summary>
        /// Deserialized checksum map header data
        /// </summary>
        public GCFChecksumMapHeader ChecksumMapHeader { get; private set; }

        /// <summary>
        /// Deserialized checksum map entries data
        /// </summary>
        public GCFChecksumMapEntry[] ChecksumMapEntries { get; private set; }

        /// <summary>
        /// Deserialized checksum entries data
        /// </summary>
        public GCFChecksumEntry[] ChecksumEntries { get; private set; }

        /// <summary>
        /// Deserialized data block header data
        /// </summary>
        public GCFDataBlockHeader DataBlockHeader { get; private set; }

        /// <summary>
        /// Deserialized directory items data
        /// </summary>
        public DirectoryItem[] DirectoryItems { get; private set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public GCFFile() : base()
        {
            HeaderView = null;
            Header = null;

            BlockEntryHeader = null;
            BlockEntries = null;

            FragmentationMapHeader = null;
            FragmentationMaps = null;

            BlockEntryMapHeader = null;
            BlockEntryMaps = null;

            DirectoryHeader = null;
            DirectoryEntries = null;
            DirectoryNames = null;
            DirectoryInfo1Entries = null;
            DirectoryInfo2Entries = null;
            DirectoryCopyEntries = null;
            DirectoryLocalEntries = null;

            DirectoryMapHeader = null;
            DirectoryMapEntries = null;

            ChecksumHeader = null;
            ChecksumMapHeader = null;
            ChecksumMapEntries = null;
            ChecksumEntries = null;

            DataBlockHeader = null;

            DirectoryItems = null;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~GCFFile() => Close();

        #region Descriptors

        /// <inheritdoc/>
        public override PackageType PackageType => PackageType.HL_PACKAGE_GCF;

        /// <inheritdoc/>
        public override string Extension => "gcf";

        /// <inheritdoc/>
        public override string Description => "Half-Life Game Cache File";

        /// <summary>
        /// Get the directory name at a particular offset
        /// </summary>
        /// <param name="nameOffset">Offset that the name should start at</param>
        /// <returns>Directory name without null terminator, null on error</returns>
        /// <remarks>
        /// Strings are null-terminated
        /// </remarks>
        public string DirectoryName(int nameOffset)
        {
            if (string.IsNullOrEmpty(DirectoryNames))
                return null;

            if (nameOffset < 0 || nameOffset >= DirectoryNames.Length)
                return null;

            List<char> temp = new List<char>();
            while (DirectoryNames[nameOffset] != '\0')
            {
                temp.Add(DirectoryNames[nameOffset++]);
            }

            return new string(temp.ToArray());
        }

        #endregion

        #region Defragmentation

        /// <inheritdoc/>
        protected override bool DefragmentInternal(bool forceDefragment)
        {
            bool error = false;
            int filesDefragmented = 0, filesTotal = 0;
            long bytesDefragmented = 0, bytesTotal = 0;

            // Check the current fragmentation state.
            {
                int blocksFragmented = 0;
                int blocksUsed = 0;

                for (uint i = 0; i < DirectoryHeader.ItemCount; i++)
                {
                    if ((DirectoryEntries[i].DirectoryFlags & HL_GCF_FLAG_FILE) != 0)
                    {
                        GetItemFragmentation(i, out int fileBlocksFragmented, out int fileBlocksUsed);

                        blocksFragmented += fileBlocksFragmented;
                        blocksUsed += fileBlocksUsed;

                        filesTotal++;
                        bytesTotal += fileBlocksUsed * DataBlockHeader.BlockSize;
                    }
                }

                // If there are no data blocks to defragment, and we don't want to sort the data blocks
                // lexicographically, then we're done.
                if ((blocksFragmented == 0 && !forceDefragment) || blocksUsed == 0)
                {
                    return true;
                }
            }

            View currentView = null, incrementedView = null;
            uint increment = 0;

            //uint uiDataBlockTerminator = DataBlockHeader.BlockCount >= 0x0000ffff ? 0xffffffff : 0x0000ffff;
            uint dataBlockTerminator = FragmentationMapHeader.Terminator == 0 ? 0x0000ffff : 0xffffffff;

            byte[] dataBlock = new byte[DataBlockHeader.BlockSize];

            // Step through each data block in each file as it appears in the directory and defragment (order each data
            // block sequentially).  This is a slow brute force approach, but since related half-life 2 files
            // often start with the same name we *may* expect a performance boost from odering files lexicographically
            // (as they appear in the directory).  That's my justification at least...
            for (int i = 0; i < DirectoryHeader.ItemCount && !error; i++)
            {
                if ((DirectoryEntries[i].DirectoryFlags & HL_GCF_FLAG_FILE) != 0)
                {
                    uint currentBlockEntryIndex = DirectoryMapEntries[i].FirstBlockIndex;

                    while (currentBlockEntryIndex != DataBlockHeader.BlockCount)
                    {
                        uint currentBlockEntrySize = 0;
                        uint currentDataBlockIndex = BlockEntries[currentBlockEntryIndex].FirstDataBlockIndex;
                        uint lastDataBlockIndex = DataBlockHeader.BlockCount;

                        while (currentDataBlockIndex < dataBlockTerminator && currentBlockEntrySize < BlockEntries[currentBlockEntryIndex].FileDataSize)
                        {
                            uint uiNextDataBlockIndex = FragmentationMaps[currentDataBlockIndex].NextDataBlockIndex;

                            // If this data block is not ordered sequentially, swap it with the sequential data block.
                            if (currentDataBlockIndex != increment)
                            {
                                // Make sure we can map the two data blocks before we alter any tables.
                                if (Mapping.Map(ref currentView, DataBlockHeader.FirstBlockOffset + currentDataBlockIndex * DataBlockHeader.BlockSize, (int)DataBlockHeader.BlockSize) &&
                                    Mapping.Map(ref incrementedView, DataBlockHeader.FirstBlockOffset + increment * DataBlockHeader.BlockSize, (int)DataBlockHeader.BlockSize))
                                {
                                    // Search to see if the sequential data block is in use, we only need to check
                                    // files after ours in the directory because everything before is sequential.
                                    bool found = false;
                                    for (int j = i; j < DirectoryHeader.ItemCount && !found; j++)
                                    {
                                        if ((DirectoryEntries[j].DirectoryFlags & HL_GCF_FLAG_FILE) != 0)
                                        {
                                            uint uiIncrementedBlockEntryIndex = DirectoryMapEntries[j].FirstBlockIndex;

                                            while (uiIncrementedBlockEntryIndex != DataBlockHeader.BlockCount && !found)
                                            {
                                                uint uiIncrementedDataBlockIndex = BlockEntries[uiIncrementedBlockEntryIndex].FirstDataBlockIndex;

                                                if (uiIncrementedDataBlockIndex == increment)
                                                {
                                                    // The sequential data block is the first data block in a block entry,
                                                    // update the tables preserving the sequence for the file we are fragmenting.

                                                    BlockEntries[uiIncrementedBlockEntryIndex].FirstDataBlockIndex = currentDataBlockIndex;

                                                    FragmentationMaps[currentDataBlockIndex].NextDataBlockIndex = FragmentationMaps[uiIncrementedDataBlockIndex].NextDataBlockIndex;

                                                    // We found it.
                                                    found = true;
                                                    break;
                                                }
                                                else
                                                {
                                                    // The sequential data block is in the middle of a block entry,
                                                    // update the tables preserving the sequence for the file we are fragmenting.

                                                    uint uiIncrementedBlockEntrySize = 0;
                                                    uint uiIncrementedLastDataBlockIndex = DataBlockHeader.BlockCount;

                                                    while (uiIncrementedDataBlockIndex < dataBlockTerminator && uiIncrementedBlockEntrySize < BlockEntries[uiIncrementedBlockEntryIndex].FileDataSize)
                                                    {
                                                        if (uiIncrementedDataBlockIndex == increment)
                                                        {
                                                            // If the data blocks are side by side, prevent circular maps.
                                                            if (increment != uiNextDataBlockIndex)
                                                            {
                                                                FragmentationMaps[uiIncrementedLastDataBlockIndex].NextDataBlockIndex = currentDataBlockIndex;
                                                            }

                                                            FragmentationMaps[currentDataBlockIndex].NextDataBlockIndex = FragmentationMaps[uiIncrementedDataBlockIndex].NextDataBlockIndex;

                                                            // We found it.
                                                            found = true;
                                                            break;
                                                        }

                                                        uiIncrementedLastDataBlockIndex = uiIncrementedDataBlockIndex;

                                                        uiIncrementedDataBlockIndex = FragmentationMaps[uiIncrementedDataBlockIndex].NextDataBlockIndex;
                                                        uiIncrementedBlockEntrySize += DataBlockHeader.BlockSize;
                                                    }
                                                }

                                                uiIncrementedBlockEntryIndex = BlockEntries[uiIncrementedBlockEntryIndex].NextBlockEntryIndex;
                                            }
                                        }
                                    }

                                    // Swap the data blocks if necessary.
                                    if (found)
                                        Array.Copy(incrementedView.ViewData, dataBlock, DataBlockHeader.BlockSize);

                                    Array.Copy(currentView.ViewData, incrementedView.ViewData, DataBlockHeader.BlockSize);
                                    if (!Mapping.Commit(incrementedView))
                                    {
                                        // At this point the GCF will be corrupt and require validating by Steam for repair.
                                        error = true;
                                    }
                                    if (found)
                                    {
                                        Array.Copy(dataBlock, incrementedView.ViewData, DataBlockHeader.BlockSize);
                                        if (!Mapping.Commit(currentView))
                                        {
                                            // At this point the GCF will be corrupt and require validating by Steam for repair.
                                            error = true;
                                        }
                                    }

                                    // Update the tables preserving the sequence of the file we are defragmenting.
                                    if (lastDataBlockIndex == DataBlockHeader.BlockCount)
                                        BlockEntries[currentBlockEntryIndex].FirstDataBlockIndex = increment;
                                    else
                                        FragmentationMaps[lastDataBlockIndex].NextDataBlockIndex = increment;

                                    if (increment != DataBlockHeader.BlockCount)
                                    {
                                        // If the data blocks are side by side, prevent circular maps.
                                        if (increment != uiNextDataBlockIndex)
                                            FragmentationMaps[increment].NextDataBlockIndex = uiNextDataBlockIndex;
                                        else
                                            FragmentationMaps[increment].NextDataBlockIndex = currentDataBlockIndex;
                                    }
                                }
                                else
                                {
                                    error = true;
                                    break;
                                }
                            }

                            // Move to the next data block.
                            lastDataBlockIndex = increment;
                            increment++;

                            currentDataBlockIndex = FragmentationMaps[lastDataBlockIndex].NextDataBlockIndex;
                            currentBlockEntrySize += DataBlockHeader.BlockSize;

                            // Update the progress.
                            bytesDefragmented += DataBlockHeader.BlockSize;
                        }

                        if (error)
                            break;

                        // Update the tables to make sure the last data block points to nothing.
                        // It would seem that this is only necessary if there is an eror in the GCF
                        // in which case we should just let it be (Steam knows better)...
                        /*if(LastDataBlockIndex != DataBlockHeader.BlockCount)
                        {
                            FragmentationMap[LastDataBlockIndex].NextDataBlockIndex = DataBlockTerminator;
                        }*/

                        currentBlockEntryIndex = BlockEntries[currentBlockEntryIndex].NextBlockEntryIndex;
                    }

                    filesDefragmented++;
                }
            }

            if (!error)
            {
                // Store the first unused fragmentation map entry.
                if (increment < FragmentationMapHeader.BlockCount)
                {
                    // Note: usually if there are no unused data blocks, uiFirstUnusedEntry is 0, but
                    // sometimes it isn't (and I don't know why) so I'm unsure if I should set it to
                    // 0 in the else case.
                    FragmentationMapHeader.FirstUnusedEntry = increment;
                    FragmentationMapHeader.Checksum = FragmentationMapHeader.BlockCount +
                                                                FragmentationMapHeader.FirstUnusedEntry +
                                                                FragmentationMapHeader.Terminator;
                }

                // Fill in the unused fragmentation map entries with uiBlockCount.
                for (uint i = increment; i < FragmentationMapHeader.BlockCount; i++)
                {
                    FragmentationMaps[i].NextDataBlockIndex = FragmentationMapHeader.BlockCount;
                }
            }
            else
            {
                bool[] touched = new bool[FragmentationMapHeader.BlockCount];

                // Figure out which fragmentation map entries are used.
                for (uint i = 0; i < DirectoryHeader.ItemCount; i++)
                {
                    if ((DirectoryEntries[i].DirectoryFlags & HL_GCF_FLAG_FILE) != 0)
                    {
                        uint blockEntryIndex = DirectoryMapEntries[i].FirstBlockIndex;

                        while (blockEntryIndex != DataBlockHeader.BlockCount)
                        {
                            uint blockEntrySize = 0;
                            uint dataBlockIndex = BlockEntries[blockEntryIndex].FirstDataBlockIndex;

                            while (dataBlockIndex < dataBlockTerminator && blockEntrySize < BlockEntries[blockEntryIndex].FileDataSize)
                            {
                                touched[dataBlockIndex] = true;
                                dataBlockIndex = FragmentationMaps[dataBlockIndex].NextDataBlockIndex;
                                blockEntrySize += DataBlockHeader.BlockSize;
                            }

                            blockEntryIndex = BlockEntries[blockEntryIndex].NextBlockEntryIndex;
                        }
                    }
                }

                // Fill in the unused fragmentation map entries with uiBlockCount.
                bool first = false;
                for (uint i = 0; i < FragmentationMapHeader.BlockCount; i++)
                {
                    if (!touched[i])
                    {
                        if (!first)
                        {
                            // Store the first unused fragmentation map entry.
                            FragmentationMapHeader.FirstUnusedEntry = i;
                            FragmentationMapHeader.Checksum = FragmentationMapHeader.BlockCount +
                                                                        FragmentationMapHeader.FirstUnusedEntry +
                                                                        FragmentationMapHeader.Terminator;

                            first = true;
                        }
                        FragmentationMaps[i].NextDataBlockIndex = FragmentationMapHeader.BlockCount;
                    }
                }
            }

            Mapping.Unmap(ref currentView);
            Mapping.Unmap(ref incrementedView);

            // Commit header changes to mapping.
            long pointer = GCFHeader.ObjectSize;
            Mapping.Commit(HeaderView, pointer, GCFBlockEntry.ObjectSize * BlockEntryHeader.BlockCount);
            pointer += GCFBlockEntry.ObjectSize * BlockEntryHeader.BlockCount;
            Mapping.Commit(HeaderView, pointer, GCFFragmentationMapHeader.ObjectSize);
            pointer += GCFFragmentationMapHeader.ObjectSize;
            Mapping.Commit(HeaderView, pointer, GCFFragmentationMap.ObjectSize * FragmentationMapHeader.BlockCount);
            pointer += GCFFragmentationMap.ObjectSize;

            return !error;
        }

        /// <summary>
        /// Get the fragmentation level for a given item
        /// </summary>
        /// <param name="directoryItemIndex">Index of the item to check</param>
        /// <param name="blocksFragmented">Total number of fragmented blocks</param>
        /// <param name="blocksUsed">Total number of blocks used</param>
        private void GetItemFragmentation(uint directoryItemIndex, out int blocksFragmented, out int blocksUsed)
        {
            blocksFragmented = 0; blocksUsed = 0;
            if ((DirectoryEntries[directoryItemIndex].DirectoryFlags & HL_GCF_FLAG_FILE) == 0)
            {
                directoryItemIndex = DirectoryEntries[directoryItemIndex].FirstIndex;
                while (directoryItemIndex != 0 && directoryItemIndex != 0xffffffff)
                {
                    GetItemFragmentation(directoryItemIndex, out blocksFragmented, out blocksUsed);
                    directoryItemIndex = DirectoryEntries[directoryItemIndex].NextIndex;
                }
            }
            else
            {
                uint dataBlockTerminator = FragmentationMapHeader.Terminator == 0 ? 0x0000ffff : 0xffffffff;

                uint lastDataBlockIndex = DataBlockHeader.BlockCount;
                uint blockEntryIndex = DirectoryMapEntries[directoryItemIndex].FirstBlockIndex;

                while (blockEntryIndex != DataBlockHeader.BlockCount)
                {
                    uint blockEntrySize = 0;
                    uint dataBlockIndex = BlockEntries[blockEntryIndex].FirstDataBlockIndex;

                    while (dataBlockIndex < dataBlockTerminator && blockEntrySize < BlockEntries[blockEntryIndex].FileDataSize)
                    {
                        if (lastDataBlockIndex != DataBlockHeader.BlockCount && lastDataBlockIndex + 1 != dataBlockIndex)
                            blocksFragmented++;

                        blocksUsed++;
                        lastDataBlockIndex = dataBlockIndex;

                        dataBlockIndex = FragmentationMaps[dataBlockIndex].NextDataBlockIndex;

                        blockEntrySize += DataBlockHeader.BlockSize;
                    }

                    blockEntryIndex = BlockEntries[blockEntryIndex].NextBlockEntryIndex;
                }
            }
        }

        #endregion

        #region Mappings

        /// <inheritdoc/>
        protected override DirectoryFolder CreateRoot()
        {
            DirectoryItems = new DirectoryItem[DirectoryHeader.ItemCount];
            DirectoryItems[0] = new DirectoryFolder("root", 0, null, this, null);
            CreateRoot((DirectoryFolder)DirectoryItems[0]);
            return (DirectoryFolder)DirectoryItems[0];
        }

        /// <inheritdoc/>
        protected override bool MapDataStructures()
        {
            #region Determine the size of the header and validate it.

            uint version;
            long headerSize = 0;

            byte[] headerViewData = HeaderView.ViewData;
            int pointer = 0;

            #region Header

            if (GCFHeader.ObjectSize > Mapping.MappingSize)
            {
                Console.WriteLine("Invalid file: the file map is too small for it's header.");
                return false;
            }

            if (!Mapping.Map(ref HeaderView, 0, GCFHeader.ObjectSize))
                return false;

            Header = GCFHeader.Create(headerViewData, ref pointer);
            if (Header == null)
            {
                Console.WriteLine("Invalid file: the file's header is null (contains no data).");
                return false;
            }

            if (Header.MajorVersion != 1 || (Header.MinorVersion != 3 && Header.MinorVersion != 5 && Header.MinorVersion != 6))
            {
                Console.WriteLine($"Invalid GCF version (v{Header.MinorVersion}): you have a version of a GCF file that HLLib does not know how to read. Check for product updates.");
                return false;
            }

            version = Header.MinorVersion;
            headerSize += GCFHeader.ObjectSize;

            #endregion

            #region Block entries

            if (!Mapping.Map(ref HeaderView, headerSize, GCFBlockEntryHeader.ObjectSize))
                return false;

            BlockEntryHeader = GCFBlockEntryHeader.Create(headerViewData, ref pointer);
            headerSize += GCFBlockEntryHeader.ObjectSize + (BlockEntryHeader.BlockCount * GCFBlockEntry.ObjectSize);

            #endregion

            #region Fragmentation map

            if (!Mapping.Map(ref HeaderView, headerSize, GCFFragmentationMapHeader.ObjectSize))
                return false;

            FragmentationMapHeader = GCFFragmentationMapHeader.Create(headerViewData, ref pointer);
            headerSize += GCFFragmentationMapHeader.ObjectSize + (FragmentationMapHeader.BlockCount * GCFFragmentationMap.ObjectSize);

            #endregion

            #region Block entry map

            if (version < 6)
            {
                if (!Mapping.Map(ref HeaderView, headerSize, GCFBlockEntryMapHeader.ObjectSize))
                    return false;

                BlockEntryMapHeader = GCFBlockEntryMapHeader.Create(headerViewData, ref pointer);
                headerSize += GCFBlockEntryMapHeader.ObjectSize + (BlockEntryMapHeader.BlockCount * GCFBlockEntryMap.ObjectSize);
            }

            #endregion

            #region Directory

            if (!Mapping.Map(ref HeaderView, headerSize, GCFDirectoryHeader.ObjectSize))
                return false;

            DirectoryHeader = GCFDirectoryHeader.Create(headerViewData, ref pointer);

            headerSize += DirectoryHeader.DirectorySize;
            if (version >= 5)
                headerSize += GCFDirectoryHeader.ObjectSize;

            headerSize += DirectoryHeader.ItemCount * GCFDirectoryMapEntry.ObjectSize;

            #endregion

            #region Checksums

            if (!Mapping.Map(ref HeaderView, headerSize, GCFChecksumHeader.ObjectSize))
                return false;

            ChecksumHeader = GCFChecksumHeader.Create(headerViewData, ref pointer);
            headerSize += GCFChecksumHeader.ObjectSize + ChecksumHeader.ChecksumSize;

            #endregion

            #region Data blocks

            if (!Mapping.Map(ref HeaderView, headerSize, GCFDataBlockHeader.ObjectSize))
                return false;

            DataBlockHeader = GCFDataBlockHeader.Create(headerViewData, ref pointer);

            // It seems that some GCF files may have allocated only the data blocks that are used.  Extraction,
            // validation and defragmentation should fail cleanly if an unallocated data block is indexed so
            // leave this check out for now.
            /*if(DataBlockHeader.FirstBlockOffset + DataBlockHeader.BlocksUsed * DataBlockHeader.BlockSize > Mapping.GetMappingSize())
            {
                Console.WriteLine("Invalid file: the file map is too small for it's data blocks.");
                return false;
            }*/

            #endregion

            // See note below.
            if (version < 5)
                headerSize += GCFDataBlockHeader.ObjectSize - 4;
            else
                headerSize += GCFDataBlockHeader.ObjectSize;

            #endregion

            #region Map the header.

            if (!Mapping.Map(ref HeaderView, 0, (int)headerSize))
                return false;

            pointer = 0;

            Header = GCFHeader.Create(headerViewData, ref pointer);

            BlockEntryHeader = GCFBlockEntryHeader.Create(headerViewData, ref pointer);
            BlockEntries = new GCFBlockEntry[BlockEntryHeader.BlockCount];
            for (int i = 0; i < BlockEntryHeader.BlockCount; i++)
            {
                BlockEntries[i] = GCFBlockEntry.Create(headerViewData, ref pointer);
            }

            FragmentationMapHeader = GCFFragmentationMapHeader.Create(headerViewData, ref pointer);
            FragmentationMaps = new GCFFragmentationMap[FragmentationMapHeader.BlockCount];
            for (int i = 0; i < FragmentationMapHeader.BlockCount; i++)
            {
                FragmentationMaps[i] = GCFFragmentationMap.Create(headerViewData, ref pointer);
            }

            if (version < 6)
            {
                BlockEntryMapHeader = GCFBlockEntryMapHeader.Create(headerViewData, ref pointer);
                BlockEntryMaps = new GCFBlockEntryMap[BlockEntryMapHeader.BlockCount];
                for (int i = 0; i < BlockEntryMapHeader.BlockCount; i++)
                {
                    BlockEntryMaps[i] = GCFBlockEntryMap.Create(headerViewData, ref pointer);
                }
            }
            else
            {
                BlockEntryMapHeader = null;
                BlockEntryMaps = null;
            }

            int directoryHeaderIndex = pointer;
            DirectoryHeader = GCFDirectoryHeader.Create(headerViewData, ref pointer);
            DirectoryEntries = new GCFDirectoryEntry[DirectoryHeader.ItemCount];
            for (int i = 0; i < DirectoryHeader.ItemCount; i++)
            {
                DirectoryEntries[i] = GCFDirectoryEntry.Create(headerViewData, ref pointer);
            }

            DirectoryNames = Encoding.ASCII.GetString(headerViewData, pointer, (int)DirectoryHeader.NameSize); pointer += (int)DirectoryHeader.NameSize;

            DirectoryInfo1Entries = new GCFDirectoryInfo1Entry[DirectoryHeader.Info1Count];
            for (int i = 0; i < DirectoryHeader.Info1Count; i++)
            {
                DirectoryInfo1Entries[i] = GCFDirectoryInfo1Entry.Create(headerViewData, ref pointer);
            }

            DirectoryInfo2Entries = new GCFDirectoryInfo2Entry[DirectoryHeader.ItemCount];
            for (int i = 0; i < DirectoryHeader.ItemCount; i++)
            {
                DirectoryInfo2Entries[i] = GCFDirectoryInfo2Entry.Create(headerViewData, ref pointer);
            }

            DirectoryCopyEntries = new GCFDirectoryCopyEntry[DirectoryHeader.CopyCount];
            for (int i = 0; i < DirectoryHeader.CopyCount; i++)
            {
                DirectoryCopyEntries[i] = GCFDirectoryCopyEntry.Create(headerViewData, ref pointer);
            }

            DirectoryLocalEntries = new GCFDirectoryLocalEntry[DirectoryHeader.LocalCount];
            for (int i = 0; i < DirectoryHeader.LocalCount; i++)
            {
                DirectoryLocalEntries[i] = GCFDirectoryLocalEntry.Create(headerViewData, ref pointer);
            }

            pointer = directoryHeaderIndex + (int)DirectoryHeader.DirectorySize;
            if (version < 5)
                DirectoryMapHeader = null;
            else
                DirectoryMapHeader = GCFDirectoryMapHeader.Create(headerViewData, ref pointer);

            DirectoryMapEntries = new GCFDirectoryMapEntry[DirectoryHeader.ItemCount];
            for (int i = 0; i < DirectoryHeader.ItemCount; i++)
            {
                DirectoryMapEntries[i] = GCFDirectoryMapEntry.Create(headerViewData, ref pointer);
            }

            ChecksumHeader = GCFChecksumHeader.Create(headerViewData, ref pointer);

            int checksumMapHeaderIndex = pointer;
            ChecksumMapHeader = GCFChecksumMapHeader.Create(headerViewData, ref pointer);

            ChecksumMapEntries = new GCFChecksumMapEntry[ChecksumMapHeader.ItemCount];
            for (int i = 0; i < ChecksumMapHeader.ItemCount; i++)
            {
                ChecksumMapEntries[i] = GCFChecksumMapEntry.Create(headerViewData, ref pointer);
            }

            ChecksumEntries = new GCFChecksumEntry[ChecksumMapHeader.ChecksumCount];
            for (int i = 0; i < ChecksumMapHeader.ChecksumCount; i++)
            {
                ChecksumEntries[i] = GCFChecksumEntry.Create(headerViewData, ref pointer);
            }

            pointer = checksumMapHeaderIndex + (int)ChecksumHeader.ChecksumSize;

            // In version 3 the GCFDataBlockHeader is missing the uiLastVersionPlayed field.
            // The below hack makes the file map correctly.
            if (version < 5)
                pointer -= 4;

            DataBlockHeader = GCFDataBlockHeader.Create(headerViewData, ref pointer);

            #endregion

            return true;
        }

        /// <inheritdoc/>
        protected override void UnmapDataStructures()
        {
            DirectoryItems = null;

            Header = null;

            BlockEntryHeader = null;
            BlockEntries = null;

            FragmentationMapHeader = null;
            FragmentationMaps = null;

            BlockEntryMapHeader = null;
            BlockEntryMaps = null;

            DirectoryHeader = null;
            DirectoryEntries = null;
            DirectoryNames = null;
            DirectoryInfo1Entries = null;
            DirectoryInfo2Entries = null;
            DirectoryCopyEntries = null;
            DirectoryLocalEntries = null;

            DirectoryMapHeader = null;
            DirectoryMapEntries = null;

            ChecksumHeader = null;
            ChecksumMapHeader = null;
            ChecksumMapEntries = null;
            ChecksumEntries = null;

            DataBlockHeader = null;

            Mapping.Unmap(ref HeaderView);
        }

        /// <summary>
        /// Create the root directory for a single directory
        /// </summary>
        /// <param name="folder">Directory to create the root for</param>
        private void CreateRoot(DirectoryFolder folder)
        {
            // Get the first directory item.
            uint index = DirectoryEntries[folder.ID].FirstIndex;

            // Loop through directory items.
            while (index != 0 && index != 0xffffffff)
            {
                // Check if the item is a folder.
                if ((DirectoryEntries[index].DirectoryFlags & HL_GCF_FLAG_FILE) == 0)
                {
                    // Add the directory item to the current folder.
                    DirectoryItems[index] = folder.AddFolder(DirectoryName((int)DirectoryEntries[index].NameOffset), index);

                    // Build the new folder.
                    CreateRoot((DirectoryFolder)DirectoryItems[index]);
                }
                else
                {
                    // Add the directory item to the current folder.
                    DirectoryItems[index] = folder.AddFile(DirectoryName((int)DirectoryEntries[index].NameOffset), index);
                }

                // Get the next directory item.
                index = DirectoryEntries[index].NextIndex;
            }
        }

        #endregion

        #region Attributes

        /// <inheritdoc/>
        protected override bool GetAttributeInternal(PackageAttributeType packageAttribute, out PackageAttribute attribute)
        {
            attribute = new PackageAttribute();
            switch (packageAttribute)
            {
                case PackageAttributeType.HL_GCF_PACKAGE_VERSION:
                    attribute.SetUnsignedInteger(AttributeNames[(int)packageAttribute], Header.MinorVersion, false);
                    return true;
                case PackageAttributeType.HL_GCF_PACKAGE_ID:
                    attribute.SetUnsignedInteger(AttributeNames[(int)packageAttribute], Header.CacheID, false);
                    return true;
                case PackageAttributeType.HL_GCF_PACKAGE_ALLOCATED_BLOCKS:
                    attribute.SetUnsignedInteger(AttributeNames[(int)packageAttribute], DataBlockHeader.BlockCount, false);
                    return true;
                case PackageAttributeType.HL_GCF_PACKAGE_USED_BLOCKS:
                    attribute.SetUnsignedInteger(AttributeNames[(int)packageAttribute], DataBlockHeader.BlocksUsed, false);
                    return true;
                case PackageAttributeType.HL_GCF_PACKAGE_BLOCK_LENGTH:
                    attribute.SetUnsignedInteger(AttributeNames[(int)packageAttribute], DataBlockHeader.BlockSize, false);
                    return true;
                case PackageAttributeType.HL_GCF_PACKAGE_LAST_VERSION_PLAYED:
                    attribute.SetUnsignedInteger(AttributeNames[(int)packageAttribute], Header.LastVersionPlayed, false);
                    return true;
                default:
                    return false;
            }
        }

        /// <inheritdoc/>
        protected override bool GetItemAttributeInternal(DirectoryItem item, PackageAttributeType packageAttribute, out PackageAttribute attribute)
        {
            attribute = new PackageAttribute();
            switch (item.ItemType)
            {
                case DirectoryItemType.HL_ITEM_FILE:
                    DirectoryFile file = (DirectoryFile)item;
                    switch (packageAttribute)
                    {
                        case PackageAttributeType.HL_GCF_ITEM_ENCRYPTED:
                            attribute.SetBoolean(ItemAttributeNames[(int)packageAttribute], (DirectoryEntries[file.ID].DirectoryFlags & HL_GCF_FLAG_ENCRYPTED) != 0);
                            return true;
                        case PackageAttributeType.HL_GCF_ITEM_COPY_LOCAL:
                            attribute.SetBoolean(ItemAttributeNames[(int)packageAttribute], (DirectoryEntries[file.ID].DirectoryFlags & HL_GCF_FLAG_COPY_LOCAL) != 0);
                            return true;
                        case PackageAttributeType.HL_GCF_ITEM_OVERWRITE_LOCAL:
                            attribute.SetBoolean(ItemAttributeNames[(int)packageAttribute], (DirectoryEntries[file.ID].DirectoryFlags & HL_GCF_FLAG_COPY_LOCAL_NO_OVERWRITE) == 0);
                            return true;
                        case PackageAttributeType.HL_GCF_ITEM_BACKUP_LOCAL:
                            attribute.SetBoolean(ItemAttributeNames[(int)packageAttribute], (DirectoryEntries[file.ID].DirectoryFlags & HL_GCF_FLAG_BACKUP_LOCAL) != 0);
                            return true;
                        case PackageAttributeType.HL_GCF_ITEM_FLAGS:
                            attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], DirectoryEntries[file.ID].DirectoryFlags, true);
                            return true;
                        case PackageAttributeType.HL_GCF_ITEM_FRAGMENTATION:
                            GetItemFragmentation(file.ID, out int blocksFragmented, out int blocksUsed);

                            if (blocksUsed == 0)
                                attribute.SetFloat(ItemAttributeNames[(int)packageAttribute], 0.0f);
                            else
                                attribute.SetFloat(ItemAttributeNames[(int)packageAttribute], (blocksFragmented / blocksUsed) * 100.0f);

                            return true;
                    }
                    break;
                case DirectoryItemType.HL_ITEM_FOLDER:
                    DirectoryFolder folder = (DirectoryFolder)item;
                    switch (packageAttribute)
                    {
                        case PackageAttributeType.HL_GCF_ITEM_FLAGS:
                            attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], DirectoryEntries[folder.ID].DirectoryFlags, true);
                            return true;
                        case PackageAttributeType.HL_GCF_ITEM_FRAGMENTATION:
                            GetItemFragmentation(folder.ID, out int blocksFragmented, out int blocksUsed);

                            if (blocksUsed == 0)
                                attribute.SetFloat(ItemAttributeNames[(int)packageAttribute], 0.0f);
                            else
                                attribute.SetFloat(ItemAttributeNames[(int)packageAttribute], (blocksFragmented / blocksUsed) * 100.0f);

                            return true;
                    }
                    break;
            }

            return false;
        }

        #endregion

        #region File Extraction Check

        /// <inheritdoc/>
        protected override bool GetFileExtractableInternal(DirectoryFile file, out bool extractable)
        {
            if ((DirectoryEntries[file.ID].DirectoryFlags & HL_GCF_FLAG_ENCRYPTED) != 0)
            {
                extractable = false;
            }
            else
            {
                // Do we have enough data to extract?
                uint size = 0;

                // Get the first data block.
                uint blockEntryIndex = DirectoryMapEntries[file.ID].FirstBlockIndex;

                // Loop through each data block.
                while (blockEntryIndex != DataBlockHeader.BlockCount)
                {
                    size += BlockEntries[blockEntryIndex].FileDataSize;

                    // Get the next data block.
                    blockEntryIndex = BlockEntries[blockEntryIndex].NextBlockEntryIndex;
                }

                extractable = size >= DirectoryEntries[file.ID].ItemSize;
            }

            return true;
        }

        #endregion

        #region File Validation

        /// <inheritdoc/>
        protected override bool GetFileValidationInternal(DirectoryFile file, out Validation validation)
        {
            // Do we have enough data to validate?
            {
                uint uiSize = 0;

                // Get the first data block.
                uint uiBlockEntryIndex = DirectoryMapEntries[file.ID].FirstBlockIndex;

                // Loop through each data block.
                while (uiBlockEntryIndex != DataBlockHeader.BlockCount)
                {
                    uiSize += BlockEntries[uiBlockEntryIndex].FileDataSize;

                    // Get the next data block.
                    uiBlockEntryIndex = BlockEntries[uiBlockEntryIndex].NextBlockEntryIndex;
                }

                if (uiSize != DirectoryEntries[file.ID].ItemSize)
                {
                    // File is incomplete.
                    validation = Validation.HL_VALIDATES_INCOMPLETE;
                    return true;
                }
            }

            if ((DirectoryEntries[file.ID].DirectoryFlags & HL_GCF_FLAG_ENCRYPTED) != 0)
            {
                // No way of checking, assume it's ok.
                validation = Validation.HL_VALIDATES_ASSUMED_OK;
                return true;
            }

            // File has no checksum.
            if (DirectoryEntries[file.ID].ChecksumIndex == 0xffffffff)
            {
                validation = Validation.HL_VALIDATES_ASSUMED_OK;
                return true;
            }

            if (CreateStreamInternal(file, true, out Stream stream))
            {
                if (stream.Open(FileModeFlags.HL_MODE_READ))
                {
                    validation = Validation.HL_VALIDATES_OK;

                    long totalBytes = 0;
                    int bufferSize;
                    byte[] buffer = new byte[HL_GCF_CHECKSUM_LENGTH];

                    GCFChecksumMapEntry checksumMapEntry = ChecksumMapEntries[DirectoryEntries[file.ID].ChecksumIndex];

                    int i = 0;
                    while ((bufferSize = stream.Read(buffer, 0, HL_GCF_CHECKSUM_LENGTH)) != 0)
                    {
                        if (i >= checksumMapEntry.ChecksumCount)
                        {
                            // Something bad happened.
                            validation = Validation.HL_VALIDATES_ERROR;
                            break;
                        }

                        uint checksum = (uint)(Checksum.Adler32(buffer, bufferSize) ^ Checksum.CRC32(buffer, bufferSize));
                        if (checksum != ChecksumEntries[checksumMapEntry.FirstChecksumIndex + i].Checksum)
                        {
                            validation = Validation.HL_VALIDATES_CORRUPT;
                            break;
                        }

                        totalBytes += bufferSize;

                        i++;
                    }

                    stream.Close();
                }
                else
                {
                    validation = Validation.HL_VALIDATES_ERROR;
                }

                ReleaseStream(stream);
            }
            else
            {
                validation = Validation.HL_VALIDATES_ERROR;
            }

            return true;
        }

        #endregion

        #region File Size

        /// <inheritdoc/>
        protected override bool GetFileSizeInternal(DirectoryFile file, out int size)
        {
            size = (int)DirectoryEntries[file.ID].ItemSize;
            return true;
        }

        /// <inheritdoc/>
        protected override bool GetFileSizeOnDiskInternal(DirectoryFile file, out int size)
        {
            // Get the first data block.
            uint blockEntryIndex = DirectoryMapEntries[file.ID].FirstBlockIndex;

            // Loop through each data block.
            size = 0;
            while (blockEntryIndex != DataBlockHeader.BlockCount)
            {
                size += (int)(((BlockEntries[blockEntryIndex].FileDataSize + DataBlockHeader.BlockSize - 1) / DataBlockHeader.BlockSize) * DataBlockHeader.BlockSize);

                // Get the next data block.
                blockEntryIndex = BlockEntries[blockEntryIndex].NextBlockEntryIndex;
            }

            return true;
        }

        #endregion

        #region Streams

        /// <inheritdoc/>
        protected override bool CreateStreamInternal(DirectoryFile file, bool readEncrypted, out Stream stream)
        {
            stream = null;
            if (!readEncrypted && (DirectoryEntries[file.ID].DirectoryFlags & HL_GCF_FLAG_ENCRYPTED) != 0)
            {
                Console.WriteLine("File is encrypted.");
                return false;
            }

            stream = new GCFStream(this, file.ID);
            return true;
        }

        #endregion
    }
}