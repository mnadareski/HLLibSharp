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
    public class ZIPFileHeader
    {
        /// <summary>
        /// Total size of a ZIPFileHeader object
        /// </summary>
        /// <remarks>
        /// This does not include variable length fields
        /// </remarks>
        public const int ObjectSize = 4 + (2 * 6) + (4 * 3) + (2 * 5) + (4 * 2);

        /// <summary>
        /// 4 bytes (0x02014b50) 
        /// </summary>
        public uint Signature { get; set; }

        /// <summary>
        /// version made by 2 bytes
        /// </summary>
        public ushort VersionMadeBy { get; set; }

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
        /// file comment length 2 bytes 
        /// </summary>
        public ushort FileCommentLength { get; set; }

        /// <summary>
        /// disk number start 2 bytes 
        /// </summary>
        public ushort DiskNumberStart { get; set; }

        /// <summary>
        /// internal file attributes 2 bytes
        /// </summary>
        public ushort InternalFileAttribs { get; set; }

        /// <summary>
        /// external file attributes 4 bytes
        /// </summary>
        public uint ExternalFileAttribs { get; set; }

        /// <summary>
        /// relative offset of local header 4 bytes
        /// </summary>
        public uint RelativeOffsetOfLocalHeader { get; set; }

        /// <summary>
        /// file name (variable size)
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// extra field (variable size)
        /// </summary>
        public string ExtraField { get; set; }

        /// <summary>
        /// file comment (variable size)
        /// </summary>
        public string FileComment { get; set; }

        public static ZIPFileHeader Create(byte[] data, ref int offset)
        {
            ZIPFileHeader fileHeader = new ZIPFileHeader();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            fileHeader.Signature = BitConverter.ToUInt32(data, offset); offset += 4;
            fileHeader.VersionMadeBy = BitConverter.ToUInt16(data, offset); offset += 2;
            fileHeader.VersionNeededToExtract = BitConverter.ToUInt16(data, offset); offset += 2;
            fileHeader.Flags = BitConverter.ToUInt16(data, offset); offset += 2;
            fileHeader.CompressionMethod = BitConverter.ToUInt16(data, offset); offset += 2;
            fileHeader.LastModifiedTime = BitConverter.ToUInt16(data, offset); offset += 2;
            fileHeader.LastModifiedDate = BitConverter.ToUInt16(data, offset); offset += 2;
            fileHeader.CRC32 = BitConverter.ToUInt32(data, offset); offset += 4;
            fileHeader.CompressedSize = BitConverter.ToUInt32(data, offset); offset += 4;
            fileHeader.UncompressedSize = BitConverter.ToUInt32(data, offset); offset += 4;
            fileHeader.FileNameLength = BitConverter.ToUInt16(data, offset); offset += 2;
            fileHeader.ExtraFieldLength = BitConverter.ToUInt16(data, offset); offset += 2;
            fileHeader.FileCommentLength = BitConverter.ToUInt16(data, offset); offset += 2;
            fileHeader.DiskNumberStart = BitConverter.ToUInt16(data, offset); offset += 2;
            fileHeader.InternalFileAttribs = BitConverter.ToUInt16(data, offset); offset += 2;
            fileHeader.ExternalFileAttribs = BitConverter.ToUInt32(data, offset); offset += 4;
            fileHeader.RelativeOffsetOfLocalHeader = BitConverter.ToUInt32(data, offset); offset += 4;

            if (offset < data.Length)
            {
                fileHeader.FileName = Encoding.ASCII.GetString(data, offset, fileHeader.FileNameLength); offset += fileHeader.FileNameLength;
                fileHeader.ExtraField = Encoding.ASCII.GetString(data, offset, fileHeader.ExtraFieldLength); offset += fileHeader.ExtraFieldLength;
                fileHeader.FileComment = Encoding.ASCII.GetString(data, offset, fileHeader.FileCommentLength); offset += fileHeader.FileCommentLength;
            }

            return fileHeader;
        }
    }
}
