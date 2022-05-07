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
    public class GCFDirectoryMapEntry
    {
        /// <summary>
        /// Total size of a GCFDirectoryMapEntry object
        /// </summary>
        public const int ObjectSize = 4;

        /// <summary>
        /// Index of the first data block. (N/A if == BlockCount.)
        /// </summary>
        public uint FirstBlockIndex { get; set; }

        public static GCFDirectoryMapEntry Create(byte[] data, ref int offset)
        {
            GCFDirectoryMapEntry directoryMapEntry = new GCFDirectoryMapEntry();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            directoryMapEntry.FirstBlockIndex = BitConverter.ToUInt32(data, offset); offset += 4;

            return directoryMapEntry;
        }
    }
}