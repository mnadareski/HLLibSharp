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
    public sealed class SGAFileHeader
    {
        /// <summary>
        /// Total size of a SGAFileHeader object
        /// </summary>
        public const int ObjectSize = 256 + 4;

        public string Name { get; set; }

        public uint CRC32 { get; set; }

        public static SGAFileHeader Create(byte[] data, ref int offset)
        {
            SGAFileHeader fileHeader = new SGAFileHeader();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            fileHeader.Name = Encoding.ASCII.GetString(data, offset, 256); offset += 256;
            fileHeader.CRC32 = BitConverter.ToUInt32(data, offset); offset += 4;

            return fileHeader;
        }
    }
}
