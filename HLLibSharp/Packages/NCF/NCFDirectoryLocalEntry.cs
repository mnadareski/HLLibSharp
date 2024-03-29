﻿/*
 * HLLib
 * Copyright (C) 2006-2010 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

using System;

namespace HLLib.Packages.NCF
{
    public sealed class NCFDirectoryLocalEntry
    {
        /// <summary>
        /// Total size of a NCFDirectoryLocalEntry object
        /// </summary>
        public const int ObjectSize = 4;

        /// <summary>
        /// Index of the directory item.
        /// </summary>
        public uint DirectoryIndex { get; set; }

        public static NCFDirectoryLocalEntry Create(byte[] data, ref int offset)
        {
            NCFDirectoryLocalEntry directoryLocalEntry = new NCFDirectoryLocalEntry();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            directoryLocalEntry.DirectoryIndex = BitConverter.ToUInt32(data, offset); offset += 4;
            return directoryLocalEntry;
        }

    }
}
