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
    public class NCFDirectoryInfo1Entry
    {
        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy0 { get; set; }

        public static NCFDirectoryInfo1Entry Create(byte[] data, ref int offset)
        {
            NCFDirectoryInfo1Entry directoryInfo1Entry = new NCFDirectoryInfo1Entry();

            // Check to see if the data is valid
            if (data == null || data.Length < Marshal.SizeOf(directoryInfo1Entry))
                return null;

            directoryInfo1Entry.Dummy0 = BitConverter.ToUInt32(data, offset); offset += 4;

            return directoryInfo1Entry;
        }

    }
}
