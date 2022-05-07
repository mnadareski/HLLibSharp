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

namespace HLLib.Packages.WAD
{
    public class WADHeader
    {
        /// <summary>
        /// Total size of a WADHeader object
        /// </summary>
        public const int ObjectSize = 4 + 4 + 4;

        public string Signature { get; set; }
        
        public uint LumpCount { get; set; }
        
        public uint LumpOffset { get; set; }

        public static WADHeader Create(byte[] data, ref int offset)
        {
            WADHeader header = new WADHeader();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            header.Signature = Encoding.ASCII.GetString(data, offset, 4); offset += 4;
            header.LumpCount = BitConverter.ToUInt32(data, offset); offset += 4;
            header.LumpOffset = BitConverter.ToUInt32(data, offset); offset += 4;

            return header;
        }
    }
}
