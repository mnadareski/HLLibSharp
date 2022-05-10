/*
 * HLLib
 * Copyright (C) 2006-2012 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

using System;

namespace HLLib.Packages.SGA
{
    public sealed class SGADirectoryHeader4 : SGADirectoryHeader<ushort>
    {
        /// <summary>
        /// Total size of a SGADirectoryHeader4 object
        /// </summary>
        public new const int ObjectSize = SGADirectoryHeader<ushort>.ObjectSize + (2 * 4);

        public static SGADirectoryHeader4 Create(byte[] data, ref int offset)
        {
            SGADirectoryHeader4 directoryHeader = new SGADirectoryHeader4();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            directoryHeader.SectionOffset = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryHeader.SectionCount = BitConverter.ToUInt16(data, offset); offset += 2;
            directoryHeader.FolderOffset = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryHeader.FolderCount = BitConverter.ToUInt16(data, offset); offset += 2;
            directoryHeader.FileOffset = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryHeader.FileCount = BitConverter.ToUInt16(data, offset); offset += 2;
            directoryHeader.StringTableOffset = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryHeader.StringTableCount = BitConverter.ToUInt16(data, offset); offset += 2;

            return directoryHeader;
        }
    }
}
