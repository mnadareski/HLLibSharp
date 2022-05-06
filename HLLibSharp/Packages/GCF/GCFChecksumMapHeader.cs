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
    public class GCFChecksumMapHeader
    {
        /// <summary>
        /// Always 0x14893721
        /// </summary>
        public uint Dummy0 { get; set; }

        /// <summary>
        /// Always 0x00000001
        /// </summary>
        public uint Dummy1 { get; set; }

        /// <summary>
        /// Number of items.
        /// </summary>
        public uint ItemCount { get; set; }

        /// <summary>
        /// Number of checksums.
        /// </summary>
        public uint ChecksumCount { get; set; }

        public static GCFChecksumMapHeader Create(byte[] data, ref int offset)
        {
            GCFChecksumMapHeader checksumMapHeader = new GCFChecksumMapHeader();

            // Check to see if the data is valid
            if (data == null || data.Length < Marshal.SizeOf(checksumMapHeader))
                return null;

            checksumMapHeader.Dummy0 = BitConverter.ToUInt32(data, offset); offset += 4;
            checksumMapHeader.Dummy1 = BitConverter.ToUInt32(data, offset); offset += 4;
            checksumMapHeader.ItemCount = BitConverter.ToUInt32(data, offset); offset += 4;
            checksumMapHeader.ChecksumCount = BitConverter.ToUInt32(data, offset); offset += 4;

            return checksumMapHeader;
        }
    }
}