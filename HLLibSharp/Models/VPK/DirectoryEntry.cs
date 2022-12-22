/*
 * HLLib
 * Copyright (C) 2006-2013 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

namespace HLLib.Models.VPK
{
    public sealed class DirectoryEntry
    {
        public uint CRC;

        public ushort PreloadBytes;

        public ushort ArchiveIndex;

        public uint EntryOffset;

        public uint EntryLength;

        /// <summary>
        /// Always 0xffff.
        /// </summary>
        public ushort Dummy0;
    }
}
