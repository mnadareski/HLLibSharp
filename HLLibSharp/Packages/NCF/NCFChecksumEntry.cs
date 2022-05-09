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
    public sealed class NCFChecksumEntry
    {
        /// <summary>
        /// Total size of a NCFChecksumEntry object
        /// </summary>
        public const int ObjectSize = 4;

        /// <summary>
        /// Checksum.
        /// </summary>
        public uint Checksum { get; set; }

        public static NCFChecksumEntry Create(byte[] data, ref int offset)
        {
            NCFChecksumEntry checksumEntry = new NCFChecksumEntry();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            checksumEntry.Checksum = BitConverter.ToUInt32(data, offset); offset += 4;

            return checksumEntry;
        }
    }
}
