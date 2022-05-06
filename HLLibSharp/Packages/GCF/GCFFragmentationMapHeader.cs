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
    public class GCFFragmentationMapHeader
    {
        /// <summary>
        /// Number of data blocks.
        /// </summary>
        public uint BlockCount { get; set; }

        /// <summary>
        /// The index of the first unused fragmentation map entry.
        /// </summary>
        public uint FirstUnusedEntry { get; set; }

        /// <summary>
        /// The block entry terminator; 0 = 0x0000ffff or 1 = 0xffffffff.
        /// </summary>
        public uint Terminator { get; set; }

        /// <summary>
        /// Header checksum.
        /// </summary>
        public uint Checksum { get; set; }

        public static GCFFragmentationMapHeader Create(byte[] data, ref int offset)
        {
            GCFFragmentationMapHeader fragmentationMapHeader = new GCFFragmentationMapHeader();

            // Check to see if the data is valid
            if (data == null || data.Length < Marshal.SizeOf(fragmentationMapHeader))
                return null;

            fragmentationMapHeader.BlockCount = BitConverter.ToUInt32(data, offset); offset += 4;
            fragmentationMapHeader.FirstUnusedEntry = BitConverter.ToUInt32(data, offset); offset += 4;
            fragmentationMapHeader.Terminator = BitConverter.ToUInt32(data, offset); offset += 4;
            fragmentationMapHeader.Checksum = BitConverter.ToUInt32(data, offset); offset += 4;

            return fragmentationMapHeader;
        }
    }
}