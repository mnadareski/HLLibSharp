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
using System.Runtime.InteropServices;

namespace HLLib.Packages.GCF
{
    public class GCFBlockEntryHeader
    {
        /// <summary>
        /// Number of data blocks.
        /// </summary>
        public uint BlockCount { get; set; }

        /// <summary>
        /// Number of data blocks that point to data.
        /// </summary>
        public uint BlocksUsed { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy0 { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy1 { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy2 { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy3 { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy4 { get; set; }

        /// <summary>
        /// Header checksum.
        /// </summary>
        public uint Checksum { get; set; }

        public static GCFBlockEntryHeader Create(byte[] data, ref int offset)
        {
            GCFBlockEntryHeader blockEntryHeader = new GCFBlockEntryHeader();

            // Check to see if the data is valid
            if (data == null || data.Length < Marshal.SizeOf(blockEntryHeader))
                return null;

            blockEntryHeader.BlockCount = BitConverter.ToUInt32(data, offset); offset += 4;
            blockEntryHeader.BlocksUsed = BitConverter.ToUInt32(data, offset); offset += 4;
            blockEntryHeader.Dummy0 = BitConverter.ToUInt32(data, offset); offset += 4;
            blockEntryHeader.Dummy1 = BitConverter.ToUInt32(data, offset); offset += 4;
            blockEntryHeader.Dummy2 = BitConverter.ToUInt32(data, offset); offset += 4;
            blockEntryHeader.Dummy3 = BitConverter.ToUInt32(data, offset); offset += 4;
            blockEntryHeader.Dummy4 = BitConverter.ToUInt32(data, offset); offset += 4;
            blockEntryHeader.Checksum = BitConverter.ToUInt32(data, offset); offset += 4;

            return blockEntryHeader;
        }
    }
}