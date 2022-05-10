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
    public sealed class SGAFile7 : SGAFile6
    {
        /// <summary>
        /// Total size of a SGAFile7 object
        /// </summary>
        public new const int ObjectSize = SGAFile6.ObjectSize + 4;

        public uint HashOffset { get; set; }

        public static new SGAFile7 Create(byte[] data, ref int offset)
        {
            SGAFile7 file = new SGAFile7();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            file.NameOffset = BitConverter.ToUInt32(data, offset); offset += 4;
            file.Offset = BitConverter.ToUInt32(data, offset); offset += 4;
            file.SizeOnDisk = BitConverter.ToUInt32(data, offset); offset += 4;
            file.Size = BitConverter.ToUInt32(data, offset); offset += 4;
            file.TimeModified = BitConverter.ToUInt32(data, offset); offset += 4;
            file.Dummy0 = data[offset++];
            file.Type = data[offset++];
            file.CRC32 = BitConverter.ToUInt32(data, offset); offset += 4;
            file.HashOffset = BitConverter.ToUInt32(data, offset); offset += 4;

            return file;
        }
    }
}
