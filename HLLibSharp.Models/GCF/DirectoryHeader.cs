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
    public sealed class DirectoryHeader
    {
        /// <summary>
        /// Always 0x00000004
        /// </summary>
        public uint Dummy0;

        /// <summary>
        /// Cache ID.
        /// </summary>
        public uint CacheID;

        /// <summary>
        /// GCF file version.
        /// </summary>
        public uint LastVersionPlayed;

        /// <summary>
        /// Number of items in the directory.
        /// </summary>
        public uint ItemCount;

        /// <summary>
        /// Number of files in the directory.
        /// </summary>
        public uint FileCount;

        /// <summary>
        /// Always 0x00008000.  Data per checksum?
        /// </summary>
        public uint Dummy1;

        /// <summary>
        /// Size of lpGCFDirectoryEntries & lpGCFDirectoryNames & lpGCFDirectoryInfo1Entries & lpGCFDirectoryInfo2Entries & lpGCFDirectoryCopyEntries & lpGCFDirectoryLocalEntries in bytes.
        /// </summary>
        public uint DirectorySize;

        /// <summary>
        /// Size of the directory names in bytes.
        /// </summary>
        public uint NameSize;

        /// <summary>
        /// Number of Info1 entires.
        /// </summary>
        public uint Info1Count;

        /// <summary>
        /// Number of files to copy.
        /// </summary>
        public uint CopyCount;

        /// <summary>
        /// Number of files to keep local.
        /// </summary>
        public uint LocalCount;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy2;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy3;

        /// <summary>
        /// Header checksum.
        /// </summary>
        public uint Checksum;
    }
}