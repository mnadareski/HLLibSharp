/*
 * HLLib
 * Copyright (C) 2006-2012 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

// TODO: Include zlib to sync with newest version
namespace HLLib.Models.SGA
{
    /// <summary>
    /// Specialization File7 and up where the CRC moved to the header and the CRC is of the compressed data and there are stronger hashes.
    /// </summary>
    public class SpecializedDirectory<THeader, TDirectoryHeader, TSection, TFolder, TFile, U> : Directory
        where THeader : Header
        where TDirectoryHeader : DirectoryHeader<U>
        where TSection : Section<U>
        where TFolder : Folder<U>
        where TFile : File4
    {
        /// <summary>
        /// Source SGA file
        /// </summary>
        public File File { get; set; }

        /// <summary>
        /// Directory header data
        /// </summary>
        public TDirectoryHeader DirectoryHeader { get; set; }

        /// <summary>
        /// Sections data
        /// </summary>
        public TSection[] Sections { get; set; }

        /// <summary>
        /// Folders data
        /// </summary>
        public TFolder[] Folders { get; set; }

        /// <summary>
        /// Files data
        /// </summary>
        public TFile[] Files { get; set; }

        /// <summary>
        /// String table data
        /// </summary>
        public string StringTable { get; set; }
    }
}
