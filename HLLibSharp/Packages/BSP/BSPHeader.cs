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

namespace HLLib.Packages.BSP
{
    public class BSPHeader
    {
        /// <summary>
        /// Total size of a BSPHeader object
        /// </summary>
        public const int ObjectSize = 4 + (HL_BSP_LUMP_COUNT * BSPLump.ObjectSize);

        private const int HL_BSP_LUMP_COUNT = 15;

        /// <summary>
        /// Version
        /// </summary>
        public uint Version { get; set; }

        /// <summary>
        /// Lumps
        /// </summary>
        public BSPLump[] Lumps { get; set; } = new BSPLump[HL_BSP_LUMP_COUNT];

        public static BSPHeader Create(byte[] data, ref int offset)
        {
            BSPHeader header = new BSPHeader();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            header.Version = BitConverter.ToUInt32(data, offset); offset += 4;
            for (int i = 0; i < HL_BSP_LUMP_COUNT; i++)
            {
                header.Lumps[i] = BSPLump.Create(data, ref offset);
            }

            return header;
        }
    }
}