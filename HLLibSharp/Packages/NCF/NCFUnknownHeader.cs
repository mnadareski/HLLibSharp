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

namespace HLLib.Packages.NCF
{
    public class NCFUnknownHeader
    {
        /// <summary>
        /// Always 0x00000001
        /// </summary>
        public uint Dummy0 { get; set; }

        /// <summary>
        /// Always 0x00000000
        /// </summary>
        public uint Dummy1 { get; set; }

        public static NCFUnknownHeader Create(byte[] data, ref int offset)
        {
            NCFUnknownHeader unknownHeader = new NCFUnknownHeader();

            // Check to see if the data is valid
            if (data == null || data.Length < Marshal.SizeOf(unknownHeader))
                return null;

            unknownHeader.Dummy0 = BitConverter.ToUInt32(data, offset); offset += 4;
            unknownHeader.Dummy1 = BitConverter.ToUInt32(data, offset); offset += 4;

            return unknownHeader;
        }

    }
}
