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

namespace HLLib.Packages.GCF
{
    /// <remarks>
    /// Added in version 4 or version 5.
    /// </remarks>
    public class GCFDirectoryMapHeader
    {
        /// <summary>
        /// Total size of a GCFDirectoryMapHeader object
        /// </summary>
        public const int ObjectSize = (4 * 2);

        /// <summary>
        /// Always 0x00000001
        /// </summary>
        public uint Dummy0 { get; set; }

        /// <summary>
        /// Always 0x00000000
        /// </summary>
        public uint Dummy1 { get; set; }

        public static GCFDirectoryMapHeader Create(byte[] data, ref int offset)
        {
            GCFDirectoryMapHeader directoryMapHeader = new GCFDirectoryMapHeader();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            directoryMapHeader.Dummy0 = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryMapHeader.Dummy1 = BitConverter.ToUInt32(data, offset); offset += 4;

            return directoryMapHeader;
        }
    }
}