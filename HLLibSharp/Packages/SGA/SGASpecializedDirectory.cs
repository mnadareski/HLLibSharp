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
using System.Text;
using HLLib.Checksums;
using HLLib.Directory;
using HLLib.Mappings;
using HLLib.Streams;

// TODO: Include zlib to sync with newest version
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
        /// Source SGA file
        /// </summary>
        public SGAFile File { get; protected set; }

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
        public string StringTable { get; protected set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public SGASpecializedDirectory(SGAFile file)
        {
            File = file;

            HeaderDirectoryView = null;

            DirectoryHeader = null;
            Sections = null;
            Folders = null;
            Files = null;
            StringTable = null;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SGASpecializedDirectory() => UnmapDataStructures();

        #region Mappings

        /// <inheritdoc/>
        public override DirectoryFolder CreateRoot()
        {
            uint sectionCount = GetSectionCount();

            DirectoryFolder root = new DirectoryFolder(File);
            for (uint i = 0; i < sectionCount; i++)
            {
                DirectoryFolder section;

                // Check if folder exists.
                DirectoryItem item = root.GetItem(Sections[i].Alias);
                if (item == null || item.ItemType == DirectoryItemType.HL_ITEM_FILE)
                {
                    // It doesn't, create it.
                    section = root.AddFolder(Sections[i].Alias);
                }
                else
                {
                    // It does, use it.
                    section = (DirectoryFolder)item;
                }

                uint folderRootIndex = GetFolderRootIndex(i);
                CreateFolder(section, folderRootIndex);
            }

            return root;
        }

        /// <inheritdoc/>
        public override bool MapDataStructures()
        {
            int objectSize = GetHeaderSize();
            int headerLength = (int)GetHeaderLength();

            if (!File.Mapping.Map(ref HeaderDirectoryView, objectSize, headerLength))
                return false;

            int pointer = CreateDirectoryHeader();

            uint sectionCount = GetSectionCount();
            int sectionSize = GetSectionSize();
            if (sectionCount > 0 && DirectoryHeader.SectionOffset + sectionSize * sectionCount > headerLength)
            {
                Console.WriteLine("Invalid file: the file map is too small for section data.");
                return false;
            }

            uint folderCount = GetFolderCount();
            int folderSize = GetFolderSize();
            if (folderCount > 0 && DirectoryHeader.FolderOffset + folderSize * folderCount > headerLength)
            {
                Console.WriteLine("Invalid file: the file map is too small for folder data.");
                return false;
            }

            uint fileCount = GetFolderCount();
            int fileSize = GetFolderSize();
            if (fileCount > 0 && DirectoryHeader.FileOffset + fileSize * fileCount > headerLength)
            {
                Console.WriteLine("Invalid file: the file map is too small for file data.");
                return false;
            }

            if (DirectoryHeader.StringTableOffset > headerLength)
            {
                Console.WriteLine("Invalid file: the file map is too small for string table data.");
                return false;
            }

            int offsetPointer = pointer + (int)DirectoryHeader.SectionOffset;
            Sections = new TSGASection[sectionCount];
            for (int i = 0; i < sectionCount; i++)
            {
                Sections[i] = CreateSection(ref offsetPointer);
            }

            offsetPointer = pointer + (int)DirectoryHeader.FolderOffset;
            Folders = new TSGAFolder[folderCount];
            for (int i = 0; i < folderCount; i++)
            {
                Folders[i] = CreateFolder(ref offsetPointer);
            }

            offsetPointer = pointer + (int)DirectoryHeader.FileOffset;
            Files = new TSGAFile[fileCount];
            for (int i = 0; i < fileCount; i++)
            {
                Files[i] = CreateFile(ref offsetPointer);
            }

            offsetPointer = pointer + (int)DirectoryHeader.StringTableOffset;
            StringTable = Encoding.ASCII.GetString(HeaderDirectoryView.ViewData, offsetPointer, HeaderDirectoryView.ViewData.Length - offsetPointer);

            return true;
        }

        /// <inheritdoc/>
        public override void UnmapDataStructures()
        {
            DirectoryHeader = null;
            Sections = null;
            Folders = null;
            Files = null;
            StringTable = null;

            File.Mapping.Unmap(ref HeaderDirectoryView);
        }

        /// <summary>
        /// Create an individual internal folder
        /// </summary>
        /// <param name="parent">Parent directory folder to fill</param>
        /// <param name="folderIndex">Index of the current folder</param>
        protected void CreateFolder(DirectoryFolder parent, uint folderIndex)
        {
            uint namePointer = Folders[folderIndex].NameOffset;
            if (StringTable[(int)namePointer] != '\0')
            {
                string name = new string(StringTable.Substring((int)namePointer).TakeWhile(c => c != '\0').ToArray());

                // Strip parent folder names.
                if (!string.IsNullOrEmpty(name))
                    name = System.IO.Path.GetFileName(name);

                // Check if folder exists.
                DirectoryItem item = parent.GetItem(name);
                if (item == null || item.ItemType == DirectoryItemType.HL_ITEM_FILE)
                {
                    // It doesn't, create it.
                    parent = parent.AddFolder(name);
                }
                else
                {
                    // It does, use it.
                    parent = (DirectoryFolder)item;
                }
            }

            uint folderStartIndex = GetFolderStartIndex(folderIndex);
            uint folderEndIndex = GetFolderEndIndex(folderIndex);

            for (uint i = folderStartIndex; i < folderEndIndex; i++)
            {
                CreateFolder(parent, i);
            }

            uint fileStartIndex = GetFileStartIndex(folderIndex);
            uint fileEndIndex = GetFileEndIndex(folderIndex);

            for (uint i = fileStartIndex; i < fileEndIndex; i++)
            {
                string name = new string(StringTable.Substring((int)Files[i].NameOffset).TakeWhile(c => c != '\0').ToArray());
                parent.AddFile(name, i);
            }
        }

        #endregion

        #region Attributes

        /// <inheritdoc/>
        public override bool GetItemAttributeInternal(DirectoryItem item, PackageAttributeType packageAttribute, out PackageAttribute attribute)
        {
            attribute = new PackageAttribute();
            if (item.ID == Package.HL_ID_INVALID)
                return false;

            uint sectionCount = GetSectionCount();
            switch (item.ItemType)
            {
                case DirectoryItemType.HL_ITEM_FOLDER:
                    DirectoryFolder folder = (DirectoryFolder)item;
                    switch (packageAttribute)
                    {
                        case PackageAttributeType.HL_SGA_ITEM_SECTION_ALIAS:
                            for (uint i = 0; i < sectionCount; i++)
                            {
                                uint folderStartIndex = GetFolderStartIndex(i);
                                uint folderEndIndex = GetFolderEndIndex(i);

                                if (folder.ID >= folderStartIndex && folder.ID < folderEndIndex)
                                {
                                    attribute.SetString(File.ItemAttributeNames[(int)packageAttribute], Sections[i].Alias);
                                    return true;
                                }
                            }
                            return false;
                        case PackageAttributeType.HL_SGA_ITEM_SECTION_NAME:
                            for (uint i = 0; i < sectionCount; i++)
                            {
                                uint folderStartIndex = GetFolderStartIndex(i);
                                uint folderEndIndex = GetFolderEndIndex(i);

                                if (folder.ID >= folderStartIndex && folder.ID < folderEndIndex)
                                {
                                    attribute.SetString(File.ItemAttributeNames[(int)packageAttribute], Sections[i].Name);
                                    return true;
                                }
                            }
                            return false;
                    }
                    break;

                case DirectoryItemType.HL_ITEM_FILE:
                    DirectoryFile file = (DirectoryFile)item;
                    TSGAFile sgaFile = Files[file.ID];
                    switch (packageAttribute)
                    {
                        case PackageAttributeType.HL_SGA_ITEM_SECTION_ALIAS:
                            for (uint i = 0; i < sectionCount; i++)
                            {
                                uint fileStartIndex = GetFileStartIndex(i);
                                uint fileEndIndex = GetFileEndIndex(i);

                                if (file.ID >= fileStartIndex && file.ID < fileEndIndex)
                                {
                                    attribute.SetString(File.ItemAttributeNames[(int)packageAttribute], Sections[i].Alias);
                                    return true;
                                }
                            }
                            return false;
                        case PackageAttributeType.HL_SGA_ITEM_SECTION_NAME:
                            for (uint i = 0; i < sectionCount; i++)
                            {
                                uint fileStartIndex = GetFileStartIndex(i);
                                uint fileEndIndex = GetFileEndIndex(i);

                                if (file.ID >= fileStartIndex && file.ID < fileEndIndex)
                                {
                                    attribute.SetString(File.ItemAttributeNames[(int)packageAttribute], Sections[i].Name);
                                    return true;
                                }
                            }
                            return false;
                        case PackageAttributeType.HL_SGA_ITEM_MODIFIED:
                            attribute.SetUnsignedInteger(File.ItemAttributeNames[(int)packageAttribute], sgaFile.TimeModified, false);
                            return true;
                        case PackageAttributeType.HL_SGA_ITEM_TYPE:
                            attribute.SetUnsignedInteger(File.ItemAttributeNames[(int)packageAttribute], sgaFile.Type, false);
                            return true;
                    }
                    break;
            }

            // SGAFile4
            if (File.Header.MajorVersion >= 4 && File.Header.MajorVersion <= 5)
            {
                switch (item.ItemType)
                {
                    case DirectoryItemType.HL_ITEM_FILE:
                        DirectoryFile file = (DirectoryFile)item;
                        TSGAFile sgaFile = Files[file.ID];
                        switch (packageAttribute)
                        {
                            case PackageAttributeType.HL_SGA_ITEM_CRC:
                                View fileHeaderView = null;
                                if (File.Mapping.Map(ref fileHeaderView, ((SGAHeader4)File.Header).FileDataOffset + sgaFile.Offset - SGAFileHeader.ObjectSize, SGAFileHeader.ObjectSize))
                                {
                                    int pointer = 0;
                                    SGAFileHeader fileHeader = SGAFileHeader.Create(fileHeaderView.ViewData, ref pointer);
                                    attribute.SetUnsignedInteger(File.ItemAttributeNames[(int)packageAttribute], fileHeader.CRC32, true);
                                    File.Mapping.Unmap(ref fileHeaderView);
                                    return true;
                                }

                                return false;
                            case PackageAttributeType.HL_SGA_ITEM_VERIFICATION:
                                attribute.SetString(File.ItemAttributeNames[(int)packageAttribute], SGAFile.VerificationNames[(int)SGAFileVerification.VERIFICATION_CRC]);
                                return true;
                        }
                        break;
                }
            }

            // SGAFile6
            if (File.Header.MajorVersion == 6)
            {
                switch (item.ItemType)
                {
                    case DirectoryItemType.HL_ITEM_FILE:
                        {
                            DirectoryFile file = (DirectoryFile)item;
                            TSGAFile sgaFile = Files[file.ID];
                            switch (packageAttribute)
                            {
                                case PackageAttributeType.HL_SGA_ITEM_CRC:
                                    attribute.SetUnsignedInteger(File.ItemAttributeNames[(int)packageAttribute], (sgaFile as SGAFile6).CRC32, true);
                                    return true;
                                case PackageAttributeType.HL_SGA_ITEM_VERIFICATION:
                                    attribute.SetString(File.ItemAttributeNames[(int)packageAttribute], SGAFile.VerificationNames[(int)SGAFileVerification.VERIFICATION_CRC]);
                                    return true;
                            }
                            break;
                        }
                }
            }

            // SGAFile7
            if (File.Header.MajorVersion == 7)
            {
                switch (item.ItemType)
                {
                    case DirectoryItemType.HL_ITEM_FILE:
                        {
                            DirectoryFile file = (DirectoryFile)item;
                            TSGAFile sgaFile = Files[file.ID];
                            switch (packageAttribute)
                            {
                                case PackageAttributeType.HL_SGA_ITEM_CRC:
                                    attribute.SetUnsignedInteger(File.ItemAttributeNames[(int)packageAttribute], (sgaFile as SGAFile7).CRC32, true);
                                    return true;
                                case PackageAttributeType.HL_SGA_ITEM_VERIFICATION:
                                    attribute.SetString(File.ItemAttributeNames[(int)packageAttribute], SGAFile.VerificationNames[sgaFile.Dummy0 < SGAFile.VerificationNames.Length ? sgaFile.Dummy0 : (int)SGAFileVerification.VERIFICATION_NONE]);
                                    return true;
                            }
                            break;
                        }
                }
            }

            return false;
        }

        #endregion

        #region File Extraction Check

        /// <inheritdoc/>
        public override bool GetFileExtractableInternal(DirectoryFile file, out bool extractable)
        {
            extractable = true;
            return true;
        }

        #endregion

        #region File Validation

        /// <inheritdoc/>
        public override bool GetFileValidationInternal(DirectoryFile file, out Validation validation)
        {
            validation = Validation.HL_VALIDATES_ERROR;

            // SGAFile4
            if (typeof(TSGAFile) == typeof(SGAFile4))
            {
                SGAFile4 sgaFile = Files[file.ID];
                if (sgaFile.Type != 0)
                {
                    validation = Validation.HL_VALIDATES_ASSUMED_OK;
                    return true;
                }

                View fileHeaderDataView = null;
                if (File.Mapping.Map(ref fileHeaderDataView, ((SGAHeader4)File.Header).FileDataOffset + sgaFile.Offset - SGAFileHeader.ObjectSize, (int)(sgaFile.SizeOnDisk + SGAFileHeader.ObjectSize)))
                {
                    uint checksum = 0;
                    int pointer = 0;
                    SGAFileHeader fileHeader = SGAFileHeader.Create(fileHeaderDataView.ViewData, ref pointer);

                    long totalBytes = 0, fileBytes = sgaFile.Size;
                    while (totalBytes < fileBytes)
                    {
                        int bufferSize = (int)(totalBytes + SGAFile.HL_SGA_CHECKSUM_LENGTH <= fileBytes ? SGAFile.HL_SGA_CHECKSUM_LENGTH : fileBytes - totalBytes);
                        byte[] buffer = new byte[bufferSize];
                        Array.Copy(fileHeaderDataView.ViewData, pointer, buffer, 0, bufferSize);

                        checksum = Checksum.CRC32(buffer, bufferSize, checksum);

                        pointer += bufferSize;
                        totalBytes += bufferSize;
                    }

                    if (validation == Validation.HL_VALIDATES_ASSUMED_OK)
                        validation = fileHeader.CRC32 == checksum ? Validation.HL_VALIDATES_OK : Validation.HL_VALIDATES_CORRUPT;

                    File.Mapping.Unmap(ref fileHeaderDataView);
                }
                else
                {
                    validation = Validation.HL_VALIDATES_ERROR;
                }

                return true;
            }

            // SGAFile6
            else if (typeof(TSGAFile) == typeof(SGAFile6))
            {
                SGAFile6 sgaFile = Files[file.ID] as SGAFile6;

                View fileHeaderDataView = null;
                if (File.Mapping.Map(ref fileHeaderDataView, ((SGAHeader6)File.Header).FileDataOffset + sgaFile.Offset, (int)sgaFile.SizeOnDisk))
                {
                    uint checksum = 0;
                    int pointer = 0;
                    long totalBytes = 0, fileBytes = sgaFile.SizeOnDisk;

                    while (totalBytes < fileBytes)
                    {
                        int bufferSize = (int)(totalBytes + SGAFile.HL_SGA_CHECKSUM_LENGTH <= fileBytes ? SGAFile.HL_SGA_CHECKSUM_LENGTH : fileBytes - totalBytes);
                        byte[] buffer = new byte[bufferSize];
                        Array.Copy(fileHeaderDataView.ViewData, pointer, buffer, 0, bufferSize);
                        checksum = Checksum.CRC32(buffer, bufferSize, checksum);

                        pointer += bufferSize;
                        totalBytes += bufferSize;
                    }

                    if (validation == Validation.HL_VALIDATES_ASSUMED_OK)
                        validation = sgaFile.CRC32 == checksum ? Validation.HL_VALIDATES_OK : Validation.HL_VALIDATES_CORRUPT;

                    File.Mapping.Unmap(ref fileHeaderDataView);
                }
                else
                {
                    validation = Validation.HL_VALIDATES_ERROR;
                }

                return true;
            }

            // SGAFile7
            else if (typeof(TSGAFile) == typeof(SGAFile7))
            {
                SGAFile7 sgaFile = Files[file.ID] as SGAFile7;

                View fileHeaderDataView = null;
                if (File.Mapping.Map(ref fileHeaderDataView, ((SGAHeader6)File.Header).FileDataOffset + sgaFile.Offset, (int)sgaFile.SizeOnDisk))
                {
                    uint checksumValue = 0;
                    int pointer = 0;
                    long totalBytes = 0, fileBytes = sgaFile.SizeOnDisk;
                    uint blockSize = (DirectoryHeader as SGADirectoryHeader7).BlockSize;
                    if (blockSize == 0)
                        blockSize = SGAFile.HL_SGA_CHECKSUM_LENGTH;

                    Checksum checksum = null;
                    switch ((SGAFileVerification)sgaFile.Dummy0)
                    {
                        case SGAFileVerification.VERIFICATION_CRC_BLOCKS:
                            checksum = new CRC32Checksum();
                            break;
                        case SGAFileVerification.VERIFICATION_MD5_BLOCKS:
                            checksum = new MD5Checksum();
                            break;
                        case SGAFileVerification.VERIFICATION_SHA1_BLOCKS:
                            checksum = new SHA1Checksum();
                            break;
                    }

                    // TODO: Figure out where the DirectoryHeader start index is in the view
                    uint hashTableOffset = (DirectoryHeader as SGADirectoryHeader7).HashTableOffset + sgaFile.HashOffset;
                    while (totalBytes < fileBytes)
                    {
                        int bufferSize = (int)(totalBytes + blockSize <= fileBytes ? blockSize : fileBytes - totalBytes);
                        byte[] buffer = new byte[bufferSize];
                        Array.Copy(fileHeaderDataView.ViewData, pointer, buffer, 0, bufferSize);
                        checksumValue = Checksum.CRC32(buffer, bufferSize, checksumValue);
                        if (checksum != null)
                        {
                            checksum.Initialize();
                            checksum.Update(buffer, bufferSize);
                            if (!checksum.Finalize(out byte[] lpHashTable))
                            {
                                validation = Validation.HL_VALIDATES_CORRUPT;
                                break;
                            }

                            hashTableOffset += checksum.DigestSize;
                        }

                        pointer += bufferSize;
                        totalBytes += bufferSize;
                    }

                    if (validation == Validation.HL_VALIDATES_ASSUMED_OK)
                        validation = sgaFile.CRC32 == checksumValue ? Validation.HL_VALIDATES_OK : Validation.HL_VALIDATES_CORRUPT;

                    File.Mapping.Unmap(ref fileHeaderDataView);
                }
                else
                {
                    validation = Validation.HL_VALIDATES_ERROR;
                }

                return true;
            }

            return false;
        }

        #endregion

        #region File Size

        /// <inheritdoc/>
        public override bool GetFileSizeInternal(DirectoryFile file, out int size)
        {
            TSGAFile sgaFile = Files[file.ID];
            size = (int)sgaFile.Size;
            return true;
        }

        /// <inheritdoc/>
        public override bool GetFileSizeOnDiskInternal(DirectoryFile file, out int size)
        {
            TSGAFile sgaFile = Files[file.ID];
            size = (int)sgaFile.SizeOnDisk;
            return true;
        }

        #endregion

        #region Streams

        /// <inheritdoc/>
        public override bool CreateStreamInternal(DirectoryFile file, bool readEncrypted, out Stream stream)
        {
            TSGAFile sgaFile = Files[file.ID];
            if (sgaFile.Type == 0)
            {
                if (File.Header.MajorVersion >= 4 && File.Header.MajorVersion <= 5)
                {
                    stream = new MappingStream(File.Mapping, ((SGAHeader4)File.Header).FileDataOffset + sgaFile.Offset, sgaFile.SizeOnDisk);
                    return true;
                }
                else if (File.Header.MajorVersion >= 6 && File.Header.MajorVersion <= 7)
                {
                    stream = new MappingStream(File.Mapping, ((SGAHeader6)File.Header).FileDataOffset + sgaFile.Offset, sgaFile.SizeOnDisk);
                    return true;
                }
                else
                {
                    stream = null;
                    return false;
                }
            }
            else
            {
                stream = null;
                return false;
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Populate the directory header for the current file verison
        /// </summary>
        private int CreateDirectoryHeader()
        {
            int pointer = 0;
            switch (File.Header.MajorVersion)
            {
                case 4:
                    DirectoryHeader = SGADirectoryHeader4.Create(HeaderDirectoryView.ViewData, ref pointer) as TSGADirectoryHeader;
                    break;

                case 5:
                case 6:
                    DirectoryHeader = SGADirectoryHeader5.Create(HeaderDirectoryView.ViewData, ref pointer) as TSGADirectoryHeader;
                    break;

                case 7:
                    DirectoryHeader = SGADirectoryHeader7.Create(HeaderDirectoryView.ViewData, ref pointer) as TSGADirectoryHeader;
                    break;

                default:
                    return default;
            }

            return pointer;
        }

        /// <summary>
        /// Create a single file at a particular offset
        /// </summary>
        /// <param name="pointer">Offset pointer in the header directory view</param>
        private TSGAFile CreateFile(ref int pointer)
        {
            switch (File.Header.MajorVersion)
            {
                case 4:
                case 5:
                    return SGAFile4.Create(HeaderDirectoryView.ViewData, ref pointer) as TSGAFile;

                case 6:
                    return SGAFile6.Create(HeaderDirectoryView.ViewData, ref pointer) as TSGAFile;

                case 7:
                    return SGAFile7.Create(HeaderDirectoryView.ViewData, ref pointer) as TSGAFile;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Create a single folder at a particular offset
        /// </summary>
        /// <param name="pointer">Offset pointer in the header directory view</param>
        private TSGAFolder CreateFolder(ref int pointer)
        {
            switch (File.Header.MajorVersion)
            {
                case 4:
                    return SGAFolder4.Create(HeaderDirectoryView.ViewData, ref pointer) as TSGAFolder;

                case 5:
                case 6:
                case 7:
                    return SGAFolder5.Create(HeaderDirectoryView.ViewData, ref pointer) as TSGAFolder;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Create a single section at a particular offset
        /// </summary>
        /// <param name="pointer">Offset pointer in the header directory view</param>
        private TSGASection CreateSection(ref int pointer)
        {
            switch (File.Header.MajorVersion)
            {
                case 4:
                    return SGASection4.Create(HeaderDirectoryView.ViewData, ref pointer) as TSGASection;

                case 5:
                case 6:
                case 7:
                    return SGASection5.Create(HeaderDirectoryView.ViewData, ref pointer) as TSGASection;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Get the file count for the current file version
        /// </summary>
        private uint GetFileCount()
        {
            switch (File.Header.MajorVersion)
            {
                case 4:
                    return (DirectoryHeader as SGADirectoryHeader4).FileCount;

                case 5:
                case 6:
                    return (DirectoryHeader as SGADirectoryHeader5).FileCount;

                case 7:
                    return (DirectoryHeader as SGADirectoryHeader7).FileCount;

                default:
                    return default;
            }
        }

        /// <summary>
        /// Get the file end index for the current file version
        /// </summary>
        private uint GetFileEndIndex(uint index)
        {
            switch (File.Header.MajorVersion)
            {
                case 4:
                    return (Sections[index] as SGASection4).FileEndIndex;

                case 5:
                case 6:
                case 7:
                    return (Sections[index] as SGASection5).FileEndIndex;

                default:
                    return default;
            }
        }

        /// <summary>
        /// Get the file size for the current file version
        /// </summary>
        private int GetFileSize()
        {
            switch (File.Header.MajorVersion)
            {
                case 4:
                case 5:
                    return SGAFile4.ObjectSize;

                case 6:
                    return SGAFile6.ObjectSize;

                case 7:
                    return SGAFile7.ObjectSize;

                default:
                    return default;
            }
        }

        /// <summary>
        /// Get the file start index for the current file version
        /// </summary>
        private uint GetFileStartIndex(uint index)
        {
            switch (File.Header.MajorVersion)
            {
                case 4:
                    return (Sections[index] as SGASection4).FileStartIndex;

                case 5:
                case 6:
                case 7:
                    return (Sections[index] as SGASection5).FileStartIndex;

                default:
                    return default;
            }
        }

        /// <summary>
        /// Get the folder count for the current file version
        /// </summary>
        private uint GetFolderCount()
        {
            switch (File.Header.MajorVersion)
            {
                case 4:
                    return (DirectoryHeader as SGADirectoryHeader4).FolderCount;

                case 5:
                case 6:
                    return (DirectoryHeader as SGADirectoryHeader5).FolderCount;

                case 7:
                    return (DirectoryHeader as SGADirectoryHeader7).FolderCount;

                default:
                    return default;
            }
        }

        /// <summary>
        /// Get the folder end index for the current file version
        /// </summary>
        private uint GetFolderEndIndex(uint index)
        {
            switch (File.Header.MajorVersion)
            {
                case 4:
                    return (Sections[index] as SGASection4).FolderEndIndex;

                case 5:
                case 6:
                case 7:
                    return (Sections[index] as SGASection5).FolderEndIndex;

                default:
                    return default;
            }
        }

        /// <summary>
        /// Get the folder root index for the current file version
        /// </summary>
        private uint GetFolderRootIndex(uint index)
        {
            switch (File.Header.MajorVersion)
            {
                case 4:
                    return (Sections[index] as SGASection4).FolderRootIndex;

                case 5:
                case 6:
                case 7:
                    return (Sections[index] as SGASection5).FolderRootIndex;

                default:
                    return default;
            }
        }

        /// <summary>
        /// Get the folder size for the current file version
        /// </summary>
        private int GetFolderSize()
        {
            switch (File.Header.MajorVersion)
            {
                case 4:
                    return SGAFolder4.ObjectSize;

                case 5:
                case 6:
                case 7:
                    return SGAFolder5.ObjectSize;

                default:
                    return default;
            }
        }

        /// <summary>
        /// Get the folder start index for the current file version
        /// </summary>
        private uint GetFolderStartIndex(uint index)
        {
            switch (File.Header.MajorVersion)
            {
                case 4:
                    return (Sections[index] as SGASection4).FolderStartIndex;

                case 5:
                case 6:
                case 7:
                    return (Sections[index] as SGASection5).FolderStartIndex;

                default:
                    return default;
            }
        }

        /// <summary>
        /// Get the header length for the current file version
        /// </summary>
        private uint GetHeaderLength()
        {
            switch (File.Header.MajorVersion)
            {
                case 4:
                case 5:
                    return ((SGAHeader4)File.Header).HeaderLength;

                case 6:
                case 7:
                    return ((SGAHeader6)File.Header).HeaderLength;

                default:
                    return default;
            }
        }

        /// <summary>
        /// Get the header size for the current file version
        /// </summary>
        private int GetHeaderSize()
        {
            switch (File.Header.MajorVersion)
            {
                case 4:
                case 5:
                    return SGAHeader4.ObjectSize;

                case 6:
                case 7:
                    return SGAHeader6.ObjectSize;

                default:
                    return default;
            }
        }

        /// <summary>
        /// Get the section count for the current file version
        /// </summary>
        private uint GetSectionCount()
        {
            switch (File.Header.MajorVersion)
            {
                case 4:
                    return (DirectoryHeader as SGADirectoryHeader4).SectionCount;

                case 5:
                case 6:
                    return (DirectoryHeader as SGADirectoryHeader5).SectionCount;

                case 7:
                    return (DirectoryHeader as SGADirectoryHeader7).SectionCount;

                default:
                    return default;
            }
        }

        /// <summary>
        /// Get the section size for the current file version
        /// </summary>
        private int GetSectionSize()
        {
            switch (File.Header.MajorVersion)
            {
                case 4:
                    return SGASection4.ObjectSize;

                case 5:
                case 6:
                case 7:
                    return SGASection5.ObjectSize;

                default:
                    return default;
            }
        }

        #endregion
    }
}
