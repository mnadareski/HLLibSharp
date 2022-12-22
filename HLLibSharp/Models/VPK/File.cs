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
    /// Valve Package File
    /// </summary>
    public sealed class File
    {
        /// <summary>
        /// Number of internal archives
        /// </summary>
        public uint ArchiveCount { get; set; }

        /// <summary>
        /// Header data
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// Extended header data
        /// </summary>
        public ExtendedHeader ExtendedHeader { get; set; }

        /// <summary>
        /// Archive hashes data
        /// </summary>
        public ArchiveHash[] ArchiveHashes { get; set; }

        /// <summary>
        /// Directory items data
        /// </summary>
        public DirectoryItem[] DirectoryItems { get; set; }
    }
}
