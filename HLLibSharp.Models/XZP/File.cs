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
    /// <summary>
    /// XBox Package File
    /// </summary>
    public class File
    {
        /// <summary>
        /// Header data
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// Directory entries data
        /// </summary>
        public DirectoryEntry[] DirectoryEntries { get; set; }

        /// <summary>
        /// Preload directory entries data
        /// </summary>
        public DirectoryEntry[] PreloadDirectoryEntries { get; set; }

        /// <summary>
        /// Preload directory mappings data
        /// </summary>
        public DirectoryMapping[] PreloadDirectoryMappings { get; set; }

        /// <summary>
        /// Directory items data
        /// </summary>
        public DirectoryItem[] DirectoryItems { get; set; }

        /// <summary>
        /// Footer data
        /// </summary>
        public Footer Footer { get; set; }
    }
}
