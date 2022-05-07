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

namespace HLLib.Packages.Common
{
    public class ZIPLocalFileHeader
    {
        /// <summary>
        /// Total size of a ZIPLocalFileHeader object
        /// </summary>
        /// <remarks>
        /// This does not include variable length fields
        /// </remarks>
        public const int ObjectSize = 4 + (2 * 5) + (4 * 3) + (2 * 2);

        /// <summary>
        /// local file header signature 4 bytes (0x04034b50)
        /// </summary>
        public uint Signature { get; set; }

        /// <summary>
        /// version needed to extract 2 bytes
        /// </summary>
        public ushort VersionNeededToExtract { get; set; }

        /// <summary>
        /// general purpose bit flag 2 bytes
        /// </summary>
        public ushort Flags { get; set; }

        /// <summary>
        /// compression method 2 bytes
        /// </summary>
        public ushort CompressionMethod { get; set; }

        /// <summary>
        /// last mod file time 2 bytes 
        /// </summary>
        public ushort LastModifiedTime { get; set; }

        /// <summary>
        /// last mod file date 2 bytes 
        /// </summary>
        public ushort LastModifiedDate { get; set; }

        /// <summary>
        /// crc-32 4 bytes
        /// </summary>
        public uint CRC32 { get; set; }

        /// <summary>
        /// compressed size 4 bytes
        /// </summary>
        public uint CompressedSize { get; set; }

        /// <summary>
        /// uncompressed size 4 bytes
        /// </summary>
        public uint UncompressedSize { get; set; }

        /// <summary>
        /// file name length 2 bytes
        /// </summary>
        public ushort FileNameLength { get; set; }

        /// <summary>
        /// extra field length 2 bytes
        /// </summary>
        public ushort ExtraFieldLength { get; set; }

        /// <summary>
        /// file name (variable size)
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// extra field (variable size)
        /// </summary>
        public string ExtraField { get; set; }

        // TODO: Read in file data?
        // file data (variable size)

        public static ZIPLocalFileHeader Create(byte[] data, ref int offset)
        {
            ZIPLocalFileHeader localFileHeader = new ZIPLocalFileHeader();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            localFileHeader.Signature = BitConverter.ToUInt32(data, offset); offset += 4;
            localFileHeader.VersionNeededToExtract = BitConverter.ToUInt16(data, offset); offset += 2;
            localFileHeader.Flags = BitConverter.ToUInt16(data, offset); offset += 2;
            localFileHeader.CompressionMethod = BitConverter.ToUInt16(data, offset); offset += 2;
            localFileHeader.LastModifiedTime = BitConverter.ToUInt16(data, offset); offset += 2;
            localFileHeader.LastModifiedDate = BitConverter.ToUInt16(data, offset); offset += 2;
            localFileHeader.CRC32 = BitConverter.ToUInt32(data, offset); offset += 4;
            localFileHeader.CompressedSize = BitConverter.ToUInt32(data, offset); offset += 4;
            localFileHeader.UncompressedSize = BitConverter.ToUInt32(data, offset); offset += 4;
            localFileHeader.FileNameLength = BitConverter.ToUInt16(data, offset); offset += 2;
            localFileHeader.ExtraFieldLength = BitConverter.ToUInt16(data, offset); offset += 2;

            if (offset < ObjectSize)
            {
                localFileHeader.FileName = Encoding.ASCII.GetString(data, offset, localFileHeader.FileNameLength); offset += localFileHeader.FileNameLength;
                localFileHeader.ExtraField = Encoding.ASCII.GetString(data, offset, localFileHeader.ExtraFieldLength); offset += localFileHeader.ExtraFieldLength;
            }

            return localFileHeader;
        }
    }
}
