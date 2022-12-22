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
    public sealed class Header
    {
        /// <summary>
        /// Always 0x00000001
        /// </summary>
        public uint Dummy0;

        /// <summary>
        /// Always 0x00000001
        /// </summary>
        public uint MajorVersion;

        /// <summary>
        /// GCF version number.
        /// </summary>
        public uint MinorVersion;

        /// <summary>
        /// Cache ID
        /// </summary>
        public uint CacheID;

        /// <summary>
        /// Last version played
        /// </summary>
        public uint LastVersionPlayed;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy1;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy2;

        /// <summary>
        /// Total size of GCF file in bytes.
        /// </summary>
        public uint FileSize;

        /// <summary>
        /// Size of each data block in bytes.
        /// </summary>
        public uint BlockSize;

        /// <summary>
        /// Number of data blocks.
        /// </summary>
        public uint BlockCount;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy3;
    }
}