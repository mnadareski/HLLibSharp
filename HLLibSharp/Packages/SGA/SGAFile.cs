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
using HLLib.Streams;

namespace HLLib.Packages.SGA
{
    public class SGAFile : Package
    {
        #region Constants

        /// <summary>
        /// Length of a SGA checksum in bytes
        /// </summary>
        private const int HL_SGA_CHECKSUM_LENGTH = 0x00008000;

        /// <inheritdoc/>
        public override string[] AttributeNames => new string[] { "Major Version", "Minor Version", "File MD5", "Name", "Header MD5" };

        /// <inheritdoc/>
        public override string[] ItemAttributeNames => new string[] { "Section Alias", "Section Name", "Modified", "Type", "CRC", "Verification" };

        /// <summary>
        /// Set of valid verification names
        /// </summary>
        public static readonly string[] VerificationNames = new string[] { "None", "CRC", "CRC Blocks", "MD5 Blocks", "SHA1 Blocks" };

        #endregion

        #region Views

        /// <summary>
        /// View representing header data
        /// </summary>
        private View HeaderView;

        #endregion

        #region Fields

        /// <summary>
        /// Deserialized header data
        /// </summary>
        public SGAHeaderBase Header { get; private set; }

        /// <summary>
        /// Deserialized directory data
        /// </summary>
        public SGADirectory Directory { get; private set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public SGAFile() : base()
        {
            HeaderView = null;

            Header = null;
            Directory = null;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SGAFile() => Close();

        #region Descriptors

        /// <inheritdoc/>
        public override PackageType PackageType => PackageType.HL_PACKAGE_SGA;

        /// <inheritdoc/>
        public override string Extension => "sga";

        /// <inheritdoc/>
        public override string Description => "Archive File";

        #endregion

        #region Mappings

        /// <inheritdoc/>
        protected override DirectoryFolder CreateRoot()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        protected override bool MapDataStructures()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        protected override void UnmapDataStructures()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Attributes

        /// <inheritdoc/>
        protected override bool GetAttributeInternal(PackageAttributeType packageAttribute, out PackageAttribute attribute)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        protected override bool GetItemAttributeInternal(DirectoryItem item, PackageAttributeType packageAttribute, out PackageAttribute attribute)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region File Extraction Check

        /// <inheritdoc/>
        protected override bool GetFileExtractableInternal(DirectoryFile file, out bool extractable)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region File Validation

        /// <inheritdoc/>
        protected override bool GetFileValidationInternal(DirectoryFile file, out Validation validation)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region File Size

        /// <inheritdoc/>
        protected override bool GetFileSizeInternal(DirectoryFile file, out int size)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        protected override bool GetFileSizeOnDiskInternal(DirectoryFile file, out int size)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Streams

        /// <inheritdoc/>
        protected override bool CreateStreamInternal(DirectoryFile file, bool readEncrypted, out Stream stream)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
