/*
 * HLLib
 * Copyright (C) 2006-2010 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

namespace HLLib.Models.GCF
{
    public sealed class BlockEntry
    {
        /// <summary>
        /// Flags for the block entry.  0x200F0000 == Not used.
        /// </summary>
        public uint EntryFlags;

        /// <summary>
        /// The offset for the data contained in this block entry in the file.
        /// </summary>
        public uint FileDataOffset;

        /// <summary>
        /// The length of the data in this block entry.
        /// </summary>
        public uint FileDataSize;

        /// <summary>
        /// The offset to the first data block of this block entry's data.
        /// </summary>
        public uint FirstDataBlockIndex;

        /// <summary>
        /// The next block entry in the series.  (N/A if == BlockCount.)
        /// </summary>
        public uint NextBlockEntryIndex;

        /// <summary>
        /// The previous block entry in the series.  (N/A if == BlockCount.)
        /// </summary>
        public uint PreviousBlockEntryIndex;

        /// <summary>
        /// The offset of the block entry in the directory.
        /// </summary>
        public uint DirectoryIndex;
    }
}