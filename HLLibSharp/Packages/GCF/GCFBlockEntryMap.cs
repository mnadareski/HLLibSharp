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
    public class GCFBlockEntryMap
    {
        /// <summary>
        /// Total size of a GCFBlockEntryMap object
        /// </summary>
        public const int ObjectSize = 4 + 4;

        /// <summary>
        /// The previous block entry.  (N/A if == BlockCount.)
        /// </summary>
        public uint PreviousBlockEntryIndex { get; set; }

        /// <summary>
        /// The next block entry.  (N/A if == BlockCount.)
        /// </summary>
        public uint NextBlockEntryIndex { get; set; }

        public static GCFBlockEntryMap Create(byte[] data, ref int offset)
        {
            GCFBlockEntryMap blockEntryMap = new GCFBlockEntryMap();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            blockEntryMap.PreviousBlockEntryIndex = BitConverter.ToUInt32(data, offset); offset += 4;
            blockEntryMap.NextBlockEntryIndex = BitConverter.ToUInt32(data, offset); offset += 4;

            return blockEntryMap;
        }
    }
}