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

namespace HLLib.Packages.WAD
{
	public class WADLump
    {
		public uint Offset { get; set; }

		public uint DiskLength { get; set; }

		public uint Length { get; set; }

		public byte Type { get; set; }

		public byte Compression { get; set; }

		public byte Padding0 { get; set; }

		public byte Padding1 { get; set; }

		public char[] Name { get; set; }

        public static WADLump Create(byte[] data, ref int offset)
        {
            WADLump lump = new WADLump();

            // Check to see if the data is valid
            if (data == null || data.Length < Marshal.SizeOf(lump))
                return null;

            lump.Offset = BitConverter.ToUInt32(data, offset); offset += 4;
            lump.DiskLength = BitConverter.ToUInt32(data, offset); offset += 4;
            lump.Length = BitConverter.ToUInt32(data, offset); offset += 4;
            lump.Type = data[offset++];
            lump.Compression = data[offset++];
            lump.Padding0 = data[offset++];
            lump.Padding1 = data[offset++];
            lump.Name = Encoding.ASCII.GetString(data, offset, 16).ToCharArray(); offset += 16;

            return lump;
        }
    }
}
