/*
 * HLLib
 * Copyright (C) 2006-2012 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

using HLLib.Directory;
using HLLib.Mappings;

namespace HLLib.Packages.SGA
{
    /// <summary>
    /// Specialization SGAFile7 and up where the CRC moved to the header and the CRC is of the compressed data and there are stronger hashes.
    /// </summary>
    public class SGASpecializedDirectory<TSGAHeader, TSGADirectoryHeader, TSGASection, TSGAFolder, TSGAFile, U> : SGADirectory
        where TSGAHeader : SGAHeaderBase
        where TSGADirectoryHeader : SGADirectoryHeader<U>
        where TSGASection : SGASection<U>
        where TSGAFolder : SGAFolder<U>
        where TSGAFile : SGAFile4
    {
        #region Views

        /// <summary>
        /// View representing the header directory data
        /// </summary>
        private View HeaderDirectoryView;

        #endregion

        #region Fields

        /// <summary>
        /// Deserialized directory header data
        /// </summary>
        public TSGADirectoryHeader DirectoryHeader { get; protected set; }

        /// <summary>
        /// Deserialized sections data
        /// </summary>
        public TSGASection[] Sections { get; protected set; }

        /// <summary>
        /// Deserialized folders data
        /// </summary>
        public TSGAFolder[] Folders { get; protected set; }

        /// <summary>
        /// Deserialized files data
        /// </summary>
        public TSGAFile[] Files { get; protected set; }

        /// <summary>
        /// Deserialized string table data
        /// </summary>
        public string[] StringTable { get; protected set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file"></param>
        public SGASpecializedDirectory(SGAFile file);

        #region Attributes

        /// <summary>
        /// Per-directory creation of an item attribute object, if possible
        /// </summary>
        /// <param name="packageAttribute">Package attribute to get derive from</param>
        /// <param name="attribute">Output attribute for that value</param>
        /// <returns>True if the value could be derived, false otherwise</returns>
        protected override bool GetItemAttributeInternal(DirectoryItem item, PackageAttributeType packageAttribute, out PackageAttribute attribute);

        #endregion

        #region File Validation

        /// <summary>
        /// Per-package implementation of file extraction checks
        /// </summary>
        /// <param name="file">DirectoryFile representing the internal file</param>
        /// <param name="validation">Output validation value</param>
        /// <returns>True if the validaiton could be performed, false otherwise</returns>
        protected override bool GetFileValidationInternal(DirectoryFile file, out Validation validation);

        #endregion
    }
}
