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
    public sealed class DataBlockHeader
    {
        /// <summary>
        /// GCF file version.  This field is not part of all file versions.
        /// </summary>
        public uint LastVersionPlayed;

        /// <summary>
        /// Number of data blocks.
        /// </summary>
        public uint BlockCount;

        /// <summary>
        /// Size of each data block in bytes.
        /// </summary>
        public uint BlockSize;

        /// <summary>
        /// Offset to first data block.
        /// </summary>
        public uint FirstBlockOffset;

        /// <summary>
        /// Number of data blocks that contain data.
        /// </summary>
        public uint BlocksUsed;

        /// <summary>
        /// Header checksum.
        /// </summary>
        public uint Checksum;
    }
}