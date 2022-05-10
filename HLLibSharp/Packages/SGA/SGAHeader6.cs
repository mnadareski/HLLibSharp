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
using System.Text;

namespace HLLib.Packages.SGA
{
    public sealed class SGAHeader6 : SGAHeaderBase
    {
        /// <summary>
        /// Total size of a SGAHeader6 object
        /// </summary>
        public new const int ObjectSize = SGAHeaderBase.ObjectSize + 64 + (4 * 3);

        public string Name { get; set; }

        public uint HeaderLength { get; set; }

        public uint FileDataOffset { get; set; }

        public uint Dummy0 { get; set; }

        public static SGAHeader6 Create(byte[] data, ref int offset)
        {
            SGAHeader6 header = new SGAHeader6();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            Fill(header, data, ref offset);

            header.Name = Encoding.ASCII.GetString(data, offset, 64); offset += 64;
            header.HeaderLength = BitConverter.ToUInt32(data, offset); offset += 4;
            header.FileDataOffset = BitConverter.ToUInt32(data, offset); offset += 4;
            header.Dummy0 = BitConverter.ToUInt32(data, offset); offset += 4;

            return header;
        }
    }
}
