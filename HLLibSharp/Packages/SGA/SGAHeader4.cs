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
    public sealed class SGAHeader4 : SGAHeaderBase
    {
        /// <summary>
        /// Total size of a SGAHeader4 object
        /// </summary>
        public new const int ObjectSize = SGAHeaderBase.ObjectSize + 16 + 64 + 16 + (4 * 3);

        public byte[] FileMD5 { get; set; }

        public string Name { get; set; }

        public byte[] HeaderMD5 { get; set; }

        public uint HeaderLength { get; set; }

        public uint FileDataOffset { get; set; }

        public uint Dummy0 { get; set; }

        public static SGAHeader4 Create(byte[] data, ref int offset)
        {
            SGAHeader4 header = new SGAHeader4();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            Fill(header, data, ref offset);

            header.FileMD5 = new byte[16];
            Array.Copy(data, offset, header.FileMD5, 0, 16); offset += 16;
            header.Name = Encoding.ASCII.GetString(data, offset, 64); offset += 64;
            header.HeaderMD5 = new byte[16];
            Array.Copy(data, offset, header.HeaderMD5, 0, 16); offset += 16;
            header.HeaderLength = BitConverter.ToUInt32(data, offset); offset += 4;
            header.FileDataOffset = BitConverter.ToUInt32(data, offset); offset += 4;
            header.Dummy0 = BitConverter.ToUInt32(data, offset); offset += 4;

            return header;
        }
    }
}
