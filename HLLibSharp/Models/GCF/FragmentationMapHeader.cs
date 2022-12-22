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
    public sealed class FragmentationMapHeader
    {
        /// <summary>
        /// Number of data blocks.
        /// </summary>
        public uint BlockCount;

        /// <summary>
        /// The index of the first unused fragmentation map entry.
        /// </summary>
        public uint FirstUnusedEntry;

        /// <summary>
        /// The block entry terminator; 0 = 0x0000ffff or 1 = 0xffffffff.
        /// </summary>
        public uint Terminator;

        /// <summary>
        /// Header checksum.
        /// </summary>
        public uint Checksum;
    }
}