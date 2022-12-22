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
    public sealed class BlockEntryHeader
    {
        /// <summary>
        /// Number of data blocks.
        /// </summary>
        public uint BlockCount;

        /// <summary>
        /// Number of data blocks that point to data.
        /// </summary>
        public uint BlocksUsed;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy0;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy1;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy2;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy3;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy4;

        /// <summary>
        /// Header checksum.
        /// </summary>
        public uint Checksum;
    }
}