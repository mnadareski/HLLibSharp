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

namespace HLLib.Packages.VBSP
{
    public sealed class VBSPHeader
    {
        /// <summary>
        /// Total size of a VBSPHeader object
        /// </summary>
        public const int ObjectSize = 4 + 4 + (VBSPLump.ObjectSize * HL_VBSP_LUMP_COUNT) + 4;

        private const int HL_VBSP_LUMP_COUNT = 64;

        /// <summary>
        /// BSP file signature.
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// BSP file version.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Lumps.
        /// </summary>
        public VBSPLump[] Lumps { get; set; }

        /// <summary>
        /// The map's revision (iteration, version) number.
        /// </summary>
        public int MapRevision { get; set; }

        public static VBSPHeader Create(byte[] data, ref int offset)
        {
            VBSPHeader header = new VBSPHeader();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            header.Signature = Encoding.ASCII.GetString(data, offset, 4); offset += 4;
            header.Version = BitConverter.ToInt32(data, offset); offset += 4;
            header.Lumps = new VBSPLump[HL_VBSP_LUMP_COUNT];
            for (int i = 0; i < HL_VBSP_LUMP_COUNT; i++)
            {
                header.Lumps[i] = VBSPLump.Create(data, ref offset);
            }
            header.MapRevision = BitConverter.ToInt32(data, offset); offset += 4;

            return header;
        }
    }
}
