/*
 * HLLib
 * Copyright (C) 2006-2012 Ryan Gregg

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

// TODO: Include zlib to sync with newest version
namespace HLLib.Packages.ZIP
{
    /// <summary>
    /// Zip File
    /// </summary>
    public sealed class ZIPFile : Package
    {
        #region Constants

        /// <summary>
        /// Zip local file header signature as an integer
        /// </summary>
        public const int HL_ZIP_LOCAL_FILE_HEADER_SIGNATURE = 0x04034b50;

        /// <summary>
        /// Zip file header signature as an integer
        /// </summary>
        public const int HL_ZIP_FILE_HEADER_SIGNATURE = 0x02014b50;

        /// <summary>
        /// Zip end of central directory record signature as an integer
        /// </summary>
        public const int HL_ZIP_END_OF_CENTRAL_DIRECTORY_RECORD_SIGNATURE = 0x06054b50;

        /// <summary>
        /// Length of a ZIP checksum in bytes
        /// </summary>
        public const int HL_ZIP_CHECKSUM_LENGTH = 0x00008000;

        /// <inheritdoc/>
        public override string[] AttributeNames => new string[] { "Disk", "Comment" };

        /// <inheritdoc/>
        public override string[] ItemAttributeNames => new string[] { "Create Version", "Extract Version", "Flags", "Compression Method", "CRC", "Disk", "Comment" };

        #endregion

        #region Views

        /// <summary>
        /// View representing file header data
        /// </summary>
        private View FileHeaderView;

        /// <summary>
        /// View representing the end of central directory record data
        /// </summary>
        private View EndOfCentralDirectoryRecordView;

        #endregion

        #region Fields

        /// <summary>
        /// Deserialized end of central directory record data
        /// </summary>
        public ZIPEndOfCentralDirectoryRecord EndOfCentralDirectoryRecord { get; private set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ZIPFile() : base()
        {
            FileHeaderView = null;
            EndOfCentralDirectoryRecordView = null;
            EndOfCentralDirectoryRecord = null;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~ZIPFile() => Close();

        #region Descriptors

        /// <inheritdoc/>
        public override PackageType PackageType => PackageType.HL_PACKAGE_ZIP;

        /// <inheritdoc/>
        public override string Extension => "zip";

        /// <inheritdoc/>
        public override string Description => "Zip File";

        #endregion

        #region Mappings

        /// <inheritdoc/>
        protected override DirectoryFolder CreateRoot()
        {
            DirectoryFolder root = new DirectoryFolder(this);

            uint test;
            int offset = 0, pointer = 0;
            while (offset < EndOfCentralDirectoryRecord.CentralDirectorySize - 4)
            {
                test = BitConverter.ToUInt32(FileHeaderView.ViewData, offset);
                switch (test)
                {
                    case HL_ZIP_FILE_HEADER_SIGNATURE:
                        pointer = offset;
                        ZIPFileHeader fileHeader = ZIPFileHeader.Create(FileHeaderView.ViewData, ref pointer);
                        string fileName = fileHeader.FileName;

                        // Check if we have just a file, or if the file has directories we need to create.
                        if (!fileName.Contains('/') && !fileName.Contains('\\'))
                        {
                            byte[] buffer = new byte[ZIPFileHeader.ObjectSize + fileHeader.FileNameLength + fileHeader.ExtraFieldLength + fileHeader.FileCommentLength];
                            Array.Copy(FileHeaderView.ViewData, offset, buffer, 0, buffer.Length);
                            root.AddFile(fileName, HL_ID_INVALID, buffer);
                        }
                        else
                        {
                            // Tokenize the file path and create the directories.
                            DirectoryFolder insertFolder = root;

                            string temp = string.Empty;
                            string[] token = fileName.Split('/', '\\');
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
                            byte[] buffer = new byte[ZIPFileHeader.ObjectSize + fileHeader.FileNameLength + fileHeader.ExtraFieldLength + fileHeader.FileCommentLength];
                            Array.Copy(FileHeaderView.ViewData, offset, buffer, 0, buffer.Length);
                            insertFolder.AddFile(temp, HL_ID_INVALID, buffer);
                        }

                        offset += ZIPFileHeader.ObjectSize + fileHeader.FileNameLength + fileHeader.ExtraFieldLength + fileHeader.FileCommentLength;
                        break;
                    default:
                        offset = (int)EndOfCentralDirectoryRecord.CentralDirectorySize;
                        break;
                }
            }

            return root;
        }

        /// <inheritdoc/>
        protected override bool MapDataStructures()
        {
            if (ZIPEndOfCentralDirectoryRecord.ObjectSize > Mapping.MappingSize)
            {
                Console.WriteLine("Invalid file: the file map is too small for it's header.");
                return false;
            }

            int pointer = 0;
            uint test = 0;
            long offset = 0, length = Mapping.MappingSize;
            while (offset < length - 4)
            {
                View testView = null;

                if (!Mapping.Map(ref testView, offset, 4))
                    return false;

                byte[] testViewData = testView.ViewData;
                test = BitConverter.ToUInt32(testViewData, 0);

                Mapping.Unmap(ref testView);

                switch (test)
                {
                    case HL_ZIP_END_OF_CENTRAL_DIRECTORY_RECORD_SIGNATURE:
                        {
                            if (!Mapping.Map(ref testView, offset, ZIPEndOfCentralDirectoryRecord.ObjectSize))
                                return false;

                            pointer = 0;
                            ZIPEndOfCentralDirectoryRecord endOfCentralDirectoryRecord = ZIPEndOfCentralDirectoryRecord.Create(testView.ViewData, ref pointer);

                            Mapping.Unmap(ref testView);

                            if (!Mapping.Map(ref EndOfCentralDirectoryRecordView, offset, ZIPEndOfCentralDirectoryRecord.ObjectSize + endOfCentralDirectoryRecord.CommentLength))
                                return false;

                            pointer = 0;
                            EndOfCentralDirectoryRecord = ZIPEndOfCentralDirectoryRecord.Create(EndOfCentralDirectoryRecordView.ViewData, ref pointer);

                            if (!Mapping.Map(ref FileHeaderView, EndOfCentralDirectoryRecord.StartOfCentralDirOffset, (int)EndOfCentralDirectoryRecord.CentralDirectorySize))
                                return false;

                            return true;
                        }
                    case HL_ZIP_FILE_HEADER_SIGNATURE:
                        {
                            if (!Mapping.Map(ref testView, offset, ZIPFileHeader.ObjectSize))
                                return false;

                            pointer = 0;
                            ZIPFileHeader fileHeader = ZIPFileHeader.Create(testView.ViewData, ref pointer);

                            Mapping.Unmap(ref testView);

                            offset += ZIPFileHeader.ObjectSize + fileHeader.FileNameLength + fileHeader.ExtraFieldLength + fileHeader.FileCommentLength;
                            break;
                        }
                    case HL_ZIP_LOCAL_FILE_HEADER_SIGNATURE:
                        {
                            if (!Mapping.Map(ref testView, offset, ZIPLocalFileHeader.ObjectSize))
                                return false;

                            pointer = 0;
                            ZIPLocalFileHeader localFileHeader = ZIPLocalFileHeader.Create(testView.ViewData, ref pointer);

                            Mapping.Unmap(ref testView);

                            offset += ZIPLocalFileHeader.ObjectSize + localFileHeader.FileNameLength + localFileHeader.ExtraFieldLength + localFileHeader.CompressedSize;
                            break;
                        }
                    default:
                        {
                            Console.WriteLine($"Invalid file: unknown section signature {test}.");
                            return false;
                        }
                }
            }

            Console.WriteLine("Invalid file: unexpected end of file while scanning for end of central directory record.");
            return false;
        }

        /// <inheritdoc/>
        protected override void UnmapDataStructures()
        {
            Mapping.Unmap(ref FileHeaderView);
            EndOfCentralDirectoryRecord = null;
            Mapping.Unmap(ref EndOfCentralDirectoryRecordView);
        }

        #endregion

        #region Attributes

        /// <inheritdoc/>
        protected override bool GetAttributeInternal(PackageAttributeType packageAttribute, out PackageAttribute attribute)
        {
            attribute = new PackageAttribute();
            switch (packageAttribute)
            {
                case PackageAttributeType.HL_ZIP_PACKAGE_DISK:
                    attribute.SetUnsignedInteger(AttributeNames[(int)packageAttribute], EndOfCentralDirectoryRecord.NumberOfThisDisk, false);
                    return true;
                case PackageAttributeType.HL_ZIP_PACKAGE_COMMENT:
                    attribute.SetString(AttributeNames[(int)packageAttribute], EndOfCentralDirectoryRecord.Comment);
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
                    int pointer = 0;
                    ZIPFileHeader directoryItem = ZIPFileHeader.Create(file.Data, ref pointer);
                    switch (packageAttribute)
                    {
                        case PackageAttributeType.HL_ZIP_ITEM_CREATE_VERSION:
                            attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], directoryItem.VersionMadeBy, false);
                            return true;
                        case PackageAttributeType.HL_ZIP_ITEM_EXTRACT_VERSION:
                            attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], directoryItem.VersionNeededToExtract, false);
                            return true;
                        case PackageAttributeType.HL_ZIP_ITEM_FLAGS:
                            attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], directoryItem.Flags, false);
                            return true;
                        case PackageAttributeType.HL_ZIP_ITEM_COMPRESSION_METHOD:
                            attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], directoryItem.CompressionMethod, false);
                            return true;
                        case PackageAttributeType.HL_ZIP_ITEM_CRC:
                            attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], directoryItem.CRC32, false);
                            return true;
                        case PackageAttributeType.HL_ZIP_ITEM_DISK:
                            attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], directoryItem.DiskNumberStart, false);
                            return true;
                        case PackageAttributeType.HL_ZIP_ITEM_COMMENT:
                            attribute.SetString(ItemAttributeNames[(int)packageAttribute], directoryItem.FileComment);
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
            int pointer = 0;
            ZIPFileHeader directoryItem = ZIPFileHeader.Create(file.Data, ref pointer);
            extractable = directoryItem.CompressionMethod == 0 && directoryItem.DiskNumberStart == EndOfCentralDirectoryRecord.NumberOfThisDisk;
            return true;
        }

        #endregion

        #region File Validation

        /// <inheritdoc/>
        protected override bool GetFileValidationInternal(DirectoryFile file, out Validation validation)
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
                    byte[] buffer = new byte[HL_ZIP_CHECKSUM_LENGTH];

                    while ((bufferSize = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        checksum = Checksum.CRC32(buffer, bufferSize, checksum);
                        totalBytes += bufferSize;
                    }

                    stream.Close();
                }

                ReleaseStream(stream);
                stream.Close();
            }

            validation = (directoryItem.CRC32 == checksum ? Validation.HL_VALIDATES_OK : Validation.HL_VALIDATES_CORRUPT);

            return true;
        }

        #endregion

        #region File Size

        /// <inheritdoc/>
        protected override bool GetFileSizeInternal(DirectoryFile file, out int uiSize)
        {
            int pointer = 0;
            ZIPFileHeader directoryItem = ZIPFileHeader.Create(file.Data, ref pointer);
            uiSize = (int)directoryItem.UncompressedSize;
            return true;
        }

        /// <inheritdoc/>
        protected override bool GetFileSizeOnDiskInternal(DirectoryFile file, out int uiSize)
        {
            int pointer = 0;
            ZIPFileHeader directoryItem = ZIPFileHeader.Create(file.Data, ref pointer);
            uiSize = (int)directoryItem.CompressedSize;
            return true;
        }

        #endregion

        #region Streams

        /// <inheritdoc/>
        protected override bool CreateStreamInternal(DirectoryFile file, bool readEncrypted, out Stream stream)
        {
            int pointer = 0;
            ZIPFileHeader directoryItem = ZIPFileHeader.Create(file.Data, ref pointer);

            if (directoryItem.CompressionMethod != 0)
            {
                stream = null;
                Console.WriteLine($"Compression format {directoryItem.CompressionMethod} not supported.");
                return false;
            }

            if (directoryItem.DiskNumberStart != EndOfCentralDirectoryRecord.NumberOfThisDisk)
            {
                stream = null;
                Console.WriteLine($"File resides on disk {directoryItem.DiskNumberStart}.");
                return false;
            }

            View directoryEntryView = null;

            if (!Mapping.Map(ref directoryEntryView, directoryItem.RelativeOffsetOfLocalHeader, ZIPLocalFileHeader.ObjectSize))
            {
                stream = null;
                return false;
            }

            pointer = 0;
            ZIPLocalFileHeader DirectoryEntry = ZIPLocalFileHeader.Create(directoryEntryView.ViewData, ref pointer);

            Mapping.Unmap(ref directoryEntryView);

            if (DirectoryEntry.Signature != HL_ZIP_LOCAL_FILE_HEADER_SIGNATURE)
            {
                stream = null;
                Console.WriteLine($"Invalid file data offset. {directoryItem.DiskNumberStart}");
                return false;
            }

            stream = new MappingStream(Mapping, directoryItem.RelativeOffsetOfLocalHeader + ZIPLocalFileHeader.ObjectSize + DirectoryEntry.FileNameLength + DirectoryEntry.ExtraFieldLength, DirectoryEntry.UncompressedSize);

            return true;
        }

        #endregion
    }
}
