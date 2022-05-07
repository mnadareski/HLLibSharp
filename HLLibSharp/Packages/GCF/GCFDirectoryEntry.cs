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
    public class GCFDirectoryEntry
    {
        /// <summary>
        /// Total size of a GCFDirectoryEntry object
        /// </summary>
        public const int ObjectSize = 4 + 4 + 4 + 4 + 4 + 4 + 4;

        /// <summary>
        /// Offset to the directory item name from the end of the directory items.
        /// </summary>
        public uint NameOffset { get; set; }

        /// <summary>
        /// Size of the item.  (If file, file size.  If folder, num items.)
        /// </summary>
        public uint ItemSize { get; set; }

        /// <summary>
        /// Checksome index. (0xFFFFFFFF == None).
        /// </summary>
        public uint ChecksumIndex { get; set; }

        /// <summary>
        /// Flags for the directory item.  (0x00000000 == Folder).
        /// </summary>
        public uint DirectoryFlags { get; set; }

        /// <summary>
        /// Index of the parent directory item.  (0xFFFFFFFF == None).
        /// </summary>
        public uint ParentIndex { get; set; }

        /// <summary>
        /// Index of the next directory item.  (0x00000000 == None).
        /// </summary>
        public uint NextIndex { get; set; }

        /// <summary>
        /// Index of the first directory item.  (0x00000000 == None).
        /// </summary>
        public uint FirstIndex { get; set; }

        public static GCFDirectoryEntry Create(byte[] data, ref int offset)
        {
            GCFDirectoryEntry directoryEntry = new GCFDirectoryEntry();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            directoryEntry.NameOffset = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryEntry.ItemSize = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryEntry.ChecksumIndex = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryEntry.DirectoryFlags = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryEntry.ParentIndex = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryEntry.NextIndex = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryEntry.FirstIndex = BitConverter.ToUInt32(data, offset); offset += 4;

            return directoryEntry;
        }
    }
}