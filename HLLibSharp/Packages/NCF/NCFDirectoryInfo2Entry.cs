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
    public class NCFDirectoryInfo2Entry
    {
        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy0 { get; set; }

        public static NCFDirectoryInfo2Entry Create(byte[] data, ref int offset)
        {
            NCFDirectoryInfo2Entry directoryInfo2Entry = new NCFDirectoryInfo2Entry();

            // Check to see if the data is valid
            if (data == null || data.Length < Marshal.SizeOf(directoryInfo2Entry))
                return null;

            directoryInfo2Entry.Dummy0 = BitConverter.ToUInt32(data, offset); offset += 4;

            return directoryInfo2Entry;
        }

    }
}
