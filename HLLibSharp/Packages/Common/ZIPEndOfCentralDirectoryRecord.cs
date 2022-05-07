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
    public class ZIPEndOfCentralDirectoryRecord
    {
        /// <summary>
        /// Total size of a ZIPEndOfCentralDirectoryRecord object
        /// </summary>
        /// <remarks>
        /// This does not include variable length fields
        /// </remarks>
        public const int ObjectSize = 4 + 2 + 2 + 2 + 2 + 4 + 4 + 2;

        /// <summary>
        /// 4 bytes (0x06054b50)
        /// </summary>
        public uint Signature { get; set; }

        /// <summary>
        /// 2 bytes
        /// </summary>
        public ushort NumberOfThisDisk { get; set; }

        /// <summary>
        /// 2 bytes
        /// </summary>
        public ushort NumberOfTheDiskWithStartOfCentralDirectory { get; set; }

        /// <summary>
        /// 2 bytes
        /// </summary>
        public ushort CentralDirectoryEntriesThisDisk { get; set; }

        /// <summary>
        /// 2 bytes
        /// </summary>
        public ushort CentralDirectoryEntriesTotal { get; set; }

        /// <summary>
        /// 4 bytes
        /// </summary>
        public uint CentralDirectorySize { get; set; }

        /// <summary>
        /// 4 bytes
        /// </summary>
        public uint StartOfCentralDirOffset { get; set; }

        /// <summary>
        /// 2 bytes
        /// </summary>
        public ushort CommentLength { get; set; }

        /// <summary>
        /// Comment
        /// </summary>
        public string Comment { get; set; }

        public static ZIPEndOfCentralDirectoryRecord Create(byte[] data, ref int offset)
        {
            ZIPEndOfCentralDirectoryRecord endOfCentralDirectoryRecord = new ZIPEndOfCentralDirectoryRecord();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            endOfCentralDirectoryRecord.Signature = BitConverter.ToUInt32(data, offset); offset += 4;
            endOfCentralDirectoryRecord.NumberOfThisDisk = BitConverter.ToUInt16(data, offset); offset += 2;
            endOfCentralDirectoryRecord.NumberOfTheDiskWithStartOfCentralDirectory = BitConverter.ToUInt16(data, offset); offset += 2;
            endOfCentralDirectoryRecord.CentralDirectoryEntriesThisDisk = BitConverter.ToUInt16(data, offset); offset += 2;
            endOfCentralDirectoryRecord.CentralDirectoryEntriesTotal = BitConverter.ToUInt16(data, offset); offset += 2;
            endOfCentralDirectoryRecord.CentralDirectorySize = BitConverter.ToUInt32(data, offset); offset += 4;
            endOfCentralDirectoryRecord.StartOfCentralDirOffset = BitConverter.ToUInt32(data, offset); offset += 4;
            endOfCentralDirectoryRecord.CommentLength = BitConverter.ToUInt16(data, offset); offset += 2;
            
            endOfCentralDirectoryRecord.Comment = Encoding.ASCII.GetString(data, offset, endOfCentralDirectoryRecord.CommentLength); offset += endOfCentralDirectoryRecord.CommentLength;

            return endOfCentralDirectoryRecord;
        }
    }
}
