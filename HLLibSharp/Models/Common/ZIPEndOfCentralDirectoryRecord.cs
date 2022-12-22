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
    public sealed class ZIPEndOfCentralDirectoryRecord
    {
        /// <summary>
        /// End of central directory signature = 0x06054b50
        /// </summary>
        public uint Signature;

        /// <summary>
        /// Number of this disk (or 0xffff for ZIP64)
        /// </summary>
        public ushort NumberOfThisDisk;

        /// <summary>
        /// Disk where central directory starts (or 0xffff for ZIP64)
        /// </summary>
        public ushort NumberOfTheDiskWithStartOfCentralDirectory;

        /// <summary>
        /// Number of central directory records on this disk (or 0xffff for ZIP64)
        /// </summary>
        public ushort CentralDirectoryEntriesThisDisk;

        /// <summary>
        /// Total number of central directory records (or 0xffff for ZIP64)
        /// </summary>
        public ushort CentralDirectoryEntriesTotal;

        /// <summary>
        /// Size of central directory (bytes) (or 0xffffffff for ZIP64)
        /// </summary>
        public uint CentralDirectorySize;

        /// <summary>
        /// Offset of start of central directory, relative to start of archive (or 0xffffffff for ZIP64)
        /// </summary>
        public uint StartOfCentralDirOffset;

        /// <summary>
        /// Comment length
        /// </summary>
        public ushort CommentLength;

        /// <summary>
        /// Comment
        /// </summary>
        public string Comment;
    }
}
