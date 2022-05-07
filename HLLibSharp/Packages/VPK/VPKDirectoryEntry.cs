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

namespace HLLib.Packages.VPK
{
    public class VPKDirectoryEntry
    {
        /// <summary>
        /// Total size of a VPKDirectoryEntry object
        /// </summary>
        public const int ObjectSize = 4 + 2 + 2 + 4 + 4 + 2;

        public uint CRC { get; set; }

        public ushort PreloadBytes { get; set; }

        public ushort ArchiveIndex { get; set; }

        public uint EntryOffset { get; set; }

        public uint EntryLength { get; set; }

        /// <summary>
        /// Always 0xffff.
        /// </summary>
        public ushort Dummy0 { get; set; }

        public static VPKDirectoryEntry Create(byte[] data, ref int offset)
        {
            VPKDirectoryEntry directoryEntry = new VPKDirectoryEntry();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            directoryEntry.CRC = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryEntry.PreloadBytes = BitConverter.ToUInt16(data, offset); offset += 2;
            directoryEntry.ArchiveIndex = BitConverter.ToUInt16(data, offset); offset += 2;
            directoryEntry.EntryOffset = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryEntry.EntryLength = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryEntry.Dummy0 = BitConverter.ToUInt16(data, offset); offset += 2;

            return directoryEntry;
        }

        public byte[] Serialize()
        {
            int offset = 0;
            byte[] data = new byte[ObjectSize];

            Array.Copy(BitConverter.GetBytes(CRC), 0, data, offset, 4); offset += 4;
            Array.Copy(BitConverter.GetBytes(PreloadBytes), 0, data, offset, 2); offset += 2;
            Array.Copy(BitConverter.GetBytes(ArchiveIndex), 0, data, offset, 2); offset += 2;
            Array.Copy(BitConverter.GetBytes(EntryOffset), 0, data, offset, 4); offset += 4;
            Array.Copy(BitConverter.GetBytes(EntryLength), 0, data, offset, 4); offset += 4;
            Array.Copy(BitConverter.GetBytes(Dummy0), 0, data, offset, 2); offset += 2;

            return data;
        }
    }
}
