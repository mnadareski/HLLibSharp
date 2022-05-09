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
using System.Linq;
using HLLib.Checksums;
using HLLib.Directory;
using HLLib.Mappings;
using HLLib.Packages.Common;
using HLLib.Streams;

namespace HLLib.Packages.VBSP
{
    /// <summary>
    /// Half-Life 2 Level
    /// </summary>
    public sealed class VBSPFile : Package
    {
        #region Constants

        /// <summary>
        /// Total number of lumps in the package
        /// </summary>
        public const int HL_VBSP_LUMP_COUNT = 64;

        /// <summary>
        /// Index for the entities lump
        /// </summary>
        public const int HL_VBSP_LUMP_ENTITIES = 0;

        /// <summary>
        /// Idnex for the pakfile lump
        /// </summary>
        public const int HL_VBSP_LUMP_PAKFILE = 40;

        /// <summary>
        /// Zip local file header signature as an integer
        /// </summary>
        public const int HL_VBSP_ZIP_LOCAL_FILE_HEADER_SIGNATURE = 0x04034b50;

        /// <summary>
        /// Zip file header signature as an integer
        /// </summary>
        public const int HL_VBSP_ZIP_FILE_HEADER_SIGNATURE = 0x02014b50;

        /// <summary>
        /// Zip end of central directory record signature as an integer
        /// </summary>
        public const int HL_VBSP_ZIP_END_OF_CENTRAL_DIRECTORY_RECORD_SIGNATURE = 0x06054b50;

        /// <summary>
        /// Length of a ZIP checksum in bytes
        /// </summary>
        public const int HL_VBSP_ZIP_CHECKSUM_LENGTH = 0x00008000;

        /// <inheritdoc/>
        public override string[] AttributeNames => new string[] { "Version", "Map Revision" };

        /// <inheritdoc/>
        public override string[] ItemAttributeNames => new string[] { "Version", "Four CC", "Zip Disk", "Zip Comment", "Create Version", "Extract Version", "Flags", "Compression Method", "CRC", "Disk", "Comment" };

        #endregion

        #region Views

        /// <summary>
        /// View representing header data
        /// </summary>
        private View HeaderView;

        /// <summary>
        /// View representing file header data
        /// </summary>
        private View FileHeaderView;

        /// <summary>
        /// View representing the end of central directory data
        /// </summary>
        private View EndOfCentralDirectoryRecordView;

        #endregion

        #region Fields

        /// <summary>
        /// Deserialized directory header data
        /// </summary>
        public VBSPHeader Header { get; private set; }

        /// <summary>
        /// Deserialized end of central directory record data
        /// </summary>
        public ZIPEndOfCentralDirectoryRecord EndOfCentralDirectoryRecord { get; private set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public VBSPFile() : base()
        {
            HeaderView = null;
            FileHeaderView = null;
            EndOfCentralDirectoryRecordView = null;
            Header = null;
            EndOfCentralDirectoryRecord = null;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~VBSPFile() => Close();

        #region Descriptors

        /// <inheritdoc/>
        public override PackageType PackageType => PackageType.HL_PACKAGE_VBSP;

        /// <inheritdoc/>
        public override string Extension => "bsp";

        /// <inheritdoc/>
        public override string Description => "Half-Life 2 Level";

        #endregion

        #region Mappings

        /// <inheritdoc/>
        protected override DirectoryFolder CreateRoot()
        {
            DirectoryFolder root = new DirectoryFolder(this);

            string fileName = string.Empty;
            if (Header.Lumps[HL_VBSP_LUMP_ENTITIES].Length != 0)
            {
                fileName = GetFileName(256 - 4);
                if (string.IsNullOrEmpty(fileName) || fileName[0] == '\0')
                    root.AddFile("entities.ent", HL_VBSP_LUMP_ENTITIES);
                else
                    root.AddFile($"{fileName}.ent", HL_VBSP_LUMP_ENTITIES);
            }

            if (Header.Lumps[HL_VBSP_LUMP_PAKFILE].Length != 0)
            {
                fileName = GetFileName(256 - 4);
                if (string.IsNullOrEmpty(fileName) || fileName[0] == '\0')
                    root.AddFile("pakfile.zip", HL_VBSP_LUMP_PAKFILE);
                else
                    root.AddFile($"{fileName}.zip", HL_VBSP_LUMP_PAKFILE);
            }

            DirectoryFolder lumpFolder = root.AddFolder("lumps");
            for (uint i = 0; i < Header.Lumps.Length; i++)
            {
                if (Header.Lumps[i].Length > 0)
                {
                    string temp = GetFileName(256 - 10);
                    if (string.IsNullOrEmpty(temp) || temp[0] == '\0')
                        lumpFolder.AddFile($"lump_l_{i}.lmp", (uint)(Header.Lumps.Length + i));
                    else
                        lumpFolder.AddFile($"{temp}_l_{i}.lmp", (uint)(Header.Lumps.Length + i));

                }
            }

            if (EndOfCentralDirectoryRecord != null)
            {
                int offset = 0, pointer = 0;
                while (offset < EndOfCentralDirectoryRecord.CentralDirectorySize - 4)
                {
                    int test = BitConverter.ToInt32(FileHeaderView.ViewData, offset);
                    switch (test)
                    {
                        case HL_VBSP_ZIP_FILE_HEADER_SIGNATURE:
                            pointer = offset;
                            ZIPFileHeader fileHeader = ZIPFileHeader.Create(FileHeaderView.ViewData, ref pointer);
                            string headerFileName = fileHeader.FileName;

                            // Check if we have just a file, or if the file has directories we need to create.
                            if (headerFileName.IndexOf('/') == 0 && headerFileName.IndexOf('\\') == 0)
                            {
                                root.AddFile(headerFileName, HL_ID_INVALID, FileHeaderView.ViewData);
                            }
                            else
                            {
                                // Tokenize the file path and create the directories.
                                DirectoryFolder insertFolder = root;

                                string temp = string.Empty;
                                string[] token = headerFileName.Split('/', '\\');
                                int index = 0;
                                while (index < token.Length)
                                {
                                    temp = token[index++];
                                    if (index < token.Length)
                                    {
                                        // Check if the directory exists.
                                        DirectoryItem item = insertFolder.GetItem(temp);
                                        if (item == null || item.ItemType == DirectoryItemType.HL_ITEM_FILE)
                                        {
                                            // It doesn't, create it.
                                            insertFolder = insertFolder.AddFolder(temp);
                                        }
                                        else
                                        {
                                            // It does, use it.
                                            insertFolder = (DirectoryFolder)item;
                                        }
                                    }
                                }

                                // The file name is the last token, add it.
                                insertFolder.AddFile(temp, HL_ID_INVALID, FileHeaderView.ViewData);
                            }

                            offset += ZIPFileHeader.ObjectSize + fileHeader.FileNameLength + fileHeader.ExtraFieldLength + fileHeader.FileCommentLength;
                            break;
                        default:
                            offset = (int)EndOfCentralDirectoryRecord.CentralDirectorySize;
                            break;
                    }
                }
            }

            return root;
        }

        /// <inheritdoc/>
        protected override bool MapDataStructures()
        {
            if (VBSPHeader.ObjectSize > Mapping.MappingSize)
            {
                Console.WriteLine("Invalid file: the file map is too small for it's header.");
                return false;
            }

            if (!Mapping.Map(ref HeaderView, 0, VBSPHeader.ObjectSize))
                return false;

            int pointer = 0;
            Header = VBSPHeader.Create(HeaderView.ViewData, ref pointer);

            if (Header.Signature != "VBSP")
            {
                Console.WriteLine("Invalid file: the file's signature does not match.");
                return false;
            }

            // Versions:
            //  19-20:			Source
            //  21:				Source - The lump version property was moved to the start of the struct.
            //  0x00040014:		Dark Messiah - Looks like the 32 bit version has been split into two 16 bit fields.
            if ((Header.Version < 19 || Header.Version > 21) && Header.Version != 0x00040014)
            {
                Console.WriteLine($"Invalid VBSP version (v{Header.Version}): you have a version of a VBSP file that HLLib does not know how to read. Check for product updates.");
                return false;
            }

            // This block was commented out because test VBSPs with header
            // version 21 had the values in the "right" order already and
            // were causing decompression issues

            //if (Header.Version >= 21 && Header.Version != 0x00040014)
            //{
            //    for (int i = 0; i < Header.Lumps.Length; i++)
            //    {
            //        uint temp = Header.Lumps[i].Version;
            //        Header.Lumps[i].Version = Header.Lumps[i].Offset;
            //        Header.Lumps[i].Offset = Header.Lumps[i].Length;
            //        Header.Lumps[i].Length = temp;
            //    }
            //}

            if (ZIPEndOfCentralDirectoryRecord.ObjectSize <= Header.Lumps[HL_VBSP_LUMP_PAKFILE].Length)
            {
                long offset = Header.Lumps[HL_VBSP_LUMP_PAKFILE].Offset;
                while (offset < Header.Lumps[HL_VBSP_LUMP_PAKFILE].Offset + Header.Lumps[HL_VBSP_LUMP_PAKFILE].Length - 4)
                {
                    View testView = null;

                    if (!Mapping.Map(ref testView, offset, 4))
                        return false;

                    int test = BitConverter.ToInt32(testView.ViewData, 0);

                    Mapping.Unmap(ref testView);

                    switch (test)
                    {
                        case HL_VBSP_ZIP_END_OF_CENTRAL_DIRECTORY_RECORD_SIGNATURE:
                            if (!Mapping.Map(ref testView, offset, ZIPEndOfCentralDirectoryRecord.ObjectSize))
                                return false;

                            pointer = 0;
                            ZIPEndOfCentralDirectoryRecord endOfCentralDirectoryRecord = ZIPEndOfCentralDirectoryRecord.Create(testView.ViewData, ref pointer);

                            Mapping.Unmap(ref testView);

                            if (!Mapping.Map(ref EndOfCentralDirectoryRecordView, offset, ZIPEndOfCentralDirectoryRecord.ObjectSize + endOfCentralDirectoryRecord.CommentLength))
                                return false;

                            pointer = 0;
                            EndOfCentralDirectoryRecord = ZIPEndOfCentralDirectoryRecord.Create(EndOfCentralDirectoryRecordView.ViewData, ref pointer);

                            if (!Mapping.Map(ref FileHeaderView, Header.Lumps[HL_VBSP_LUMP_PAKFILE].Offset + EndOfCentralDirectoryRecord.StartOfCentralDirOffset, (int)EndOfCentralDirectoryRecord.CentralDirectorySize))
                                return false;

                            return true;
                        case HL_VBSP_ZIP_FILE_HEADER_SIGNATURE:
                            if (!Mapping.Map(ref testView, offset, ZIPFileHeader.ObjectSize))
                                return false;

                            pointer = 0;
                            ZIPFileHeader fileHeader = ZIPFileHeader.Create(testView.ViewData, ref pointer);

                            Mapping.Unmap(ref testView);

                            offset += ZIPFileHeader.ObjectSize + fileHeader.FileNameLength + fileHeader.ExtraFieldLength + fileHeader.FileCommentLength;
                            break;
                        case HL_VBSP_ZIP_LOCAL_FILE_HEADER_SIGNATURE:
                            if (!Mapping.Map(ref testView, offset, ZIPLocalFileHeader.ObjectSize))
                                return false;

                            pointer = 0;
                            ZIPLocalFileHeader localFileHeader = ZIPLocalFileHeader.Create(testView.ViewData, ref pointer);

                            Mapping.Unmap(ref testView);

                            offset += ZIPLocalFileHeader.ObjectSize + localFileHeader.FileNameLength + localFileHeader.ExtraFieldLength + localFileHeader.CompressedSize;
                            break;
                        default:
                            Console.WriteLine($"Invalid file: unknown ZIP section signature {test}.");
                            return false;
                    }
                }

                Console.WriteLine("Invalid file: unexpected end of file while scanning for end of ZIP central directory record.");
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        protected override void UnmapDataStructures()
        {
            Mapping.Unmap(ref FileHeaderView);

            EndOfCentralDirectoryRecord = null;
            Mapping.Unmap(ref EndOfCentralDirectoryRecordView);

            Header = null;
            Mapping.Unmap(ref HeaderView);
        }

        /// <summary>
        /// Get a filename based on the mapping
        /// </summary>
        /// <param name="bufferSize">Buffer size to limit data to</param>
        /// <returns>Required filename, null on error</returns>
        private string GetFileName(uint bufferSize)
        {
            if (bufferSize == 0)
                return null;

            string mappingName = System.IO.Path.GetFileName(Mapping.FileName);
            return mappingName.Substring(0, Math.Min((int)bufferSize, mappingName.Length));
        }

        #endregion

        #region Attributes

        /// <inheritdoc/>
        protected override bool GetAttributeInternal(PackageAttributeType packageAttribute, out PackageAttribute attribute)
        {
            attribute = new PackageAttribute();
            switch (packageAttribute)
            {
                case PackageAttributeType.HL_VBSP_PACKAGE_VERSION:
                    attribute.SetInteger(AttributeNames[(int)packageAttribute], Header.Version);
                    return true;
                case PackageAttributeType.HL_VBSP_PACKAGE_MAP_REVISION:
                    attribute.SetInteger(AttributeNames[(int)packageAttribute], Header.MapRevision);
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
                    if (file.Data != null)
                    {
                        int pointer = 0;
                        ZIPFileHeader directoryItem = ZIPFileHeader.Create(file.Data, ref pointer);
                        switch (packageAttribute)
                        {
                            case PackageAttributeType.HL_VBSP_ZIP_ITEM_CREATE_VERSION:
                                attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], directoryItem.VersionMadeBy, false);
                                return true;
                            case PackageAttributeType.HL_VBSP_ZIP_ITEM_EXTRACT_VERSION:
                                attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], directoryItem.VersionNeededToExtract, false);
                                return true;
                            case PackageAttributeType.HL_VBSP_ZIP_ITEM_FLAGS:
                                attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], directoryItem.Flags, true);
                                return true;
                            case PackageAttributeType.HL_VBSP_ZIP_ITEM_COMPRESSION_METHOD:
                                attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], directoryItem.CompressionMethod, true);
                                return true;
                            case PackageAttributeType.HL_VBSP_ZIP_ITEM_CRC:
                                attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], directoryItem.CRC32, true);
                                return true;
                            case PackageAttributeType.HL_VBSP_ZIP_ITEM_DISK:
                                attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], directoryItem.DiskNumberStart, false);
                                return true;
                            case PackageAttributeType.HL_VBSP_ZIP_ITEM_COMMENT:
                                attribute.SetString(ItemAttributeNames[(int)packageAttribute], directoryItem.FileComment);
                                return true;
                        }
                    }
                    else
                    {
                        uint uiID = file.ID;
                        if (uiID >= HL_VBSP_LUMP_COUNT)
                            uiID -= HL_VBSP_LUMP_COUNT;

                        switch (packageAttribute)
                        {
                            case PackageAttributeType.HL_VBSP_ITEM_VERSION:
                                attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], Header.Lumps[uiID].Version, false);
                                return true;
                            case PackageAttributeType.HL_VBSP_ITEM_FOUR_CC:
                                uint fourCC = BitConverter.ToUInt32(Header.Lumps[uiID].FourCC.Select(c => (byte)c).ToArray(), 0);
                                attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], fourCC, true);
                                return true;
                        }

                        if (item.ID == HL_VBSP_LUMP_PAKFILE)
                        {
                            switch (packageAttribute)
                            {
                                case PackageAttributeType.HL_VBSP_ZIP_PACKAGE_DISK:
                                    attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], EndOfCentralDirectoryRecord.NumberOfThisDisk, false);
                                    return true;
                                case PackageAttributeType.HL_VBSP_ZIP_PACKAGE_COMMENT:
                                    attribute.SetString(ItemAttributeNames[(int)packageAttribute], EndOfCentralDirectoryRecord.Comment);
                                    return true;
                            }
                        }
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
            if (file.Data != null)
            {
                int pointer = 0;
                ZIPFileHeader directoryItem = ZIPFileHeader.Create(file.Data, ref pointer);
                extractable = directoryItem.CompressionMethod == 0 && directoryItem.DiskNumberStart == EndOfCentralDirectoryRecord.NumberOfThisDisk;
            }
            else
            {
                extractable = true;
            }

            return true;
        }

        #endregion

        #region File Validation

        /// <inheritdoc/>
        protected override bool GetFileValidationInternal(DirectoryFile file, out Validation validation)
        {
            if (file.Data != null)
            {
                int pointer = 0;
                ZIPFileHeader directoryItem = ZIPFileHeader.Create(file.Data, ref pointer);

                if (directoryItem.CompressionMethod != 0 || directoryItem.DiskNumberStart != EndOfCentralDirectoryRecord.NumberOfThisDisk)
                {
                    validation = Validation.HL_VALIDATES_ASSUMED_OK;
                    return true;
                }

                uint checksum = 0;
                if (CreateStreamInternal(file, true, out Stream stream))
                {
                    if (stream.Open(FileModeFlags.HL_MODE_READ))
                    {
                        long totalBytes = 0;
                        int bufferSize;
                        byte[] buffer = new byte[HL_VBSP_ZIP_CHECKSUM_LENGTH];

                        while ((bufferSize = stream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            checksum = Checksum.CRC32(buffer, bufferSize, checksum);
                            totalBytes += bufferSize;
                        }

                        stream.Close();
                    }

                    ReleaseStream(stream);
                }

                validation = directoryItem.CRC32 == checksum ? Validation.HL_VALIDATES_OK : Validation.HL_VALIDATES_CORRUPT;
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
            if (file.Data != null)
            {
                int pointer = 0;
                ZIPFileHeader directoryItem = ZIPFileHeader.Create(file.Data, ref pointer);
                size = (int)directoryItem.UncompressedSize;
            }
            else if (file.ID < HL_VBSP_LUMP_COUNT)
            {
                size = (int)Header.Lumps[file.ID].Length;
            }
            else
            {
                size = (int)(LMPHeader.ObjectSize + Header.Lumps[file.ID - HL_VBSP_LUMP_COUNT].Length);
            }

            return true;
        }

        /// <inheritdoc/>
        protected override bool GetFileSizeOnDiskInternal(DirectoryFile file, out int size)
        {
            if (file.Data != null)
            {
                int pointer = 0;
                ZIPFileHeader directoryItem = ZIPFileHeader.Create(file.Data, ref pointer);
                size = (int)directoryItem.CompressedSize;
            }
            else if (file.ID < HL_VBSP_LUMP_COUNT)
            {
                size = (int)Header.Lumps[file.ID].Length;
            }
            else
            {
                size = (int)Header.Lumps[file.ID - HL_VBSP_LUMP_COUNT].Length;
            }

            return true;
        }

        #endregion

        #region Streams

        /// <inheritdoc/>
        protected override bool CreateStreamInternal(DirectoryFile file, bool readEncrypted, out Stream stream)
        {
            stream = null;
            if (file.Data != null)
            {
                int pointer = 0;
                ZIPFileHeader directoryItem = ZIPFileHeader.Create(file.Data, ref pointer);
                if (directoryItem.CompressionMethod != 0)
                {
                    Console.WriteLine($"Compression format {directoryItem.CompressionMethod} not supported.");
                    return false;
                }

                if (directoryItem.DiskNumberStart != EndOfCentralDirectoryRecord.NumberOfThisDisk)
                {
                    Console.WriteLine($"File resides on disk {directoryItem.DiskNumberStart}.");
                    return false;
                }

                View directoryEntryView = null;
                if (!Mapping.Map(ref directoryEntryView, Header.Lumps[HL_VBSP_LUMP_PAKFILE].Offset + directoryItem.RelativeOffsetOfLocalHeader, ZIPLocalFileHeader.ObjectSize))
                    return false;

                pointer = 0;
                ZIPLocalFileHeader directoryEntry = ZIPLocalFileHeader.Create(directoryEntryView.ViewData, ref pointer);

                Mapping.Unmap(ref directoryEntryView);

                if (directoryEntry.Signature != HL_VBSP_ZIP_LOCAL_FILE_HEADER_SIGNATURE)
                {
                    Console.WriteLine($"Invalid file data offset {directoryItem.DiskNumberStart}.");
                    return false;
                }

                stream = new MappingStream(Mapping, Header.Lumps[HL_VBSP_LUMP_PAKFILE].Offset + directoryItem.RelativeOffsetOfLocalHeader + ZIPLocalFileHeader.ObjectSize + directoryEntry.FileNameLength + directoryEntry.ExtraFieldLength, directoryEntry.UncompressedSize);
            }
            else if (file.ID < HL_VBSP_LUMP_COUNT)
            {
                stream = new MappingStream(Mapping, Header.Lumps[file.ID].Offset, Header.Lumps[file.ID].Length);
            }
            else
            {
                uint id = file.ID - HL_VBSP_LUMP_COUNT;

                View lumpView = null;
                if (!Mapping.Map(ref lumpView, Header.Lumps[id].Offset, (int)Header.Lumps[id].Length))
                    return false;

                int bufferSize = (int)(LMPHeader.ObjectSize + Header.Lumps[id].Length);
                byte[] buffer = new byte[bufferSize];

                LMPHeader lmpHeader = new LMPHeader()
                {
                    LumpOffset = LMPHeader.ObjectSize,
                    LumpID = (int)id,
                    LumpVersion = (int)Header.Lumps[id].Version,
                    LumpLength = (int)Header.Lumps[id].Length,
                    MapRevision = Header.MapRevision,
                };

                Array.Copy(lmpHeader.Serialize(), 0, buffer, 0, LMPHeader.ObjectSize);
                Array.Copy(lumpView.ViewData, 0, buffer, LMPHeader.ObjectSize, Header.Lumps[id].Length);
                stream = new MemoryStream(buffer, bufferSize);

                Mapping.Unmap(ref lumpView);
            }

            return true;
        }

        #endregion
    }
}
