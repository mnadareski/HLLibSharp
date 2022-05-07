/*
 * HLLib
 * Copyright (C) 2006-2010 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

using System;
using System.Text;
using HLLib.Directory;
using HLLib.Mappings;
using HLLib.Streams;

namespace HLLib.Packages.NCF
{
    /// <summary>
    /// Half-Life No Cache File
    /// </summary>
    public sealed class NCFFile : Package
    {
        #region Constants

        /// <summary>
        /// The item is a file.
        /// </summary>
        public const int HL_NCF_FLAG_FILE = 0x00004000;

        /// <summary>
        /// The item is encrypted.
        /// </summary>
        public const int HL_NCF_FLAG_ENCRYPTED = 0x00000100;

        /// <summary>
        /// Backup the item before overwriting it.
        /// </summary>
        public const int HL_NCF_FLAG_BACKUP_LOCAL = 0x00000040;

        /// <summary>
        /// The item is to be copied to the disk.
        /// </summary>
        public const int HL_NCF_FLAG_COPY_LOCAL = 0x0000000a;

        /// <summary>
        /// Don't overwrite the item if copying it to the disk and the item already exis
        /// </summary>
        public const int HL_NCF_FLAG_COPY_LOCAL_NO_OVERWRITE = 0x00000001;

        /// <inheritdoc/>
        public override string[] AttributeNames => new string[] { "Version", "Cache ID", "Last Version Played" };

        /// <inheritdoc/>
        public override string[] ItemAttributeNames => new string[] { "Encrypted", "Copy Locally", "Overwrite Local Copy", "Backup Local Copy", "Flags" };

        #endregion

        #region Views

        /// <summary>
        /// View representing header data
        /// </summary>
        private View HeaderView;

        #endregion

        #region Fields

        /// <summary>
        /// Root path of the package
        /// </summary>
        public string RootPath
        {
            get
            {
                return rootPath;
            }
            set
            {
                if (Opened)
                    return;

                rootPath = null;
                if (string.IsNullOrEmpty(value))
                    return;

                rootPath = value;
            }
        }

        /// <summary>
        /// Internal representation of the root path of the package
        /// </summary>
        private string rootPath;

        /// <summary>
        /// Deserialized header data
        /// </summary>
        public NCFHeader Header { get; private set; }

        /// <summary>
        /// Deserialized directory header data
        /// </summary>
        public NCFDirectoryHeader DirectoryHeader { get; private set; }

        /// <summary>
        /// Deserialized directory entries data
        /// </summary>
        public NCFDirectoryEntry[] DirectoryEntries { get; private set; }

        /// <summary>
        /// Deserialized directory names data
        /// </summary>
        public string DirectoryNames { get; private set; }

        /// <summary>
        /// Deserialized directory info 1 entries data
        /// </summary>
        public NCFDirectoryInfo1Entry[] DirectoryInfo1Entries { get; private set; }

        /// <summary>
        /// Deserialized directory info 2 entries data
        /// </summary>
        public NCFDirectoryInfo2Entry[] DirectoryInfo2Entries { get; private set; }

        /// <summary>
        /// Deserialized directory copy entries data
        /// </summary>
        public NCFDirectoryCopyEntry[] DirectoryCopyEntries { get; private set; }

        /// <summary>
        /// Deserialized directory local entries data
        /// </summary>
        public NCFDirectoryLocalEntry[] DirectoryLocalEntries { get; private set; }

        /// <summary>
        /// Deserialized unknown header data
        /// </summary>
        public NCFUnknownHeader UnknownHeader { get; private set; }

        /// <summary>
        /// Deserialized unknown entries data
        /// </summary>
        public NCFUnknownEntry[] UnknownEntries { get; private set; }

        /// <summary>
        /// Deserialized checksum header data
        /// </summary>
        public NCFChecksumHeader ChecksumHeader { get; private set; }

        /// <summary>
        /// Deserialized checksum map header data
        /// </summary>
        public NCFChecksumMapHeader ChecksumMapHeader { get; private set; }

        /// <summary>
        /// Deserialized checksum map entries data
        /// </summary>
        public NCFChecksumMapEntry[] ChecksumMapEntries { get; private set; }

        /// <summary>
        /// Deserialized checksum entries data
        /// </summary>
        public NCFChecksumEntry[] ChecksumEntries { get; private set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public NCFFile() : base()
        {
            Header = null;

            DirectoryHeader = null;
            DirectoryEntries = null;
            DirectoryNames = null;
            DirectoryInfo1Entries = null;
            DirectoryInfo2Entries = null;
            DirectoryCopyEntries = null;
            DirectoryLocalEntries = null;

            UnknownHeader = null;
            UnknownEntries = null;

            ChecksumHeader = null;
            ChecksumMapHeader = null;
            ChecksumMapEntries = null;
            ChecksumEntries = null;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~NCFFile() => Close();

        #region Descriptors

        /// <inheritdoc/>
        public override PackageType PackageType => PackageType.HL_PACKAGE_NCF;

        /// <inheritdoc/>
        public override string Extension => "ncf";

        /// <inheritdoc/>
        public override string Description => "Half-Life No Cache File";

        #endregion

        #region Mappings

        /// <inheritdoc/>
        protected override DirectoryFolder CreateRoot()
        {
            DirectoryFolder root = new DirectoryFolder("root", 0, null, this, null);
            CreateRoot(root);
            return root;
        }

        /// <inheritdoc/>
        protected override bool MapDataStructures()
        {
            #region Determine the size of the header and validate it.

            long headerSize = 0;

            #region Header.

            if (NCFHeader.ObjectSize > Mapping.MappingSize)
            {
                Console.WriteLine("Invalid file: the file map is too small for it's header.");
                return false;
            }

            if (!Mapping.Map(ref HeaderView, 0, NCFHeader.ObjectSize))
                return false;

            byte[] headerViewData = HeaderView.ViewData;
            int pointer = 0;

            Header = NCFHeader.Create(headerViewData, ref pointer);
            if (Header == null)
            {
                Console.WriteLine("Invalid file: the file's header is null (contains no data).");
                return false;
            }

            if (Header.MajorVersion != 2 || Header.MinorVersion != 1)
            {
                Console.WriteLine($"Invalid NCF version (v{Header.MajorVersion}.{Header.MinorVersion}): you have a version of a NCF file that HLLib does not know how to read. Check for product updates.");
                return false;
            }

            headerSize += NCFHeader.ObjectSize;

            #endregion

            #region Directory.

            if (!Mapping.Map(ref HeaderView, headerSize, NCFDirectoryHeader.ObjectSize))
                return false;

            DirectoryHeader = NCFDirectoryHeader.Create(headerViewData, ref pointer);

            headerSize += DirectoryHeader.DirectorySize;
            headerSize += NCFUnknownHeader.ObjectSize;
            headerSize += DirectoryHeader.ItemCount * NCFUnknownEntry.ObjectSize;

            #endregion

            #region Checksums.

            if (!Mapping.Map(ref HeaderView, headerSize, NCFChecksumHeader.ObjectSize))
                return false;

            ChecksumHeader = NCFChecksumHeader.Create(headerViewData, ref pointer);
            headerSize += NCFChecksumHeader.ObjectSize + ChecksumHeader.ChecksumSize;

            #endregion

            #endregion

            #region Map the header.

            if (!Mapping.Map(ref HeaderView, 0, (int)headerSize))
                return false;

            pointer = 0;

            Header = NCFHeader.Create(headerViewData, ref pointer);

            DirectoryHeader = NCFDirectoryHeader.Create(headerViewData, ref pointer);
            DirectoryEntries = new NCFDirectoryEntry[DirectoryHeader.ItemCount];
            for (int i = 0; i < DirectoryHeader.ItemCount; i++)
            {
                DirectoryEntries[i] = NCFDirectoryEntry.Create(headerViewData, ref pointer);
            }

            DirectoryNames = Encoding.ASCII.GetString(headerViewData, pointer, (int)DirectoryHeader.NameSize);

            DirectoryInfo1Entries = new NCFDirectoryInfo1Entry[DirectoryHeader.Info1Count];
            for (int i = 0; i < DirectoryHeader.Info1Count; i++)
            {
                DirectoryInfo1Entries[i] = NCFDirectoryInfo1Entry.Create(headerViewData, ref pointer);
            }

            DirectoryInfo2Entries = new NCFDirectoryInfo2Entry[DirectoryHeader.ItemCount];
            for (int i = 0; i < DirectoryHeader.ItemCount; i++)
            {
                DirectoryInfo2Entries[i] = NCFDirectoryInfo2Entry.Create(headerViewData, ref pointer);
            }

            DirectoryCopyEntries = new NCFDirectoryCopyEntry[DirectoryHeader.CopyCount];
            for (int i = 0; i < DirectoryHeader.CopyCount; i++)
            {
                DirectoryCopyEntries[i] = NCFDirectoryCopyEntry.Create(headerViewData, ref pointer);
            }

            DirectoryLocalEntries = new NCFDirectoryLocalEntry[DirectoryHeader.LocalCount];
            for (int i = 0; i < DirectoryHeader.LocalCount; i++)
            {
                DirectoryLocalEntries[i] = NCFDirectoryLocalEntry.Create(headerViewData, ref pointer);
            }

            pointer = (int)DirectoryHeader.DirectorySize;
            UnknownHeader = NCFUnknownHeader.Create(headerViewData, ref pointer);
            UnknownEntries = new NCFUnknownEntry[DirectoryHeader.ItemCount];
            for (int i = 0; i < DirectoryHeader.ItemCount; i++)
            {
                UnknownEntries[i] = NCFUnknownEntry.Create(headerViewData, ref pointer);
            }

            ChecksumHeader = NCFChecksumHeader.Create(headerViewData, ref pointer);
            ChecksumMapHeader = NCFChecksumMapHeader.Create(headerViewData, ref pointer);

            ChecksumMapEntries = new NCFChecksumMapEntry[ChecksumMapHeader.ItemCount];
            for (int i = 0; i < ChecksumMapHeader.ItemCount; i++)
            {
                ChecksumMapEntries[i] = NCFChecksumMapEntry.Create(headerViewData, ref pointer);
            }

            ChecksumEntries = new NCFChecksumEntry[ChecksumMapHeader.ChecksumCount];
            for (int i = 0; i < ChecksumMapHeader.ChecksumCount; i++)
            {
                ChecksumEntries[i] = NCFChecksumEntry.Create(headerViewData, ref pointer);
            }

            #endregion

            return true;
        }

        /// <inheritdoc/>
        protected override void UnmapDataStructures()
        {
            RootPath = null;

            Header = null;

            DirectoryHeader = null;
            DirectoryEntries = null;
            DirectoryNames = null;
            DirectoryInfo1Entries = null;
            DirectoryInfo2Entries = null;
            DirectoryCopyEntries = null;
            DirectoryLocalEntries = null;

            UnknownHeader = null;
            UnknownEntries = null;

            ChecksumHeader = null;
            ChecksumMapHeader = null;
            ChecksumMapEntries = null;
            ChecksumEntries = null;

            Mapping.Unmap(HeaderView);
        }

        /// <summary>
        /// Create the root directory for a single directory
        /// </summary>
        /// <param name="folder">Directory to create the root for</param>
        private void CreateRoot(DirectoryFolder folder)
        {
            // Get the first directory item.
            uint index = DirectoryEntries[folder.ID].FirstIndex;

            // Loop through directory items.
            while (index != 0 && index != 0xffffffff)
            {
                // Check if the item is a folder.
                if ((DirectoryEntries[index].DirectoryFlags & HL_NCF_FLAG_FILE) == 0)
                {
                    // Add the directory item to the current folder.
                    DirectoryFolder subFolder = folder.AddFolder(DirectoryNames.Substring((int)DirectoryEntries[index].NameOffset), index);

                    // Build the new folder.
                    CreateRoot(subFolder);
                }
                else
                {
                    // Add the directory item to the current folder.
                    folder.AddFile(DirectoryNames.Substring((int)DirectoryEntries[index].NameOffset), index);
                }

                // Get the next directory item.
                index = DirectoryEntries[index].NextIndex;
            }
        }

        #endregion

        #region Attributes

        /// <inheritdoc/>
        protected override bool GetAttributeInternal(PackageAttributeType packageAttribute, out PackageAttribute attribute)
        {
            attribute = new PackageAttribute();
            switch (packageAttribute)
            {
                case PackageAttributeType.HL_NCF_PACKAGE_VERSION:
                    attribute.SetUnsignedInteger(AttributeNames[(int)packageAttribute], Header.MinorVersion, false);
                    return true;
                case PackageAttributeType.HL_NCF_PACKAGE_ID:
                    attribute.SetUnsignedInteger(AttributeNames[(int)packageAttribute], Header.CacheID, false);
                    return true;
                case PackageAttributeType.HL_NCF_PACKAGE_LAST_VERSION_PLAYED:
                    attribute.SetUnsignedInteger(AttributeNames[(int)packageAttribute], Header.LastVersionPlayed, false);
                    return true;
                default:
                    return false;
            }
        }

        /// <inheritdoc/>
        protected override bool GetItemAttributeInternal(DirectoryItem item, PackageAttributeType packageAttribute, out PackageAttribute attribute)
        {
            attribute = new PackageAttribute();
            switch (item.ItemType)
            {
                case DirectoryItemType.HL_ITEM_FILE:
                    DirectoryFile file = (DirectoryFile)item;
                    switch (packageAttribute)
                    {
                        case PackageAttributeType.HL_NCF_ITEM_ENCRYPTED:
                            attribute.SetBoolean(ItemAttributeNames[(int)packageAttribute], (DirectoryEntries[file.ID].DirectoryFlags & HL_NCF_FLAG_ENCRYPTED) != 0);
                            return true;
                        case PackageAttributeType.HL_NCF_ITEM_COPY_LOCAL:
                            attribute.SetBoolean(ItemAttributeNames[(int)packageAttribute], (DirectoryEntries[file.ID].DirectoryFlags & HL_NCF_FLAG_COPY_LOCAL) != 0);
                            return true;
                        case PackageAttributeType.HL_NCF_ITEM_OVERWRITE_LOCAL:
                            attribute.SetBoolean(ItemAttributeNames[(int)packageAttribute], (DirectoryEntries[file.ID].DirectoryFlags & HL_NCF_FLAG_COPY_LOCAL_NO_OVERWRITE) == 0);
                            return true;
                        case PackageAttributeType.HL_NCF_ITEM_BACKUP_LOCAL:
                            attribute.SetBoolean(ItemAttributeNames[(int)packageAttribute], (DirectoryEntries[file.ID].DirectoryFlags & HL_NCF_FLAG_BACKUP_LOCAL) != 0);
                            return true;
                        case PackageAttributeType.HL_NCF_ITEM_FLAGS:
                            attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], DirectoryEntries[file.ID].DirectoryFlags, true);
                            return true;
                    }
                    break;
                case DirectoryItemType.HL_ITEM_FOLDER:
                    DirectoryFolder folder = (DirectoryFolder)item;
                    switch (packageAttribute)
                    {
                        case PackageAttributeType.HL_NCF_ITEM_FLAGS:
                            attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], DirectoryEntries[folder.ID].DirectoryFlags, true);
                            return true;
                    }
                    break;
            }

            return false;
        }

        #endregion

        #region File Extraction Check

        /// <inheritdoc/>
        protected override bool GetFileExtractableInternal(DirectoryFile file, out bool extractable)
        {
            extractable = false;
            if (RootPath != null)
            {
                string temp = GetPath(file, 512);
                if (Utility.GetFileSize(temp, out long size))
                {
                    if (size >= DirectoryEntries[file.ID].ItemSize)
                        extractable = true;
                }
                else
                {
                    if (DirectoryEntries[file.ID].ItemSize == 0)
                        extractable = true;
                }
            }

            return true;
        }

        #endregion

        #region File Validation

        /// <inheritdoc/>
        protected override bool GetFileValidationInternal(DirectoryFile file, out Validation validation)
        {
            if (RootPath != null)
            {
                string temp = GetPath(file, 512);
                if (Utility.GetFileSize(temp, out long size))
                {
                    if (size < DirectoryEntries[file.ID].ItemSize)
                    {
                        validation = Validation.HL_VALIDATES_INCOMPLETE;
                    }
                    else if ((DirectoryEntries[file.ID].DirectoryFlags & HL_NCF_FLAG_ENCRYPTED) != 0)
                    {
                        // No way of checking, assume it's ok.
                        validation = Validation.HL_VALIDATES_ASSUMED_OK;
                    }
                    else if (DirectoryEntries[file.ID].ChecksumIndex == 0xffffffff)
                    {
                        // File has no checksum.
                        validation = Validation.HL_VALIDATES_ASSUMED_OK;
                    }
                    else
                    {
                        FileStream Stream = new FileStream(temp);

                        if (Stream.Open(FileModeFlags.HL_MODE_READ))
                        {
                            validation = Validation.HL_VALIDATES_OK;

                            long totalBytes = 0;
                            int bufferSize;
                            byte[] buffer = new byte[DirectoryHeader.ChecksumDataLength];

                            NCFChecksumMapEntry checksumMapEntry = ChecksumMapEntries[DirectoryEntries[file.ID].ChecksumIndex];

                            uint i = 0;
                            while ((bufferSize = Stream.Read(buffer, 0, DirectoryHeader.ChecksumDataLength)) != 0)
                            {
                                if (i >= checksumMapEntry.ChecksumCount)
                                {
                                    // Something bad happened.
                                    validation = Validation.HL_VALIDATES_ERROR;
                                    break;
                                }

                                uint checksum = Checksum.Adler32(buffer, bufferSize) ^ Checksum.CRC32(buffer, bufferSize);
                                if (checksum != ChecksumEntries[checksumMapEntry.FirstChecksumIndex + i].Checksum)
                                {
                                    validation = Validation.HL_VALIDATES_CORRUPT;
                                    break;
                                }

                                totalBytes += bufferSize;
                                i++;
                            }

                            Stream.Close();
                        }
                        else
                        {
                            validation = Validation.HL_VALIDATES_ERROR;
                        }
                    }
                }
                else
                {
                    // Not found.
                    if (DirectoryEntries[file.ID].ItemSize != 0)
                    {
                        validation = Validation.HL_VALIDATES_INCOMPLETE;
                    }
                    else
                    {
                        validation = Validation.HL_VALIDATES_OK;
                    }
                }
            }
            else
            {
                validation = Validation.HL_VALIDATES_ASSUMED_OK;
            }

            return true;
        }

        #endregion

        #region File Size

        /// <inheritdoc/>
        protected override bool GetFileSizeInternal(DirectoryFile file, out int size)
        {
            size = (int)DirectoryEntries[file.ID].ItemSize;
            return true;
        }

        /// <inheritdoc/>
        protected override bool GetFileSizeOnDiskInternal(DirectoryFile file, out int size)
        {
            size = 0;
            if (RootPath != null)
            {
                string temp = GetPath(file, 512);
                Utility.GetFileSize(temp, out long longSize);
                size = (int)longSize;
            }

            return true;
        }

        #endregion

        #region Streams

        /// <inheritdoc/>
        protected override bool CreateStreamInternal(DirectoryFile file, bool readEncrypted, out Stream stream)
        {
            stream = null;
            if (!readEncrypted && (DirectoryEntries[file.ID].DirectoryFlags & HL_NCF_FLAG_ENCRYPTED) != 0)
            {
                Console.WriteLine("File is encrypted.");
                return false;
            }

            if (RootPath != null)
            {
                string temp = GetPath(file, 512);
                if (Utility.GetFileSize(temp, out long uiSize))
                {
                    if (uiSize >= DirectoryEntries[file.ID].ItemSize)
                    {
                        stream = new FileStream(temp);
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("File is incomplete.");
                        return false;
                    }
                }
                else
                {
                    if (DirectoryEntries[file.ID].ItemSize == 0)
                    {
                        // Fake an empty stream.
                        stream = new NullStream();
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("File not found.");
                        return false;
                    }
                }
            }
            else
            {
                Console.WriteLine("NCF files are indexes and do not contain any file data.");
                return false;
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Get the path from an internal file, limited by path length
        /// </summary>
        /// <param name="file">DirectoryFile representing the internal file</param>
        /// <param name="pathSize">Path length cap, in characters</param>
        /// <returns>Path for the file, null on error</returns>
        private string GetPath(DirectoryFile file, int pathSize)
        {
            string path = file.Name;
            DirectoryItem item = file.Parent;
            while (item != null)
            {
                string temp = path;
                if (item.Parent == null)
                    path = RootPath.Substring(0, pathSize);
                else
                    path = item.Name.Substring(0, pathSize);

                path = System.IO.Path.Combine(path, temp);
                item = item.Parent;
            }

            return path;
        }

        #endregion
    }
}
