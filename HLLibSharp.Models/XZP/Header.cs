/*
 * HLLib
 * Copyright (C) 2006-2010 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

namespace HLLib.Models.XZP
{
    public sealed class Header
    {
        public string Signature;

        public uint Version;

        public uint PreloadDirectoryEntryCount;

        public uint DirectoryEntryCount;

        public uint PreloadBytes;

        public uint HeaderLength;

        public uint DirectoryItemCount;

        public uint DirectoryItemOffset;

        public uint DirectoryItemLength;
    }
}
