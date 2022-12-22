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
    /// <remarks>
    /// Part of version 5 but not version 6.
    /// </remarks>
    public sealed class BlockEntryMapHeader
    {
        /// <summary>
        /// Number of data blocks.
        /// </summary>
        public uint BlockCount;

        /// <summary>
        /// Index of the first block entry.
        /// </summary>
        public uint FirstBlockEntryIndex;

        /// <summary>
        /// Index of the last block entry.
        /// </summary>
        public uint LastBlockEntryIndex;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy0;

        /// <summary>
        /// Header checksum.
        /// </summary>
        public uint Checksum;
    }
}