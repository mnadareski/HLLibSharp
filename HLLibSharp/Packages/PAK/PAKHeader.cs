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
using System.Text;

namespace HLLib.Packages.PAK
{
    public sealed class PAKHeader
    {
        /// <summary>
        /// Total size of a PAKHeader object
        /// </summary>
        public const int ObjectSize = 4 + 4 + 4;

        /// <summary>
        /// Signature
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// Directory Offset
        /// </summary>
        public uint DirectoryOffset { get; set; }

        /// <summary>
        /// Directory Length
        /// </summary>
        public uint DirectoryLength { get; set; }

        public static PAKHeader Create(byte[] data, ref int offset)
        {
            PAKHeader header = new PAKHeader();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            header.Signature = Encoding.ASCII.GetString(data, offset, 4); offset += 4;
            header.DirectoryOffset = BitConverter.ToUInt32(data, offset); offset += 4;
            header.DirectoryLength = BitConverter.ToUInt32(data, offset); offset += 4;

            return header;
        }
    }
}
