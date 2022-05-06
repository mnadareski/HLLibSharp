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
using System.Runtime.InteropServices;
using System.Text;

namespace HLLib.Packages.PAK
{
    public class PAKDirectoryItem
    {
        /// <summary>
        /// Item Name
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Item Offset
        /// </summary>
        public uint ItemOffset { get; set; }

        /// <summary>
        /// Item Length
        /// </summary>
        public uint ItemLength { get; set; }

        public static PAKDirectoryItem Create(byte[] data, ref int offset)
        {
            PAKDirectoryItem directoryItem = new PAKDirectoryItem();

            // Check to see if the data is valid
            if (data == null || data.Length < Marshal.SizeOf(directoryItem))
                return null;

            directoryItem.ItemName = Encoding.ASCII.GetString(data, offset, 56); offset += 56;
            directoryItem.ItemOffset = BitConverter.ToUInt32(data, offset); offset += 4;
            directoryItem.ItemLength = BitConverter.ToUInt32(data, offset); offset += 4;

            return directoryItem;
        }
    }
}
