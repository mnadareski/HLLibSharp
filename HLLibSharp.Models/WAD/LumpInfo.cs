/*
 * HLLib
 * Copyright (C) 2006-2010 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

namespace HLLib.Models.WAD
{
    public sealed class LumpInfo
    {
        public string Name;

        public uint Width;

        public uint Height;

        public uint PixelOffset;

        // 12 bytes of unknown data

        public byte[] PixelData;

        public uint PaletteSize;

        public byte[] PaletteData;
    }
}
