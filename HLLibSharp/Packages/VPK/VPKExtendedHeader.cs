/*
 * HLLib
 * Copyright (C) 2006-2013 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

using System;

namespace HLLib.Packages.VPK
{
    /// <summary>
    /// Added in version 2.
    /// </summary>
    public sealed class VPKExtendedHeader
    {
        /// <summary>
        /// Total size of a VPKExtendedHeader object
        /// </summary>
        public const int ObjectSize = (4 * 4);

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy0 { get; set; }

        /// <summary>
        /// Archive hash length
        /// </summary>
        public uint ArchiveHashLength { get; set; }

        /// <summary>
        /// Looks like some more MD5 hashes.
        /// </summary>
        public uint ExtraLength { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy1 { get; set; }

        public static VPKExtendedHeader Create(byte[] data, ref int offset)
        {
            VPKExtendedHeader extendedHeader = new VPKExtendedHeader();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            extendedHeader.Dummy0 = BitConverter.ToUInt32(data, offset); offset += 4;
            extendedHeader.ArchiveHashLength = BitConverter.ToUInt32(data, offset); offset += 4;
            extendedHeader.ExtraLength = BitConverter.ToUInt32(data, offset); offset += 4;
            extendedHeader.Dummy1 = BitConverter.ToUInt32(data, offset); offset += 4;

            return extendedHeader;
        }

    }
}
