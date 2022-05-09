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
    public sealed class GCFChecksumMapEntry
    {
        /// <summary>
        /// Total size of a GCFChecksumMapEntry object
        /// </summary>
        public const int ObjectSize = (4 * 2);

        /// <summary>
        /// Number of checksums.
        /// </summary>
        public uint ChecksumCount { get; set; }

        /// <summary>
        /// Index of first checksum.
        /// </summary>
        public uint FirstChecksumIndex { get; set; }

        public static GCFChecksumMapEntry Create(byte[] data, ref int offset)
        {
            GCFChecksumMapEntry checksumMapEntry = new GCFChecksumMapEntry();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            checksumMapEntry.ChecksumCount = BitConverter.ToUInt32(data, offset); offset += 4;
            checksumMapEntry.FirstChecksumIndex = BitConverter.ToUInt32(data, offset); offset += 4;

            return checksumMapEntry;
        }
    }
}