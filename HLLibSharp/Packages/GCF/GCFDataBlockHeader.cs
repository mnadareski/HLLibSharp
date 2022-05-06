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
    public class GCFDataBlockHeader
    {
        /// <summary>
        /// GCF file version.  This field is not part of all file versions.
        /// </summary>
        public uint LastVersionPlayed { get; set; }

        /// <summary>
        /// Number of data blocks.
        /// </summary>
        public uint BlockCount { get; set; }

        /// <summary>
        /// Size of each data block in bytes.
        /// </summary>
        public uint BlockSize { get; set; }

        /// <summary>
        /// Offset to first data block.
        /// </summary>
        public uint FirstBlockOffset { get; set; }

        /// <summary>
        /// Number of data blocks that contain data.
        /// </summary>
        public uint BlocksUsed { get; set; }

        /// <summary>
        /// Header checksum.
        /// </summary>
        public uint Checksum { get; set; }

        public static GCFDataBlockHeader Create(byte[] data, ref int offset)
        {
            GCFDataBlockHeader dataBlockHeader = new GCFDataBlockHeader();

            // Check to see if the data is valid
            if (data == null || data.Length < Marshal.SizeOf(dataBlockHeader))
                return null;

            dataBlockHeader.LastVersionPlayed = BitConverter.ToUInt32(data, offset); offset += 4;
            dataBlockHeader.BlockCount = BitConverter.ToUInt32(data, offset); offset += 4;
            dataBlockHeader.BlockSize = BitConverter.ToUInt32(data, offset); offset += 4;
            dataBlockHeader.FirstBlockOffset = BitConverter.ToUInt32(data, offset); offset += 4;
            dataBlockHeader.BlocksUsed = BitConverter.ToUInt32(data, offset); offset += 4;
            dataBlockHeader.Checksum = BitConverter.ToUInt32(data, offset); offset += 4;

            return dataBlockHeader;
        }
    }
}