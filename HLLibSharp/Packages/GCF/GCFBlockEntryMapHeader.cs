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
    /// <remarks>
    /// Part of version 5 but not version 6.
    /// </remarks>
    public sealed class GCFBlockEntryMapHeader
    {
        /// <summary>
        /// Total size of a GCFBlockEntryMapHeader object
        /// </summary>
        public const int ObjectSize = (4 * 5);

        /// <summary>
        /// Number of data blocks.
        /// </summary>
        public uint BlockCount { get; set; }

        /// <summary>
        /// Index of the first block entry.
        /// </summary>
        public uint FirstBlockEntryIndex { get; set; }

        /// <summary>
        /// Index of the last block entry.
        /// </summary>
        public uint LastBlockEntryIndex { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy0 { get; set; }

        /// <summary>
        /// Header checksum.
        /// </summary>
        public uint Checksum { get; set; }

        public static GCFBlockEntryMapHeader Create(byte[] data, ref int offset)
        {
            GCFBlockEntryMapHeader blockEntryMapHeader = new GCFBlockEntryMapHeader();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            blockEntryMapHeader.BlockCount = BitConverter.ToUInt32(data, offset); offset += 4;
            blockEntryMapHeader.FirstBlockEntryIndex = BitConverter.ToUInt32(data, offset); offset += 4;
            blockEntryMapHeader.LastBlockEntryIndex = BitConverter.ToUInt32(data, offset); offset += 4;
            blockEntryMapHeader.Dummy0 = BitConverter.ToUInt32(data, offset); offset += 4;
            blockEntryMapHeader.Checksum = BitConverter.ToUInt32(data, offset); offset += 4;

            return blockEntryMapHeader;
        }
    }
}