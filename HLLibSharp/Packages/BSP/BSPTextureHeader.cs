﻿/*
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
    public sealed class BSPTextureHeader
    {
        /// <summary>
        /// Total size of a BSPTextureHeader object
        /// </summary>
        /// <remarks>
        /// This does not include variable length fields
        /// </remarks>
        public const int ObjectSize = 4;

        /// <summary>
        /// Texture count
        /// </summary>
        public uint TextureCount { get; set; }

        /// <summary>
        /// Offsets
        /// </summary>
        public uint[] Offsets { get; set; }

        public static BSPTextureHeader Create(byte[] data, ref int offset)
        {
            BSPTextureHeader textureHeader = new BSPTextureHeader();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            textureHeader.TextureCount = BitConverter.ToUInt32(data, offset); offset += 4;
            textureHeader.Offsets = new uint[textureHeader.TextureCount];
            for (int i = 0; i < textureHeader.TextureCount; i++)
            {
                textureHeader.Offsets[i] = BitConverter.ToUInt32(data, offset); offset += 4;
            }

            return textureHeader;
        }
    }
}