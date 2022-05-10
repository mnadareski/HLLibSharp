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
    public sealed class SGASection5 : SGASection<uint>
    {
        /// <summary>
        /// Total size of a SGASection5 object
        /// </summary>
        public new const int ObjectSize = SGASection<uint>.ObjectSize + (4 * 5);

        public static SGASection5 Create(byte[] data, ref int offset)
        {
            SGASection5 section = new SGASection5();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            section.Alias = Encoding.ASCII.GetString(data, offset, 64);
            section.Name = Encoding.ASCII.GetString(data, offset, 64);
            section.FolderStartIndex = BitConverter.ToUInt32(data, offset); offset += 4;
            section.FolderEndIndex = BitConverter.ToUInt32(data, offset); offset += 4;
            section.FileStartIndex = BitConverter.ToUInt32(data, offset); offset += 4;
            section.FileEndIndex = BitConverter.ToUInt32(data, offset); offset += 4;
            section.FolderRootIndex = BitConverter.ToUInt32(data, offset); offset += 4;

            return section;
        }
    }
}
