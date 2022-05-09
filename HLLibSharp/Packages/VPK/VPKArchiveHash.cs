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
    public class VPKArchiveHash
    {
        /// <summary>
        /// Total size of a VPKArchiveHash object
        /// </summary>
        public const int ObjectSize = (4 * 3) + 16;

        public uint ArchiveIndex { get; set; }

        public uint ArchiveOffset { get; set; }

        public uint Length { get; set; }

        /// <summary>
        /// MD5
        /// </summary>
        public byte[] Hash { get; set; }

        public static VPKArchiveHash Create(byte[] data, ref int offset)
        {
            VPKArchiveHash archiveHash = new VPKArchiveHash();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            archiveHash.ArchiveIndex = BitConverter.ToUInt32(data, offset); offset += 4;
            archiveHash.ArchiveOffset = BitConverter.ToUInt32(data, offset); offset += 4;
            archiveHash.Length = BitConverter.ToUInt32(data, offset); offset += 4;
            archiveHash.Hash = new byte[16];
            Array.Copy(data, offset, archiveHash.Hash, 0, 16); offset += 16;

            return archiveHash;
        }
    }
}
