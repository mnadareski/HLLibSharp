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

namespace HLLib.Packages.BSP
{
    public class BSPLump
    {
        /// <summary>
        /// Offset
        /// </summary>
        public uint Offset { get; set; }

        /// <summary>
        /// Length
        /// </summary>
        public uint Length { get; set; }

        public static BSPLump Create(byte[] data, ref int offset)
        {
            BSPLump lump = new BSPLump();

            // Check to see if the data is valid
            if (data == null || data.Length < Marshal.SizeOf(lump))
                return null;

            lump.Offset = BitConverter.ToUInt32(data, offset); offset += 4;
            lump.Length = BitConverter.ToUInt32(data, offset); offset += 4;

            return lump;
        }
    }
}