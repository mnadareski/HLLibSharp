/*
 * HLLib
 * Copyright (C) 2006-2012 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

using System;

namespace HLLib.Packages.SGA
{
    public sealed class SGAFolder4 : SGAFolder<ushort>
    {
        /// <summary>
        /// Total size of a SGAFolder4 object
        /// </summary>
        public new const int ObjectSize = SGAFolder<ushort>.ObjectSize + (2 * 4);

        public static SGAFolder4 Create(byte[] data, ref int offset)
        {
            SGAFolder4 folder = new SGAFolder4();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            folder.NameOffset = BitConverter.ToUInt32(data, offset); offset += 4;
            folder.FolderStartIndex = BitConverter.ToUInt16(data, offset); offset += 2;
            folder.FolderEndIndex = BitConverter.ToUInt16(data, offset); offset += 2;
            folder.FileStartIndex = BitConverter.ToUInt16(data, offset); offset += 2;
            folder.FileEndIndex = BitConverter.ToUInt16(data, offset); offset += 2;

            return folder;
        }
    }
}
