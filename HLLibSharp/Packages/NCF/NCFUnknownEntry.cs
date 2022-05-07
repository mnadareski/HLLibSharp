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
    public class NCFUnknownEntry
    {
        /// <summary>
        /// Total size of a NCFUnknownEntry object
        /// </summary>
        public const int ObjectSize = 4;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy0 { get; set; }

        public static NCFUnknownEntry Create(byte[] data, ref int offset)
        {
            NCFUnknownEntry unknownEntry = new NCFUnknownEntry();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            unknownEntry.Dummy0 = BitConverter.ToUInt32(data, offset); offset += 4;

            return unknownEntry;
        }

    }
}
