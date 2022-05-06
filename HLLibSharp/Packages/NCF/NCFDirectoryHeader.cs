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

namespace HLLib.Packages.NCF
{
    public class NCFDirectoryHeader
    {
        /// <summary>
        /// Always 0x00000004
        /// </summary>
        public uint Dummy0 { get; set; }

        /// <summary>
        /// Cache ID.
        /// </summary>
        public uint CacheID { get; set; }

        /// <summary>
        /// NCF file version.
        /// </summary>
        public uint LastVersionPlayed { get; set; }

        /// <summary>
        /// Number of items in the directory.
        /// </summary>
        public uint ItemCount { get; set; }

        /// <summary>
        /// Number of files in the directory.
        /// </summary>
        public uint FileCount { get; set; }

        /// <summary>
        /// Always 0x00008000.  Data per checksum?
        /// </summary>
        public uint ChecksumDataLength { get; set; }

        /// <summary>
        /// Size of lpNCFDirectoryEntries & lpNCFDirectoryNames & lpNCFDirectoryInfo1Entries & lpNCFDirectoryInfo2Entries & lpNCFDirectoryCopyEntries & lpNCFDirectoryLocalEntries in bytes.
        /// </summary>
        public uint DirectorySize { get; set; }

        /// <summary>
        /// Size of the directory names in bytes.
        /// </summary>
        public uint NameSize { get; set; }

        /// <summary>
        /// Number of Info1 entires.
        /// </summary>
        public uint Info1Count { get; set; }

        /// <summary>
        /// Number of files to copy.
        /// </summary>
        public uint CopyCount { get; set; }

        /// <summary>
        /// Number of files to keep local.
        /// </summary>
        public uint LocalCount { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy1 { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy2 { get; set; }

        /// <summary>
        /// Header checksum.
        /// </summary>
        public uint Checksum { get; set; }

        public static NCFDirectoryHeader Create(byte[] data, ref int offset)
        {
            NCFDirectoryHeader directoryHeader = new NCFDirectoryHeader();

            // Check to see if the data is valid
            if (data == null || data.Length < Marshal.SizeOf(directoryHeader))
                return null;

            directoryHeader.Dummy0 = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryHeader.CacheID = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryHeader.LastVersionPlayed = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryHeader.ItemCount = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryHeader.FileCount = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryHeader.ChecksumDataLength = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryHeader.DirectorySize = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryHeader.NameSize = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryHeader.Info1Count = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryHeader.CopyCount = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryHeader.LocalCount = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryHeader.Dummy1 = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryHeader.Dummy2 = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryHeader.Checksum = BitConverter.ToUInt32(data, offset); offset += 4;

            return directoryHeader;
        }

    }
}
