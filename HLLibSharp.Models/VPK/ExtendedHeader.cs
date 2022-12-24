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
    /// <summary>
    /// Added in version 2.
    /// </summary>
    public sealed class ExtendedHeader
    {
        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy0;

        /// <summary>
        /// Archive hash length
        /// </summary>
        public uint ArchiveHashLength;

        /// <summary>
        /// Looks like some more MD5 hashes.
        /// </summary>
        public uint ExtraLength;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy1;
    }
}
