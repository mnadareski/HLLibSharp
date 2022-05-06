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
using System.Runtime.InteropServices;

namespace HLLib.Packages.GCF
{
    public class GCFChecksumHeader
    {
        /// <summary>
        /// Always 0x00000001
        /// </summary>
        public uint Dummy0 { get; set; }

        /// <summary>
        /// Size of LPGCFCHECKSUMHEADER & LPGCFCHECKSUMMAPHEADER & in bytes.
        /// </summary>
        public uint ChecksumSize { get; set; }

        public static GCFChecksumHeader Create(byte[] data, ref int offset)
        {
            GCFChecksumHeader checksumHeader = new GCFChecksumHeader();

            // Check to see if the data is valid
            if (data == null || data.Length < Marshal.SizeOf(checksumHeader))
                return null;

            checksumHeader.Dummy0 = BitConverter.ToUInt32(data, offset); offset += 4;
            checksumHeader.ChecksumSize = BitConverter.ToUInt32(data, offset); offset += 4;

            return checksumHeader;
        }
    }
}