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
using System.Text;

namespace HLLib.Packages.SGA
{
    public sealed class SGASection4 : SGASection<ushort>
    {
        /// <summary>
        /// Total size of a SGASection4 object
        /// </summary>
        public new const int ObjectSize = SGASection<ushort>.ObjectSize + (2 * 5);

        public static SGASection4 Create(byte[] data, ref int offset)
        {
            SGASection4 section = new SGASection4();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            section.Alias = Encoding.ASCII.GetString(data, offset, 64);
            section.Name = Encoding.ASCII.GetString(data, offset, 64);
            section.FolderStartIndex = BitConverter.ToUInt16(data, offset); offset += 2;
            section.FolderEndIndex = BitConverter.ToUInt16(data, offset); offset += 2;
            section.FileStartIndex = BitConverter.ToUInt16(data, offset); offset += 2;
            section.FileEndIndex = BitConverter.ToUInt16(data, offset); offset += 2;
            section.FolderRootIndex = BitConverter.ToUInt16(data, offset); offset += 2;

            return section;
        }
    }
}
