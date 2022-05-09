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

namespace HLLib.Packages.XZP
{
    public sealed class XZPHeader
    {
        /// <summary>
        /// Total size of a XZPHeader object
        /// </summary>
        public const int ObjectSize = 4 * 9;

        public string Signature { get; set; }

        public uint Version { get; set; }

        public uint PreloadDirectoryEntryCount { get; set; }

        public uint DirectoryEntryCount { get; set; }

        public uint PreloadBytes { get; set; }

        public uint HeaderLength { get; set; }

        public uint DirectoryItemCount { get; set; }

        public uint DirectoryItemOffset { get; set; }

        public uint DirectoryItemLength { get; set; }

        public static XZPHeader Create(byte[] data, ref int offset)
        {
            XZPHeader header = new XZPHeader();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            header.Signature = Encoding.ASCII.GetString(data, offset, 4); offset += 4;
            header.Version = BitConverter.ToUInt32(data, offset); offset += 4;
            header.PreloadDirectoryEntryCount = BitConverter.ToUInt32(data, offset); offset += 4;
            header.DirectoryEntryCount = BitConverter.ToUInt32(data, offset); offset += 4;
            header.PreloadBytes = BitConverter.ToUInt32(data, offset); offset += 4;
            header.HeaderLength = BitConverter.ToUInt32(data, offset); offset += 4;
            header.DirectoryItemCount = BitConverter.ToUInt32(data, offset); offset += 4;
            header.DirectoryItemOffset = BitConverter.ToUInt32(data, offset); offset += 4;
            header.DirectoryItemLength = BitConverter.ToUInt32(data, offset); offset += 4;

            return header;
        }
    }
}
