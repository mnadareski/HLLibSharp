/*
 * HLLib
 * Copyright (C) 2006-2012 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

namespace HLLib.Models.Common
{
    public sealed class ZIPLocalFileHeader
    {
        /// <summary>
        /// local file header signature 4 bytes (0x04034b50)
        /// </summary>
        public uint Signature;

        /// <summary>
        /// version needed to extract 2 bytes
        /// </summary>
        public ushort VersionNeededToExtract;

        /// <summary>
        /// general purpose bit flag 2 bytes
        /// </summary>
        public ushort Flags;

        /// <summary>
        /// compression method 2 bytes
        /// </summary>
        public ushort CompressionMethod;

        /// <summary>
        /// last mod file time 2 bytes 
        /// </summary>
        public ushort LastModifiedTime;

        /// <summary>
        /// last mod file date 2 bytes 
        /// </summary>
        public ushort LastModifiedDate;

        /// <summary>
        /// crc-32 4 bytes
        /// </summary>
        public uint CRC32;

        /// <summary>
        /// compressed size 4 bytes
        /// </summary>
        public uint CompressedSize;

        /// <summary>
        /// uncompressed size 4 bytes
        /// </summary>
        public uint UncompressedSize;

        /// <summary>
        /// file name length 2 bytes
        /// </summary>
        public ushort FileNameLength;

        /// <summary>
        /// extra field length 2 bytes
        /// </summary>
        public ushort ExtraFieldLength;

        /// <summary>
        /// file name (variable size)
        /// </summary>
        public string FileName;

        /// <summary>
        /// extra field (variable size)
        /// </summary>
        public string ExtraField;

        // file data (variable size)
    }
}
