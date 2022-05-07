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

namespace HLLib.Packages.XZP
{
    public class XZPDirectoryEntry
    {
        /// <summary>
        /// Total size of a XZPDirectoryEntry object
        /// </summary>
        public const int ObjectSize = 4 + 4 + 4;

        public uint FileNameCRC { get; set; }

        public uint EntryLength { get; set; }

        public uint EntryOffset { get; set; }

        public static XZPDirectoryEntry Create(byte[] data, ref int offset)
        {
            XZPDirectoryEntry directoryEntry = new XZPDirectoryEntry();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            directoryEntry.FileNameCRC = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryEntry.EntryLength = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryEntry.EntryOffset = BitConverter.ToUInt32(data, offset); offset += 4;

            return directoryEntry;
        }
    }
}
