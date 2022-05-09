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

namespace HLLib.Packages.BSP
{
    public sealed class BSPTexture
    {
        /// <summary>
        /// Total size of a BSPTexture object
        /// </summary>
        public const int ObjectSize = 16 + 4 + 4 + (4 * 4);

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
    
        /// <summary>
        /// Width
        /// </summary>
        public uint Width { get; set; }

        /// <summary>
        /// Height
        /// </summary>
        public uint Height { get; set; }

        /// <summary>
        /// Offsets
        /// </summary>
        public uint[] Offsets { get; set; }

        public static BSPTexture Create(byte[] data, ref int offset)
        {
            BSPTexture texture = new BSPTexture();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            texture.Name = Encoding.ASCII.GetString(data, offset, 16); offset += 16;
            texture.Width = BitConverter.ToUInt32(data, offset); offset += 4;
            texture.Height = BitConverter.ToUInt32(data, offset); offset += 4;
            texture.Offsets = new uint[4];
            for (int i = 0; i < 4; i++)
            {
                texture.Offsets[i] = BitConverter.ToUInt32(data, offset); offset += 4;
            }

            return texture;
        }
    }
}