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
    public class SGAFile6 : SGAFile4
    {
        /// <summary>
        /// Total size of a SGAFile6 object
        /// </summary>
        public new const int ObjectSize = SGAFile4.ObjectSize + 4;

        public uint CRC32 { get; set; }

        public static new SGAFile6 Create(byte[] data, ref int offset)
        {
            SGAFile6 file = new SGAFile6();

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

            return file;
        }
    }
}
