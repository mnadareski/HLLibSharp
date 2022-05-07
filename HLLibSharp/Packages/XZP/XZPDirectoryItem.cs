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
    public class XZPDirectoryItem
    {
        /// <summary>
        /// Total size of a XZPDirectoryItem object
        /// </summary>
        public const int ObjectSize = 4 + 4 + 4;

        public uint FileNameCRC { get; set; }

        public uint NameOffset { get; set; }

        public uint TimeCreated { get; set; }

        public static XZPDirectoryItem Create(byte[] data, ref int offset)
        {
            XZPDirectoryItem directoryItem = new XZPDirectoryItem();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            directoryItem.FileNameCRC = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryItem.NameOffset = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryItem.TimeCreated = BitConverter.ToUInt32(data, offset); offset += 4;

            return directoryItem;
        }
    }
}
