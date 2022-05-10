/*
 * HLLib
 * Copyright (C) 2006-2012 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

namespace HLLib.Packages.SGA
{
    public class SGADirectoryHeader<T>
    {
        /// <summary>
        /// Total size of a SGADirectoryHeader object
        /// </summary>
        /// <remarks>
        /// This does not account for the typed variables
        /// </remarks>
        public const int ObjectSize = (4 * 4);

        public uint SectionOffset { get; set; }

        public T SectionCount { get; set; }

        public uint FolderOffset { get; set; }

        public T FolderCount { get; set; }

        public uint FileOffset { get; set; }

        public T FileCount { get; set; }

        public uint StringTableOffset { get; set; }

        public T StringTableCount { get; set; }
    }
}
