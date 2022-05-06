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
using System.Linq;
using System.Runtime.InteropServices;

namespace HLLib.Packages.GCF
{
    public class GCFHeader
    {
        /// <summary>
        /// Always 0x00000001
        /// </summary>
        public uint Dummy0 { get; set; }

        /// <summary>
        /// Always 0x00000001
        /// </summary>
        public uint MajorVersion { get; set; }

        /// <summary>
        /// GCF version number.
        /// </summary>
        public uint MinorVersion { get; set; }

        /// <summary>
        /// Cache ID
        /// </summary>
        public uint CacheID { get; set; }

        /// <summary>
        /// Last version played
        /// </summary>
        public uint LastVersionPlayed { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy1 { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy2 { get; set; }

        /// <summary>
        /// Total size of GCF file in bytes.
        /// </summary>
        public uint FileSize { get; set; }

        /// <summary>
        /// Size of each data block in bytes.
        /// </summary>
        public uint BlockSize { get; set; }

        /// <summary>
        /// Number of data blocks.
        /// </summary>
        public uint BlockCount { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy3 { get; set; }

        public static GCFHeader Create(byte[] data, ref int offset)
        {
            GCFHeader header = new GCFHeader();

            // Check to see if the data is valid
            if (data == null
                || data.Length < Marshal.SizeOf(header)
                || data.Take(Marshal.SizeOf(header)).All(b => b == 0x00))
            {
                return null;
            }

            header.Dummy0 = BitConverter.ToUInt32(data, offset); offset += 4;
            if (header.Dummy0 != 0x00000001)
                return null;
            header.MajorVersion = BitConverter.ToUInt32(data, offset); offset += 4;
            if (header.MajorVersion != 0x00000001)
                return null;
            header.MinorVersion = BitConverter.ToUInt32(data, offset); offset += 4;
            header.CacheID = BitConverter.ToUInt32(data, offset); offset += 4;
            header.LastVersionPlayed = BitConverter.ToUInt32(data, offset); offset += 4;
            header.Dummy1 = BitConverter.ToUInt32(data, offset); offset += 4;
            header.Dummy2 = BitConverter.ToUInt32(data, offset); offset += 4;
            header.FileSize = BitConverter.ToUInt32(data, offset); offset += 4;
            header.BlockSize = BitConverter.ToUInt32(data, offset); offset += 4;
            header.Dummy3 = BitConverter.ToUInt32(data, offset); offset += 4;

            return header;
        }
    }
}