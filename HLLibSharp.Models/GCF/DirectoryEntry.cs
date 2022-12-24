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
    public sealed class DirectoryEntry
    {
        /// <summary>
        /// Offset to the directory item name from the end of the directory items.
        /// </summary>
        public uint NameOffset;

        /// <summary>
        /// Size of the item.  (If file, file size.  If folder, num items.)
        /// </summary>
        public uint ItemSize;

        /// <summary>
        /// Checksome index. (0xFFFFFFFF == None).
        /// </summary>
        public uint ChecksumIndex;

        /// <summary>
        /// Flags for the directory item.  (0x00000000 == Folder).
        /// </summary>
        public uint DirectoryFlags;

        /// <summary>
        /// Index of the parent directory item.  (0xFFFFFFFF == None).
        /// </summary>
        public uint ParentIndex;

        /// <summary>
        /// Index of the next directory item.  (0x00000000 == None).
        /// </summary>
        public uint NextIndex;

        /// <summary>
        /// Index of the first directory item.  (0x00000000 == None).
        /// </summary>
        public uint FirstIndex;
    }
}