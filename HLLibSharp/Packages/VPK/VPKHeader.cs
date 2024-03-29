﻿/*
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
    public sealed class VPKHeader
    {
        /// <summary>
        /// Total size of a VPKHeader object
        /// </summary>
        public const int ObjectSize = (4 * 3);

        /// <summary>
        /// Always 0x55aa1234.
        /// </summary>
        public uint Signature { get; set; }

        public uint Version { get; set; }

        public uint DirectoryLength { get; set; }

        public static VPKHeader Create(byte[] data, ref int offset)
        {
            VPKHeader header = new VPKHeader();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            header.Signature = BitConverter.ToUInt32(data, offset); offset += 4;
            header.Version = BitConverter.ToUInt32(data, offset); offset += 4;
            header.DirectoryLength = BitConverter.ToUInt32(data, offset); offset += 4;

            return header;
        }

    }
}
