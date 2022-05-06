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

namespace HLLib.Packages.VBSP
{
    public class LMPHeader
    {
        public int LumpOffset { get; set; }

        public int LumpID { get; set; }

        public int LumpVersion { get; set; }

        public int LumpLength { get; set; }

        public int MapRevision { get; set; }

        public static LMPHeader Create(byte[] data, ref int offset)
        {
            LMPHeader header = new LMPHeader();

            // Check to see if the data is valid
            if (data == null || data.Length < Marshal.SizeOf(header))
                return null;

            header.LumpOffset = BitConverter.ToInt32(data, offset); offset += 4;
            header.LumpID = BitConverter.ToInt32(data, offset); offset += 4;
            header.LumpVersion = BitConverter.ToInt32(data, offset); offset += 4;
            header.LumpLength = BitConverter.ToInt32(data, offset); offset += 4;
            header.MapRevision = BitConverter.ToInt32(data, offset); offset += 4;

            return header;
        }

        public byte[] Serialize()
        {
            int offset = 0;
            byte[] data = new byte[Marshal.SizeOf(new LMPHeader())];

            Array.Copy(BitConverter.GetBytes(LumpOffset), 0, data, offset, 4); offset += 4;
            Array.Copy(BitConverter.GetBytes(LumpID), 0, data, offset, 4); offset += 4;
            Array.Copy(BitConverter.GetBytes(LumpVersion), 0, data, offset, 4); offset += 4;
            Array.Copy(BitConverter.GetBytes(LumpLength), 0, data, offset, 4); offset += 4;
            Array.Copy(BitConverter.GetBytes(MapRevision), 0, data, offset, 4); offset += 4;

            return data;
        }
    }
}
