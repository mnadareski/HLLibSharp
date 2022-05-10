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
    public sealed class SGAFolder5 : SGAFolder<uint>
    {
        /// <summary>
        /// Total size of a SGAFolder5 object
        /// </summary>
        public new const int ObjectSize = SGAFolder<uint>.ObjectSize + (4 * 4);

        public static SGAFolder5 Create(byte[] data, ref int offset)
        {
            SGAFolder5 folder = new SGAFolder5();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            folder.NameOffset = BitConverter.ToUInt32(data, offset); offset += 4;
            folder.FolderStartIndex = BitConverter.ToUInt32(data, offset); offset += 4;
            folder.FolderEndIndex = BitConverter.ToUInt32(data, offset); offset += 4;
            folder.FileStartIndex = BitConverter.ToUInt32(data, offset); offset += 4;
            folder.FileEndIndex = BitConverter.ToUInt32(data, offset); offset += 4;

            return folder;
        }
    }
}
