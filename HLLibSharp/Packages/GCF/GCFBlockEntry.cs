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

namespace HLLib.Packages.GCF
{
    public sealed class GCFBlockEntry
    {
        /// <summary>
        /// Total size of a GCFBlockEntry object
        /// </summary>
        public const int ObjectSize = (4 * 7);

        /// <summary>
        /// Flags for the block entry.  0x200F0000 == Not used.
        /// </summary>
        public uint EntryFlags { get; set; }

        /// <summary>
        /// The offset for the data contained in this block entry in the file.
        /// </summary>
        public uint FileDataOffset { get; set; }

        /// <summary>
        /// The length of the data in this block entry.
        /// </summary>
        public uint FileDataSize { get; set; }

        /// <summary>
        /// The offset to the first data block of this block entry's data.
        /// </summary>
        public uint FirstDataBlockIndex { get; set; }

        /// <summary>
        /// The next block entry in the series.  (N/A if == BlockCount.)
        /// </summary>
        public uint NextBlockEntryIndex { get; set; }

        /// <summary>
        /// The previous block entry in the series.  (N/A if == BlockCount.)
        /// </summary>
        public uint PreviousBlockEntryIndex { get; set; }

        /// <summary>
        /// The offset of the block entry in the directory.
        /// </summary>
        public uint DirectoryIndex { get; set; }

        public static GCFBlockEntry Create(byte[] data, ref int offset)
        {
            GCFBlockEntry blockEntry = new GCFBlockEntry();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            blockEntry.EntryFlags = BitConverter.ToUInt32(data, offset); offset += 4;
            blockEntry.FileDataOffset = BitConverter.ToUInt32(data, offset); offset += 4;
            blockEntry.FileDataSize = BitConverter.ToUInt32(data, offset); offset += 4;
            blockEntry.FirstDataBlockIndex = BitConverter.ToUInt32(data, offset); offset += 4;
            blockEntry.NextBlockEntryIndex = BitConverter.ToUInt32(data, offset); offset += 4;
            blockEntry.PreviousBlockEntryIndex = BitConverter.ToUInt32(data, offset); offset += 4;
            blockEntry.DirectoryIndex = BitConverter.ToUInt32(data, offset); offset += 4;

            return blockEntry;
        }
    }
}