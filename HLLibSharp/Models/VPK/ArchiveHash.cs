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
    public sealed class ArchiveHash
    {
        public uint ArchiveIndex;

        public uint ArchiveOffset;

        public uint Length;

        /// <summary>
        /// MD5
        /// </summary>
        public byte[] Hash;
    }
}
