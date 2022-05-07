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

namespace HLLib.Packages.GCF
{
    public class GCFFragmentationMap
    {
        /// <summary>
        /// Total size of a GCFFragmentationMap object
        /// </summary>
        public const int ObjectSize = 4;

        /// <summary>
        /// The index of the next data block.
        /// </summary>
        public uint NextDataBlockIndex { get; set; }

        public static GCFFragmentationMap Create(byte[] data, ref int offset)
        {
            GCFFragmentationMap fragmentationMap = new GCFFragmentationMap();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            fragmentationMap.NextDataBlockIndex = BitConverter.ToUInt32(data, offset); offset += 4;

            return fragmentationMap;
        }
    }
}