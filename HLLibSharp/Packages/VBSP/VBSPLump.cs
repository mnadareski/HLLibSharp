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

namespace HLLib.Packages.VBSP
{
    public sealed class VBSPLump
    {
        /// <summary>
        /// Total size of a VBSPLump object
        /// </summary>
        public const int ObjectSize = (4 * 4);

        public uint Offset { get; set; }

        public uint Length { get; set; }

        /// <summary>
        /// Default to zero.
        /// </summary>
        public uint Version { get; set; }

        /// <summary>
        /// Default to (char)0, (char)0, (char)0, (char)0.
        /// </summary>
        public char[] FourCC { get; set; }

        public static VBSPLump Create(byte[] data, ref int offset)
        {
            VBSPLump lump = new VBSPLump();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            lump.Offset = BitConverter.ToUInt32(data, offset); offset += 4;
            lump.Length = BitConverter.ToUInt32(data, offset); offset += 4;
            lump.Version = BitConverter.ToUInt32(data, offset); offset += 4;
            lump.FourCC = new char[4];
            for (int i = 0; i < 4; i++)
            {
                lump.FourCC[i] = (char)data[offset++];
            }

            return lump;
        }
    }
}
