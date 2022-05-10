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
    public class SGAFile4
    {
        /// <summary>
        /// Total size of a SGAFile4 object
        /// </summary>
        public const int ObjectSize = (4 * 5) + (1 * 2);

        public uint NameOffset { get; set; }

        public uint Offset { get; set; }

        public uint SizeOnDisk { get; set; }

        public uint Size { get; set; }

        public uint TimeModified { get; set; }

        public byte Dummy0 { get; set; }

        public byte Type { get; set; }

        public static SGAFile4 Create(byte[] data, ref int offset)
        {
            SGAFile4 file = new SGAFile4();

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

            return file;
        }
    }
}
