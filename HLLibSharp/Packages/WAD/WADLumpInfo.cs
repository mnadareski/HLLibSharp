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

namespace HLLib.Packages.WAD
{
    public class WADLumpInfo
    {
        public uint Width { get; set; }

        public uint Height { get; set; }

        public uint PaletteSize { get; set; }

        public static WADLumpInfo Create(byte[] data, ref int offset)
        {
            WADLumpInfo lumpInfo = new WADLumpInfo();

            // Check to see if the data is valid
            if (data == null || data.Length < Marshal.SizeOf(lumpInfo))
                return null;

            lumpInfo.Width = BitConverter.ToUInt32(data, offset); offset += 4;
            lumpInfo.Height = BitConverter.ToUInt32(data, offset); offset += 4;
            lumpInfo.PaletteSize = BitConverter.ToUInt32(data, offset); offset += 4;

            return lumpInfo;
        }
    }
}
