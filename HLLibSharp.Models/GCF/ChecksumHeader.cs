/*
 * HLLib
 * Copyright (C) 2006-2010 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

namespace HLLib.Models.GCF
{
    public sealed class ChecksumHeader
    {
        /// <summary>
        /// Always 0x00000001
        /// </summary>
        public uint Dummy0;

        /// <summary>
        /// Size of LPGCFCHECKSUMHEADER & LPGCFCHECKSUMMAPHEADER & in bytes.
        /// </summary>
        public uint ChecksumSize;
    }
}