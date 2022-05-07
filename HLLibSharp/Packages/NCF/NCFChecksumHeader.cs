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

namespace HLLib.Packages.NCF
{
    public class NCFChecksumHeader
    {
        /// <summary>
        /// Total size of a NCFChecksumHeader object
        /// </summary>
        public const int ObjectSize = 4 + 4;

        /// <summary>
        /// Always 0x00000001
        /// </summary>
        public uint Dummy0 { get; set; }

        /// <summary>
        /// Size of LPNCFCHECKSUMHEADER & LPNCFCHECKSUMMAPHEADER & in bytes.
        /// </summary>
        public uint ChecksumSize { get; set; }

        public static NCFChecksumHeader Create(byte[] data, ref int offset)
        {
            NCFChecksumHeader checksumHeader = new NCFChecksumHeader();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            checksumHeader.Dummy0 = BitConverter.ToUInt32(data, offset); offset += 4;
            checksumHeader.ChecksumSize = BitConverter.ToUInt32(data, offset); offset += 4;

            return checksumHeader;
        }

    }
}
