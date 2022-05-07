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

namespace HLLib.Packages.XZP
{
    public class XZPDirectoryMapping
    {
        /// <summary>
        /// Total size of a XZPDirectoryMapping object
        /// </summary>
        public const int ObjectSize = 2;

        public ushort PreloadDirectoryEntryIndex { get; set; }

        public static XZPDirectoryMapping Create(byte[] data, ref int offset)
        {
            XZPDirectoryMapping directoryMapping = new XZPDirectoryMapping();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            directoryMapping.PreloadDirectoryEntryIndex = BitConverter.ToUInt16(data, offset); offset += 2;

            return directoryMapping;
        }
    }
}
